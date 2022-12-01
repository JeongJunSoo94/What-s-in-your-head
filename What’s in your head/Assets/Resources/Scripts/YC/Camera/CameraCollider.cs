using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace YC.YC_Camera
{
    public class CameraCollider : MonoBehaviour
    {
        CameraController cameraController;
        Camera mainCamera;

        string interactionLayer = "Platform";

        private void Awake()
        {
            //DontDestroyOnLoad(this);
        }

        private void FixedUpdate()
        {
            if (!mainCamera) return;

            transform.position = mainCamera.transform.position;
        }
        public void SetVariables(CameraController controller)
        {
            cameraController = controller;
            mainCamera = controller.FindCamera();

            if (!cameraController) return;
            Debug.Log("## ¼Â ¿Ï·á!");
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer(interactionLayer))
            {
                cameraController.SetCinemachineCollider(true);
                Debug.Log("## ON!");
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer(interactionLayer))
            {
                cameraController.SetCinemachineCollider(false);
                Debug.Log("## OFF!");

            }
        }

    }

}
