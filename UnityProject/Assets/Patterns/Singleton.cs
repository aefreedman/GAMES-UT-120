// Singleton.cs
// Last edited 7:43 PM 04/15/2015 by Aaron Freedman

using UnityEngine;

namespace Patterns
{
    // This is a super lazy, but dead-simple way of doing a Singleton using a Template
    // It is definitely not performant in Unity (because of Reflection)
    // It also doesn't enforce a single instance of an object in any way.
    // It's convenient for prototyping, but I don't recommend using it long-term
    namespace Assets.PrototypingKit.Patterns
    {
        public abstract class Singleton<T> : MonoBehaviour where T : Object
        {
            private static T _instance;

            /// <summary>
            ///     Gets or sets the instance.
            /// </summary>
            /// <value>The instance.</value>
            public static T Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        Instance = FindObjectOfType<T>();
                    }

                    return _instance;
                }
                private set { _instance = value; }
            }
        }
    }
}