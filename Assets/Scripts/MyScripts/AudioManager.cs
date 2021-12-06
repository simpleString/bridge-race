using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    AudioSource _audio;

    public List<Sound> sounds = new List<Sound>();

    void Awake() {
        DontDestroyOnLoad(gameObject);
        foreach (var sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.loop = sound.isLoop;
            sound.source.volume = sound.volume;
        }
    }

    public void Play(string name) {
        if (!Store.Store.IsSoundOn) return;
        var sound = sounds.Find(sound => sound.name == name);
        sound.source.Play();
    }

    public void Stop(string name) {
        var sound = sounds.Find(sound => sound.name == name);
        sound.source.Stop();
    }
    public void StopAll() {
        foreach (var sound in sounds) {
            sound.source.Stop();
        }
    }

}
