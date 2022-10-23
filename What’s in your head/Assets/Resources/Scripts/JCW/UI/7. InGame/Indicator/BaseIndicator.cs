using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YC.CameraManager_;

namespace JCW.UI.InGame.Indicator
{
    abstract public class BaseIndicator : MonoBehaviour
    {
        // UI�� ����� ī�޶�
        protected Camera mainCamera;
        protected GameObject detectUI;
        protected RectTransform imgTransform;

        // �Ÿ���, Ÿ�ݷ��� ���� �ٸ��� ������ ������
        protected Image gauge;
        protected RectTransform canvasSize;

        // ȭ�� ũ��
        protected Rect screenSize;
        // ȭ�鿡�� ����� ���ϰ� �ϴ� ������ ��
        protected float screenLimitOffset;

        // ȭ�� �ȿ� ���� ���� ����� ���� �̹��� ũ��
        protected Vector3 initImgScale;
        protected Vector3 outOfSightImgScale;

        // �⺻ ��������Ʈ
        protected Sprite nella_DetectSprite;
        protected Sprite steady_DetectSprite;
        protected Image interactiveImg;

        protected bool isNella;

        // UI�� ��������
        protected bool isActive;

        virtual protected void Awake()
        {
            mainCamera = isNella ? CameraManager.Instance.cameras[0] : CameraManager.Instance.cameras[1];
        }

        // ī�޶� ������ ����� ���� ���� ����
        protected Vector3 OutOfRange(Vector3 indicatorPosition)
        {
            imgTransform.localScale = outOfSightImgScale;
            gauge.transform.localScale = outOfSightImgScale;
            indicatorPosition.z = 0f;

            // ���� ī�޶� ȭ���� �߽� ��ġ ���
            Vector3 canvasCenter = new Vector3((screenSize.x + screenSize.width / 2f), (screenSize.y + screenSize.height / 2f), 0f);

            // UI ��ġ-> ȭ�� �߽� ����
            indicatorPosition -= canvasCenter;

            // ȭ�� ���� ����
            float divX = (screenSize.width / 2f - screenLimitOffset) / Mathf.Abs(indicatorPosition.x);
            float divY = (screenSize.height / 2f - screenLimitOffset) / Mathf.Abs(indicatorPosition.y);

            // x ��ǥ�� ���� �׵θ��� ����� ��
            if (divX < divY)
            {
                // z���� �������� x�࿡������ UI������ ����
                float angle = Vector3.SignedAngle(Vector3.right, indicatorPosition, Vector3.forward);

                // ȭ�� �ִ� ������ x�� ��ġ.
                indicatorPosition.x = Mathf.Sign(indicatorPosition.x) * (screenSize.width / 2f - screenLimitOffset);
                indicatorPosition.y = Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.x;
            }
            else
            {
                // z���� �������� y�࿡������ UI������ ����
                float angle = Vector3.SignedAngle(Vector3.up, indicatorPosition, Vector3.forward);

                // ȭ�� �ִ� ������ y�� ��ġ.
                indicatorPosition.y = Mathf.Sign(indicatorPosition.y) * (screenSize.height / 2f - screenLimitOffset);
                indicatorPosition.x = -Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.y;
            }

            // ���󺹱�
            indicatorPosition += canvasCenter;
            return indicatorPosition;
        }

        // ��ũ�� ������ Rect���� �°Բ� ����
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