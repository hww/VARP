using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VARP
{
    public static class UiSingletone
    {
        #region Components Creation

        /// <summary>
        /// Get or create component of given type to this game object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            T comp = gameObject.GetComponent<T>();

            if (!comp)
                comp = gameObject.AddComponent<T>();

            return comp;
        }

        /// <summary>
        /// Get object of this type. And if it does not exists, create new object with 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FindSingletoneOfType<T>() where T : Component
        {
            var component = UnityEngine.Object.FindObjectOfType<T>();
            if (component != null) return component;

            var gameObject = new GameObject(typeof(T).ToString());
            component = gameObject.AddComponent<T>();
            return component;
        }

        #endregion

    }
}
