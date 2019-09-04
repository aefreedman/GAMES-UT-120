using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ArraysAndLists
{
    public class Arrays : MonoBehaviour
    {
        // Declaring arrays
        //Simple declaration:
        public int[] IntArray; // Declaring a public int array -- will show up in Unity Inspector
        private int[] _privateInts; // Declaring a private array -- will not show up in Unity Inspector


        //Advanced Usage: declaring and initializing
        private int[] _tenInts = new int[10]; // declaring a private int array and initializing it with length 10
        // Int is a Value Type, which means this array initializes all elements with a value of 0
        // If this were a GameObject[] (a Reference Type), the elements would be NULL (null references)

        public int[] _IntsWithDefaults = { 0, 1, 2, 3, 4, 5, 6, 8, 9 }; // declaring and populating an array
        // Initializing arrays like this allows you to provide default values that you can override

        #region Advanced Stuff

        public int[][] TwoDimensionalArray; // Unity doesn't know how to serialize two-dimensional arrays
        // You can use regular multi-dimensional arrays in code as normal, though

        [Serializable] // Unity won't show this in the inspector unless you tag it as Serializable
        public struct IntContainer
        {
            public string name; // Unity will use "name" as the name of the element in serialized structs
            public int[] IntArray;
        }

        public IntContainer[] IntContainers;

        #endregion Advanced stuff


        private void Start()
        {
            IntArray = new int[10];
            // If you initialize a public array in Start(),
            // anything you enter in the inspector will be overwritten when you run the scene

            #region Declaration and Initialization

            // Local Arrays

            // Explicitly declared
            int[] localInts = new int[] { 1, 2, 4 };

            //Implicitly declared
            var implicitInts = new[] { 1, 2, 4 };
            var implicitFloats = new[] { 1f, 3f, 4f }; // type is inferred from values
            // You can't mix types in an Array

            //FindObjectsOfType returns T[] <-- notice the [], that means an Array
            //"T" means "of the specified type", which is "Arrays" here
            var monoBehaviourArray = FindObjectsOfType<Arrays>();
//            Arrays[] monoBehaviourArray = FindObjectsOfType<Arrays>();

            // Finds every gameObject and retains the array in the local variable
            // (Don't actually do this example in your projects, it's not a good idea)
            // Remember to check the return type: This method returns Object[],
            // NOT GameObject[]
            var gameObjectsArray = FindObjectsOfType(typeof(GameObject));
//            Object[] gameObjectsArray = FindObjectsOfType(typeof(GameObject));

            #endregion

            #region Copying Arrays

            var myArrayCopy = IntArray;
            // Arrays are Reference Types, which means myArrayCopy and IntArray refer to the same Array in memory

            Debug.Log("Arrays before modification");
            Debug.LogFormat("IntArray[0] == {0}", IntArray[0]);
            Debug.LogFormat("myArray[0] == {0}", myArrayCopy[0]);

            myArrayCopy[0] = 1000;

            Debug.Log("Arrays after changing myArray[0]");
            Debug.LogFormat("IntArray[0] == {0}", IntArray[0]);
            Debug.LogFormat("myArray[0] == {0}", myArrayCopy[0]);
            
            // in order to copy the values of an array (not just copy the reference), you need to make a NEW array and copy each value
            var myRealArrayCopy = new int[IntArray.Length];
            for (var i = 0; i < IntArray.Length; i++)
            {
                myRealArrayCopy[i] = IntArray[i];
            }

            myRealArrayCopy[0] = 9999;
            Debug.Log("Arrays after changing myRealArrayCopy[0]");
            Debug.LogFormat("IntArray[0] == {0}", IntArray[0]);
            Debug.LogFormat("myRealArrayCopy[0] == {0}", myRealArrayCopy[0]);

            #endregion

            #region Accessing Arrays

            //Accessing Arrays

            // Remember that the last index number is always one less than the total Length
            // If you try to access an Index that doesn't exist you will get an IndexOutOfRangeException
            for (int i = 0; i < IntArray.Length; i++)
            {
                var myValue = IntArray[i];
                // The name of the element is IntArray[i]
                // the value of the element is the contents of the element
                // the Index of the element is i

                Debug.Log(myValue.ToString("D"));
//                Debug.LogFormat("{0:D}", myValue); // This does the same as the above
            }

            // foreach iterates over Arrays in order in C# (0, 1, 2, 3, etc.)
            // this is convenient when you don't need to access the index of the elements
            foreach (var t in IntArray)
            {
                Debug.Log(t.ToString("D"));
//                Debug.LogFormat("{0:D}", IntArray[i]); // This does the same as the above
            }

            #endregion
        }
    }
}