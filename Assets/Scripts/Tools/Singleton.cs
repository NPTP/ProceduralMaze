using System;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// Singleton class that either finds an existing instance or creates one in the scene.
    /// Prevents creating a new instance on application quit.
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