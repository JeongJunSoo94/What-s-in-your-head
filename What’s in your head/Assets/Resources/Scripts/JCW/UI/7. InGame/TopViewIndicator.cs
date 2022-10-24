using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using YC.Camera_;

namespace JCW.UI.InGame
{
    public class TopViewIndicator : MonoBehaviour
    {

        [Header("넬라 / 스테디 UI")]
        [SerializeField] Sprite nellaIndicator;
        [SerializeField] Sprite steadyIndicator;

        //내 이미지 & 상대방 이미지
        RectTransform myImgTransform;
        RectTransform otherImgTransform;

        RectTransform canvasRT;
        // UI를 띄워줄 카메라
        Camera mainCamera;

        // 플레이어 위치
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
            // 포지션 설정
            Vector3 myIndicatorPosition = mainCamera.WorldToScreenPoint(myPlayerTF.position);
            Vector3 otherIndicatorPosition = mainCamera.WorldToScreenPoint(otherPlayerTF.position);

            myImgTransform.position = myIndicatorPosition;
            otherImgTransform.position = otherIndicatorPosition;

            // 방향 설정
            // 플레이어의 Rotation.y값과 UI의 Rotation.z값이 연동되어야함.

            float curEulerY = myPlayerTF.rotation.eulerAngles.y > 180 ? myPlayerTF.rotation.eulerAngles.y - 360 :
                (myPlayerTF.rotation.eulerAngles.y < -180 ? myPlayerTF.rotation.eulerAngles.y + 360 : myPlayerTF.rotation.eulerAngles.y);
            float otherEulerY = otherPlayerTF.rotation.eulerAngles.y > 180 ? otherPlayerTF.rotation.eulerAngles.y - 360 :
                (otherPlayerTF.rotation.eulerAngles.y < -180 ? otherPlayerTF.rotation.eulerAngles.y + 360 : otherPlayerTF.rotation.eulerAngles.y);

            // 그냥 쿼터니언으로 넣으면 왜 안되는지 알아내야함.
            myImgTransform.rotation = Quaternion.Euler(0, 0, -curEulerY);
            otherImgTransform.rotation = Quaternion.Euler(0, 0, -otherEulerY);

        }
    }
}
