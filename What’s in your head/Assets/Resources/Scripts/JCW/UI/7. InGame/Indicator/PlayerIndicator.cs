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
        RectTransform canvasRect;
        RectTransform myImgTransform;
        // 내 플레이어 위치
        Transform myTF;

        PhotonView photonView;

        Image myImg;
        Image otherImg;

        bool wasTopView;
        bool isStart_Player = false;

        protected void Awake()
        {            
            photonView = GetComponent<PhotonView>();
            if (!photonView.IsMine)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(nameof(Wait));
        }

        void Update()
        {
            //canvasRect.transform.rotation = new Quaternion(0, 0, 0,0);
            if (isStart_Player == false)
                return;            
            SetSreenInfo();
            if (target == null)
            {
                if (GameManager.Instance.otherPlayerTF == null)
                    return;
                target = GameManager.Instance.otherPlayerTF;
            }
            // 타겟의 위치를 메인카메라의 스크린 좌표로 변경
            Vector3 indicatorPosition = mainCamera.WorldToScreenPoint(target.position);
            if (!GameManager.Instance.isTopView)
            {
                if(wasTopView)
                {
                    wasTopView = false;
                    otherImg.sprite = otherIndicatorNormal;
                    myImg.enabled = false;
                }
                // 타겟이 화면 안에 들어올 때
                if (indicatorPosition.z >= 0f)
                {
                    if (indicatorPosition.x <= screenSize.x + screenSize.width && indicatorPosition.x >= screenSize.x
                       && indicatorPosition.y <= screenSize.y + screenSize.height && indicatorPosition.y >= screenSize.y)
                    {
                        //Debug.Log("화면 안에 들어옴");
                        otherImg.enabled = false;
                        indicatorPosition.z = 0f;
                    }
                    else
                    {
                        //Debug.Log("카메라 앞인데 화면 안에 안 들어옴");
                        otherImg.enabled = true;
                        indicatorPosition = OutOfRange(indicatorPosition);
                    }
                }
                //타겟이 내 카메라 뒤에 있을 때, 화면에 그림
                else
                {
                    //Debug.Log("카메라 뒤라 화면 안에 안 들어옴");
                    imgTransform.sizeDelta = new Vector2(otherIndicatorNormal.bounds.size.x, otherIndicatorNormal.bounds.size.y);
                    otherImg.enabled = true;
                    SetSreenInfo();
                    indicatorPosition *= -1f;
                    indicatorPosition = OutOfRange(indicatorPosition);
                }
            }            
            // 탑뷰일때
            else
            {
                if(!wasTopView)
                {
                    myImg.enabled = true;
                    otherImg.enabled = true;
                    imgTransform.sizeDelta = new Vector2(otherIndicatorTop.bounds.size.x, otherIndicatorTop.bounds.size.y);
                    otherImg.sprite = otherIndicatorTop;
                    wasTopView = true;
                    imgTransform.localScale = initImgScale;
                }
                if (mainCamera.rect.width <= 0.1f)
                    return;
                // 포지션 설정
                Vector3 myIndicatorPosition = mainCamera.WorldToScreenPoint(myTF.position);

                myImgTransform.position = myIndicatorPosition;
                imgTransform.position = indicatorPosition;

                // 방향 설정
                // 플레이어의 Rotation.y값과 UI의 Rotation.z값이 연동되어야함.

                float myCurEulerY = myTF.rotation.eulerAngles.y > 180 ? myTF.rotation.eulerAngles.y - 360 :
                    (myTF.rotation.eulerAngles.y < -180 ? myTF.rotation.eulerAngles.y + 360 : myTF.rotation.eulerAngles.y);

                float otherCurEulerY = target.rotation.eulerAngles.y > 180 ? target.rotation.eulerAngles.y - 360 :
                    (target.rotation.eulerAngles.y < -180 ? target.rotation.eulerAngles.y + 360 : target.rotation.eulerAngles.y);

                // 그냥 쿼터니언으로 넣으면 왜 안되는지 알아내야함.
                myImgTransform.rotation = Quaternion.Euler(0, 0, -myCurEulerY);
                imgTransform.rotation = Quaternion.Euler(0, 0, -otherCurEulerY);
            }
            imgTransform.position = indicatorPosition;
        }

        IEnumerator Wait()
        {
            yield return new WaitUntil(() => isStart == true);

            myTF = GameManager.Instance.myPlayerTF;

            if (isNella)
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
            detectUI = transform.GetChild(0).gameObject;
            canvasRect = detectUI.GetComponent<RectTransform>();
            myImgTransform = detectUI.transform.GetChild(0).GetComponent<RectTransform>();
            myImgTransform.sizeDelta = new Vector2(myIndicatorTop.bounds.size.x, myIndicatorTop.bounds.size.y);
            imgTransform = detectUI.transform.GetChild(1).GetComponent<RectTransform>();

            myImg = myImgTransform.gameObject.GetComponent<Image>();
            myImg.sprite = myIndicatorTop;



            // 노멀 뷰일때만 쓰임 =======================================================================================
            // 기존에 설정된 스프라이트 크기만큼 범위 조절
            imgTransform.sizeDelta = new Vector2(otherIndicatorNormal.bounds.size.x, otherIndicatorNormal.bounds.size.y);
            otherImg = imgTransform.gameObject.GetComponent<Image>();
            otherImg.sprite = otherIndicatorNormal;

            canvasSize = detectUI.GetComponent<RectTransform>();
            screenLimitOffset = imgTransform.rect.width * 0.4f;
            outOfSightImgScale = imgTransform.localScale * 0.8f;
            initImgScale = imgTransform.localScale;
            // ========================================================================================================

            isStart_Player = true;

            yield break;
        }
    }
}

