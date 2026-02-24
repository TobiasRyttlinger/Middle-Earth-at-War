using BFME2.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace BFME2.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Mixer")]
        [SerializeField] private AudioMixerGroup _musicGroup;
        [SerializeField] private AudioMixerGroup _sfxGroup;
        [SerializeField] private AudioMixerGroup _uiGroup;
        [SerializeField] private AudioMixerGroup _voiceGroup;

        [Header("Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _voiceSource;
        [SerializeField] private int _sfxPoolSize = 20;

        private AudioSource[] _sfxPool;
        private int _sfxPoolIndex;

        private void Awake()
        {
            ServiceLocator.Register(this);

            // Create music source if not assigned
            if (_musicSource == null)
            {
                _musicSource = gameObject.AddComponent<AudioSource>();
                _musicSource.loop = true;
                _musicSource.playOnAwake = false;
                if (_musicGroup != null) _musicSource.outputAudioMixerGroup = _musicGroup;
            }

            // Create voice source if not assigned
            if (_voiceSource == null)
            {
                _voiceSource = gameObject.AddComponent<AudioSource>();
                _voiceSource.loop = false;
                _voiceSource.playOnAwake = false;
                if (_voiceGroup != null) _voiceSource.outputAudioMixerGroup = _voiceGroup;
            }

            // Create SFX pool
            _sfxPool = new AudioSource[_sfxPoolSize];
            for (int i = 0; i < _sfxPoolSize; i++)
            {
                var go = new GameObject($"SFX_Source_{i}");
                go.transform.SetParent(transform);
                _sfxPool[i] = go.AddComponent<AudioSource>();
                _sfxPool[i].playOnAwake = false;
                _sfxPool[i].spatialBlend = 1f; // 3D sound
                if (_sfxGroup != null) _sfxPool[i].outputAudioMixerGroup = _sfxGroup;
            }
        }

        public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return;

            var source = GetNextSFXSource();
            source.transform.position = position;
            source.clip = clip;
            source.volume = volume;
            source.spatialBlend = 1f;
            source.Play();
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null || _musicSource == null) return;

            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        public void StopMusic(float fadeTime = 1f)
        {
            if (_musicSource == null) return;

            if (fadeTime <= 0)
            {
                _musicSource.Stop();
            }
            else
            {
                // Simple fade — in production, use a coroutine or DOTween
                _musicSource.Stop();
            }
        }

        public void PlayUISound(AudioClip clip)
        {
            if (clip == null) return;

            var source = GetNextSFXSource();
            source.spatialBlend = 0f; // 2D sound for UI
            source.clip = clip;
            source.volume = 1f;
            source.Play();
        }

        public void PlayVoiceLine(AudioClip clip)
        {
            if (clip == null || _voiceSource == null) return;

            // Voice lines interrupt each other
            _voiceSource.Stop();
            _voiceSource.clip = clip;
            _voiceSource.Play();
        }

        public void SetMixerVolume(AudioMixerGroup group, float volume)
        {
            if (group?.audioMixer == null) return;

            // Convert linear 0-1 to decibels (-80 to 0)
            float db = volume > 0.001f ? Mathf.Log10(volume) * 20f : -80f;
            group.audioMixer.SetFloat(group.name + "Volume", db);
        }

        private AudioSource GetNextSFXSource()
        {
            var source = _sfxPool[_sfxPoolIndex];
            _sfxPoolIndex = (_sfxPoolIndex + 1) % _sfxPool.Length;
            return source;
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<AudioManager>();
        }
    }
}
