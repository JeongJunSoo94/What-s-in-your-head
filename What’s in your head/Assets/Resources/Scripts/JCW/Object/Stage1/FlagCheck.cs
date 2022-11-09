using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object.Stage1
{
    public class FlagCheck : MonoBehaviour
    {
        [Header("���� ������Ʈ�� BothButton ��ũ��Ʈ")] [SerializeField] BothButton bothButton;
        [Header("���� �� ������Ʈ")] [SerializeField] Transform lightButton;
        [Header("�ʱ�ȭ �� �ð�")] [SerializeField] [Range(0,50)] float resetTime = 5f;

        PhotonView photonView;

        private void Awake()
        {
            photonView = PhotonView.Get(this);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                if (!photonView.IsMine)
                    return;
                photonView.RPC(nameof(SetCount), RpcTarget.AllViaServer, true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                if (!photonView.IsMine)
                    return;                
                photonView.RPC(nameof(SetCount), RpcTarget.AllViaServer, false);
            }
        }

        IEnumerator Reset()
        {
            float curTime = 0f;
            while(curTime < resetTime)
            {
                curTime += Time.deltaTime;
                yield return null;
            }
            GetComponent<Animator>().SetBool("isCheck", false);
            lightButton.GetChild(0).gameObject.SetActive(true);
            lightButton.GetChild(1).gameObject.SetActive(false);
            bothButton.SetBothCount(bothButton.bothCount - 1);
        }

        [PunRPC]
        void SetCount(bool isOn)
        {
            StopAllCoroutines();
            if(!isOn)
                StartCoroutine(nameof(Reset));
            else
            {
                bothButton.SetBothCount(bothButton.bothCount + 1);
                lightButton.GetChild(0).gameObject.SetActive(!isOn);
                lightButton.GetChild(1).gameObject.SetActive(isOn);
                GetComponent<Animator>().SetBool("isCheck", isOn);
            }
            
        }
    }
}

