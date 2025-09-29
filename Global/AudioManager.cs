using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    private Dictionary<string, AudioSource> loopSources = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVol) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVol) * 20);

    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip != null)
        {
            StopMusic();
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);

        

    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);

        

    }

    public void PlayLoop(string key, AudioClip clip, float normalizedVolume = 1f)
    {
        if (!loopSources.ContainsKey(key))
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = sfxMixerGroup;
            source.clip = clip;
            source.loop = true;
            source.Play();
            loopSources[key] = source;
        }

    }

    public void StopLoop(string key)
    {
        if (loopSources.ContainsKey(key))
        {
            AudioSource source = loopSources[key];
            source.Stop();
            Destroy(source);
            loopSources.Remove(key);
        }
    }


}