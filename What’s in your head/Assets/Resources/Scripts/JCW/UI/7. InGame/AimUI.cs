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
        RectTransform imgTransform;
        bool isTargeting;
        bool wasTopView;

        bool isNella;

        Transform targetTF;
        Transform playerTF;

        Canvas thisCanvas;
       
        private void Awake()
        {
            mainCamera = GetComponent<Canvas>().worldCamera;
            imgTransform = transform.GetChild(0).GetComponent<RectTransform>();
            if (!GetComponent<PhotonView>().IsMine)
            {
                Destroy(this.gameObject);
                return;
            }
            StartCoroutine(nameof(WaitForPlayer));
            playerTF = this.transform.parent;
            thisCanvas = GetComponent<Canvas>();

        }

        private void Update()
        {
            if (GameManager.Instance.isTopView)
            {
                if (!wasTopView)
                {
                    wasTopView = true;
                    this.transform.parent = targetTF;
                    thisCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                }
                //Vector3 pos = Input.mousePosition;
                //pos.z = 0.15f;
                //imgTransform.position = mainCamera.ScreenToWorldPoint(pos);
                if (mainCamera.rect.width <= 0.1f)
                    return;
                imgTransform.position = mainCamera.WorldToScreenPoint(targetTF.position);
                //mainCamera.ScreenToWorldPoint
            }
            else if (wasTopView)
            {
                wasTopView = false;
                this.transform.parent = playerTF;
                imgTransform.localPosition = Vector3.zero;
                thisCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                thisCanvas.worldCamera = mainCamera;
            }
        }

        //private void LateUpdate()
        //{
        //    if(isTargeting)
        //        isTargeting = false;
        //    else
        //    {                 
        //        imgTransform.localPosition = Vector3.zero;            
        //    }
        //}


        public void MoveTarget(Vector3 targetPosition)
        {
            // 그림의 위치를 타겟의 위치로 변경
            imgTransform.position = mainCamera.WorldToScreenPoint(targetPosition);
            isTargeting = true;
        }
        protected IEnumerator WaitForPlayer()
        {
            while (GameManager.Instance.characterOwner.Count <= 1)
                yield return new WaitForSeconds(0.2f);

            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
            Debug.Log("isNella : " + isNella);
            targetTF = isNella ? GameObject.FindWithTag("NellaMousePoint").transform : GameObject.FindWithTag("SteadyMousePoint").transform;
            Debug.Log(targetTF);
            yield break;
        }
    }
}

