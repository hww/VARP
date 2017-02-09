﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DMenu
{
    /// <summary>
    /// The base class for all menu items
    /// </summary>
    public class MenuItem : BindingValue
    {
        //public static MenuItem Create(string text, string shortcut = null, string name)
        //{
        //    return new MenuItemSimple(text, name);
        //}

        public virtual string Text { get { return string.Empty; } }
        public virtual string Help { get { return string.Empty; } }
        public virtual string Shorcut { get { return string.Empty; } }
    }

    public class MenuItemSimple : MenuItem
    { 
        protected string text;
        protected string help;
        protected string shortcut;

        public MenuItemSimple(string text, string shortcut = null, string help = null) 
        {
            this.text = text;
            this.text = help;
            this.shortcut = shortcut;
        }

        public override string Text { get { return text; } }
        public override string Help { get { return help; } }
        public override string Shorcut { get { return shortcut; } }
    }

    public class MenuItemComplex : MenuItemSimple
    {
        public delegate void Precodition();
        public delegate MenuItemComplex Filter(MenuItemComplex menu_item);

        public enum ButtonType
        {
            NoButton,
            Toggle,
            Radio
        }

        public Binding binding;
        public Binding enable;
        public Binding visible;
        public ButtonType buttonType;

        /// <summary>
        /// Non selectable string
        /// </summary>
        /// <param name="text"></param>
        public MenuItemComplex(string text, string shortcut = null, string help = null) : base(text, shortcut, help)
        {
        }

        /// <summary>
        /// Non selectable string
        /// </summary>
        /// <param name="text"></param>
        public MenuItemComplex(string text, Binding real_binding, string shortcut = null, string help = null) : base(text, shortcut, help)
        {
            this.binding = real_binding;
        }

        public MenuItemComplex(string text, 
            Binding realBinging,
            Precodition enable = null, 
            Precodition visible = null,
            Filter filter = null,
            string shortcut = null,
            string help = null) : base(text, shortcut, help)
        {

        }

        public MenuItemComplex(string text, 
            Binding realBinging,
            Precodition enable = null,
            Precodition visible = null,
            Filter filter = null,
            ButtonType buttonType = ButtonType.NoButton,
            Precodition buttonState = null,
            string shortcut = null,
            string help = null) : base(text, shortcut, help)
        {

        }
    }

    /// <summary>
    /// The menu separator class
    /// </summary>
    public class MenuSeparator : MenuItem
    {
        public enum Type
        {
            NoLine,
            Space,
            SingleLine,
            DashedLine
        }

        public Type type;

        /// <summary>
        /// The separator type
        /// </summary>
        /// <param name="separatorType"></param>
        public MenuSeparator(Type separatorType) 
        {
            this.type = separatorType;
        }
    }

}