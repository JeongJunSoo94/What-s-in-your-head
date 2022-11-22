using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Object;
using JCW.AudioCtrl;
namespace JJS
{ 
    public class Singing : MonoBehaviour
    {
        [HideInInspector] public Dictionary<int, RecieverMic> MicDictionary = new Dictionary<int, RecieverMic>();
        [HideInInspector] public Queue<int> indexQueue = new();

        public GameObject effect;
        AudioSource audioSource;
        public bool playSound;

        int audioID = 0;
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioID = JCW.AudioCtrl.AudioSettings.SetAudio(audioSource, 1f, 60f, true);
            playSound = false;
        }

        private void Update()
        {
            Play(true);
        }

        private void OnDisable()
        {
            Play(false);
        }

        public void Play(bool enable)
        {
            foreach (var mic in MicDictionary)
            {
                mic.Value.Play(enable);
            }

            if (!playSound)
            {
                audioSource.loop = true;
                playSound = true;
                SoundManager.Instance.PlayIndirect3D_RPC("S2_NellaSing", audioID);
            }
            if(!enable)
            {
                playSound = false;
                SoundManager.Instance.Stop3D_RPC(audioID);
            }


            effect?.SetActive(enable);
        }

        public int GetMicIndex()
        {
            if (indexQueue.Count == 0)
                return MicDictionary.Count;
            else
                return indexQueue.Dequeue();
        }

        public void InitSinging()
        {
            Play(false);
            MicDictionary.Clear();
        }

    }
}
