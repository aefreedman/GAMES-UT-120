using System.Collections.Generic;
using UnityEngine;

namespace ArraysAndLists
{
    public class Lists : MonoBehaviour
    {
        //If Unity knows how to Serialize the elements in the array,
        //they will appear in the Inspector
        public List<int> MyPublicIntList;
        public List<GameObject> MyPublicGameObjectsList;
        public List<Arrays> MyPublicArraysList;

        private List<GameObject> _myGameObjectList;

        public void Start()
        {
            Debug.LogFormat("MyPublicArrayList is called {0}", MyPublicArraysList[0].name);
            
            _myGameObjectList = new List<GameObject>();
//            _myGameObjectList = new List<GameObject> { gameObject }; // you can add elements in the initialization, too
            
            _myGameObjectList.Add(gameObject); 
            _myGameObjectList.Add(gameObject); 
            
            foreach (var obj in _myGameObjectList)
            {
                Debug.LogFormat("Object name is {0}", obj.name);
            }
            
            for (var i = 0; i < _myGameObjectList.Count; i++)
            {
                Debug.LogFormat("Object name is {0}", _myGameObjectList[i].name);

            }
            
            _myGameObjectList.Remove(gameObject); // removes the first instance of the element

            Debug.LogFormat("_myGameObjectList has {0} elements", _myGameObjectList.Count);
            
            _myGameObjectList.Clear(); // removes all elements
            
            Debug.LogFormat("_myGameObjectList has {0} elements", _myGameObjectList.Count);
            
            //Copying between Lists and Arrays

            var myListAsArray = _myGameObjectList.ToArray(); // new array with same contents
            // ToList() also exists for Arrays
            
            var myListCopy = _myGameObjectList; // copies reference to list -- behaves list Array example
            
            var myNewListWithValues = new List<GameObject>();
            myNewListWithValues.AddRange(_myGameObjectList); // AddRange() copies the elements of the array to the other array

        }
    }
}