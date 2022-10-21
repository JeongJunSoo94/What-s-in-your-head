using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YC.Camera_;
using YC.Camera_Single;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class CharUI : MonoBehaviour
    {
        [Header("HP UI�� ���� ��, ������ UI�� �з����� ����")] [SerializeField] float offset = 150f;

        // ������ UI ��ġ��
        RectTransform itemUI_RT;

        // HP UI
        GameObject hpUI;

        PhotonView photonView;

        // ���� �ڶ�����
        bool isNella;

        // ���� ķ
        Camera mainCamera;


        private void Awake()
        {
            if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
                mainCamera = transform.parent.GetComponent<CameraController>().FindCamera(); // ��Ƽ��
            else
                mainCamera = transform.parent.GetComponent<CameraController_Single>().FindCamera(); // �̱ۿ�            

            GetComponent<Canvas>().worldCamera = mainCamera;
            GetComponent<Canvas>().planeDistance = 0.15f;

            photonView = GetComponent<PhotonView>();
            if (GameManager.Instance.characterOwner.Count == 0)
                isNella = true;
            else
                isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];

            itemUI_RT = transform.GetChild(0).gameObject.GetComponent<RectTransform>();
            hpUI = transform.GetChild(1).gameObject;
        }
        private void Update()
        {
            if (!photonView.IsMine)
                return;
            
            // üũ��
            if (Input.GetKeyDown(KeyCode.KeypadDivide))
                SetHP(true);
            if (Input.GetKeyDown(KeyCode.KeypadMultiply))
                SetHP(false);
        }

        public void SetHP(bool isOn)
        {
            if(hpUI.activeSelf != isOn)
                photonView.RPC(nameof(SetHP_RPC), RpcTarget.AllViaServer, isOn);
        }

        [PunRPC]
        void SetHP_RPC(bool isOn)
        {
            Debug.Log("isMine : " + photonView.IsMine + " / isNella : " + isNella + " �����ش�");
            Vector2 ogPos = itemUI_RT.anchoredPosition;
            float tempOffset = isOn ? offset : -offset;
            Vector2 movePos;
            if(photonView.IsMine)
                movePos = isNella ? new Vector2(ogPos.x + tempOffset, ogPos.y) : new Vector2(ogPos.x - tempOffset, ogPos.y);
            else
                movePos = isNella ? new Vector2(ogPos.x - tempOffset, ogPos.y) : new Vector2(ogPos.x + tempOffset, ogPos.y);
            itemUI_RT.anchoredPosition = movePos;
            hpUI.SetActive(isOn);
        }

    }
}
