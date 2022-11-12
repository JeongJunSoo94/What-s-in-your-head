using KSU;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YC.CameraManager_;

namespace JCW.UI.InGame
{
    public class AimUI : MonoBehaviour
    {
        [Header("오토 타겟팅 속도")] [Range(0, 5f)] [SerializeField] float targetingTime;
        Camera mainCamera;
        Camera curCam;
        RectTransform imgTransform;
        bool wasTopView;

        bool isNella;

        Transform targetTopviewTF;
        Transform targetTF;
        Transform playerTF;

        Image aimImage;

        bool isTargeting = false;
        bool isNormal = true;
        float curTargetTime = 0f;
        float curNormalTime = 0f;
        float detectAngle;
        PlayerState aimState;

        private void Awake()
        {
            imgTransform = transform.GetChild(0).GetComponent<RectTransform>();
            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
            targetTopviewTF = isNella ? GameObject.FindWithTag("NellaMousePoint").transform : GameObject.FindWithTag("SteadyMousePoint").transform;

            mainCamera = isNella ? CameraManager.Instance.cameras[0] : CameraManager.Instance.cameras[1];
            curCam = mainCamera;
            imgTransform.position = new Vector3((curCam.rect.x + curCam.rect.width + curCam.rect.x) * 960f, 540f, 0);
            targetTF = null;
            aimImage = imgTransform.GetComponent<Image>();
        }

        private void Start()
        {
            playerTF = GameManager.Instance.myPlayerTF;
            aimState = playerTF.GetComponent<PlayerController>().characterState;
        }


        private void FixedUpdate()
        {
            if (targetTopviewTF == null)
                return;
            aimImage.enabled = aimState.aim;
            // 현재 탑뷰일 때
            if (GameManager.Instance.isTopView || GameManager.Instance.isSideView)
            {
                if (!wasTopView)
                {
                    wasTopView = true;
                    this.transform.parent = targetTopviewTF;

                    // 현재 캠이 완전 축소된 경우 다른 캠으로 설정.
                    if (curCam.rect.width <= 0.1f)
                        curCam = isNella ? CameraManager.Instance.cameras[1] : CameraManager.Instance.cameras[0];
                }
                imgTransform.position = curCam.WorldToScreenPoint(targetTopviewTF.position);
            }
            // 직전엔 탑뷰였지만 지금은 아닌 경우
            else if (wasTopView)
            {
                wasTopView = false;
                this.transform.parent = playerTF;
                imgTransform.position = new Vector3((curCam.rect.x + curCam.rect.width + curCam.rect.x) * 960f, 540f, 0);
            }
            else
            {
                // 타겟이 들어옴
                if (targetTF != null)
                {
                    isNormal = false;
                    curNormalTime = 0f;
                    if (!isTargeting)
                    {
                        imgTransform.position = Vector3.Lerp(imgTransform.position, curCam.WorldToScreenPoint(targetTF.position), Time.fixedDeltaTime * (detectAngle * 0.5f + 3.5f));
                        curTargetTime += Time.fixedDeltaTime;
                        if (curTargetTime >= targetingTime)
                        {
                            curTargetTime = 0f;
                            isTargeting = true;
                        }
                    }
                    else
                        imgTransform.position = curCam.WorldToScreenPoint(targetTF.position);
                }
                else
                {
                    isTargeting = false;
                    curTargetTime = 0f;
                    if (!isNormal)
                    {
                        imgTransform.position = Vector3.Lerp(imgTransform.position, new Vector3((curCam.rect.x + curCam.rect.width + curCam.rect.x) * 960f, 540f, 0), Time.fixedDeltaTime * (detectAngle * 0.5f + 3.5f));
                        curNormalTime += Time.fixedDeltaTime;
                        if (curNormalTime >= targetingTime)
                        {
                            curNormalTime = 0f;
                            isNormal = true;
                        }
                    }
                    else
                        imgTransform.position = new Vector3((curCam.rect.x + curCam.rect.width + curCam.rect.x) * 960f, 540f, 0);
                }
            }
        }

        public void SetTarget(Transform posTF, float angle)
        {
            targetTF = posTF;
            detectAngle = angle;            
        }
    }
}
