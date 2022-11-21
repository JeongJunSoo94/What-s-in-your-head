using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using JCW.AudioCtrl;

namespace KSU.AutoAim.Object.Monster
{
    //[RequireComponent(typeof(AudioSource))]
    public class PoisonSnake : DefenseMonster
    {
        // Update is called once per frame
        //AudioSource audioSource;
        //private void Awake()
        //{
        //    //audioSource = GetComponent<AudioSource>();
        //    //JCW.AudioCtrl.AudioSettings.SetAudio(audioSource, 1, 50f);            
        //}

        void Update()
        {
            Detect();
        }

        private void OnEnable()
        {
            InitMonster();
        }

        //public void PlayAttackSound()
        //{
        //    SoundManager.Instance.Play3D_RPC("Snake_Attack", audioSource);
        //}
    }
}
