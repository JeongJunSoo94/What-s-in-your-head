using Photon.Pun;
using UnityEngine;
using YC.Camera_;

namespace JCW.UI.InGame
{
    public class TargetIndicator : MonoBehaviour
    {
        public float OutOfSightOffset = 5f;
        [Header("���� ���� ��ġ")] [SerializeField] [Range(0, 100)] float range;
        [Header("UI")] [SerializeField] GameObject detectUI;
        [Header("Ÿ�� ������Ʈ")] [SerializeField] GameObject target;
        //[Header("�׷��� ĵ���� Ʈ������")] [SerializeField] private RectTransform canvasRect;
        [Header("�̹��� Ʈ������")] [SerializeField] private RectTransform imgTransform;

        private RectTransform canvasRect;
        private Camera mainCamera;
        bool isDetected = false;
        Rect cameraPos;
        Rect relativeCamPos;

        private void Awake()
        {
            canvasRect = detectUI.GetComponent<RectTransform>();
            transform.localScale = new Vector3(range, range, range);
            //canvas = canvasRect.gameObject.GetComponent<Canvas>();
        }
        private void Update()
        {
            if (!isDetected)
                return;
            Vector3 indicatorPosition = mainCamera.WorldToScreenPoint(target.transform.position);


            // �տ� �ְ�, x�� ȭ�� �ʺ� �ȿ� ���� �� , y�� ȭ�� ���� �ȿ� ���� ��
            if (indicatorPosition.z >= 0f)
            {
                if (indicatorPosition.x <= relativeCamPos.width &&
                    indicatorPosition.y <= relativeCamPos.height &&
                    indicatorPosition.x >= 0f && indicatorPosition.y >= 0f)
                {
                    //Set z to zero since it's not needed and only causes issues (too far away from Camera to be shown!)
                    indicatorPosition.z = 0f;
                }
                else
                {
                    //In case the target is in front of the ship, but out of sight
                    indicatorPosition = OutOfRangeindicatorPositionB(indicatorPosition);
                }
            }
            else
            {
                // ȭ�� �ڿ� ���� ��, ��ġ ���������� ���� ���� ���� ����
                indicatorPosition *= -1f;

                //Set indicatorposition and set targetIndicator to outOfSight form.
                indicatorPosition = OutOfRangeindicatorPositionB(indicatorPosition);

            }

            imgTransform.position = indicatorPosition;
        }




        private Vector3 OutOfRangeindicatorPositionB(Vector3 indicatorPosition)
        {
            // ī�޶� ������ ����� ���� ���� ����
            indicatorPosition.z = 0f;

            //Calculate Center of Canvas and subtract from the indicator position to have indicatorCoordinates from the Canvas Center instead the bottom left!
            Vector3 canvasCenter = new Vector3(relativeCamPos.width / 2f, relativeCamPos.height / 2f, 0f);
            indicatorPosition -= canvasCenter;

            //Calculate if Vector to target intersects (first) with y border of canvas rect or if Vector intersects (first) with x border:
            //This is required to see which border needs to be set to the max value and at which border the indicator needs to be moved (up & down or left & right)
            float divX = (relativeCamPos.width / 2f - OutOfSightOffset) / Mathf.Abs(indicatorPosition.x);
            float divY = (relativeCamPos.height / 2f - OutOfSightOffset) / Mathf.Abs(indicatorPosition.y);

            //In case it intersects with x border first, put the x-one to the border and adjust the y-one accordingly (Trigonometry)
            if (divX < divY)
            {
                float angle = Vector3.SignedAngle(Vector3.right, indicatorPosition, Vector3.forward);
                indicatorPosition.x = Mathf.Sign(indicatorPosition.x) * (relativeCamPos.width * 0.5f - OutOfSightOffset);
                indicatorPosition.y = Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.x;
            }

            //In case it intersects with y border first, put the y-one to the border and adjust the x-one accordingly (Trigonometry)
            else
            {
                float angle = Vector3.SignedAngle(Vector3.up, indicatorPosition, Vector3.forward);

                indicatorPosition.y = Mathf.Sign(indicatorPosition.y) * (relativeCamPos.height / 2f - OutOfSightOffset);
                indicatorPosition.x = -Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.y;
            }

            //Change the indicator Position back to the actual imgTransform coordinate system and return indicatorPosition
            indicatorPosition += canvasCenter;
            return indicatorPosition;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                if (other.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    isDetected = true;
                    mainCamera = other.gameObject.GetComponent<CameraController>().FindCamera();                    
                    //canvas.planeDistance = 1;
                    cameraPos = mainCamera.rect;

                    relativeCamPos = new(canvasRect.rect.width * cameraPos.x,
                                                canvasRect.rect.height * cameraPos.y,
                                                canvasRect.rect.width * cameraPos.width,
                                                canvasRect.rect.height * cameraPos.height);

                    detectUI.SetActive(true);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            isDetected = false;
            detectUI.SetActive(false);
        }
    }
}
