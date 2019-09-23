using System;
using System.IO;
using Patterns.Assets.PrototypingKit.Patterns;
using UnityEngine;

namespace Examples.DataLogging
{
    public class DataLogger : Singleton<DataLogger>
    {
        private Data _data;

        private void Start()
        {
            // Create an instance of the Data class
            _data = new Data();
        }

//        private void Update()
//        {
//        }

        public void OnDidJump()
        {
            _data.Jumps++;
        }

        /// <summary>
        /// Writes the Json data to a text file
        /// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-write-to-a-text-file
        /// </summary>
        private string SaveData()
        {
            // Turn the serializable class into a Json file
            var jsonData = JsonUtility.ToJson(_data, true);

            // Name the file something that won't get duplicated, or your file will get overwritten. DateTime is good for this.
            var filename = Application.productName + "-DataLog-" + DateTime.Now.ToString("yyyy-M-d-HH-mm-ss") + ".json";

            // Hacky way to get the path to your project folder. See docs page for filepaths
            // https://docs.unity3d.com/ScriptReference/Application-dataPath.html
            // You can use Application.persistentDataPath for a permanent path. See
            // https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
#if UNITY_EDITOR
            var path = Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - "/Assets".Length), "Logs", filename);
#elif UNITY_STANDALONE
            //On standalone, write the files to the desktop
            // You'll need to make the folder!
            // Use Application.persistentDataPath for an automatic solution
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var path = Path.Combine(desktopPath, "logs", filename);
#endif

            // Easiest way to write a string in C#
            File.WriteAllText(path, jsonData);
            return path;
        }

        public void DoLog()
        {
            _data.PlaytimeSeconds = Time.realtimeSinceStartup;
            var filepath = SaveData();
            Debug.Log("Attempted to save file to: " + filepath);
        }

        // Use this method to do things when the game is ending
        private void OnDestroy()
        {
#if UNITY_EDITOR
            DoLog();
#endif
        }

        // OnApplicationQuit() doesn't happen in the editor
        private void OnApplicationQuit()
        {
#if UNITY_STANDALONE
            DoLog();
#endif
        }
    }
}