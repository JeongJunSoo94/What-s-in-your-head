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
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
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
                playSound = true;
                SoundManager.Instance.Play3D_RPC("S2_NellaSing", audioSource);
            }
            if(!enable)
            {
                playSound = false;
                SoundManager.Instance.Stop3D_RPC(audioSource);
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
