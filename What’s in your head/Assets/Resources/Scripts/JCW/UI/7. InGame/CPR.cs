using JCW.UI.Options.InputBindings;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class CPR : MonoBehaviour
    {
        [Header("��Ȱ ������ �̹���")] [SerializeField] Image heartGauge;
        [Header("��Ȱ ������ ������")] [SerializeField] [Range(0f,0.05f)] float increaseValue;
        [Header("��ư �Է� �� ������")] [SerializeField] [Range(0f,0.05f)] float addIncreaseValue = 0.01f;
        [Header("��ư �Է� �� ����� ����")] [SerializeField] VideoPlayer heartBeat;

        PhotonView photonView;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
        }


        void Update()
        {
            photonView.RPC(nameof(IncreaseValue), RpcTarget.AllViaServer, increaseValue * Time.deltaTime);
            if (KeyManager.Instance.GetKeyDown(PlayerAction.Interaction))
                photonView.RPC(nameof(IncreaseValue), RpcTarget.AllViaServer, addIncreaseValue, GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient]);
        }

        [PunRPC]
        void IncreaseValue(float value, bool isNella)
        {
            heartGauge.fillAmount += value;
            if (!heartBeat.isPlaying)
                heartBeat.Play();
            if (heartGauge.fillAmount >= 1f)
            {
                GameManager.Instance.isAlive[isNella] = true;
                heartGauge.fillAmount = 0f;
                //transform.parent.gameObject.SetActive(false);
                GameManager.Instance.MediateRevive(false);
            }
        }
    }
}

