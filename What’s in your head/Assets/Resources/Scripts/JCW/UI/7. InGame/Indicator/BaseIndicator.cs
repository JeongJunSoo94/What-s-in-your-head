using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YC.CameraManager_;

namespace JCW.UI.InGame.Indicator
{
    abstract public class BaseIndicator : MonoBehaviour
    {
        // UI를 띄워줄 카메라
        protected Camera mainCamera;
        protected GameObject detectUI;
        protected RectTransform imgTransform;

        // 거리나, 타격량에 따라 다르게 보여줄 게이지
        protected Image gauge;
        protected RectTransform canvasSize;

        // 화면 크기
        protected Rect screenSize;
        // 화면에서 벗어나지 못하게 하는 오프셋 값
        protected float screenLimitOffset;

        // 화면 안에 있을 때와 벗어났을 때의 이미지 크기
        protected Vector3 initImgScale;
        protected Vector3 outOfSightImgScale;

        // 기본 스프라이트
        protected Sprite nella_DetectSprite;
        protected Sprite steady_DetectSprite;
        protected Image interactiveImg;

        protected bool isNella;

        // UI가 켜졌는지
        protected bool isActive;

        virtual protected void Awake()
        {
            mainCamera = isNella ? CameraManager.Instance.cameras[0] : CameraManager.Instance.cameras[1];
        }

        // 카메라 범위를 벗어났을 때를 위한 설정
        protected Vector3 OutOfRange(Vector3 indicatorPosition)
        {
            imgTransform.localScale = outOfSightImgScale;
            gauge.transform.localScale = outOfSightImgScale;
            indicatorPosition.z = 0f;

            // 현재 카메라 화면의 중심 위치 잡기
            Vector3 canvasCenter = new Vector3((screenSize.x + screenSize.width / 2f), (screenSize.y + screenSize.height / 2f), 0f);

            // UI 위치-> 화면 중심 벡터
            indicatorPosition -= canvasCenter;

            // 화면 범위 제한
            float divX = (screenSize.width / 2f - screenLimitOffset) / Mathf.Abs(indicatorPosition.x);
            float divY = (screenSize.height / 2f - screenLimitOffset) / Mathf.Abs(indicatorPosition.y);

            // x 좌표가 먼저 테두리에 닿았을 때
            if (divX < divY)
            {
                // z축을 기준으로 x축에서부터 UI까지의 각도
                float angle = Vector3.SignedAngle(Vector3.right, indicatorPosition, Vector3.forward);

                // 화면 최대 범위에 x값 위치.
                indicatorPosition.x = Mathf.Sign(indicatorPosition.x) * (screenSize.width / 2f - screenLimitOffset);
                indicatorPosition.y = Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.x;
            }
            else
            {
                // z축을 기준으로 y축에서부터 UI까지의 각도
                float angle = Vector3.SignedAngle(Vector3.up, indicatorPosition, Vector3.forward);

                // 화면 최대 범위에 y값 위치.
                indicatorPosition.y = Mathf.Sign(indicatorPosition.y) * (screenSize.height / 2f - screenLimitOffset);
                indicatorPosition.x = -Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.y;
            }

            // 원상복귀
            indicatorPosition += canvasCenter;
            return indicatorPosition;
        }

        // 스크린 사이즈 Rect값에 맞게끔 설정
        protected void SetSreenInfo()
        {
            Rect cameraPos = mainCamera.rect;
            screenSize = new(canvasSize.rect.width * cameraPos.x,
                             canvasSize.rect.height * cameraPos.y,
                             canvasSize.rect.width * cameraPos.width,
                             canvasSize.rect.height * cameraPos.height);
        }
    }
}