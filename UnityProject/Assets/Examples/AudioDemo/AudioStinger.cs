using UnityEngine;

public class AudioStinger : MonoBehaviour
{
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private KeyCode _key;

    [Range(0, 1f)]
    public float Volume;

//    private void Start()
//    {
//        // Best practice is to check that _audio != null and get the AudioSource if it is null
//    }

    private void Update()
    {
        if (Input.GetKeyDown(_key))
            _audio.PlayOneShot(_clip, Volume);
    }
}