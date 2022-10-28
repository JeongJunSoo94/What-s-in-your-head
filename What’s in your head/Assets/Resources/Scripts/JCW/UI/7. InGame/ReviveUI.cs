using Photon.Pun;
using UnityEngine;
using YC.CameraManager_;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class ReviveUI : MonoBehaviour
    {
        bool isNella;
        PhotonView photonView;
        RectTransform deadUI_Rect;
        RectTransform aliveUI_Rect;

        bool isStop = false;

        void Awake()
        {           
            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
            photonView = GetComponent<PhotonView>();
            deadUI_Rect = transform.GetChild(0).GetComponent<RectTransform>();
            aliveUI_Rect = transform.GetChild(1).GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if(photonView.IsMine)
                photonView.RPC(nameof(TurnOnUI_RPC), RpcTarget.AllViaServer, (bool)GameManager.Instance.isAlive[isNella]);
            isStop = false;
        }

        private void Update()
        {
            //SetScreenSize(isNella);
            if (photonView.IsMine && !isStop)
            {
                photonView.RPC(nameof(SetScreenSize), RpcTarget.AllViaServer, isNella);                
            }
        }
        [PunRPC]
        private void TurnOnUI_RPC(bool _isAlive)
        {
            if (_isAlive)
            {
                if(!GameManager.Instance.isTopView)
                    aliveUI_Rect.gameObject.SetActive(true);
                deadUI_Rect.gameObject.SetActive(false);
            }
            else
            {
                deadUI_Rect.gameObject.SetActive(true);
                aliveUI_Rect.gameObject.SetActive(false);
            }
        }

        [PunRPC]
        void SetScreenSize(bool _isNella)
        {
            int index = _isNella ? 0 : 1;
            deadUI_Rect.sizeDelta = new Vector2(CameraManager.Instance.cameras[index].rect.width * 1920f , deadUI_Rect.rect.height);
            aliveUI_Rect.sizeDelta = new Vector2(CameraManager.Instance.cameras[index].rect.width * 1920f , aliveUI_Rect.rect.height);

            if (CameraManager.Instance.cameras[index].rect.width == 0.36f || CameraManager.Instance.cameras[index].rect.width == 0.64f)
                isStop = true;
        }
    }
}

