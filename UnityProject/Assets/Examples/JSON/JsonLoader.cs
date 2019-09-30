using UnityEngine;

namespace Examples.JSON
{
    public class JsonLoader : MonoBehaviour
    {
        public Content Content;
        public TextAsset JsonFile;

        private void Start()
        {
            Content = JsonUtility.FromJson<Content>(JsonFile.text);
        }
    }
}