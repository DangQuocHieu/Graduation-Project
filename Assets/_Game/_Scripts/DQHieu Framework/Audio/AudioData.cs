namespace DQHieu.Framework.Audio
{
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.Audio;

    [CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/DQHieu/AudioData")]
    public class AudioData : ScriptableObject
    {
        public AudioClip Clip;
        public AudioMixerGroup MixerGroup;
        [Range(0f, 1f)]
        public float Volume = 1f;
        [Range(-3f, 3f)]
        public float Pitch = 1f;
        public bool Loop = false;
        [Range(0f, 1f)] public float spatialBlend = 0f;
        public float MinDistance = 1f;
        public float MaxDistance = 50f;
        public AudioRolloffMode RolloffMode = AudioRolloffMode.Linear;
    }

}