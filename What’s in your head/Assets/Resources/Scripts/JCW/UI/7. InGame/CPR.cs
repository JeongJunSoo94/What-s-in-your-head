using JCW.UI.Options.InputBindings;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using YC.CameraManager_;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class CPR : MonoBehaviour
    {
        [Header("��Ȱ ������ �̹���")] [SerializeField] Image heartGauge;
        [Header("��Ȱ ������ ������")] [SerializeField] [Range(0f,0.05f)] float increaseValue = 0.005f;
        [Header("��ư �Է� �� ������")] [SerializeField] [Range(0f,0.05f)] float addIncreaseValue = 0.02f;
        [Header("��ư �Է� �� ����� ����")] [SerializeField] VideoPlayer heartBeat;

        PhotonView photonView;
        bool isNella;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
        }


        void Update()
        {
            if (!photonView.IsMine)
                return;
            photonView.RPC(nameof(IncreaseValue), RpcTarget.AllViaServer, increaseValue * Time.deltaTime, isNella, false);
            if (KeyManager.Instance.GetKeyDown(PlayerAction.Interaction))
                photonView.RPC(nameof(IncreaseValue), RpcTarget.AllViaServer, (float)addIncreaseValue, isNella, true);
        }

        [PunRPC]
        void IncreaseValue(float value, bool isNella, bool isPress)
        {
            heartGauge.fillAmount += value;
            if (isPress && !heartBeat.isPlaying)
                heartBeat.Play();
            if (heartGauge.fillAmount >= 1f)
            {
                GameManager.Instance.SetAliveState(isNella, true);
                heartGauge.fillAmount = 0f;
                GameManager.Instance.MediateRevive(false);
                CameraManager.Instance.ReviveCam();
            }
        }
    }
}

