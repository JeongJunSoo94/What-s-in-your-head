using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
namespace JJS.BT
{
    [RequireComponent(typeof(PhotonView))]
    public class ObjectInfo : MonoBehaviour, IPunObservable
    {
        GameObject prefabObject;
        public PhotonView photonView;
        public int syncIndex;
        public bool delayEnable;
        private void Awake()
        {
            prefabObject = gameObject;
            photonView = GetComponent<PhotonView>();
            syncIndex =0;
            delayEnable = false;
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
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(syncIndex);
            }
            else
            {
                syncIndex = (int)stream.ReceiveNext();
            }
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
                }
                else
                {
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
            }
            delayEnable = false;
            yield break;
        }
    }
}
