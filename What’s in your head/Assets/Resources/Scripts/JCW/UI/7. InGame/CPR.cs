using Cinemachine;
using JCW.Object;
using JCW.UI.Options.InputBindings;
using Photon.Pun;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using YC.YC_CameraManager;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class CPR : MonoBehaviour
    {
        [Header("부활 게이지 이미지")] [SerializeField] Image heartGauge;
        [Header("부활 게이지 증가량")] [SerializeField] [Range(0f, 0.05f)] float increaseValue = 0.005f;
        [Header("버튼 입력 시 증가량")] [SerializeField] [Range(0f, 0.05f)] float addIncreaseValue = 0.02f;
        [Header("버튼 입력 시 재생될 비디오")] [SerializeField] VideoPlayer heartBeat;

        PhotonView photonView;
        bool isNella;
        Transform curPlayer;
        PlayerState curPlayerState;

        Camera mainCam;
        CinemachineBrain mainCamCine;

        Vector3 originalPos;

        RectTransform heartRect;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
            curPlayer = GameManager.Instance.myPlayerTF;
            curPlayerState = curPlayer.GetComponent<PlayerState>();
            if (photonView.IsMine)
            {
                mainCam = isNella ? CameraManager.Instance.cameras[0] : CameraManager.Instance.cameras[1];
                mainCamCine = mainCam.GetComponent<CinemachineBrain>();
            }
            originalPos = transform.GetChild(2).gameObject.GetComponent<RectTransform>().position;
            heartRect = transform.GetChild(2).gameObject.GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if (photonView.IsMine)
            {
                // 카메라 움직임과 플레이어 움직임 막기
                mainCamCine.enabled = false;
                curPlayerState.isOutOfControl = true;
            }
            if (GameManager.Instance.isTopView)
            {
                // 블러와 검은 화면 꺼두기
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
            // 아이템 UI
            transform.parent.parent.parent.GetChild(0).gameObject.SetActive(false);

        }

        private void OnDisable()
        {
            //여기서 카메라나 플레이어 움직임 켜야함.
            if (photonView.IsMine)
            {
                // 카메라 움직임과 플레이어 움직임 정상화
                mainCamCine.enabled = false;
                curPlayerState.isOutOfControl = true;
            }

            // 블러와 검은 화면 켜기
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);

            transform.GetChild(2).gameObject.GetComponent<RectTransform>().position = originalPos;
            transform.GetChild(2).gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            heartRect.anchoredPosition = new Vector2(0, heartRect.anchoredPosition.y);

            // 아이템 UI
            transform.parent.parent.parent.GetChild(0).gameObject.SetActive(true);
        }


        void Update()
        {
            if (!photonView.IsMine)
                return;
            photonView.RPC(nameof(IncreaseValue), RpcTarget.AllViaServer, increaseValue * Time.deltaTime, false);
            if (KeyManager.Instance.GetKeyDown(PlayerAction.Interaction))
                photonView.RPC(nameof(IncreaseValue), RpcTarget.AllViaServer, (float)addIncreaseValue, true);
        }

        [PunRPC]
        void IncreaseValue(float value, bool isPress)
        {
            heartGauge.fillAmount += value;
            if (isPress && !heartBeat.isPlaying)
                heartBeat.Play();
            if (heartGauge.fillAmount >= 1f)
            {
                heartGauge.fillAmount = 0f;
                GameManager.Instance.MediateRevive(false);
                curPlayer.GetComponent<Animator>().SetBool("isDead", false);
            }
        }
    }
}

