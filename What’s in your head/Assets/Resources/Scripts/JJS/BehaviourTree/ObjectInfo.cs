using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
namespace JJS.BT
{
    [RequireComponent(typeof(PhotonView))]
    public class ObjectInfo : MonoBehaviour
    {
        GameObject prefabObject;
        public Animator animator;
        public string currentAniState;
        public PhotonView photonView;
        public int syncIndex;
        public bool localSync;
        public bool delayEnable;
        public bool delayCheck;
        private void Awake()
        {
            prefabObject = gameObject;
            photonView = GetComponent<PhotonView>();
            syncIndex =0;
            delayEnable = false;
            delayCheck = true;
            localSync = true;
            animator = GetComponent<Animator>();
            currentAniState = "";
        }
        public GameObject PrefabObject
        {
            get
            {
                return prefabObject;
            }
            set
            {
                prefabObject = value;
            }
        }
        [PunRPC]
        public void SyncCheck(int index)
        {
            syncIndex = index;
            delayCheck = true;
            localSync = true;
        }


        public void DelayCoroutine(float delayTime)
        {
            delayEnable = true;
            StartCoroutine(Delay(delayTime));
        }

        public void DelayCoroutine(float startTime, float maxTime)
        {
            float time = Random.Range(startTime, maxTime+1);
            delayEnable = true;
            StartCoroutine(Delay(time));
        }

        IEnumerator Delay(float delayTime)
        {
            float curCool = 0;
            while (curCool < delayTime)
            {
                curCool += 0.01f;
                yield return new WaitForSeconds(0.01f);
                //if (curCool*10 % 10 < 0.1)
                //{ 
                //    Debug.Log(curCool);
                //}
            }
            delayEnable = false;
            yield break;
        }
    }
}
