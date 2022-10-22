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
        [Header("ž���� ���� �ڶ�/���׵� ��������Ʈ")] [SerializeField] Sprite nellaTopView;
                                                [SerializeField] Sprite steadyTopView;
        [Header("������ �ڶ�/���׵� ��������Ʈ")] [SerializeField] Sprite nellaNormal;
                                                [SerializeField] Sprite steadyNormal;

        [Header("ž���� ���� �ڱ� UI")] [SerializeField] Sprite myIndicatorTop;
        [Header("��� ���� ���� UI")] [SerializeField] Sprite otherIndicatorNormal;

        //�� �̹��� & ���� �̹���
        RectTransform myImgTransform;
        RectTransform otherImgTransform;

        RectTransform canvasRT;

        // �÷��̾� ��ġ
        Transform otherPlayerTF;

        protected void Awake()
        {
            // �������� ����� ���� �Ʒ� �ڵ� ����
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

            // �ӽ�
            //isNella = true;
            detectUI = transform.GetChild(0).gameObject;
            myImgTransform = detectUI.transform.GetChild(0).GetComponent<RectTransform>();
            otherImgTransform = detectUI.transform.GetChild(1).GetComponent<RectTransform>();

            //TopView������ �޾ƿ;���
            //isTopView = 

            // ��� ���϶��� ���� =======================================================================================
            // ������ ������ ��������Ʈ ũ�⸸ŭ ���� ����
            myImgTransform.sizeDelta = new Vector2(nella_DetectSprite.bounds.size.x, nella_DetectSprite.bounds.size.y);
            interactiveImg = myImgTransform.gameObject.GetComponent<Image>();
            interactiveImg.sprite = nella_DetectSprite;

            canvasSize = detectUI.GetComponent<RectTransform>();
            screenLimitOffset = myImgTransform.rect.width * 0.4f;
            outOfSightImgScale = myImgTransform.localScale * 0.8f;
            initImgScale = myImgTransform.localScale;
            // ========================================================================================================

            

            //Ÿ���� �� �ڽ�
            target = transform.parent;

            // ���� ī�޶� �����;���
            //mainCamera = 

            otherPlayerTF = GameManager.Instance.otherPlayerTF;
        }

        void Update()
        {
            // Ÿ���� ��ġ�� ����ī�޶��� ��ũ�� ��ǥ�� ����
            Vector3 indicatorPosition = mainCamera.WorldToScreenPoint(target.position);

            // Ÿ���� ī�޶� �ڿ� ���� ��
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
            // ž���϶�
            else
            {
                // ������ ����
                Vector3 myIndicatorPosition = mainCamera.WorldToScreenPoint(target.position);
                Vector3 otherIndicatorPosition = mainCamera.WorldToScreenPoint(otherPlayerTF.position);

                myImgTransform.position = myIndicatorPosition;
                otherImgTransform.position = otherIndicatorPosition;

                // ���� ����
                // �÷��̾��� Rotation.y���� UI�� Rotation.z���� �����Ǿ����.

                float myCurEulerY = target.rotation.eulerAngles.y > 180 ? target.rotation.eulerAngles.y - 360 :
                    (target.rotation.eulerAngles.y < -180 ? target.rotation.eulerAngles.y + 360 : target.rotation.eulerAngles.y);

                float otherCurEulerY = otherPlayerTF.rotation.eulerAngles.y > 180 ? otherPlayerTF.rotation.eulerAngles.y - 360 :
                    (otherPlayerTF.rotation.eulerAngles.y < -180 ? otherPlayerTF.rotation.eulerAngles.y + 360 : otherPlayerTF.rotation.eulerAngles.y);

                // �׳� ���ʹϾ����� ������ �� �ȵǴ��� �˾Ƴ�����.
                myImgTransform.rotation = Quaternion.Euler(0, 0, -myCurEulerY);
                otherImgTransform.rotation = Quaternion.Euler(0, 0, -otherCurEulerY);
            }

        }
    }
}

