using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneTutorial
{
    public class BasicSceneLoadUnload : MonoBehaviour
    {

        public int FirstSceneIndex;
        public string FirstSceneName;
        
        private void Start()
        {
            LoadSceneExamples();
        }

        // Don't actually run this on the Load scene or you just load the same scene infinitely
        private void LoadSceneExamples()
        {
            // Use SceneManager.LoadScene() to load a scene
            // Loading a scene this way REPLACES the current scene with the scene you are loading
            // This method is "synchronous", meaning your application will hang while the scene loads
            // This isn't a problem in our small games, but in BIG games, it would feel like the game crashed

            // Scenes may be loaded by an int that refers to their index in the Build Settings list of scenes
            // Scene 0 is loaded when the application loads
            SceneManager.LoadScene(FirstSceneIndex);

            // Scenes may also be loaded by a string that refers to the name of the scene file (.unity)
            // The string must match the name of the scene exactly or the SceneManager will throw an exception
            // The scene must ALSO be in the build list in build settings
            SceneManager.LoadScene(FirstSceneName);

            // You cannot load a scene using SceneManager that is not in the build settings list
            // Of course, there are ways around this, but for a simple game like this these workarounds are not necessary
            // See the official Unity documentation for a more thorough explanation of these scene loading methods
        }

        
    }
}