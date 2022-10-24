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

        private void Awake()
        {
            mainCamera = GetComponent<Canvas>().worldCamera;
            imgTransform = transform.GetChild(0).GetComponent<RectTransform>();
            isTargeting = false;
        }

        private void LateUpdate()
        {
            if(isTargeting)
                isTargeting = false;
            else
                imgTransform.position = Vector3.zero;            
        }


        public void MoveTarget(Vector3 targetPosition)
        {
            // 그림의 위치를 타겟의 위치로 변경
            imgTransform.position = mainCamera.WorldToScreenPoint(targetPosition);
            isTargeting = true;
        }

    }
}

