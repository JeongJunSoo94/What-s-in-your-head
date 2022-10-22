using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.UI.InGame.Indicator
{
    public class PlayerIndicator : BaseIndicator
    {
        Transform target;
        bool isTopView;
        [Header("탑뷰일 때의 넬라/스테디 스프라이트")] [SerializeField] Sprite nellaTopView;
                                                [SerializeField] Sprite steadyTopView;
        [Header("평상시의 넬라/스테디 스프라이트")] [SerializeField] Sprite nellaNormal;
                                                [SerializeField] Sprite steadyNormal;

        [Header("탑뷰일 때의 자기 UI")] [SerializeField] Sprite myIndicatorTop;
        [Header("평상 시의 상대방 UI")] [SerializeField] Sprite otherIndicatorNormal;

        //내 이미지 & 상대방 이미지
        RectTransform myImgTransform;
        RectTransform otherImgTransform;

        RectTransform canvasRT;

        // 플레이어 위치
        Transform otherPlayerTF;

        protected void Awake()
        {
            // 정식으로 사용할 때엔 아래 코드 쓸것
            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
            if(isNella)
            {
                myIndicatorTop = nellaTopView;
                otherIndicatorNormal = steadyNormal;
            }
            else
            {
                myIndicatorTop = steadyTopView;
                otherIndicatorNormal = nellaNormal;
            }

            // 임시
            //isNella = true;
            detectUI = transform.GetChild(0).gameObject;
            myImgTransform = detectUI.transform.GetChild(0).GetComponent<RectTransform>();
            otherImgTransform = detectUI.transform.GetChild(1).GetComponent<RectTransform>();

            //TopView인지를 받아와야함
            //isTopView = 

            // 노멀 뷰일때만 쓰임 =======================================================================================
            // 기존에 설정된 스프라이트 크기만큼 범위 조절
            myImgTransform.sizeDelta = new Vector2(nella_DetectSprite.bounds.size.x, nella_DetectSprite.bounds.size.y);
            interactiveImg = myImgTransform.gameObject.GetComponent<Image>();
            interactiveImg.sprite = nella_DetectSprite;

            canvasSize = detectUI.GetComponent<RectTransform>();
            screenLimitOffset = myImgTransform.rect.width * 0.4f;
            outOfSightImgScale = myImgTransform.localScale * 0.8f;
            initImgScale = myImgTransform.localScale;
            // ========================================================================================================

            

            //타겟은 나 자신
            target = transform.parent;

            // 상대방 카메라를 가져와야함
            //mainCamera = 

            otherPlayerTF = GameManager.Instance.otherPlayerTF;
        }

        void Update()
        {
            // 타겟의 위치를 메인카메라의 스크린 좌표로 변경
            Vector3 indicatorPosition = mainCamera.WorldToScreenPoint(target.position);

            // 타겟이 카메라 뒤에 있을 때
            if (indicatorPosition.z < 0f)
            {
                interactiveImg.enabled = true;
                SetSreenInfo();
                indicatorPosition *= -1f;
                indicatorPosition = OutOfRange(indicatorPosition);
                myImgTransform.position = indicatorPosition;
            }
            else if (!isTopView)
            {
                interactiveImg.enabled = false;
            }
            // 탑뷰일때
            else
            {
                // 포지션 설정
                Vector3 myIndicatorPosition = mainCamera.WorldToScreenPoint(target.position);
                Vector3 otherIndicatorPosition = mainCamera.WorldToScreenPoint(otherPlayerTF.position);

                myImgTransform.position = myIndicatorPosition;
                otherImgTransform.position = otherIndicatorPosition;

                // 방향 설정
                // 플레이어의 Rotation.y값과 UI의 Rotation.z값이 연동되어야함.

                float myCurEulerY = target.rotation.eulerAngles.y > 180 ? target.rotation.eulerAngles.y - 360 :
                    (target.rotation.eulerAngles.y < -180 ? target.rotation.eulerAngles.y + 360 : target.rotation.eulerAngles.y);

                float otherCurEulerY = otherPlayerTF.rotation.eulerAngles.y > 180 ? otherPlayerTF.rotation.eulerAngles.y - 360 :
                    (otherPlayerTF.rotation.eulerAngles.y < -180 ? otherPlayerTF.rotation.eulerAngles.y + 360 : otherPlayerTF.rotation.eulerAngles.y);

                // 그냥 쿼터니언으로 넣으면 왜 안되는지 알아내야함.
                myImgTransform.rotation = Quaternion.Euler(0, 0, -myCurEulerY);
                otherImgTransform.rotation = Quaternion.Euler(0, 0, -otherCurEulerY);
            }

        }
    }
}

