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
        [Header("ž���� ���� �ڶ�/���׵� ��������Ʈ")] [SerializeField] Sprite nellaTopView;
                                                [SerializeField] Sprite steadyTopView;
        [Header("������ �ڶ�/���׵� ��������Ʈ")] [SerializeField] Sprite nellaNormal;
                                                [SerializeField] Sprite steadyNormal;

        //ž���� ���� �ڱ� UI
        Sprite myIndicatorTop;
        //���� UI
        Sprite otherIndicatorTop;
        Sprite otherIndicatorNormal;

        //�� �̹��� & ���� �̹���
        RectTransform myImgTransform;
        RectTransform otherImgTransform;

        RectTransform canvasRT;

        // �� �÷��̾� ��ġ
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
            // �������� ����� ���� �Ʒ� �ڵ� ����
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

            // �ӽ�
            //isNella = true;
            detectUI = transform.GetChild(0).gameObject;
            myImgTransform = detectUI.transform.GetChild(0).GetComponent<RectTransform>();
            myImgTransform.sizeDelta = new Vector2(myIndicatorTop.bounds.size.x, myIndicatorTop.bounds.size.y);
            otherImgTransform = detectUI.transform.GetChild(1).GetComponent<RectTransform>();

            //TopView������ �޾ƿ;���            

            // ��� ���϶��� ���� =======================================================================================
            // ������ ������ ��������Ʈ ũ�⸸ŭ ���� ����
            otherImgTransform.sizeDelta = new Vector2(otherIndicatorNormal.bounds.size.x, otherIndicatorNormal.bounds.size.y);
            interactiveImg = otherImgTransform.gameObject.GetComponent<Image>();
            interactiveImg.sprite = otherIndicatorNormal;

            canvasSize = detectUI.GetComponent<RectTransform>();
            screenLimitOffset = otherImgTransform.rect.width * 0.4f;
            outOfSightImgScale = otherImgTransform.localScale * 0.8f;
            initImgScale = otherImgTransform.localScale;
            // ========================================================================================================

            

            //Ÿ���� ����
            target = GameManager.Instance.otherPlayerTF;            
            myTF = transform.parent;

            // �� ī�޶� �����;���
            //mainCamera = isNella ? CameraManager.Instance.cameras[0] : CameraManager.Instance.cameras[1];

        }

        void Update()
        {
            // Ÿ���� ��ġ�� ����ī�޶��� ��ũ�� ��ǥ�� ����
            Vector3 indicatorPosition = mainCamera.WorldToScreenPoint(target.position);

            if (GameManager.Instance.isTopView)
            {
                interactiveImg.sprite = otherIndicatorNormal;
                //Ÿ���� �� ī�޶� �ڿ� ���� ��, ȭ�鿡 �׸�
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
            // ž���϶�
            else
            {
                otherImgTransform.sizeDelta = new Vector2(otherIndicatorTop.bounds.size.x, otherIndicatorTop.bounds.size.y);
                interactiveImg.sprite = otherIndicatorTop;
                // ������ ����
                Vector3 myIndicatorPosition = mainCamera.WorldToScreenPoint(myTF.position);

                myImgTransform.position = myIndicatorPosition;
                otherImgTransform.position = indicatorPosition;

                // ���� ����
                // �÷��̾��� Rotation.y���� UI�� Rotation.z���� �����Ǿ����.

                float myCurEulerY = myTF.rotation.eulerAngles.y > 180 ? myTF.rotation.eulerAngles.y - 360 :
                    (myTF.rotation.eulerAngles.y < -180 ? myTF.rotation.eulerAngles.y + 360 : myTF.rotation.eulerAngles.y);

                float otherCurEulerY = target.rotation.eulerAngles.y > 180 ? target.rotation.eulerAngles.y - 360 :
                    (target.rotation.eulerAngles.y < -180 ? target.rotation.eulerAngles.y + 360 : target.rotation.eulerAngles.y);

                // �׳� ���ʹϾ����� ������ �� �ȵǴ��� �˾Ƴ�����.
                myImgTransform.rotation = Quaternion.Euler(0, 0, -myCurEulerY);
                otherImgTransform.rotation = Quaternion.Euler(0, 0, -otherCurEulerY);
            }

        }
    }
}

