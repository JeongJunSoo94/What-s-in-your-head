using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.CameraManager_;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class AimUI : MonoBehaviour
    {
        Camera mainCamera;
        Camera curCam;
        RectTransform imgTransform;
        bool wasTopView;

        bool isNella;

        Transform targetTF;
        Transform playerTF;
       
        private void Awake()
        {
            imgTransform = transform.GetChild(0).GetComponent<RectTransform>();
            if (!GetComponent<PhotonView>().IsMine)
            {
                Destroy(this.gameObject);
                return;
            }
            StartCoroutine(nameof(WaitForPlayer));
            playerTF = this.transform.parent;

        }

        private void Update()
        {
            if (targetTF == null)
                return;
            if (GameManager.Instance.isTopView)
            {
                if (!wasTopView)
                {
                    wasTopView = true;
                    this.transform.parent = targetTF;
                    if(curCam.rect.width <= 0.1f)
                        curCam = isNella ? CameraManager.Instance.cameras[1] : CameraManager.Instance.cameras[0];
                }
                if (curCam.rect.width <= 0.1f)
                    return;
                imgTransform.position = curCam.WorldToScreenPoint(targetTF.position);                
                //mainCamera.ScreenToWorldPoint
            }
            else if (wasTopView)
            {
                wasTopView = false;
                this.transform.parent = playerTF;
                imgTransform.localPosition = Vector3.zero;
            }
        }

        protected IEnumerator WaitForPlayer()
        {
            while (GameManager.Instance.characterOwner.Count <= 1)
                yield return new WaitForSeconds(0.2f);

            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
            Debug.Log("isNella : " + isNella);
            targetTF = isNella ? GameObject.FindWithTag("NellaMousePoint").transform : GameObject.FindWithTag("SteadyMousePoint").transform;
            Debug.Log(targetTF);

            mainCamera = isNella ? CameraManager.Instance.cameras[0] : CameraManager.Instance.cameras[1];
            curCam = mainCamera;
            yield break;
        }
    }
}

