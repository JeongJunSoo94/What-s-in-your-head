using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YC.CameraManager_;

namespace JCW.UI.InGame.Indicator
{
    [RequireComponent(typeof(PhotonView))]
    public class PlayerIndicator : BaseIndicator
    {
        Transform target;
        [Header("탑뷰일 때의 넬라/스테디 스프라이트")] [SerializeField] Sprite nellaTopView;
                                                [SerializeField] Sprite steadyTopView;
        [Header("평상시의 넬라/스테디 스프라이트")] [SerializeField] Sprite nellaNormal;
                                                [SerializeField] Sprite steadyNormal;

        //탑뷰일 때의 자기 UI
        Sprite myIndicatorTop;
        //상대방 UI
        Sprite otherIndicatorTop;
        Sprite otherIndicatorNormal;

        //내 이미지 & 상대방 이미지
        RectTransform myImgTransform;
        RectTransform otherImgTransform;

        RectTransform canvasRT;

        // 내 플레이어 위치
        Transform myTF;

        PhotonView photonView;

        override protected void Awake()
        {
            base.Awake();
            photonView = GetComponent<PhotonView>();
            if (!photonView.IsMine)
            {
                Destroy(this);
                return;
            }
            // 정식으로 사용할 때엔 아래 코드 쓸것
            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
            if(isNella)
            {
                myIndicatorTop = nellaTopView;
                otherIndicatorTop = steadyTopView;
                otherIndicatorNormal = steadyNormal;
            }
            else
            {
                myIndicatorTop = steadyTopView;
                otherIndicatorTop = nellaTopView;
                otherIndicatorNormal = nellaNormal;
            }

            // 임시
            //isNella = true;
            detectUI = transform.GetChild(0).gameObject;
            myImgTransform = detectUI.transform.GetChild(0).GetComponent<RectTransform>();
            myImgTransform.sizeDelta = new Vector2(myIndicatorTop.bounds.size.x, myIndicatorTop.bounds.size.y);
            otherImgTransform = detectUI.transform.GetChild(1).GetComponent<RectTransform>();

            //TopView인지를 받아와야함            

            // 노멀 뷰일때만 쓰임 =======================================================================================
            // 기존에 설정된 스프라이트 크기만큼 범위 조절
            otherImgTransform.sizeDelta = new Vector2(otherIndicatorNormal.bounds.size.x, otherIndicatorNormal.bounds.size.y);
            interactiveImg = otherImgTransform.gameObject.GetComponent<Image>();
            interactiveImg.sprite = otherIndicatorNormal;

            canvasSize = detectUI.GetComponent<RectTransform>();
            screenLimitOffset = otherImgTransform.rect.width * 0.4f;
            outOfSightImgScale = otherImgTransform.localScale * 0.8f;
            initImgScale = otherImgTransform.localScale;
            // ========================================================================================================

            

            //타겟은 상대방
            target = GameManager.Instance.otherPlayerTF;            
            myTF = transform.parent;

            // 내 카메라를 가져와야함
            //mainCamera = isNella ? CameraManager.Instance.cameras[0] : CameraManager.Instance.cameras[1];

        }

        void Update()
        {
            // 타겟의 위치를 메인카메라의 스크린 좌표로 변경
            Vector3 indicatorPosition = mainCamera.WorldToScreenPoint(target.position);

            if (GameManager.Instance.isTopView)
            {
                interactiveImg.sprite = otherIndicatorNormal;
                //타겟이 내 카메라 뒤에 있을 때, 화면에 그림
                if (indicatorPosition.z < 0f)
                {
                    otherImgTransform.sizeDelta = new Vector2(otherIndicatorNormal.bounds.size.x, otherIndicatorNormal.bounds.size.y);
                    interactiveImg.enabled = true;
                    SetSreenInfo();
                    indicatorPosition *= -1f;
                    indicatorPosition = OutOfRange(indicatorPosition);
                    otherImgTransform.position = indicatorPosition;
                }
                else
                    interactiveImg.enabled = false;
            }            
            // 탑뷰일때
            else
            {
                otherImgTransform.sizeDelta = new Vector2(otherIndicatorTop.bounds.size.x, otherIndicatorTop.bounds.size.y);
                interactiveImg.sprite = otherIndicatorTop;
                // 포지션 설정
                Vector3 myIndicatorPosition = mainCamera.WorldToScreenPoint(myTF.position);

                myImgTransform.position = myIndicatorPosition;
                otherImgTransform.position = indicatorPosition;

                // 방향 설정
                // 플레이어의 Rotation.y값과 UI의 Rotation.z값이 연동되어야함.

                float myCurEulerY = myTF.rotation.eulerAngles.y > 180 ? myTF.rotation.eulerAngles.y - 360 :
                    (myTF.rotation.eulerAngles.y < -180 ? myTF.rotation.eulerAngles.y + 360 : myTF.rotation.eulerAngles.y);

                float otherCurEulerY = target.rotation.eulerAngles.y > 180 ? target.rotation.eulerAngles.y - 360 :
                    (target.rotation.eulerAngles.y < -180 ? target.rotation.eulerAngles.y + 360 : target.rotation.eulerAngles.y);

                // 그냥 쿼터니언으로 넣으면 왜 안되는지 알아내야함.
                myImgTransform.rotation = Quaternion.Euler(0, 0, -myCurEulerY);
                otherImgTransform.rotation = Quaternion.Euler(0, 0, -otherCurEulerY);
            }

        }
    }
}

