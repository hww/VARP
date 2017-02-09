﻿using System;
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
        public string text;
        public string help;
        public string shortcut;

        public Menu(string text, string shortcut = null, string help = null) : base()
        {
            this.text = text;
            this.text = help;
            this.shortcut = shortcut;
        }

        public Menu(KeyMap parentKeyMap, string text, string shortcut = null, string help = null) : base(parentKeyMap)
        {
            this.text = text;
            this.text = help;
            this.shortcut = shortcut;
        }

        public virtual string Text { get { return text; } }
        public virtual string Help { get { return help; } }
        public virtual string Shorcut { get { return shortcut; } }

        public bool Define(string[] sequence, BindingValue value, bool acceptDefaults = false)
        {
            var newsequence = Kbd.ParsePseudo(sequence);
            return Define(newsequence, value, acceptDefaults);
        }

        public override bool Define(int[] sequence, BindingValue value, bool acceptDefaults = false)
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
                            throw new Exception(Log.ExceptionFormat("Expect KeyMap at '{0}' found: '{1}' in: '{2}'", sequence[i], tmp, sequence.ToString()));
                    }
                }
            }
            throw new Exception(Log.ExceptionFormat("We can't be here"));
        }

    }



}