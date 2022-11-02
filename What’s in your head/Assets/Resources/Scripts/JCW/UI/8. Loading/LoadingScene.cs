using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace JCW.UI
{
    [RequireComponent(typeof(PhotonView))]
    public class LoadingScene : MonoBehaviour, IPunObservable
    {
        PhotonView photonView;

        float curLoadingValue = 0f;
        bool isMaster = false;
        bool isLoading = false;
        Image image;

        

        private void Awake()
        {
            photonView = PhotonView.Get(this);
            image = GetComponent<Image>();
            isMaster = PhotonNetwork.IsMasterClient;
        }

        void Update()
        {
            if(isMaster && !isLoading)
                PhotonNetwork.LoadLevel(++GameManager.Instance.curStageIndex);
            isLoading = true;
            curLoadingValue = PhotonNetwork.LevelLoadingProgress;
            image.fillAmount = curLoadingValue;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(curLoadingValue);
            }
            else
            {
                curLoadingValue = (float)stream.ReceiveNext();
            }
        }
    }
}

