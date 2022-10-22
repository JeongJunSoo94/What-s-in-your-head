using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
namespace JCW.UI.InGame.Indicator
{
    public class ConvertIndicator : BaseIndicator
    {
        [Header("UI�� ���")] [SerializeField] Transform target;
        [Header("�ڶ� - ���� & ��ȣ�ۿ� ��������Ʈ �� Ŭ��")] [SerializeField] new Sprite nella_DetectSprite;
        [SerializeField] Sprite nella_InteractableSprite;
        [Tooltip("��ȯ�ϴ� ��쿡�� �־��ִ� �ִϸ��̼� ����")] [SerializeField] VideoClip nella_SetOnClip;
        [SerializeField] VideoClip nella_SetOffClip;
        [Header("���׵� - ���� & ��ȣ�ۿ� ��������Ʈ �� Ŭ��")] [SerializeField] new Sprite steady_DetectSprite;
        [SerializeField] Sprite steady_InteractableSprite;
        [Tooltip("��ȯ�ϴ� ��쿡�� �־��ִ� �ִϸ��̼� ����")] [SerializeField] VideoClip steady_SetOnClip;
        [SerializeField] VideoClip steady_SetOffClip;

        VideoPlayer videoPlayer;

        float detectRange;
        float interactableRange;
        //��ȣ�ۿ� ��������
        bool isInteractable;

        void Awake()
        {
            detectUI = transform.GetChild(0).gameObject;
            imgTransform = detectUI.transform.GetChild(0).GetComponent<RectTransform>();


            // ������ ������ ��������Ʈ ũ�⸸ŭ ���� ����
            imgTransform.sizeDelta = new Vector2(nella_DetectSprite.bounds.size.x, nella_DetectSprite.bounds.size.y);
            interactiveImg = imgTransform.gameObject.GetComponent<Image>();
            interactiveImg.sprite = nella_DetectSprite;

            canvasSize = detectUI.GetComponent<RectTransform>();
            screenLimitOffset = imgTransform.rect.width * 0.4f;
            outOfSightImgScale = imgTransform.localScale * 0.8f;
            initImgScale = imgTransform.localScale;

            videoPlayer = imgTransform.GetChild(0).GetComponent<VideoPlayer>();

            // �������� ����� ���� �Ʒ� �ڵ� ����
            //isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];

            // �ӽ�
            isNella = true;
        }

        private void Update()
        {
            if (!isActive)
                return;
            detectRange = transform.lossyScale.x / 2f;
            // Ÿ���� ��ġ�� ����ī�޶��� ��ũ�� ��ǥ�� ����
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

        // ���� RopeSpawner���� ����
        public void SetInteractableRange(float _range)
        {
            interactableRange = _range;
        }

        // ������ : ���� & ��ȣ�ۿ� ��������Ʈ ���� / ���� ��ȯ�ϴ� �ִϸ��̼ǵ� ����
        // Ÿ���� �ν����� â���� �־���
        public void SetUI(bool isUIActive, bool isSetOn, float distance, Camera _cam)
        {
            detectUI.SetActive(isUIActive);
            isActive = isUIActive;

            if (isActive)
            {
                videoPlayer.targetCamera = _cam;
                mainCamera = videoPlayer.targetCamera;
                SetSreenInfo();
                // ��ȣ�ۿ� ���� ��->�� & ��->�� ���� ���� �ִϸ��̼� ����� �Բ� ��������Ʈ ����
                if (isInteractable != isSetOn)
                {
                    isInteractable = isSetOn;
                    ConvertVideo(isInteractable);
                }

                // ������ �ʿ� ������ ���� X
                if (gauge != null && !isSetOn)
                {
                    // �Ÿ��� ���� ������ �پ��� �� ���ֱ�
                    // 1 - (_dist-��ȣ�ۿ� ����)/(�������� - ��ȣ�ۿ� ����) == FillValue�� �־���.
                    gauge.fillAmount = 1 - (distance - interactableRange) / (detectRange - interactableRange);
                }
            }
        }

        public void ConvertVideo(bool _isSetOn)
        {
            // �̹����� ��� ���ְ� ������ ���ֱ�
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

            // ������ ���� �̹��� ���ֱ�
            videoPlayer.gameObject.GetComponent<RawImage>().enabled = false;
            interactiveImg.enabled = true;


            yield return null;
        }
    }
}