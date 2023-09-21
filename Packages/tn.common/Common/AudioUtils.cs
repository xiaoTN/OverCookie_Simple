using UnityEngine;

namespace TN.Common
{
    public static class AudioUtils
    {
        public static AudioSource AddAudioSource3D(GameObject go)
        {
            float audioSourceSpatialBlend = 1f;
            return AddAudioSource(go, audioSourceSpatialBlend);
        }

        public static AudioSource AddAudioSource(GameObject go, float audioSourceSpatialBlend)
        {
            AudioSource audioSource = go.AddComponent<AudioSource>();

            // audioSource.maxDistance = 70;
            audioSource.spatialBlend = audioSourceSpatialBlend;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            return audioSource;
        }

        public static AudioSource AddAudioSource3D(Component component)
        {
            return AddAudioSource3D(component.gameObject);
        }

        public static AudioSource AddAudioSource2D(GameObject go)
        {
            return AddAudioSource(go, 0);
        }

        public static AudioSource AddAudioSource2D(Component component)
        {
            return AddAudioSource2D(component.gameObject);
        }
    }
}