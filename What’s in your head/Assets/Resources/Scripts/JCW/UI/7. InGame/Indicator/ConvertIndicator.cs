using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using YC.Photon;

namespace JCW.UI.InGame.Indicator
{
    public class ConvertIndicator : BaseIndicator
    {
        [Header("UI�� ���")] [SerializeField] Transform target;
        [Header("�ڶ� - ���� & ��ȣ�ۿ� ��������Ʈ �� Ŭ��")] [SerializeField] Sprite nella_DetectSprite;
        [SerializeField] Sprite nella_InteractableSprite;
        [Tooltip("��ȯ�ϴ� ��쿡�� �־��ִ� �ִϸ��̼� ����")] [SerializeField] VideoClip nella_SetOnClip;
        [SerializeField] VideoClip nella_SetOffClip;
        [Header("���׵� - ���� & ��ȣ�ۿ� ��������Ʈ �� Ŭ��")] [SerializeField] Sprite steady_DetectSprite;
        [SerializeField] Sprite steady_InteractableSprite;
        [Tooltip("��ȯ�ϴ� ��쿡�� �־��ִ� �ִϸ��̼� ����")] [SerializeField] VideoClip steady_SetOnClip;
        [SerializeField] VideoClip steady_SetOffClip;

        VideoPlayer videoPlayer;

        float detectRange;
        float interactableRange;
        //��ȣ�ۿ� ��������
        bool isInteractable;

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

            videoPlayer = imgTransform.GetChild(0).GetComponent<VideoPlayer>();

        }

        override protected void Start()
        {
            base.Start();
            StartCoroutine(nameof(Wait));
        }

        private void Update()
        {
            if (isStart == false)
                return;
            if (!isActive)
                return;
            //detectRange = transform.lossyScale.x / 2f;
            // Ÿ���� ��ġ�� ����ī�޶��� ��ũ�� ��ǥ�� ����

            SetSreenInfo();
            if (mainCamera.rect.width <= 0.1f)
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
                    if(gauge!=null)
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
            if(gauge!=null)
                gauge.transform.position = indicatorPosition;
        }

        // ���� RopeSpawner���� ����
        public void SetInteractableRange(float _detectRange, float _interactableRange)
        {
            detectRange = _detectRange;
            interactableRange = _interactableRange;
        }

        // ������ : ���� & ��ȣ�ۿ� ��������Ʈ ���� / ���� ��ȯ�ϴ� �ִϸ��̼ǵ� ����
        // Ÿ���� �ν����� â���� �־���
        public void SetUI(bool isUIActive, bool isSetOn, float distance)
        {
            detectUI.SetActive(isUIActive);
            isActive = isUIActive;

            if (isActive)
            {
                videoPlayer.targetCamera = mainCamera;
                SetSreenInfo();
                // ������ �ʿ� ������ ���� X / �Ÿ��� ���� ������ �پ��Բ�
                if (gauge != null && !isSetOn)
                    gauge.fillAmount = 1f - (distance - interactableRange) / (detectRange - interactableRange);

                // ��ȣ�ۿ� ���� ��->�� & ��->�� ���� ���� �ִϸ��̼� ����� �Բ� ��������Ʈ ����
                if (isInteractable != isSetOn)
                {
                    isInteractable = isSetOn;
                    ConvertVideo(isInteractable);
                }
            }
        }

        public void ConvertVideo(bool _isSetOn)
        {
            // �̹����� ��� ���ְ� ������ ���ֱ�
            if (gauge != null && _isSetOn)
                gauge.enabled = false;

            interactiveImg.enabled = false;
            videoPlayer.Stop();
            StopAllCoroutines();
            videoPlayer.gameObject.GetComponent<RawImage>().enabled = true;
            if (isNella)
                videoPlayer.clip = _isSetOn ? nella_SetOnClip : nella_SetOffClip;
            else
                videoPlayer.clip = _isSetOn ? steady_SetOnClip : steady_SetOffClip;

            StartCoroutine(nameof(PlayVideoClip), _isSetOn);
        }

        IEnumerator PlayVideoClip(bool _isSetOn)
        {
            videoPlayer.Prepare();
            yield return new WaitUntil(() => videoPlayer.isPrepared);
            videoPlayer.Play();
            yield return new WaitUntil(() => !videoPlayer.isPlaying);
            if (isNella)
                interactiveImg.sprite = _isSetOn ? nella_InteractableSprite : nella_DetectSprite;
            else
                interactiveImg.sprite = _isSetOn ? steady_InteractableSprite : steady_DetectSprite;

            // ������ ���� �̹��� ���ֱ�
            videoPlayer.gameObject.GetComponent<RawImage>().enabled = false;
            interactiveImg.enabled = true;

            if (gauge != null && !_isSetOn)
                gauge.enabled = true;

            yield break;
        }

        IEnumerator Wait()
        {
            yield return new WaitUntil(() => isStart == true);
            interactiveImg.sprite = isNella ? nella_DetectSprite : steady_DetectSprite;
            yield break;
        }
    }
}