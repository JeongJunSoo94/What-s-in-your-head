using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JCW.Network;

namespace JCW.UI
{
    public class CheckPointManager : MonoBehaviour
    {
        [Header("일시정지 타이틀")] [SerializeField] GameObject titleObj;
        [Header("일시정지 메뉴")] [SerializeField] GameObject menuObj;
        [Header("체크포인트 메뉴")]
        [SerializeField] Button loadMenu;
        [SerializeField] Button backMenu;

        private void Awake()
        {
            loadMenu.onClick.AddListener(() =>
            {
                Debug.Log("아직 미구현");
                GameManager.Instance.curStageIndex = 0;
                PhotonManager.Instance.ChangeStage();
            });
            backMenu.onClick.AddListener(() =>
            {
                this.gameObject.SetActive(false);
            });
        }
        private void OnEnable()
        {
            titleObj.SetActive(false);
            menuObj.SetActive(false);
        }
        private void OnDisable()
        {
            titleObj.SetActive(true);
            menuObj.SetActive(true);
        }
    }
}
