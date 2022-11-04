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
        [Header("UI의 대상")] [SerializeField] Transform target;
        // 기본 스프라이트
        [Header("넬라 전용 스프라이트")] [SerializeField] protected Sprite nella_DetectSprite;
        [Header("스테디 전용 스프라이트")] [SerializeField] protected Sprite steady_DetectSprite;

        protected void Awake()
        {
            detectUI = transform.GetChild(0).gameObject;
            imgTransform = detectUI.transform.GetChild(0).GetComponent<RectTransform>();
            gauge = detectUI.transform.GetChild(1).GetComponent<Image>();


            // 기존에 설정된 스프라이트 크기만큼 범위 조절
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


            // 타겟이 카메라 앞에 있을 때
            if (indicatorPosition.z >= 0f)
            {
                // 타겟이 화면 안에 들어올 때
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
                // 화면 뒤에 있을 때, 위치 뒤집어지는 것을 막기 위한 설정
                indicatorPosition *= -1f;
                indicatorPosition = OutOfRange(indicatorPosition);
            }

            imgTransform.position = indicatorPosition;
            gauge.transform.position = indicatorPosition;
        }

        // 레일 : 스프라이트는 하나만 있음. 따라서 변환 애니메이션 필요 없음
        // 타겟을 다른 스크립트에서 받음, 띄워야하는 위치가 고정된 오브젝트의 특정 위치가 아니므로, 실시간으로 변경될 수 있음.
        public void SetUI(bool _isActive, Vector3 _pos)
        {
            // _isActive : UI를 켜야하는 지,  _pos : 띄워야하는 위치, _cam : 띄워야하는 화면의 카메라
            detectUI.SetActive(_isActive);
            isActive = _isActive;
            if (target == null)
                target = transform;
            target.position = _pos;

            if (isActive)
                SetSreenInfo();
        }
        // 그 외 : 스프라이트 하나만 존재 / 따라서 변환하는 애니메이션 X
        // 타겟은 인스펙터 창에서 넣어줌, 물총 맞아서 자라나는 식물에 넣어주면 될듯
        public void SetUI(bool _isUIActive)
        {
            // _isActive : UI를 켜야하는 지, _cam : 띄워야하는 화면의 카메라
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