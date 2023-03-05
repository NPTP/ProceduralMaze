using System;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// Either find an instance or create one in the scene.
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : Component, new()
    {
        private static bool allowInstanceCreation = true;
        
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null && allowInstanceCreation)
                    {
                        GameObject g = new GameObject(typeof(T).Name);
                        instance = g.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        protected void OnApplicationQuit()
        {
            allowInstanceCreation = false;
        }
    }
}