using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using JCW.AudioCtrl;

namespace JCW.UI.Options
{
    [RequireComponent(typeof(PhotonView))]
    public class SelectTab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private GameObject textObj = null;
        private Text text = null;
        private Button button = null;

        private int textInitSize = 0;
        [Header("다른 플레이어와 공유해야 하는 지")] [SerializeField] bool isShare;
        PhotonView photonView;

        private void Awake()
        {
            button = GetComponent<Button>();
            textObj = this.gameObject.transform.GetChild(0).gameObject;
            text = textObj.GetComponent<Text>();
            if(isShare)
                photonView = GetComponent<PhotonView>();
        }
        void Start()
        {
            textInitSize = text.fontSize;
            button.onClick.AddListener(() =>
            {
                if (isShare)
                {
                    if(PhotonNetwork.IsMasterClient)
                        photonView.RPC(nameof(ClickTab), RpcTarget.AllViaServer);
                }
                else
                    TabManager.Instance.ClickTab(button);
            });
        }

        // 마우스 포인터가 닿았을 때
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerEnter == textObj)
                text.fontSize = (int)(textInitSize * 1.2f);
            SoundManager.Instance.PlayUI("MouseHovering");
        }
        // 마우스 포인터가 떨어졌을 때
        public void OnPointerExit(PointerEventData eventData)
        {
            text.fontSize = textInitSize;
        }

        [PunRPC]
        void ClickTab()
        {
            TabManager.Instance.ClickTab(button);
        }
    }
}

