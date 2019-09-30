using System;
using System.IO;
using Libraries;
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

        public void DoLog()
        {
            _data.PlaytimeSeconds = Time.realtimeSinceStartup;
            
            // JsonTools.SavaData is a static method shared between many examples
            var filepath = JsonTools.SaveJsonData(this);
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