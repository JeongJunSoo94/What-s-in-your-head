using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using YC.Camera_;

namespace JCW.UI.InGame
{
    public class TopViewIndicator : MonoBehaviour
    {

        [Header("�ڶ� / ���׵� UI")]
        [SerializeField] Sprite nellaIndicator;
        [SerializeField] Sprite steadyIndicator;

        //�� �̹��� & ���� �̹���
        RectTransform myImgTransform;
        RectTransform otherImgTransform;

        RectTransform canvasRT;
        // UI�� ����� ī�޶�
        Camera mainCamera;

        // �÷��̾� ��ġ
        Transform myPlayerTF;
        Transform otherPlayerTF = null;

        private void Awake()
        {
            if(!transform.parent.GetComponent<PhotonView>().IsMine)
            {
                Destroy(this);
                return;
            }

            myImgTransform = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
            otherImgTransform = transform.GetChild(0).GetChild(1).GetComponent<RectTransform>();

            if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
            {
                myImgTransform.gameObject.GetComponent<Image>().sprite = nellaIndicator;
                otherImgTransform.gameObject.GetComponent<Image>().sprite = steadyIndicator;
            }
            else
            {
                myImgTransform.gameObject.GetComponent<Image>().sprite = steadyIndicator;
                otherImgTransform.gameObject.GetComponent<Image>().sprite = nellaIndicator;
            }

            myPlayerTF = transform.parent;
            mainCamera = transform.parent.GetComponent<CameraController>().FindCamera();
            //otherPlayerTF = GameManager.Instance.otherPlayerTF;
        }

        void Update()
        {
            if(otherPlayerTF == null)
            {
                if (GameManager.Instance.otherPlayerTF == null)
                    return;
                otherPlayerTF = GameManager.Instance.otherPlayerTF;
            }
            // ������ ����
            Vector3 myIndicatorPosition = mainCamera.WorldToScreenPoint(myPlayerTF.position);
            Vector3 otherIndicatorPosition = mainCamera.WorldToScreenPoint(otherPlayerTF.position);

            myImgTransform.position = myIndicatorPosition;
            otherImgTransform.position = otherIndicatorPosition;

            // ���� ����
            // �÷��̾��� Rotation.y���� UI�� Rotation.z���� �����Ǿ����.

            float curEulerY = myPlayerTF.rotation.eulerAngles.y > 180 ? myPlayerTF.rotation.eulerAngles.y - 360 :
                (myPlayerTF.rotation.eulerAngles.y < -180 ? myPlayerTF.rotation.eulerAngles.y + 360 : myPlayerTF.rotation.eulerAngles.y);
            float otherEulerY = otherPlayerTF.rotation.eulerAngles.y > 180 ? otherPlayerTF.rotation.eulerAngles.y - 360 :
                (otherPlayerTF.rotation.eulerAngles.y < -180 ? otherPlayerTF.rotation.eulerAngles.y + 360 : otherPlayerTF.rotation.eulerAngles.y);

            // �׳� ���ʹϾ����� ������ �� �ȵǴ��� �˾Ƴ�����.
            myImgTransform.rotation = Quaternion.Euler(0, 0, -curEulerY);
            otherImgTransform.rotation = Quaternion.Euler(0, 0, -otherEulerY);

        }
    }
}
