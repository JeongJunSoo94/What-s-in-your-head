using System.Collections;
using System.Collections.Generic;
using KSU.Object.Interaction;
using Photon.Pun;
using UnityEngine;


namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
    public class Portal : InteractableObject
    {
        [Header("�ε� UI")] public GameObject LoadingUI;
        [Space(10f)]
        [Tooltip("0 : �޴�, 1~3 : ���� ����, 4 : ����")]
        [Header("�̵��� �������� (0~4)")] public int nextStageIndex;
        [Tooltip("0 : ��Ʈ��, 1~2 : ���� ����, 3 : �ƿ�Ʈ��")]
        [Header("�̵��� ���� (0~3)")] public int nextStageType;        

        bool canStart = true;
        int touchCount = 0;

        private void FixedUpdate()
        {
            if (isInteractable || !canStart)
                return;

            canStart = false;
            if (touchCount >= 2)
                photonView.RPC(nameof(Loading), RpcTarget.AllViaServer);
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((other.CompareTag("Nella") || other.CompareTag("Steady"))
                && other.GetComponent<PlayerState>().isMine)
            {
                ++touchCount;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((other.CompareTag("Nella") || other.CompareTag("Steady"))
                && other.GetComponent<PlayerState>().isMine)
            {
                --touchCount;
                if (touchCount < 0)
                    touchCount = 0;
            }
        }

        [PunRPC]
        void Loading()
        {
            canStart = true;
            GameManager.Instance.curStageType = nextStageType;
            GameManager.Instance.curStageIndex = nextStageIndex;
            LoadingUI.SetActive(true);
        }
    }
}

