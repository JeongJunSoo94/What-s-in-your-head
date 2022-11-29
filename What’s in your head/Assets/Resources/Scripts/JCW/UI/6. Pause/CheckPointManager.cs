using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JCW.Network;
using KSU;
using Photon.Pun;

namespace JCW.UI
{
    [RequireComponent(typeof(PhotonView))]
    public class CheckPointManager : MonoBehaviour
    {
        [Header("�Ͻ����� Ÿ��Ʋ")] [SerializeField] GameObject titleObj;
        [Header("�Ͻ����� �޴�")] [SerializeField] GameObject menuObj;
        [Header("üũ����Ʈ �޴�")]
        [SerializeField] Button loadMenu;
        [SerializeField] Button backMenu;
        PhotonView photonView;

        bool isFirst = false;

        private void Awake()
        {
            photonView = PhotonView.Get(this);
            loadMenu.onClick.AddListener(() =>
            {
                if (GameManager.Instance.curSection > 1)
                    photonView.RPC(nameof(LoadCP), RpcTarget.AllViaServer);
                else
                {
                    this.gameObject.SetActive(false);
                    titleObj.transform.parent.gameObject.SetActive(false);
                }
            });
            backMenu.onClick.AddListener(() =>
            {
                this.gameObject.SetActive(false);
            });
        }
        private void OnEnable()
        {
            if (!isFirst)
            {
                this.gameObject.SetActive(false);
                return;
            }
            titleObj.SetActive(false);
            menuObj.SetActive(false);
        }
        private void OnDisable()
        {
            if (!isFirst)
            {
                isFirst = true;
                return;
            }
            titleObj.SetActive(true);
            menuObj.SetActive(true);
        }

        [PunRPC]
        void LoadCP()
        {
            GameManager.Instance.myPlayerTF.GetComponent<PlayerController>().Resurrect();
            this.gameObject.SetActive(false);
            titleObj.transform.parent.gameObject.SetActive(false);
        }
    }
}
