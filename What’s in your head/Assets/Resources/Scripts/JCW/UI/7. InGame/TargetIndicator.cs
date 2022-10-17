using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using YC.Camera_;

namespace JCW.UI.InGame
{
    public class TargetIndicator : MonoBehaviour
    {
        [Header("���� ���� ��ġ")] [SerializeField] [Range(0, 100)] float range;
        [Header("��ȣ�ۿ� ���� ���� ��ġ")] [SerializeField] [Range(0, 100)] float interactableRange;
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
            canvasSize = detectUI.GetComponent<RectTransform>();
            transform.localScale = new Vector3(range, range, range);
            screenLimitOffset = imgTransform.rect.width * 0.4f;
            outOfSightImgScale = imgTransform.localScale * 0.8f;
            initImgScale = imgTransform.localScale;
            interactiveSprite = imgTransform.gameObject.GetComponent<Image>();

            // �������� ����� ���� �Ʒ� �ڵ� ����
            //isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];

            // �ӽ�
            isNella = true;

            Debug.Log("Ÿ�� ���ñ� ����");
        }
        protected void Update()
        {
            if (!isActive)
                return;

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
        }



        // ī�޶� ������ ����� ���� ���� ����
        protected Vector3 OutOfRange(Vector3 indicatorPosition)
        {
            imgTransform.localScale = outOfSightImgScale;
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

        public void SetUI(bool _isActive, bool _isSetOn, float _dist, Camera _cam)
        {
            detectUI.SetActive(_isActive);
            isActive = _isActive;

            if (isActive)
            {
                videoPlayer.targetCamera = _cam;
                mainCamera = videoPlayer.targetCamera;
                SetSreenInfo();
                // ��ȣ�ۿ� ���� ��->�� & ��->�� ���� ���� �ִϸ��̼� ����� �Բ� ��������Ʈ ����
                if (isInteractable != _isSetOn)
                {
                    isInteractable = _isSetOn;
                    ConvertVideo(_isSetOn);
                }
                
                if(!isInteractable)
                {
                    // �Ÿ��� ���� ������ �پ��� �� ���ֱ�
                    // 1 - (_dist-��ȣ�ۿ� ����)/(�������� - ��ȣ�ۿ� ����) == FillValue�� �־���.
                    gauge.fillAmount = 1 - (_dist - interactableRange) / (range - interactableRange);
                }
            }
        }

        public void SetUI(bool _isActive, bool _isSetOn, Vector3 _pos, Camera _cam)
        {
            detectUI.SetActive(_isActive);
            isActive = _isActive;
            if (target == null)
                target = transform;
            target.position = _pos;

            if (isActive)
            {
                videoPlayer.targetCamera = _cam;
                mainCamera = videoPlayer.targetCamera;
                SetSreenInfo();
                // ��ȣ�ۿ� ���� ��->�� & ��->�� ���� ���� �ִϸ��̼� ����� �Բ� ��������Ʈ ����
                if (isInteractable != _isSetOn)
                {
                    isInteractable = _isSetOn;
                    ConvertVideo(_isSetOn);
                }
            }
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
