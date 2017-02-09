using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DMenu
{

    /// <summary>
    /// The base class for any binding value
    /// </summary>
    public abstract class BindingValue
    {
        public delegate void Binding();

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
        public string name;
        public string help;
        Binding binding;

        public MethodBinding(string name, Binding binding, string help = null)
        {
            this.name = name;
            this.binding = binding;
            this.help = help;
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
        public KeyMap parent;
        public List<KeyMapItem> items;

        /// <summary>
        /// Create emty keymap
        /// </summary>
        public KeyMap()
        {
            items = new List<KeyMapItem>();
        }

        public KeyMap([NotNull] KeyMap parentKeyMap)
        {
            if (parentKeyMap == null) throw new ArgumentNullException("parentKeyMap");
            parent = parentKeyMap;
            items = new List<KeyMapItem>();
        }

        public void CopyTo(KeyMap other)
        {
            foreach (var item in items)
                other.SetLocal(item.key, item.value);
        }

        /// <summary>
        /// Set key value pair. Replace existing
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="value"></param>
        public void SetLocal(int evt, BindingValue value)
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

        public KeyMapItem GetLocal(int key, bool acceptDefaults = false)
        {
            var index = GetIndexOf(key);
            return index >= 0 ? items[index] : (parent!=null ? parent.GetLocal(key, acceptDefaults) : null);
        }

        #region Use full expression to define and lookup the definition

        public KeyMapItem LokupKey(int[] sequence, bool acceptDefaults = false)
        {
            return LokupKey(sequence, 0, acceptDefaults);
        }

        public KeyMapItem LokupKey(int[] sequence, int starts, bool acceptDefaults = false)
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
                        throw new Exception(Log.ExceptionFormat("Expect KeyMap at '{0}' found: '{1}' in: '{2}'",
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
                            throw new Exception(Log.ExceptionFormat("Expect KeyMap at '{0}' found: '{1}' in: '{2}'", sequence[i], tmp, sequence.ToString()));
                    }
                }
            }
            throw new Exception(Log.ExceptionFormat("We can't be here"));
        }

        #endregion
    }



}