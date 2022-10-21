using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.UI.InGame
{
    public class TopViewIndicator : MonoBehaviour
    {
        [Header("UI ĵ���� ũ��")] [SerializeField] RectTransform canvasRT;
        [Header("�� �÷��̾� ��ġ")] [SerializeField] Transform myPlayerTF;
        [Header("��� �÷��̾� ��ġ")] [SerializeField] Transform otherPlayerTF;
        [Header("UI�� ������ ī�޶�")] [SerializeField] Camera mainCamera;

        [Header("�� �̹��� & ���� �̹���")][SerializeField] RectTransform myImgTransform;
                                            [SerializeField] RectTransform otherImgTransform;

        [Header("�ڶ� / ���׵� UI")]
        [SerializeField] Sprite nellaIndicator;
        [SerializeField] Sprite steadyIndicator;

        private void Awake()
        {
            if(GameManager.Instance.characterOwner.Count != 0)
            {
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
            }
            else
            {
                myImgTransform.gameObject.GetComponent<Image>().sprite = nellaIndicator;
                otherImgTransform.gameObject.GetComponent<Image>().sprite = steadyIndicator;
            }

                     
        }

        void Update()
        {
            // ������ ����
            Vector3 myIndicatorPosition = mainCamera.WorldToScreenPoint(myPlayerTF.position);
            Vector3 otherIndicatorPosition = mainCamera.WorldToScreenPoint(otherPlayerTF.position);

            myImgTransform.position = myIndicatorPosition;
            otherImgTransform.position = otherIndicatorPosition;

            // ���� ����
            // �÷��̾��� Rotation.y���� UI�� Rotation.z���� �����Ǿ����.


            //Debug.Log("���� �÷��̾��� Forward �� : " + myPlayerTF.forward);
            //Debug.Log("myImgTransform�� up �� : " + myImgTransform.up);
            myImgTransform.rotation = new Quaternion(0, 0, -myPlayerTF.rotation.y, 0);
            //otherImgTransform.rotation = new Quaternion(0, 0, -otherPlayerTF.rotation.y, 0);
            //myImgTransform.up = temp;
            //
            //myImgTransform.up = -myPlayerTF.forward;
            //otherImgTransform.up = -otherPlayerTF.forward;
            //myImgTransform.up = -myPlayerTF.forward;
            //otherImgTransform.up = -otherPlayerTF.forward;
        }
    }
}
