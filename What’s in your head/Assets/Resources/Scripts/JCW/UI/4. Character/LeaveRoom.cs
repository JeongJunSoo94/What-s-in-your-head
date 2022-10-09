using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace JCW.UI
{
    [RequireComponent(typeof(PhotonView))]
    public class LeaveRoom : MonoBehaviour
    {
        //[Header("������ �� �� UI")] [SerializeField] private GameObject msgUI;
        [Header("�ڷΰ��� ��ư")] [SerializeField] private Button backButton;
        [Header("�ڶ�/���׵� ��ư ���� �÷��̾�")] [SerializeField] private GameObject nellaButtonOwner;
                                                [SerializeField] private GameObject steadyButtonOwner;
        [Header("�ڶ�/���׵� ���� �̹���")] [SerializeField] private GameObject nellaSelectImg;
                                         [SerializeField] private GameObject steadySelectImg;
        [Header("�ڶ�/���׵� �̸� �� ����")] [SerializeField] private GameObject nellaName;
                                         [SerializeField] private GameObject steadyName;

        readonly List<GameObject> nellaHoveringPlayer = new();
        readonly List<GameObject> steadyHoveringPlayer = new();

        private void Awake()
        {
            for (int i=0 ; i<nellaButtonOwner.transform.childCount; ++i)
            {
                nellaHoveringPlayer.Add(nellaButtonOwner.transform.GetChild(i).gameObject);
                steadyHoveringPlayer.Add(steadyButtonOwner.transform.GetChild(i).gameObject);
            }

            backButton.onClick.AddListener(() =>
            {
                this.gameObject.GetComponent<PhotonView>().RPC(nameof(Leave), RpcTarget.AllViaServer);
            });
        }

        [PunRPC]
        public void Leave()
        {
            nellaButtonOwner.GetComponent<Text>().text = "";
            steadyButtonOwner.GetComponent<Text>().text = "";

            nellaHoveringPlayer[0].SetActive(false);
            nellaHoveringPlayer[1].SetActive(false);

            steadyHoveringPlayer[0].SetActive(false);
            steadyHoveringPlayer[1].SetActive(false);

            nellaSelectImg.SetActive(false);
            steadySelectImg.SetActive(false);

            Color transparentColor = new(0, 0, 0, 0);

            nellaName.GetComponent<Text>().color = transparentColor;
            nellaName.transform.GetChild(0).gameObject.GetComponent<Text>().color = transparentColor;

            steadyName.GetComponent<Text>().color = transparentColor;
            steadyName.transform.GetChild(0).gameObject.GetComponent<Text>().color = transparentColor;

            this.gameObject.SetActive(false);
        }
    }
}

