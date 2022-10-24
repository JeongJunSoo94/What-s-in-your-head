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
        [Header("��Ȱ ������ ������")] [SerializeField] [Range(0f,0.05f)] float increaseValue = 0.005f;
        [Header("��ư �Է� �� ������")] [SerializeField] [Range(0f,0.05f)] float addIncreaseValue = 0.02f;
        [Header("��ư �Է� �� ����� ����")] [SerializeField] VideoPlayer heartBeat;

        PhotonView photonView;
        bool isNella;
        GameObject curPlayer;
        Camera mainCam;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
            curPlayer = transform.parent.parent.parent.parent.gameObject;
            if(photonView.IsMine)
                mainCam = isNella ? CameraManager.Instance.cameras[0] : CameraManager.Instance.cameras[1];
        }

        private void OnEnable()
        {
            // ���⼭ ī�޶� �÷��̾� ������ ���ƾ���
            //curPlayer.GetComponent<>
            if(photonView.IsMine)
            {                
                mainCam.GetComponent<CinemachineBrain>().enabled = false;
                curPlayer.GetComponent<PlayerState>().isOutOfControl = true;
            }
            
        }

        private void OnDisable()
        {
            //���⼭ ī�޶� �÷��̾� ������ �Ѿ���.
            if (photonView.IsMine)
            {
                mainCam.GetComponent<CinemachineBrain>().enabled = true;
                curPlayer.GetComponent<PlayerState>().isOutOfControl = false;
            }
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
                Resurrect();
            }
        }

        public void Resurrect()
        {
            if (!File.Exists(Application.dataPath + "/Resources/CheckPointInfo/Stage" +
                GameManager.Instance.curStageIndex + "/Section" + GameManager.Instance.curSection + ".json"))
            {
                Debug.Log(GameManager.Instance.curSection);
                Debug.Log("üũ����Ʈ �ҷ����� ����");
                return;
            }

            string jsonString = File.ReadAllText(Application.dataPath + "/Resources/CheckPointInfo/Stage" +
                GameManager.Instance.curStageIndex + "/Section" + GameManager.Instance.curSection + ".json");

            SavePosition.PlayerInfo data = JsonUtility.FromJson<SavePosition.PlayerInfo>(jsonString);
            curPlayer.transform.SetPositionAndRotation(new Vector3((float)data.position[0], (float)data.position[1], (float)data.position[2]), new Quaternion((float)data.rotation[0], (float)data.rotation[1], (float)data.rotation[2], (float)data.rotation[3]));
        }
    }
}

