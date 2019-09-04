using UnityEngine;
using System.Collections;

public class CompleteRotator : MonoBehaviour
{
    //Update is called every frame
    void Update()
    {
        //Rotate thet transform of the game object this is attached to by 45 degrees, taking into account the time elapsed since last frame.
        transform.Rotate(new Vector3(0, 0, 45) * Time.deltaTime);
        // Time.deltaTime -> 60fps -> 1 / 60 - > 0.

    }
}