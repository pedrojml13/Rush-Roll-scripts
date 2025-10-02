using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace PJML.RushAndRoll
{
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
            // Singleton: asegura una única instancia persistente
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            // Carga volúmenes guardados y los aplica al mixer
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1f));
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1f));
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip != null)
                sfxSource.PlayOneShot(clip);
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null) return;

            StopMusic();
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }

        public void StopMusic()
        {
            if (musicSource.isPlaying)
                musicSource.Stop();
        }

        public void SetSFXVolume(float volume)
        {
            // Convierte volumen lineal a decibelios
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        }

        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        }

        public void PlayLoop(string key, AudioClip clip, float normalizedVolume = 1f)
        {
            if (loopSources.ContainsKey(key) || clip == null) return;

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = sfxMixerGroup;
            source.clip = clip;
            source.loop = true;
            source.volume = Mathf.Clamp01(normalizedVolume);
            source.Play();

            loopSources[key] = source;
        }

        public void StopLoop(string key)
        {
            if (!loopSources.TryGetValue(key, out AudioSource source)) return;

            source.Stop();
            Destroy(source);
            loopSources.Remove(key);
        }
    }
}