using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

using KSU;
using Photon.Pun;

namespace JCW.UI.InGame
{

    public class TargetIndicator : MonoBehaviour
    {

        [Header("UI")] [SerializeField] GameObject detectUI;
        [Header("Ÿ�� ������Ʈ�� ��ġ")] [SerializeField] Transform target;
        [Header("�̹��� Ʈ������")] [SerializeField] RectTransform imgTransform;
        [Header("������")] [SerializeField] Image gauge;
        [Header("Ŭ�� �÷��̾�")] [SerializeField] VideoPlayer videoPlayer;

        [Header("�ڶ� - ���� & ��ȣ�ۿ� ��������Ʈ �� Ŭ��")]
        [SerializeField] Sprite nella_DetectSprite;
        [SerializeField] Sprite nella_InteractableSprite;
        [SerializeField] VideoClip nella_SetOnClip;
        [SerializeField] VideoClip nella_SetOffClip;
        [Header("���׵� - ���� & ��ȣ�ۿ� ��������Ʈ �� Ŭ��")]
        [SerializeField] Sprite steady_DetectSprite;
        [SerializeField] Sprite steady_InteractableSprite;
        [SerializeField] VideoClip steady_SetOnClip;
        [SerializeField] VideoClip steady_SetOffClip;

        private RectTransform canvasSize;
        private Camera mainCamera;

        // ���� ���� & ��ȣ�ۿ� ���� ����
        float detectRange;
        float interactableRange;

        // ����<->��ȣ�ۿ� ��������Ʈ
        Image interactiveSprite;


        // ���� �÷��̾ �����Ǵ� ��
        bool isActive = false;
        bool isInteractable = false;

        // ���� ĳ���Ͱ� �ڶ�����
        bool isNella;


        Rect screenSize;
        // ȭ�鿡�� ����� ���ϰ� �ϴ� ������ ��
        private float screenLimitOffset;

        // ȭ�� �ȿ� ���� ���� ����� ���� �̹��� ũ��
        Vector3 initImgScale;
        Vector3 outOfSightImgScale;



        private void Awake()
        {
            // ������ ������ ��������Ʈ ũ�⸸ŭ ���� ����
            imgTransform.sizeDelta = new Vector2(nella_DetectSprite.bounds.size.x, nella_DetectSprite.bounds.size.y);
            interactiveSprite = imgTransform.gameObject.GetComponent<Image>();
            interactiveSprite.sprite = nella_DetectSprite;

            canvasSize = detectUI.GetComponent<RectTransform>();
            screenLimitOffset = imgTransform.rect.width * 0.4f;
            outOfSightImgScale = imgTransform.localScale * 0.8f;
            initImgScale = imgTransform.localScale;

            // �������� ����� ���� �Ʒ� �ڵ� ����
            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];

            // �ӽ�
            //isNella = true;
        }
        protected void Update()
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

        // ���Ͽ� : ��������Ʈ�� �ϳ��� ����. ���� ��ȯ �ִϸ��̼� �ʿ� ����
        // Ÿ���� �ٸ� ��ũ��Ʈ���� ����
        public void SetUI(bool _isActive, Vector3 _pos, Camera _cam)
        {
            // _isActive : UI�� �Ѿ��ϴ� ��, _isSetOn : ��ȣ�ۿ��ؾ��ϴ� ��, _pos : ������ϴ� ��ġ, _cam : ������ϴ� ȭ���� ī�޶�
            detectUI.SetActive(_isActive);
            isActive = _isActive;
            if (target == null)
                target = transform;
            target.position = _pos;

            if (isActive)
            {
                mainCamera = _cam;
                SetSreenInfo();
            }
        }
        // �� �� : ��������Ʈ �ϳ��� ���� / ���� ��ȯ�ϴ� �ִϸ��̼� X
        // Ÿ���� �ν����� â���� �־���
        public void SetUI(bool _isUIActive, float gaugeValue, Camera _cam)
        {
            // _isActive : UI�� �Ѿ��ϴ� ��, _isSetOn : ��ȣ�ۿ��ؾ��ϴ� ��, _pos : ������ϴ� ��ġ, _cam : ������ϴ� ȭ���� ī�޶�
            detectUI.SetActive(_isUIActive);
            isActive = _isUIActive;

            if (isActive)
            {
                mainCamera = _cam;
                SetSreenInfo();
                // ������ �ʿ� ������ ���� X
                if (gauge != null && !isInteractable)
                    gauge.fillAmount = gaugeValue;
            }
        }

        // ���� RopeSpawner���� ����
        public void SetInteractableRange(float _range)
        {
            interactableRange = _range;
        }

        public void ConvertVideo(bool _isSetOn)
        {
            // �̹����� ��� ���ְ� ������ ���ֱ�
            interactiveSprite.enabled = false;
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
                interactiveSprite.sprite = _isSetOn ? nella_InteractableSprite : nella_DetectSprite;
            else
                interactiveSprite.sprite = _isSetOn ? steady_InteractableSprite : steady_DetectSprite;

            // ������ ���� �̹��� ���ֱ�
            videoPlayer.gameObject.GetComponent<RawImage>().enabled = false;
            interactiveSprite.enabled = true;


            yield return null;
        }

        // ��ũ�� ������
        void SetSreenInfo()
        {
            Rect cameraPos = mainCamera.rect;
            screenSize = new(canvasSize.rect.width * cameraPos.x,
                             canvasSize.rect.height * cameraPos.y,
                             canvasSize.rect.width * cameraPos.width,
                             canvasSize.rect.height * cameraPos.height);
        }

    }
}