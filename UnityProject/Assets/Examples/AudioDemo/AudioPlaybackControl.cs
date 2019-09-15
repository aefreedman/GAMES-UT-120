using UnityEngine;

public class AudioPlaybackControl : MonoBehaviour
{
    [SerializeField] private AudioSource _audio;
    [SerializeField] private KeyCode _playKey;
    [SerializeField] private KeyCode _stopKey;

    private void Update()
    {
        if (Input.GetKeyDown(_playKey))
            _audio.Play();
        if (Input.GetKeyDown(_stopKey))
            _audio.Stop();
    }
}