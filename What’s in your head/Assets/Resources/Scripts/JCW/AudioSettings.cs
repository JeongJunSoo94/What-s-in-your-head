using Photon.Pun;
using UnityEngine;
using System;
using System.Collections;

//namespace JCW.AudioCtrl
//{
//    [RequireComponent(typeof(PhotonView))]
//    public class AudioSettings : MonoBehaviour
//    {
//        PhotonView pv;
//        static AudioSource audioSource_temp;
//
//        private void Awake()
//        {
//            pv = PhotonView.Get(this);
//        }
//
//        public int SetAudio(AudioSource audioSource, float volume = 1f, float maxDist = 120f, bool isLoop = false)
//        {
//            audioSource.rolloffMode = AudioRolloffMode.Custom;
//            audioSource.spatialBlend = 1f;
//            audioSource.playOnAwake = false;
//            audioSource.maxDistance = maxDist;
//            audioSource.loop = isLoop;
//            audioSource.dopplerLevel = 0f;
//            audioSource.volume = volume * SoundManager.Instance.audioSources[(int)Sound.DISTANCE].volume;
//            SoundManager.Instance.dict3D.Add(SoundManager.Instance.dict3D.Count, audioSource);
//            //if (pv == null)
//            //    pv = PhotonView.Get(Instance);
//            //if (audioSource_temp == null)
//            //{
//            //    audioSource_temp = audioSource;
//            //    if (pv.IsMine)
//            //        pv.RPC(nameof(AddDict3D), RpcTarget.AllViaServer);
//            //}
//
//            return SoundManager.Instance.dict3D.Count -1;
//        }
//
//        [PunRPC]
//        void AddDict3D()
//        {            
//            StartCoroutine(nameof(WaitForPlayer));
//        }
//
//        IEnumerator WaitForPlayer()
//        {
//            yield return new WaitUntil(() => GameManager.Instance.GetCharOnScene(true) && GameManager.Instance.GetCharOnScene(false));
//
//            SoundManager.Instance.dict3D.Add(SoundManager.Instance.dict3D.Count, audioSource_temp);
//            audioSource_temp = null;
//            yield break;
//        }
//    }
//}

