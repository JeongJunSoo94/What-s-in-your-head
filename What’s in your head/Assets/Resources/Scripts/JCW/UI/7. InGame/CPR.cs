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
        [Header("부활 게이지 이미지")] [SerializeField] Image heartGauge;
        [Header("부활 게이지 증가량")] [SerializeField] [Range(0f,0.05f)] float increaseValue;
        [Header("버튼 입력 시 증가량")] [SerializeField] [Range(0f,0.05f)] float addIncreaseValue = 0.01f;
        [Header("버튼 입력 시 재생될 비디오")] [SerializeField] VideoPlayer heartBeat;

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

