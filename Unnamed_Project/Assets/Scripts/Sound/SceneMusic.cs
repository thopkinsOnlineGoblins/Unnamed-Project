using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public AudioClip music;
    public float volume = 1f;

    AudioSource _source;

    void Start()
    {
        if (music == null) return;

        _source = gameObject.AddComponent<AudioSource>();
        _source.clip = music;
        _source.volume = volume;
        _source.loop = true;
        _source.playOnAwake = false;
        _source.Play();
    }

    void OnDestroy()
    {
        if (_source != null) _source.Stop();
    }
}