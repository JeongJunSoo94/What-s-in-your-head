using Cinemachine;
using JCW.Object;
using JCW.UI.Options.InputBindings;
using Photon.Pun;
using System.IO;
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
        [Header("��Ȱ ������ ������")] [SerializeField] [Range(0f, 0.05f)] float increaseValue = 0.005f;
        [Header("��ư �Է� �� ������")] [SerializeField] [Range(0f, 0.05f)] float addIncreaseValue = 0.02f;
        [Header("��ư �Է� �� ����� ����")] [SerializeField] VideoPlayer heartBeat;

        PhotonView photonView;
        bool isNella;
        Transform curPlayer;
        Camera mainCam;

        Vector3 originalPos;

        RectTransform heartRect;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
            curPlayer = GameManager.Instance.myPlayerTF;
            if (photonView.IsMine)
                mainCam = isNella ? CameraManager.Instance.cameras[0] : CameraManager.Instance.cameras[1];
            originalPos = transform.GetChild(2).gameObject.GetComponent<RectTransform>().position;
            heartRect = transform.GetChild(2).gameObject.GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if (photonView.IsMine)
            {
                mainCam.GetComponent<CinemachineBrain>().enabled = false;
                curPlayer.GetComponent<PlayerState>().isOutOfControl = true;
            }
            if (GameManager.Instance.isTopView)
            {
                // ���� ���� ȭ�� ���α�
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);

                // 
                Vector3 pos = originalPos;
                if (photonView.IsMine)
                    pos.x = isNella ? pos.x - 100f : pos.x + 100f;
                else
                    pos.x = isNella ? pos.x + 100f : pos.x - 100f;
                pos.y -= 300f;
                heartRect.position = pos;
                heartRect.localScale *= 0.7f;
            }
            // ������ UI
            transform.parent.parent.parent.GetChild(0).gameObject.SetActive(false);

        }

        private void OnDisable()
        {
            //���⼭ ī�޶� �÷��̾� ������ �Ѿ���.
            if (photonView.IsMine)
            {
                if(mainCam)
                    mainCam.GetComponent<CinemachineBrain>().enabled = true;
                if(curPlayer)
                    curPlayer.GetComponent<PlayerState>().isOutOfControl = false;
            }

            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);

            transform.GetChild(2).gameObject.GetComponent<RectTransform>().position = originalPos;
            transform.GetChild(2).gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            heartRect.anchoredPosition = new Vector2(0, heartRect.anchoredPosition.y);

            // ������ UI
            transform.parent.parent.parent.GetChild(0).gameObject.SetActive(true);
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
                heartGauge.fillAmount = 0f;
                GameManager.Instance.MediateRevive(false);
                //if(!GameManager.Instance.isTopView && photonView.IsMine)
                //    CameraManager.Instance.ReviveCam(isNella);
                curPlayer.GetComponent<Animator>().SetBool("isDead", false);
            }
        }
    }
}

