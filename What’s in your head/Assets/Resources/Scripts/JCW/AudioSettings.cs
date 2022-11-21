using UnityEngine;

namespace JCW.AudioCtrl
{
    public class AudioSettings
    {
        public static void SetAudio(AudioSource audioSource, float volume = 1f, float maxDist = 120f, bool isLoop = false)
        {
            audioSource.rolloffMode = AudioRolloffMode.Custom;
            audioSource.spatialBlend = 1f;
            audioSource.playOnAwake = false;
            audioSource.maxDistance = maxDist;
            audioSource.loop = isLoop;
            audioSource.dopplerLevel = 0f;
            audioSource.volume = volume * SoundManager.Instance.audioSources[(int)Sound.DISTANCE].volume;
        }
    }
}

