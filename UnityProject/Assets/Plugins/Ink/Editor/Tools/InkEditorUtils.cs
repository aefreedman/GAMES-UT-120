﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ink.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ink.UnityIntegration
{
    class CreateInkAssetAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var text = "";
            if (File.Exists(resourceFile))
            {
                StreamReader streamReader = new StreamReader(resourceFile);
                text = streamReader.ReadToEnd();
                streamReader.Close();
            }

            Object asset = CreateScriptAsset(pathName, text);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }

        internal static Object CreateScriptAsset(string pathName, string text)
        {
            string fullPath = Path.GetFullPath(pathName);
            UTF8Encoding encoding = new UTF8Encoding(true, false);
            bool append = false;
            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();
            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(DefaultAsset));
        }
    }

    [InitializeOnLoad]
    public static class InkEditorUtils
    {
        public const string inkFileExtension = ".ink";
        const string lastCompileTimeKey = "InkIntegrationLastCompileTime";

        static InkEditorUtils()
        {
            float lastCompileTime = LoadAndSaveLastCompileTime();
            if (EditorApplication.timeSinceStartup < lastCompileTime)
                OnOpenUnityEditor();
        }

        static float LoadAndSaveLastCompileTime()
        {
            float lastCompileTime = 0;
            if (EditorPrefs.HasKey(lastCompileTimeKey))
                lastCompileTime = EditorPrefs.GetFloat(lastCompileTimeKey);
            EditorPrefs.SetFloat(lastCompileTimeKey, (float) EditorApplication.timeSinceStartup);
            return lastCompileTime;
        }

        static void OnOpenUnityEditor()
        {
            InkLibrary.Rebuild();
        }

        [MenuItem("Assets/Rebuild Ink Library", false, 60)]
        public static void RebuildLibrary()
        {
            InkLibrary.Rebuild();
        }

        [MenuItem("Assets/Recompile Ink", false, 61)]
        public static void RecompileAll()
        {
            List<InkFile> masterInkFiles = InkLibrary.GetMasterInkFiles();
            List<string> compiledFiles = new List<string>();
            foreach (InkFile masterInkFile in masterInkFiles)
            {
                if (InkSettings.Instance.compileAutomatically || masterInkFile.compileAutomatically)
                {
                    InkCompiler.CompileInk(masterInkFile);
                    compiledFiles.Add(Path.GetFileName(masterInkFile.filePath));
                }
            }

            string logString = compiledFiles.Count == 0
                ? "No valid ink found. Note that only files with 'Compile Automatic' checked are compiled if not set to compile all files automatically in InkSettings file."
                : "Recompile All will compile " + string.Join(", ", compiledFiles.ToArray());
            Debug.Log(logString);
        }


        [MenuItem("Assets/Create/Ink", false, 120)]
        public static void CreateNewInkFile()
        {
            string fileName = "New Ink.ink";
            string filePath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(GetSelectedPathOrFallback(), fileName));
            CreateNewInkFile(filePath, InkSettings.Instance.templateFilePath);
        }

        public static void CreateNewInkFile(string filePath, string templateFileLocation)
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateInkAssetAction>(), filePath,
                InkBrowserIcons.inkFileIcon, templateFileLocation);
        }

        private static string GetSelectedPathOrFallback()
        {
            string path = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }

            return path;
        }


        [MenuItem("Help/Ink/About")]
        public static void OpenAbout()
        {
            Application.OpenURL("https://github.com/inkle/ink#ink");
        }

        [MenuItem("Help/Ink/API Documentation...")]
        public static void OpenWritingDocumentation()
        {
            Application.OpenURL("https://github.com/inkle/ink/blob/master/Documentation/RunningYourInk.md");
        }

        [MenuItem("Help/Ink/Donate...")]
        public static void Donate()
        {
            Application.OpenURL("https://www.patreon.com/inkle");
        }

        [PostProcessBuildAttribute(-1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (!Debug.isDebugBuild)
            {
                Debug.Log(
                    "<color=blue>Thanks for using ink, and best of luck with your release!\nIf you're doing well, please help fund the project via Patreon https://www.patreon.com/inkle</color>");
            }
        }

        public static TextAsset CreateStoryStateTextFile(string jsonStoryState, string defaultPath = "Assets/Ink", string defaultName = "storyState")
        {
            string name = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(defaultPath, defaultName + ".json")).Substring(defaultPath.Length + 1);
            string fullPathName = EditorUtility.SaveFilePanel("Save Story State", defaultPath, name, "json");
            if (fullPathName == "")
                return null;
            using (StreamWriter outfile = new StreamWriter(fullPathName))
            {
                outfile.Write(jsonStoryState);
            }

            string relativePath = AbsoluteToUnityRelativePath(fullPathName);
            AssetDatabase.ImportAsset(relativePath);
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(relativePath);
            return textAsset;
        }

        public static bool StoryContainsVariables(Story story)
        {
            return story.variablesState.GetEnumerator().MoveNext();
        }

        public static bool CheckStoryIsValid(string storyJSON, out Exception exception)
        {
            try
            {
                new Story(storyJSON);
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }

            exception = null;
            return true;
        }

        public static bool CheckStoryIsValid(string storyJSON, out Story story)
        {
            try
            {
                story = new Story(storyJSON);
            }
            catch
            {
                story = null;
                return false;
            }

            return true;
        }

        public static bool CheckStoryIsValid(string storyJSON, out Exception exception, out Story story)
        {
            try
            {
                story = new Story(storyJSON);
            }
            catch (Exception ex)
            {
                exception = ex;
                story = null;
                return false;
            }

            exception = null;
            return true;
        }

        public static bool CheckStoryStateIsValid(string storyJSON, string storyStateJSON)
        {
            Story story;
            if (CheckStoryIsValid(storyJSON, out story))
            {
                try
                {
                    story.state.LoadJson(storyStateJSON);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetInklecateFilePath()
        {
            if (InkSettings.Instance.customInklecateOptions.inklecate != null)
            {
                return Path.GetFullPath(AssetDatabase.GetAssetPath(InkSettings.Instance.customInklecateOptions.inklecate));
            }
            else
            {
#if UNITY_EDITOR
#if UNITY_EDITOR_WIN
                string inklecateName = "inklecate_win.exe";
#endif
                // Unfortunately inklecate's implementation uses newer features of C# that aren't
                // available in the version of mono that ships with Unity, so we can't make use of
                // it. This means that we need to compile the mono runtime directly into it, inflating
                // the size of the executable quite dramatically :-( Hopefully we can improve that
                // when Unity ships with a newer version.
#if UNITY_EDITOR_OSX
				string inklecateName = "inklecate_mac";
#endif
                // Experimental linux build
#if UNITY_EDITOR_LINUX
				string inklecateName = "inklecate_win.exe";
#endif
#endif

                string[] inklecateDirectories = Directory.GetFiles(Application.dataPath, inklecateName, SearchOption.AllDirectories);
                if (inklecateDirectories.Length == 0)
                    return null;

                return Path.GetFullPath(inklecateDirectories[0]);
            }
        }

        // Returns a sanitized version of the supplied string by:
        //    - swapping MS Windows-style file separators with Unix/Mac style file separators.
        //
        // If null is provided, null is returned.
        public static string SanitizePathString(string path)
        {
            if (path == null)
            {
                return null;
            }

            return path.Replace('\\', '/');
        }

        // Combines two file paths and returns that path.  Unlike C#'s native Paths.Combine, regardless of operating 
        // system this method will always return a path which uses forward slashes ('/' characters) exclusively to ensure
        // equality checks on path strings return equalities as expected.
        public static string CombinePaths(string firstPath, string secondPath)
        {
            return SanitizePathString(Path.Combine(firstPath, secondPath));
        }

        public static string AbsoluteToUnityRelativePath(string fullPath)
        {
            return SanitizePathString(fullPath.Substring(Application.dataPath.Length - 6));
        }

        public static string UnityRelativeToAbsolutePath(string filePath)
        {
            return CombinePaths(Application.dataPath, filePath.Substring(7));
        }

        /// <summary>
        /// Draws a property field for a story using GUILayout, allowing you to attach stories to the player window for debugging.
        /// </summary>
        /// <param name="story">Story.</param>
        /// <param name="label">Label.</param>
        public static void DrawStoryPropertyField(Story story, GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            if (EditorApplication.isPlaying && story != null)
            {
                if (InkPlayerWindow.isOpen)
                {
                    InkPlayerWindow window = InkPlayerWindow.GetWindow(false);
                    if (window.attached && window.story == story)
                    {
                        if (GUILayout.Button("Detach"))
                        {
                            InkPlayerWindow.Detach();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Attach"))
                        {
                            InkPlayerWindow.Attach(story);
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button("Open Player Window"))
                    {
                        InkPlayerWindow.GetWindow();
                    }
                }
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.Button("Enter play mode to attach to editor");
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws a property field for a story using GUI, allowing you to attach stories to the player window for debugging.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="story">Story.</param>
        /// <param name="label">Label.</param>
        public static void DrawStoryPropertyField(Rect position, Story story, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);
            InkPlayerWindow window = InkPlayerWindow.GetWindow(false);
            if (EditorApplication.isPlaying && story != null /* && story.state != null*/)
            {
                if (window.attached && window.story == story)
                {
                    if (GUI.Button(position, "Detach"))
                    {
                        InkPlayerWindow.Detach();
                    }
                }
                else
                {
                    if (GUI.Button(position, "Attach"))
                    {
                        InkPlayerWindow.Attach(story);
                    }
                }
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                GUI.Button(position, "Enter play mode to attach to editor");
                EditorGUI.EndDisabledGroup();
            }
        }

        public static T FindOrCreateSingletonScriptableObjectOfType<T>(string defaultPath, string playerPrefsKeyOfSavedLocation) where T : ScriptableObject
        {
            T tmpSettings = FastFindAndEnforceSingletonScriptableObjectOfType<T>(playerPrefsKeyOfSavedLocation);
            // If we couldn't find the asset in the project, create a new one.
            if (tmpSettings == null)
            {
                tmpSettings = CreateScriptableObject<T>(defaultPath, playerPrefsKeyOfSavedLocation);
                Debug.Log("Created a new " + typeof(T).Name + " file at " + defaultPath + " because one was not found.");
            }

            return tmpSettings;
        }

        static T CreateScriptableObject<T>(string defaultPath, string playerPrefsKeyOfSavedLocation) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, defaultPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));
            EditorPrefs.SetString(playerPrefsKeyOfSavedLocation, defaultPath);
            return asset;
        }

        public static T FastFindAndEnforceSingletonScriptableObjectOfType<T>(string playerPrefsKeyOfSavedLocation) where T : ScriptableObject
        {
            return FindStoredSingletonScriptableObjectOfType<T>(playerPrefsKeyOfSavedLocation) ??
                   FindAndEnforceSingletonScriptableObjectOfType<T>(playerPrefsKeyOfSavedLocation);
        }

        static T FindStoredSingletonScriptableObjectOfType<T>(string playerPrefsKeyOfSavedLocation) where T : ScriptableObject
        {
            if (EditorPrefs.HasKey(playerPrefsKeyOfSavedLocation))
            {
                T settings = AssetDatabase.LoadAssetAtPath<T>(EditorPrefs.GetString(playerPrefsKeyOfSavedLocation));
                if (settings != null) return settings;
                else EditorPrefs.DeleteKey(playerPrefsKeyOfSavedLocation);
            }

            return null;
        }

        static T FindAndEnforceSingletonScriptableObjectOfType<T>(string playerPrefsKeyOfSavedLocation) where T : ScriptableObject
        {
            string typeName = typeof(T).Name;
            string[] GUIDs = AssetDatabase.FindAssets("t:" + typeName);
            if (GUIDs.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(GUIDs[0]);
                if (GUIDs.Length > 1)
                {
                    var remainingGUID = DeleteAllButOldestScriptableObjects(GUIDs, typeName);
                    path = AssetDatabase.GUIDToAssetPath(remainingGUID);
                }

                EditorPrefs.SetString(playerPrefsKeyOfSavedLocation, path);
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }
            else
            {
                EditorPrefs.DeleteKey(playerPrefsKeyOfSavedLocation);
            }

            return null;
        }

        public static string DeleteAllButOldestScriptableObjects(string[] GUIDs, string typeName)
        {
            if (GUIDs.Length == 0) return null;
            if (GUIDs.Length == 1) return GUIDs[0];
            int oldestIndex = -1;
            DateTime oldestTime = DateTime.Now;
            for (int i = 0; i < GUIDs.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
                var absolutePath = UnityRelativeToAbsolutePath(path);
                var lastWriteTime = File.GetLastWriteTime(absolutePath);
                if (oldestIndex == -1 || lastWriteTime < oldestTime)
                {
                    oldestTime = lastWriteTime;
                    oldestIndex = i;
                }
            }

            for (int i = 0; i < GUIDs.Length; i++)
            {
                if (i == oldestIndex) continue;
                var path = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
                AssetDatabase.DeleteAsset(path);
            }

            Debug.LogWarning("More than one " + typeName + " was found. Deleted newer excess asset instances.");
            return GUIDs[oldestIndex];
        }

        //Replacement until Unity upgrades .Net
        public static bool IsNullOrWhiteSpace(string s)
        {
            return (string.IsNullOrEmpty(s) || IsWhiteSpace(s));
        }

        //Returns true if string is only white space
        public static bool IsWhiteSpace(string s)
        {
            foreach (char c in s)
            {
                if (c != ' ' && c != '\t') return false;
            }

            return true;
        }
    }
}