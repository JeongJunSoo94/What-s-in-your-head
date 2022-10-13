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
        [Header("UI")] [SerializeField] GameObject detectUI;
        [Header("Ÿ�� ������Ʈ")] [SerializeField] GameObject target;
        [Header("�̹��� Ʈ������")] [SerializeField] RectTransform imgTransform;
        [Header("Ŭ�� �÷��̾�")] [SerializeField] VideoPlayer videoPlayer;

        private RectTransform canvasSize;
        private Camera mainCamera;
        
                
        bool isDetected = false;
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
            
        }
        private void Update()
        {
            if (!isDetected)
                return;

            // Ÿ���� ��ġ�� ����ī�޶��� ��ũ�� ��ǥ�� ����
            Vector3 indicatorPosition = mainCamera.WorldToScreenPoint(target.transform.position);


            // Ÿ���� ī�޶� �տ� ���� ��
            if (indicatorPosition.z >= 0f)
            {
                // Ÿ���� ȭ�� �ȿ� ���� ��
                if (indicatorPosition.x <= screenSize.width && indicatorPosition.x >= 0f
                   && indicatorPosition.y <= screenSize.height && indicatorPosition.y >= 0f)
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
        private Vector3 OutOfRange(Vector3 indicatorPosition)
        {
            imgTransform.localScale = outOfSightImgScale;
            indicatorPosition.z = 0f;

            // ���� ī�޶� ȭ���� �߽� ��ġ ���
            Vector3 canvasCenter = new Vector3(screenSize.width / 2f, screenSize.height / 2f, 0f);

            // UI ��ġ-> ȭ�� �߽� ����
            indicatorPosition -= canvasCenter;

            // ȭ�� ���� ����
            float divX = (screenSize.width / 2f - screenLimitOffset) / Mathf.Abs(indicatorPosition.x);
            float divY = (screenSize.height / 2f - screenLimitOffset) / Mathf.Abs(indicatorPosition.y);

            // x ��ǥ�� ���� �׵θ��� ����� ��
            if (divX < divY)
            {
                // x�࿡ ���� ����
                float angle = Vector3.SignedAngle(Vector3.right, indicatorPosition, Vector3.forward);
                indicatorPosition.x = Mathf.Sign(indicatorPosition.x) * (screenSize.width / 2f - screenLimitOffset);
                indicatorPosition.y = Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.x;
            }
            else
            {
                // y�࿡ ���� ����
                float angle = Vector3.SignedAngle(Vector3.up, indicatorPosition, Vector3.forward);

                indicatorPosition.y = Mathf.Sign(indicatorPosition.y) * (screenSize.height / 2f - screenLimitOffset);
                indicatorPosition.x = -Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.y;
            }

            // ���󺹱�
            indicatorPosition += canvasCenter;
            return indicatorPosition;
        }


        // ������ �ӽ÷� Ʈ���� Enter/Exit���� �ϰ� ������
        // �������� ����� ���� �÷��̾ ���̸� ���� �Ÿ��� ���� �� ���� ���Ѿ���.
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                // �ڱⲨ�϶��� �ѱ�
                if (other.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    // ���⼭�� �ٷ� ���ִ°ɷ� �Ǿ�������, �������� ����� ���� �÷��̾ ���̸� ��� �Լ��� ����� ��.
                    // other.gameObject.SendMessage("���� ��� �Լ�", �Ű�����-������Ʈ);
                    Detected(other.gameObject);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            isDetected = false;
            detectUI.SetActive(false);
        }

        public void Detected(GameObject player)
        {
            detectUI.SetActive(true);
            isDetected = true;
            mainCamera = player.GetComponent<CameraController>().FindCamera();
            videoPlayer.targetCamera = mainCamera;
            Init();
        }

        void Init()
        {
            Rect cameraPos = mainCamera.rect;
            screenSize = new(canvasSize.rect.width  * cameraPos.x,
                             canvasSize.rect.height * cameraPos.y,
                             canvasSize.rect.width  * cameraPos.width,
                             canvasSize.rect.height * cameraPos.height);
        }
        
    }
}
