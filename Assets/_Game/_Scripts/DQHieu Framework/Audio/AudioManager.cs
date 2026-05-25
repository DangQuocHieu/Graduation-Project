namespace DQHieu.Framework.Audio
{
    using UnityEngine;
    using System.Collections.Generic;
    using DG.Tweening;
    using JetBrains.Annotations;

    public class AudioManager : PersistentSingleton<AudioManager>
    {
        [SerializeField] private int _initialPoolSize = 15;
        private Queue<AudioEmitter> _sfxPool = new();
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioEmitter _audioEmitterPrefab;

        [SerializeField] private AudioData _buttonClickSound;
        [SerializeField] private UnityEngine.Audio.AudioMixer _mainMixer;

        protected override void Awake()
        {
            base.Awake();
            InitializeSystem();
        }

        private void Start()
        {
            if (DataManager.Instance != null && DataManager.Instance.settingData != null)
            {
                ToggleMusic(DataManager.Instance.settingData.isMusicOn);
                ToggleSFX(DataManager.Instance.settingData.isSfxOn);
            }
        }

        private void InitializeSystem()
        {
            if (_bgmSource != null)
            {
                _bgmSource.loop = true;
                _bgmSource.spatialBlend = 0f;
                _bgmSource.playOnAwake = false;

            }

            for(int i = 0; i < _initialPoolSize; i++)
            {
                CreateNewPoolItem();
            }
        }

        private AudioEmitter CreateNewPoolItem()
        {
            AudioEmitter emitter = Instantiate(_audioEmitterPrefab, transform);
            emitter.gameObject.SetActive(false);
            _sfxPool.Enqueue(emitter);
            return emitter;
        }

        public void StopSFX(AudioEmitter emitter, float fadeDuration = 0f)
        {
            emitter.Stop(fadeDuration);
        }

        public AudioEmitter PlaySFX(AudioData data, Vector3 position, float fadeInDuration = 0f)
        {
            AudioEmitter emitter;
            if(_sfxPool.Count > 0)
            {
                emitter = _sfxPool.Dequeue();
            }
            else
            {
                emitter = CreateNewPoolItem();
                _sfxPool.Dequeue();
            }
            emitter.gameObject.SetActive(true);
            emitter.Play(data, position, ReturnItemToPool, fadeInDuration);
            return emitter;
        }

        public AudioEmitter PlaySFX(AudioData data, Transform parent, float fadeInDuration = 0f)
        {
            AudioEmitter emitter = PlaySFX(data, parent.position, fadeInDuration);
            emitter.transform.SetParent(parent);
            return emitter;
        }

        public AudioEmitter PlaySFX(AudioData data, float fadeInDuration = 0f)
        {
            return PlaySFX(data, Vector3.zero, fadeInDuration);
        }

        private void ReturnItemToPool(AudioEmitter emitter)
        {
            emitter.transform.SetParent(transform);
            emitter.gameObject.SetActive(false);
            _sfxPool.Enqueue(emitter);
        }


        public void PlayBGM(AudioData data, float fadeDuration = 1f)
        {
            if (data == null || data.Clip == null) return;
            if (_bgmSource.isPlaying)
            {
                _bgmSource.DOFade(0f, fadeDuration).OnComplete(() =>
                {
                    StartNewBGM(data, fadeDuration);
                });
            }
            else
            {
                StartNewBGM(data, fadeDuration);
            }
        }

        private void StartNewBGM(AudioData data, float fadeDuration)
        {
            _bgmSource.clip = data.Clip;
            _bgmSource.outputAudioMixerGroup = data.MixerGroup;
            _bgmSource.pitch = data.Pitch;
            _bgmSource.volume = 0f;
            _bgmSource.Play();
            _bgmSource.DOFade(data.Volume, fadeDuration);
        }

        public void StopBGM(float fadeDuration = 1f)
        {
            // Fade out nhạc nền từ từ trước khi dừng hẳn
            _bgmSource.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                _bgmSource.Stop();
            });
        }

        public void PlayButtonClickSound()
        {
            PlaySFX(_buttonClickSound);
        }

        public void ToggleMusic(bool isOn)
        {
            if (_mainMixer != null)
            {
                // Sử dụng AudioMixer để mute, -80dB là mute hoàn toàn, 0dB là mức bình thường
                _mainMixer.SetFloat("MusicVolume", isOn ? 0f : -80f);
            }
        }

        public void ToggleSFX(bool isOn)
        {
            if (_mainMixer != null)
            {
                _mainMixer.SetFloat("SFXVolume", isOn ? 0f : -80f);
            }
        }
    }

}