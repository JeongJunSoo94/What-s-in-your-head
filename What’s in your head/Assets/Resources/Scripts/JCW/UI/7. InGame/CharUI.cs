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

        bool isStart = false;


        private void Awake()
        {
            mainCamera = transform.parent.GetComponent<CameraController>().FindCamera(); // ��Ƽ��       

            photonView = GetComponent<PhotonView>();
            itemUI_RT = transform.GetChild(0).gameObject.GetComponent<RectTransform>();

            hpUI = transform.GetChild(1).gameObject;

            StartCoroutine(nameof(WaitForPlayer));
        }
        private void Update()
        {
            if (!photonView.IsMine || !isStart)
                return;

            // üũ��
            if (Input.GetKeyDown(KeyCode.KeypadDivide) && !hpUI.activeSelf)
                GameManager.Instance.MediateHP(true);
            if (Input.GetKeyDown(KeyCode.KeypadMultiply) && hpUI.activeSelf)
                GameManager.Instance.MediateHP(false);
        }

        public void SetHP(bool isOn)
        {
            if(photonView.IsMine)
                photonView.RPC(nameof(SetHP_RPC), RpcTarget.AllViaServer, isOn);
        }

        [PunRPC]
        void SetHP_RPC(bool isOn)
        {
            if(itemUI_RT.gameObject.activeSelf)
            {
                Vector2 ogPos = itemUI_RT.anchoredPosition;
                float tempOffset = isOn ? offset : -offset;
                Vector2 movePos;
                if (photonView.IsMine)
                    movePos = isNella ? new Vector2(ogPos.x + tempOffset, ogPos.y) : new Vector2(ogPos.x - tempOffset, ogPos.y);
                else
                    movePos = isNella ? new Vector2(ogPos.x - tempOffset, ogPos.y) : new Vector2(ogPos.x + tempOffset, ogPos.y);
                itemUI_RT.anchoredPosition = movePos;
            }            
            hpUI.SetActive(isOn);
        }

        protected IEnumerator WaitForPlayer()
        {
            while (GameManager.Instance.characterOwner.Count <= 1)
                yield return new WaitForSeconds(0.2f);


            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];

            if (photonView.IsMine)
                GameManager.Instance.hpAllPairs.Add(isNella, this);
            else
                GameManager.Instance.hpAllPairs.Add(!isNella, this);

            isStart = true;
            yield break;
        }

    }
}
