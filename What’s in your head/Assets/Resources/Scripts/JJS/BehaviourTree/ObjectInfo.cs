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
        public PhotonView photonView;
        public int syncIndex;
        public bool delayEnable;
        public bool delayCheck;
        private void Awake()
        {
            prefabObject = gameObject;
            photonView = GetComponent<PhotonView>();
            syncIndex =0;
            delayEnable = false;
            delayCheck = true;
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
        }


        public void DelayCoroutin(bool enable,float delayTime)
        {
            if (enable)
            {
                delayEnable = true;
                StartCoroutine(Delay(delayTime));
            }
            else
            {
                if (delayEnable)
                {
                    delayEnable = false;
                    StopCoroutine(Delay(delayTime));
                }
            }
        }
        IEnumerator Delay(float delayTime)
        {
            float curCool = 0;
            while (curCool < delayTime)
            {
                curCool += 0.01f;
                yield return new WaitForSeconds(0.01f);
                if (curCool*10 % 10 < 0.1)
                { 
                    Debug.Log(curCool);
                }
            }
            delayEnable = false;
            yield break;
        }
    }
}
