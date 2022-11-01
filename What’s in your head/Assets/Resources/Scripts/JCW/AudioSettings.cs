using UnityEngine;

namespace JCW.AudioCtrl
{
    public class AudioSettings
    {
        public static AudioSource SetAudio(AudioSource audioSource, float volume = 1f, float maxDist = 120f)
        {
            audioSource.rolloffMode = AudioRolloffMode.Custom;
            audioSource.spatialBlend = 1f;
            audioSource.playOnAwake = false;
            audioSource.maxDistance = maxDist;
            audioSource.dopplerLevel = 0f;
            return audioSource;
        }
    }
}

