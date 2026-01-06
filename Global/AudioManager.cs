using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestor de audio global del juego.
    /// Permite reproducir música, efectos de sonido, loops y guardar volúmenes.
    /// Singleton accesible desde cualquier parte del proyecto.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del AudioManager.
        /// </summary>
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;

        private Dictionary<string, AudioSource> loopSources = new();

        /// <summary>
        /// Singleton, asegura que solo exista una instancia
        /// y evita su destrucción al cambiar de escena.
        /// </summary>
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

        /// <summary>
        /// Inicializa los volúmenes guardados de música y SFX.
        /// </summary>
        void Start()
        {
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1f));
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1f));
        }

        /// <summary>
        /// Guarda los volúmenes actuales de música y efectos de sonido.
        /// </summary>
        public void SaveVolumes(float musicVolume, float sfxVolume)
        {
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Reproduce un efecto de sonido una sola vez.
        /// </summary>
        /// <param name="clip">Clip de audio a reproducir.</param>
        public void PlaySFX(AudioClip clip)
        {
            if (clip != null)
                sfxSource.PlayOneShot(clip);
        }

        /// <summary>
        /// Reproduce música de fondo.
        /// </summary>
        /// <param name="clip">Clip de música.</param>
        /// <param name="loop">Indica si debe repetirse en bucle.</param>
        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null) return;

            StopMusic();
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }

        /// <summary>
        /// Detiene la música de fondo actualmente reproducida.
        /// </summary>
        public void StopMusic()
        {
            if (musicSource.isPlaying)
                musicSource.Stop();
        }

        /// <summary>
        /// Ajusta el volumen de los efectos de sonido.
        /// </summary>
        /// <param name="volume">Valor lineal entre 0 y 1.</param>
        public void SetSFXVolume(float volume)
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        }

        /// <summary>
        /// Ajusta el volumen de la música.
        /// </summary>
        /// <param name="volume">Valor lineal entre 0 y 1.</param>
        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        }

        /// <summary>
        /// Reproduce un clip en bucle asociado a una clave única.
        /// </summary>
        /// <param name="key">Clave que identifica el loop.</param>
        /// <param name="clip">Clip de audio a reproducir en bucle.</param>
        /// <param name="normalizedVolume">Volumen relativo (0 a 1).</param>
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

        /// <summary>
        /// Detiene y elimina un loop de audio asociado a una clave.
        /// </summary>
        /// <param name="key">Clave del loop a detener.</param>
        public void StopLoop(string key)
        {
            if (!loopSources.TryGetValue(key, out AudioSource source)) return;

            source.Stop();
            Destroy(source);
            loopSources.Remove(key);
        }
    }
}
