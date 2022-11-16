using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
    public class Portal : MonoBehaviour
    {
        [Header("로딩 UI")] public GameObject LoadingUI;
        [Space(10f)]
        [Tooltip("0 : 메뉴, 1~3 : 실제 게임, 4 : 엔딩")]
        [Header("이동할 스테이지 (0~4)")] public int nextStageIndex;
        [Tooltip("0 : 인트로, 1~2 : 실제 게임, 3 : 아웃트로")]
        [Header("이동할 섹션 (0~3")] public int nextStageType;
        PhotonView photonView;

        bool isStart = false;

        private void Awake()
        {
            photonView = PhotonView.Get(this);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                if(other.GetComponent<PlayerState>().isMine && !isStart)
                {
                    isStart = true;
                    photonView.RPC(nameof(Loading), RpcTarget.AllViaServer);
                }
            }
        }

        [PunRPC]
        void Loading()
        {
            isStart = true;
            GameManager.Instance.curStageType = nextStageType;
            GameManager.Instance.curStageIndex = nextStageIndex;
            LoadingUI.SetActive(true);
        }
    }
}

