using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
    public class Portal : MonoBehaviour
    {
        [Header("�ε� UI")] public GameObject LoadingUI;
        [Space(10f)]
        [Tooltip("0 : �޴�, 1~3 : ���� ����, 4 : ����")]
        [Header("�̵��� �������� (0~4)")] public int nextStageIndex;
        [Tooltip("0 : ��Ʈ��, 1~2 : ���� ����, 3 : �ƿ�Ʈ��")]
        [Header("�̵��� ���� (0~3")] public int nextStageType;
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

