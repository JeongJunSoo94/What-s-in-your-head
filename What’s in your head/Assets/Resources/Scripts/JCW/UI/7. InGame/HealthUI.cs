using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.UI.InGame
{
    public enum bgState { NORMAL, DAMAGED, DANGEROUS, END }
    public enum hpState { NORMAL, DAMAGED, HEAL, END }
    public class HealthUI : MonoBehaviour
    {
        //HP 담당하는 스크립트 가져오기
        // ex ) PlayerController player = GetComponent<PlayerController>();

        GameObject curCharUI;
        List<Image> backgrounds = new();
        List<Image> hpImages = new();

        void Awake()
        {
            if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
                curCharUI = transform.GetChild(0).gameObject;
            else
                curCharUI = transform.GetChild(1).gameObject;

            curCharUI.SetActive(true);

            // 배경과, HP를 미리 할당
            for (int i = 0 ; i<(int)bgState.END ; ++i)
            {
                backgrounds.Add(curCharUI.transform.GetChild(i).GetComponent<Image>());
                hpImages.Add(curCharUI.transform.GetChild((int)hpState.END).GetChild(i).GetComponent<Image>());
            }
        }

        void Update()
        {

        }
    }
}

