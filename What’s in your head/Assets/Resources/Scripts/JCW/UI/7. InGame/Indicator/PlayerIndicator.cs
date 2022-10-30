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
        RectTransform canvasRect;
        RectTransform myImgTransform;
        // �� �÷��̾� ��ġ
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
            // Ÿ���� ��ġ�� ����ī�޶��� ��ũ�� ��ǥ�� ����
            Vector3 indicatorPosition = mainCamera.WorldToScreenPoint(target.position);
            if (!GameManager.Instance.isTopView)
            {
                if(wasTopView)
                {
                    wasTopView = false;
                    otherImg.sprite = otherIndicatorNormal;
                    myImg.enabled = false;
                }
                // Ÿ���� ȭ�� �ȿ� ���� ��
                if (indicatorPosition.z >= 0f)
                {
                    if (indicatorPosition.x <= screenSize.x + screenSize.width && indicatorPosition.x >= screenSize.x
                       && indicatorPosition.y <= screenSize.y + screenSize.height && indicatorPosition.y >= screenSize.y)
                    {
                        //Debug.Log("ȭ�� �ȿ� ����");
                        otherImg.enabled = false;
                        indicatorPosition.z = 0f;
                    }
                    else
                    {
                        //Debug.Log("ī�޶� ���ε� ȭ�� �ȿ� �� ����");
                        otherImg.enabled = true;
                        indicatorPosition = OutOfRange(indicatorPosition);
                    }
                }
                //Ÿ���� �� ī�޶� �ڿ� ���� ��, ȭ�鿡 �׸�
                else
                {
                    //Debug.Log("ī�޶� �ڶ� ȭ�� �ȿ� �� ����");
                    imgTransform.sizeDelta = new Vector2(otherIndicatorNormal.bounds.size.x, otherIndicatorNormal.bounds.size.y);
                    otherImg.enabled = true;
                    SetSreenInfo();
                    indicatorPosition *= -1f;
                    indicatorPosition = OutOfRange(indicatorPosition);
                }
            }            
            // ž���϶�
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
                // ������ ����
                Vector3 myIndicatorPosition = mainCamera.WorldToScreenPoint(myTF.position);

                myImgTransform.position = myIndicatorPosition;
                imgTransform.position = indicatorPosition;

                // ���� ����
                // �÷��̾��� Rotation.y���� UI�� Rotation.z���� �����Ǿ����.

                float myCurEulerY = myTF.rotation.eulerAngles.y > 180 ? myTF.rotation.eulerAngles.y - 360 :
                    (myTF.rotation.eulerAngles.y < -180 ? myTF.rotation.eulerAngles.y + 360 : myTF.rotation.eulerAngles.y);

                float otherCurEulerY = target.rotation.eulerAngles.y > 180 ? target.rotation.eulerAngles.y - 360 :
                    (target.rotation.eulerAngles.y < -180 ? target.rotation.eulerAngles.y + 360 : target.rotation.eulerAngles.y);

                // �׳� ���ʹϾ����� ������ �� �ȵǴ��� �˾Ƴ�����.
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



            // ��� ���϶��� ���� =======================================================================================
            // ������ ������ ��������Ʈ ũ�⸸ŭ ���� ����
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

