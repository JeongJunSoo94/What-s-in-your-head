using System.Collections;
using System.Collections.Generic;
using JCW.UI;
using KSU.Object.Interaction;
using Photon.Pun;
using UnityEngine;


namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
    public class Portal : InteractableObject
    {
        //[Header("�ε� UI")] public GameObject LoadingUI;
        [Space(10f)]
        //[Tooltip("0 : �޴�, 1~3 : ���� ����, 4 : ����")]
        //[Header("�̵��� �������� (0~4)")] public int nextStageIndex;
        //[Tooltip("0 : ��Ʈ��, 1~2 : ���� ����, 3 : �ƿ�Ʈ��")]
        //[Header("�̵��� ���� (0~3)")] public int nextStageType;
        [Header("��� Ű�� �Ѿ ������")][SerializeField] bool doUseInteraction = true;
        

        bool canStart = true;
        int touchCount = 0;

        private void FixedUpdate()
        {
            if (isInteractable || !canStart || !doUseInteraction)
                return;

            Debug.Log("��ȣ�ۿ� Ű �۵�");

            canStart = false;
            if (touchCount >= 2)
                photonView.RPC(nameof(Loading), RpcTarget.AllViaServer);
            else
                canStart = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                Debug.Log("Ʈ���� ���� !");
                if (!doUseInteraction)
                    photonView.RPC(nameof(Loading), RpcTarget.AllViaServer);
                else
                {
                    touchCount = touchCount >= 2 ? 2 : touchCount + 1;
                    Debug.Log("ī��Ʈ : " +touchCount);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((other.CompareTag("Nella") || other.CompareTag("Steady"))
                && other.GetComponent<PlayerState>().isMine)
            {
                if (doUseInteraction)
                    touchCount = touchCount < 0 ? 0 : touchCount - 1;
            }
        }

        [PunRPC]
        void Loading()
        {
            Debug.Log("�ε� UI ON");
            canStart = true;
            //GameManager.Instance.curStageType = nextStageType;
            //GameManager.Instance.curStageIndex = nextStageIndex;
            LoadingUI.Instance.gameObject.SetActive(true);
            //LoadingUI.SetActive(true);
        }
    }
}

