using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using YC.Photon;

namespace JCW.UI.InGame.Indicator
{
    public class OneIndicator : BaseIndicator
    {
        [Header("UI�� ���")] [SerializeField] Transform target;
        // �⺻ ��������Ʈ
        [Header("�ڶ� ���� ��������Ʈ")] [SerializeField] protected Sprite nella_DetectSprite;
        [Header("���׵� ���� ��������Ʈ")] [SerializeField] protected Sprite steady_DetectSprite;

        protected void Awake()
        {
            detectUI = transform.GetChild(0).gameObject;
            imgTransform = detectUI.transform.GetChild(0).GetComponent<RectTransform>();
            gauge = detectUI.transform.GetChild(1).GetComponent<Image>();


            // ������ ������ ��������Ʈ ũ�⸸ŭ ���� ����
            imgTransform.sizeDelta = new Vector2(nella_DetectSprite.bounds.size.x, nella_DetectSprite.bounds.size.y);
            interactiveImg = imgTransform.gameObject.GetComponent<Image>();
            

            canvasSize = detectUI.GetComponent<RectTransform>();
            screenLimitOffset = imgTransform.rect.width * 0.4f;
            outOfSightImgScale = imgTransform.localScale * 0.8f;
            initImgScale = imgTransform.localScale;            
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(nameof(Wait));
        }

        private void Update()
        {
            if (!isStart || !isActive || !mainCamera)
                return;
            Vector3 indicatorPosition = mainCamera.WorldToScreenPoint(target.position);


            // Ÿ���� ī�޶� �տ� ���� ��
            if (indicatorPosition.z >= 0f)
            {
                // Ÿ���� ȭ�� �ȿ� ���� ��
                if (indicatorPosition.x <= screenSize.x + screenSize.width && indicatorPosition.x >= screenSize.x
                   && indicatorPosition.y <= screenSize.y + screenSize.height && indicatorPosition.y >= screenSize.y)
                {
                    imgTransform.localScale = initImgScale;
                    gauge.transform.localScale = initImgScale;
                    indicatorPosition.z = 0f;
                }
                else
                    indicatorPosition = OutOfRange(indicatorPosition);
            }
            else
            {
                // ȭ�� �ڿ� ���� ��, ��ġ ���������� ���� ���� ���� ����
                indicatorPosition *= -1f;
                indicatorPosition = OutOfRange(indicatorPosition);
            }

            imgTransform.position = indicatorPosition;
            gauge.transform.position = indicatorPosition;
        }

        // ���� : ��������Ʈ�� �ϳ��� ����. ���� ��ȯ �ִϸ��̼� �ʿ� ����
        // Ÿ���� �ٸ� ��ũ��Ʈ���� ����, ������ϴ� ��ġ�� ������ ������Ʈ�� Ư�� ��ġ�� �ƴϹǷ�, �ǽð����� ����� �� ����.
        public void SetUI(bool _isActive, Vector3 _pos)
        {
            // _isActive : UI�� �Ѿ��ϴ� ��,  _pos : ������ϴ� ��ġ, _cam : ������ϴ� ȭ���� ī�޶�
            detectUI.SetActive(_isActive);
            isActive = _isActive;
            if (target == null)
                target = transform;
            target.position = _pos;

            if (isActive)
                SetSreenInfo();
        }
        // �� �� : ��������Ʈ �ϳ��� ���� / ���� ��ȯ�ϴ� �ִϸ��̼� X
        // Ÿ���� �ν����� â���� �־���, ���� �¾Ƽ� �ڶ󳪴� �Ĺ��� �־��ָ� �ɵ�
        public void SetUI(bool _isUIActive)
        {
            // _isActive : UI�� �Ѿ��ϴ� ��, _cam : ������ϴ� ȭ���� ī�޶�
            detectUI.SetActive(_isUIActive);
            isActive = _isUIActive;

            if (isActive)
                SetSreenInfo();
        }

        public void SetGauge(float value)
        {
            gauge.fillAmount = value;
        }

        IEnumerator Wait()
        {
            yield return new WaitUntil(() => isStart == true);
            interactiveImg.sprite = isNella ? nella_DetectSprite : steady_DetectSprite;
            yield break;
        }
    }
}