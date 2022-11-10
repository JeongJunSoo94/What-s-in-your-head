using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object.Stage1
{
    public class FlagCheck : MonoBehaviour
    {
        [Header("열릴 오브젝트의 BothButton 스크립트")] [SerializeField] BothButton bothButton;
        [Header("빛을 낼 오브젝트")] [SerializeField] Transform lightButton;
        [Header("초기화 할 시간")] [SerializeField] [Range(0,50)] float resetTime = 5f;

        PhotonView photonView;
        bool isStart = false;

        WaitForSeconds ws;

        private void Awake()
        {
            photonView = PhotonView.Get(this);
            ws = new(resetTime);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                if (other.GetComponent<PlayerState>().isMine && !isStart)
                {
                    isStart = true;
                    photonView.RPC(nameof(SetCount), RpcTarget.AllViaServer, true);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                if (other.GetComponent<PlayerState>().isMine && isStart)
                {
                    photonView.RPC(nameof(SetCount), RpcTarget.AllViaServer, false);
                }
            }
        }

        IEnumerator Reset()
        {            
            yield return ws;
            isStart = false;
            GetComponent<Animator>().SetBool("isCheck", false);
            lightButton.GetChild(0).gameObject.SetActive(true);
            lightButton.GetChild(1).gameObject.SetActive(false);
            bothButton.SetBothCount(bothButton.bothCount - 1);
        }

        [PunRPC]
        void SetCount(bool isOn)
        {
            if (!isOn)
                StartCoroutine(nameof(Reset));
            else
            {
                isStart = true;
                StopAllCoroutines();
                bothButton.SetBothCount(bothButton.bothCount + 1);
                lightButton.GetChild(0).gameObject.SetActive(false);
                lightButton.GetChild(1).gameObject.SetActive(true);
                GetComponent<Animator>().SetBool("isCheck", true);
            }
            
        }
    }
}

