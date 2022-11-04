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

        private void Awake()
        {
            prefabObject = gameObject;
            photonView = GetComponent<PhotonView>();
            syncIndex =0;
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
    }
}
