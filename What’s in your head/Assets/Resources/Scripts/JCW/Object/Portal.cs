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
        //[Header("로딩 UI")] public GameObject LoadingUI;
        [Space(10f)]
        //[Tooltip("0 : 메뉴, 1~3 : 실제 게임, 4 : 엔딩")]
        //[Header("이동할 스테이지 (0~4)")] public int nextStageIndex;
        //[Tooltip("0 : 인트로, 1~2 : 실제 게임, 3 : 아웃트로")]
        //[Header("이동할 섹션 (0~3)")] public int nextStageType;
        [Header("사용 키로 넘어갈 것인지")][SerializeField] bool doUseInteraction = true;
        

        bool canStart = true;
        int touchCount = 0;

        private void FixedUpdate()
        {
            if (isInteractable || !canStart || !doUseInteraction)
                return;

            Debug.Log("상호작용 키 작동");

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
                Debug.Log("트리거 감지 !");
                if (!doUseInteraction)
                    photonView.RPC(nameof(Loading), RpcTarget.AllViaServer);
                else
                {
                    touchCount = touchCount >= 2 ? 2 : touchCount + 1;
                    Debug.Log("카운트 : " +touchCount);
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
            Debug.Log("로딩 UI ON");
            canStart = true;
            //GameManager.Instance.curStageType = nextStageType;
            //GameManager.Instance.curStageIndex = nextStageIndex;
            LoadingUI.Instance.gameObject.SetActive(true);
            //LoadingUI.SetActive(true);
        }
    }
}

