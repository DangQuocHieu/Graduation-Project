namespace DQHieu.Framework.Audio
{
    using System;
    using System.Collections;
    using UnityEngine;
    using DG.Tweening;
    [RequireComponent(typeof(AudioEmitter))]
    public class AudioEmitter : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        private Action<AudioEmitter> _returnToPoolCallback;

        private void Awake()
        {
            if(_audioSource != null)
            {
                _audioSource = GetComponent<AudioSource>();
            }
            _audioSource.playOnAwake = false;
        }

        public void Play(AudioData data, Vector3 position, Action<AudioEmitter> callback, float fadeInDuration = 0f)
        {
            _audioSource.DOKill();
            _returnToPoolCallback = callback;
            transform.position = position;
            _audioSource.clip = data.Clip;
            _audioSource.outputAudioMixerGroup = data.MixerGroup;
            
            _audioSource.pitch = data.Pitch;
            _audioSource.loop = data.Loop;

            _audioSource.spatialBlend = data.spatialBlend;
            _audioSource.minDistance = data.MinDistance;
            _audioSource.maxDistance = data.MaxDistance;
            _audioSource.rolloffMode = data.RolloffMode;

            if (fadeInDuration > 0f)
            {
                _audioSource.volume = 0f;
                _audioSource.DOFade(data.Volume, fadeInDuration);
            }
            else
            {
                _audioSource.volume = data.Volume;
            }

            _audioSource.Play();
            if (!data.Loop)
            {
                float duration = data.Clip.length / Mathf.Abs(data.Pitch != 0 ? data.Pitch : 1f);
                StartCoroutine(WaitAndReturn(duration));
            }

        }

        public void Stop(float fadeDuration = 0f)
        {
            StopAllCoroutines();
            _audioSource.DOKill();

            if (fadeDuration > 0f)
            {
                _audioSource.DOFade(0f, fadeDuration).OnComplete(() =>
                {
                    _audioSource.Stop();
                    _returnToPoolCallback?.Invoke(this);
                });
            }
            else
            {
                _audioSource.Stop();
                _returnToPoolCallback?.Invoke(this);
            }
        }

        private IEnumerator WaitAndReturn(float delay)
        {
            yield return new WaitForSeconds(delay);
            _returnToPoolCallback?.Invoke(this);
        }

    }

}