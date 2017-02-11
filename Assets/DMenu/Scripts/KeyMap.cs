using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace DMenu
{

    /// <summary>
    /// The base class for any binding value
    /// </summary>
    public abstract class BindingValue
    {
        public virtual BindingValue Clone()
        {
            throw new System.Exception("Abstract class can't be cloned");
        }

        public virtual void Copy(BindingValue other)
        {
            throw new System.Exception("Abstract class can't be copyied");
        }

    }

    /// <summary>
    /// This is simple binding to any key.
    /// </summary>
    public class MethodBinding : BindingValue
    {
        private NativeFunction function;

        public MethodBinding(NativeFunction function)
        {
            this.function = function;
        }
    }

    /// <summary>
    /// This is simple keysequence binding to any key.
    /// </summary>
    public class SequenceBinding : BindingValue
    {
        public string name;
        public string help;
        public readonly int[] sequence;

        public SequenceBinding(string name, int[] sequence, string help = null)
        {
            this.name = name;
            this.sequence = sequence;
            this.help = help;
        }
    }

    /// <summary>
    /// Any class wich can be in the keymap have to be
    /// based on this class
    /// </summary>
    public class KeyMapItem
    {
        public static readonly KeyMapItem Empty = new KeyMapItem(0, null);

        public int key;          //< this is the fake key
        public BindingValue value;  //< there can be any avaiable value

        public KeyMapItem(int key, BindingValue value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Compare with single event
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public bool Compare(int evt)
        {
            return evt == key;
        }

        public override string ToString()
        {
            return string.Format("key: {0} value: {1}", key, value);
        }
    }
 

    /// <summary>
    /// This class have to alow to build the tree of the keymaps
    /// </summary>
    public class KeyMap : BindingValue
    {
        public string title;
        public KeyMap parent;
        public List<KeyMapItem> items;

        /// <summary>
        /// Create emty keymap
        /// </summary>
        /// <param name="title"></param>
        public KeyMap(string title = null)
        {
            this.title = title;
            items = new List<KeyMapItem>();
        }

        /// <summary>
        /// Create empty keymap based on parent keymap
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="title"></param>
        public KeyMap([NotNull] KeyMap parent, string title = null)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            this.title = title;
            this.parent = parent;
            items = new List<KeyMapItem>();
        }

        public virtual void CopyTo(KeyMap other)
        {
            other.title = title;
            other.parent = parent;
            foreach (var item in items)
                other.SetLocal(item.key, item.value);
        }

        /// <summary>
        /// Get index of element which has given sequence
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        private int GetIndexOf(int evt)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Compare(evt))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Set key value pair. Replace existing
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="value"></param>
        public virtual void SetLocal(int evt, BindingValue value)
        {
            if (!Event.IsValid(evt))
                throw new ArgumentOutOfRangeException("evt");
            var index = GetIndexOf(evt);
            if (index >= 0)
            {
                items[index] = new KeyMapItem(evt, value);
            }
            else
            {
                var item = new KeyMapItem(evt, value);
                items.Add(item);
            }
        }

        public virtual KeyMapItem GetLocal(int evt, bool acceptDefaults = false)
        {
            if (!Event.IsValid(evt))
                throw new ArgumentOutOfRangeException("evt");
            var index = GetIndexOf(evt);
            return index >= 0 ? items[index] : null;
        }

        #region Use full expression to define and lookup the definition

        public virtual KeyMapItem LokupKey(int[] sequence, bool acceptDefaults = false)
        {
            return LokupKey(sequence, 0, acceptDefaults);
        }

        public virtual KeyMapItem LokupKey(int[] sequence, int starts, bool acceptDefaults = false)
        {
            var curentMap = this;
            var lastIndex = sequence.Length - 1;
            var tmp = null as KeyMapItem;

            for (var i=starts; i<sequence.Length; i++)
            { 
                tmp = curentMap.GetLocal(sequence[i]);
                if (tmp == null)
                    return curentMap.parent != null ? curentMap.parent.LokupKey(sequence, acceptDefaults) : null;

                if (i != lastIndex)
                {
                    var map = tmp.value as KeyMap;
                    if (map == null)
                        throw new Exception(Dbg.LogExceptionFormat("Expect KeyMap at '{0}' found: '{1}' in: '{2}'",
                            sequence[i], tmp, sequence.ToString()));
                    curentMap = map;
                }
            }
            return tmp;
        }

        public virtual bool Define(int[] sequence, BindingValue value, bool acceptDefaults = false)
        {
            var curentMap = this;
            var lastIndex = sequence.Length - 1;

            for (var i = 0; i < sequence.Length; i++)
            {
                var key = sequence[i];
                var tmp = curentMap.GetLocal(key);
                if (tmp == null)
                {
                    // there is no this binding
                    // N.B. Do not look at the parent one!
                    if (i == lastIndex)
                    {
                        // the curentMap is the target map and it does not have definition 
                        curentMap.SetLocal(key, value);
                        return true;
                    }
                    else
                    {
                        // the curentMap is the map in the sequence and it does not have definition 
                        var newMap = new KeyMap();
                        curentMap.SetLocal(key, newMap);
                        curentMap = newMap;
                    }
                }
                else
                {
                    // we found binding in curentMap
                    if (i == lastIndex)
                    {
                        // curentMap is target map, it has binding but we have to redefine it
                        curentMap.SetLocal(key, value);
                    }
                    else
                    {
                        // the curentMap is the map in the sequence and it has definition 
                        var map = tmp.value as KeyMap;
                        if (map != null)
                            curentMap = map;
                        else
                            throw new Exception(Dbg.LogExceptionFormat("Expect KeyMap at '{0}' found: '{1}' in: '{2}'", sequence[i], tmp, sequence.ToString()));
                    }
                }
            }
            throw new Exception(Dbg.LogExceptionFormat("We can't be here"));
        }

        #endregion
    }


    /// <summary>
    /// If an element of a keymap is a char-table, it counts as holding bindings for all
    /// character events with no modifier element n
    /// is the binding for the character with code n.This is a compact way to record
    /// lots of bindings.A keymap with such a char-table is called a full keymap. Other
    /// keymaps are called sparse keymaps.
    /// </summary>
    public class FullKeymap : KeyMap
    {

        /// <summary>
        /// Create emty keymap
        /// </summary>
        /// <param name="title"></param>
        public FullKeymap(string title = null) : base(title)
        {
        }

        /// <summary>
        /// Create empty keymap based on parent keymap
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="title"></param>
        public FullKeymap([NotNull] KeyMap parent, string title = null) : base(parent, title)
        {
        }

        /// <summary>
        /// Set key value pair. Replace existing.
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="value"></param>
        public override void SetLocal(int evt, BindingValue value)
        {
            if (!Event.IsValid(evt))
                throw new ArgumentOutOfRangeException("evt");
            // do not support keys with modificators
            if (evt >= Event.Alt)
                throw new ArgumentOutOfRangeException("evt");

            items[evt] = new KeyMapItem(evt, value);
        }

        public override KeyMapItem GetLocal(int evt, bool acceptDefaults = false)
        {
            if (!Event.IsValid(evt))
                throw new ArgumentOutOfRangeException("evt");
            // do not support keys with modificators
            if (evt >= Event.Alt)
                throw new ArgumentOutOfRangeException("evt");

            return (evt < items.Count && items[evt]!=null) ? items[evt] : (parent != null ? parent.GetLocal(evt, acceptDefaults) : null);
        }

        #region Use full expression to define and lookup the definition

        public override KeyMapItem LokupKey(int[] sequence, bool acceptDefaults = false)
        {
            return LokupKey(sequence, 0, acceptDefaults);
        }

        public override KeyMapItem LokupKey(int[] sequence, int starts, bool acceptDefaults = false)
        {
            var curentMap = this as KeyMap;
            var lastIndex = sequence.Length - 1;
            var tmp = null as KeyMapItem;

            for (var i = starts; i < sequence.Length; i++)
            {
                tmp = curentMap.GetLocal(sequence[i]);
                if (tmp == null)
                    return curentMap.parent != null ? curentMap.parent.LokupKey(sequence, acceptDefaults) : null;

                if (i != lastIndex)
                {
                    var map = tmp.value as KeyMap;
                    if (map == null)
                        throw new Exception(Dbg.LogExceptionFormat("Expect KeyMap at '{0}' found: '{1}' in: '{2}'",
                            sequence[i], tmp, sequence.ToString()));
                    curentMap = map;
                }
            }
            return tmp;
        }

        public override bool Define(int[] sequence, BindingValue value, bool acceptDefaults = false)
        {
            var curentMap = this as KeyMap;
            var lastIndex = sequence.Length - 1;

            for (var i = 0; i < sequence.Length; i++)
            {
                var key = sequence[i];
                var tmp = curentMap.GetLocal(key);
                if (tmp == null)
                {
                    // there is no this binding
                    // N.B. Do not look at the parent one!
                    if (i == lastIndex)
                    {
                        // the curentMap is the target map and it does not have definition 
                        curentMap.SetLocal(key, value);
                        return true;
                    }
                    else
                    {
                        // the curentMap is the map in the sequence and it does not have definition 
                        var newMap = new KeyMap();
                        curentMap.SetLocal(key, newMap);
                        curentMap = newMap;
                    }
                }
                else
                {
                    // we found binding in curentMap
                    if (i == lastIndex)
                    {
                        // curentMap is target map, it has binding but we have to redefine it
                        curentMap.SetLocal(key, value);
                    }
                    else
                    {
                        // the curentMap is the map in the sequence and it has definition 
                        var map = tmp.value as KeyMap;
                        if (map != null)
                            curentMap = map;
                        else
                            throw new Exception(Dbg.LogExceptionFormat("Expect KeyMap at '{0}' found: '{1}' in: '{2}'", sequence[i], tmp, sequence.ToString()));
                    }
                }
            }
            throw new Exception(Dbg.LogExceptionFormat("We can't be here"));
        }

        #endregion
    }

}