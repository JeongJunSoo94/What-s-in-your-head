using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Object;
namespace JJS
{ 
    public class Singing : MonoBehaviour
    {
        [HideInInspector] public Dictionary<int, RecieverMic> MicDictionary = new Dictionary<int, RecieverMic>();
        [HideInInspector] public Queue<int> indexQueue = new();



        private void Update()
        {
            Play(true);
            if (GameManager.Instance.curPlayerHP == 0)
            {
                Play(false);
                MicDictionary.Clear();
            }
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
        }

        public int GetMicIndex()
        {
            if (indexQueue.Count == 0)
                return MicDictionary.Count;
            else
                return indexQueue.Dequeue();
        }

    }
}
