using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Object;
using JCW.AudioCtrl;
using Photon.Pun;

namespace JJS
{
    [RequireComponent(typeof(PhotonView))]
    public class Singing : MonoBehaviour
    {
        [HideInInspector] public Dictionary<int, RecieverMic> MicDictionary = new Dictionary<int, RecieverMic>();
        [HideInInspector] public Queue<int> indexQueue = new();

        public GameObject effect;
        AudioSource audioSource;
        public bool playSound;
        PhotonView pv;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            pv = GetComponent<PhotonView>();
            SoundManager.Set3DAudio(pv.ViewID, audioSource, 1f, 60f, true);
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
                SoundManager.Instance.PlayIndirect3D_RPC("S2_NellaSing", pv.ViewID);
            }
            if (!enable)
            {
                playSound = false;
                SoundManager.Instance.Stop3D_RPC(pv.ViewID);
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