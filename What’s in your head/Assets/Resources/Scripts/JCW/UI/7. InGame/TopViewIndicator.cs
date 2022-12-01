using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using YC.YC_Camera;

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
        WaitUntil wu;

        bool isStart = false;
        bool isNella;

        private void Awake()
        {
            wu = new(() => GameManager.Instance.GetCharOnScene());

            myImgTransform = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
            otherImgTransform = transform.GetChild(0).GetChild(1).GetComponent<RectTransform>();

            StartCoroutine(nameof(WaitForPlayer));
        }

        void Update()
        {
            if (!isStart || myPlayerTF == null || myPlayerTF == null)
                return;

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

            myImgTransform.rotation = Quaternion.Euler(0, 0, -curEulerY);
            otherImgTransform.rotation = Quaternion.Euler(0, 0, -otherEulerY);

        }

        IEnumerator WaitForPlayer()
        {
            yield return wu;

            myPlayerTF = GameManager.Instance.myPlayerTF;
            otherPlayerTF = GameManager.Instance.otherPlayerTF;

            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];

            myImgTransform.GetComponent<Image>().sprite = isNella ? nellaIndicator : steadyIndicator;
            otherImgTransform.GetComponent<Image>().sprite = isNella ? steadyIndicator : nellaIndicator;

            mainCamera = myPlayerTF.GetComponent<CameraController>().FindCamera();

            transform.GetChild(0).gameObject.SetActive(true);

            isStart = true;
            yield break;
        }
    }
}
