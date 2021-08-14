using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource musicSource;
    public GameObject soundHolder;
    public float soundDestroyThreshold;

    public void SetMusicVolume(float volume){
        musicSource.volume = volume;
    }

    public float GetMusicVolume(){
        return musicSource.volume;
    }

    public void PlaySound(AudioClip clip, float volume, bool loop){
        GameObject soundObject = new GameObject(Time.time.ToString());

        AudioSource soundSource = soundObject.AddComponent<AudioSource>();

        soundSource.clip = clip;
        soundSource.volume = volume;
        soundSource.loop = loop;
        soundSource.Play();

        Destroy(soundObject, clip.length + soundDestroyThreshold);
    }
}
