using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Sprite active;
    public Sprite notActive;
    public Button volume;
    public Button volumePause;
    public Sound[] sounds;
    private bool toggle = true;
    public AudioClip meh;


    private void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Start()
    {
        play("MenuTheme");
    }

    public void play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public void stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

    public void mute()
    {
        toggle = !toggle;
        if (toggle)
        {
            AudioListener.volume = 1f;
            volume.image.sprite = active;
            volumePause.image.sprite = active;
        }
        else
        {
            AudioListener.volume = 0f;
            volume.image.sprite = notActive;
            volumePause.image.sprite = notActive;
        }
    }

}
