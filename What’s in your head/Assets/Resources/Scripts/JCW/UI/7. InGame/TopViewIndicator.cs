using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.UI.InGame
{
    public class TopViewIndicator : MonoBehaviour
    {
        [Header("UI 캔버스 크기")] [SerializeField] RectTransform canvasRT;
        [Header("내 플레이어 위치")] [SerializeField] Transform myPlayerTF;
        [Header("상대 플레이어 위치")] [SerializeField] Transform otherPlayerTF;
        [Header("UI를 보여줄 카메라")] [SerializeField] Camera mainCamera;

        [Header("내 이미지 & 상대방 이미지")][SerializeField] RectTransform myImgTransform;
                                            [SerializeField] RectTransform otherImgTransform;

        [Header("넬라 / 스테디 UI")]
        [SerializeField] Sprite nellaIndicator;
        [SerializeField] Sprite steadyIndicator;

        private void Awake()
        {
            if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
            {
                myImgTransform.gameObject.GetComponent<Image>().sprite = nellaIndicator;
                otherImgTransform.gameObject.GetComponent<Image>().sprite = steadyIndicator;
            }
            else
            {
                myImgTransform.gameObject.GetComponent<Image>().sprite = steadyIndicator;
                otherImgTransform.gameObject.GetComponent<Image>().sprite = nellaIndicator;
            }            
        }

        void Update()
        {
            // 포지션 설정
            Vector3 myIndicatorPosition = mainCamera.WorldToScreenPoint(myPlayerTF.position);
            Vector3 otherIndicatorPosition = mainCamera.WorldToScreenPoint(otherPlayerTF.position);

            myImgTransform.position = myIndicatorPosition;
            otherImgTransform.position = otherIndicatorPosition;

            // 방향 설정
            myImgTransform.up = -myPlayerTF.forward;
            otherImgTransform.up = -otherPlayerTF.forward;
        }
    }
}
