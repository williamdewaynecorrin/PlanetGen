using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance = null;

    void Awake()
    {
        if (instance == null)
        {
            InitInstance();
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }
    void InitInstance()
    {
        instance = this;
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    public static AudioSource PlayClip2D(AudioClip clip)
    {
        AudioSource audio = new GameObject("audio_" + clip.name).AddComponent<AudioSource>();
        audio.clip = clip;
        audio.spatialBlend = 0.0f;
        audio.Play();
        GameObject.Destroy(audio.gameObject, clip.length);

        return audio;
    }

    public static AudioSource PlayRandomClip2D(AudioClip[] clips)
    {
        int i = Random.Range(0, clips.Length);
        return PlayClip2D(clips[i]);
    }
}