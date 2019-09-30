using Libraries;
using UnityEditor;
using UnityEngine;

namespace Examples.JSON
{
    public class JsonPrinter : MonoBehaviour
    {
        public int[] Ints;
        private void Start()
        {
            var myJsonData = new Data {MyInts = Ints};
            var filepath = JsonTools.SaveJsonData(myJsonData);
            Debug.Log(filepath);
            EditorApplication.isPlaying = false;
        }
    }
}