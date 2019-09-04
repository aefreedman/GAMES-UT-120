using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneTutorial
{
    public class AsyncLoadExamples : MonoBehaviour
    {
        public int SecondSceneIndex;
        public bool LoadAdditive;

        private void Start()
        {
            HowToStartCoroutines();
        }

        private void HowToStartCoroutines()
        {
            // You do it like this
            StartCoroutine(LoadSceneAsyncExamples());
        }

        // You can use an async load as a coroutine to do things while the scene is loading
        // This is how you make loading screens
        private IEnumerator LoadSceneAsyncExamples()
        {
            // Loading scenes asynchronously loads the scene on a separate thread
            // This means that your application will continue to run while the scene loads
            // There are much more situations to consider when you load asynchronously
            // especially because you can't assume that every object exists before the first Update() loop
            // because the scene may not have finished loading yet

            // LoadSceneAsync() returns an AsyncOperation object
            AsyncOperation asyncOperation;

            if (!LoadAdditive)
            {
                asyncOperation = SceneManager.LoadSceneAsync(SecondSceneIndex, LoadSceneMode.Single);
            }
            else
            {
                // You can also load scenes additively, if you do this remember the scene you load is not
                // the active scene
                asyncOperation = SceneManager.LoadSceneAsync(SecondSceneIndex, LoadSceneMode.Additive);
            }

            // you can reference the operation while it is loading the screen
            while (!asyncOperation.isDone)
            {
                Debug.LogFormat("Loading scene! {0:P}", asyncOperation.progress);
                yield return new WaitForEndOfFrame(); // Don't forget to yield or Unity will crash
            }

            // If you loaded the scene additively, the scene you loaded is not the active scene
            // If you want to instantiate into this new scene, you must make it the active scene
            if (LoadAdditive)
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(SecondSceneIndex));
        }

        private IEnumerator UnloadSceneExamples()
        {
            // Unloading of scene is only necessary when you are using the multi-scene asynchronous workflow
            // (You can't unload a scene if you only have one scene active)

            var asyncOperation = SceneManager.UnloadSceneAsync(SecondSceneIndex);

            while (!asyncOperation.isDone)
            {
                Debug.LogFormat("Unloading scene! {0:P}", asyncOperation.progress);
                yield return new WaitForEndOfFrame(); // Don't forget to yield or Unity will crash
            }
        }
    }
}