using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
namespace JCW.UI.InGame.Indicator
{
    public class ConvertIndicator : BaseIndicator
    {
        [Header("UI의 대상")] [SerializeField] Transform target;
        [Header("넬라 - 감지 & 상호작용 스프라이트 및 클립")] [SerializeField] Sprite nella_DetectSprite;
        [SerializeField] Sprite nella_InteractableSprite;
        [Tooltip("변환하는 경우에만 넣어주는 애니메이션 영상")] [SerializeField] VideoClip nella_SetOnClip;
        [SerializeField] VideoClip nella_SetOffClip;
        [Header("스테디 - 감지 & 상호작용 스프라이트 및 클립")] [SerializeField] Sprite steady_DetectSprite;
        [SerializeField] Sprite steady_InteractableSprite;
        [Tooltip("변환하는 경우에만 넣어주는 애니메이션 영상")] [SerializeField] VideoClip steady_SetOnClip;
        [SerializeField] VideoClip steady_SetOffClip;

        VideoPlayer videoPlayer;

        float detectRange;
        float interactableRange;
        //상호작용 가능한지
        bool isInteractable;

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

            videoPlayer = imgTransform.GetChild(0).GetComponent<VideoPlayer>();

        }

        override protected void Start()
        {
            base.Start();
            interactiveImg.sprite = isNella ? nella_DetectSprite : steady_DetectSprite;
        }

        private void Update()
        {
            if (mainCamera == null)
                SetCam();
            if (!isActive)
                return;
            //detectRange = transform.lossyScale.x / 2f;
            // 타겟의 위치를 메인카메라의 스크린 좌표로 변경
            Vector3 indicatorPosition = mainCamera.WorldToScreenPoint(target.position);


            // 타겟이 카메라 앞에 있을 때
            if (indicatorPosition.z >= 0f)
            {
                // 타겟이 화면 안에 들어올 때
                if (indicatorPosition.x <= screenSize.x + screenSize.width && indicatorPosition.x >= screenSize.x
                   && indicatorPosition.y <= screenSize.y + screenSize.height && indicatorPosition.y >= screenSize.y)
                {
                    imgTransform.localScale = initImgScale;
                    if(gauge!=null)
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
            if(gauge!=null)
                gauge.transform.position = indicatorPosition;
        }

        // 현재 RopeSpawner에서 참조
        public void SetInteractableRange(float _detectRange, float _interactableRange)
        {
            detectRange = _detectRange;
            interactableRange = _interactableRange;
        }

        // 로프용 : 감지 & 상호작용 스프라이트 존재 / 따라서 변환하는 애니메이션도 존재
        // 타겟은 인스펙터 창에서 넣어줌
        public void SetUI(bool isUIActive, bool isSetOn, float distance)
        {
            detectUI.SetActive(isUIActive);
            isActive = isUIActive;

            if (isActive)
            {
                videoPlayer.targetCamera = mainCamera;
                SetSreenInfo();
                // 상호작용 범위 밖->안 & 안->밖 들어갔을 때만 애니메이션 재생과 함께 스프라이트 변경
                if (isInteractable != isSetOn)
                {
                    isInteractable = isSetOn;
                    ConvertVideo(isInteractable);
                }

                // 게이지 필요 없으면 실행 X
                if (gauge != null && !isSetOn)
                {                    
                    // 거리에 따라 게이지 줄어들게 끔 해주기
                    // 1 - (_dist-상호작용 범위)/(감지범위 - 상호작용 범위) == FillValue에 넣어줌.
                    gauge.fillAmount = 1f - (distance - interactableRange) / (detectRange - interactableRange);
                }
            }
        }

        public void ConvertVideo(bool _isSetOn)
        {
            // 이미지를 잠깐 꺼주고 동영상 켜주기
            if(gauge != null)
                gauge.enabled = !_isSetOn;
            interactiveImg.enabled = false;
            videoPlayer.Stop();
            StopCoroutine(nameof(PlayVideoClip));
            videoPlayer.gameObject.GetComponent<RawImage>().enabled = true;
            if (isNella)
                videoPlayer.clip = _isSetOn ? nella_SetOnClip : nella_SetOffClip;
            else
                videoPlayer.clip = _isSetOn ? steady_SetOnClip : steady_SetOffClip;

            StartCoroutine(nameof(PlayVideoClip), _isSetOn);
        }

        IEnumerator PlayVideoClip(bool _isSetOn)
        {
            videoPlayer.Play();
            yield return new WaitForSeconds(1f);
            videoPlayer.Stop();
            if (isNella)
                interactiveImg.sprite = _isSetOn ? nella_InteractableSprite : nella_DetectSprite;
            else
                interactiveImg.sprite = _isSetOn ? steady_InteractableSprite : steady_DetectSprite;

            // 동영상 끄고 이미지 켜주기
            videoPlayer.gameObject.GetComponent<RawImage>().enabled = false;
            interactiveImg.enabled = true;


            yield return null;
        }
    }
}