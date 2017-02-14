using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DMenu
{
    /// <summary>
    /// The menu object behave same as key map
    /// </summary>
    public class Menu : KeyMap
    {
        public string help;
        public string keys;

        public Menu(string title, string keys = null, string help = null) : base(title)
        {
            this.title = help;
            this.keys = keys;
        }

        public Menu(KeyMap parent, string title, string keys = null, string help = null) : base(parent, title)
        {
            this.title = help;
            this.keys = keys;
        }

        public virtual string Title { get { return title; } }
        public virtual string Help { get { return help; } }
        public virtual string Keys { get { return keys; } }

        public bool Define(string[] sequence, object value, bool acceptDefaults = false)
        {
            var newsequence = Kbd.ParsePseudo(sequence);
            return Define(newsequence, value, acceptDefaults);
        }

        public override bool Define(int[] sequence, object value, bool acceptDefaults = false)
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
                        var newMap = new Menu(Event.GetName(key));
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
                        return true;
                    }
                    else
                    {
                        // the curentMap is the map in the sequence and it has definition 
                        var map = tmp.value as Menu;
                        if (map != null)
                            curentMap = map;
                        else
                            throw new Exception(string.Format("Expect KeyMap at '{0}' found: '{1}' in: '{2}'", sequence[i], tmp, sequence.ToString()));
                    }
                }
            }
            throw new Exception(string.Format("We can't be here"));
        }

    }



}