using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using YC.Camera_;
using YC.Camera_Single;

using JCW.UI.InGame.Indicator;
using JCW.UI.InGame;
using KSU.AutoAim.Player.Object;

namespace KSU.AutoAim.Player
{
    public class SteadyCymbalsAction : SteadyAutoAimAction
    {
        SteadyCymbals cymbals;
        PlayerController playerController;
        Vector3 shootPosition;

        override protected void Awake()
        {
            base.Awake();
            playerAnimator = GetComponent<Animator>();
            photonView = GetComponent<PhotonView>();

            playerController = GetComponent<PlayerController>();
            playerState = GetComponent<PlayerState>();
            steadyInteractionState = GetComponent<SteadyInteractionState>();
            //lookAtObj = this.gameObject.GetComponent<CameraController>().lookatBackObj;

            playerCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // ¸ÖÆ¼¿ë

            autoAimObject = Instantiate(autoAimObject);
            cymbals = autoAimObject.GetComponent<SteadyCymbals>();
            cymbals.player = this.gameObject;
            cymbals.spawner = autoAimObjectSpawner;
            autoAimObject.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                Debug.Log(Input.mousePosition);
            SearchAutoAimTargetdObject();
            if (photonView.IsMine)
            {
                SendInfoUI();
                SendInfoAImUI();
            }
        }


        protected override void SearchAutoAimTargetdObject()
        {
            if (!playerAnimator.GetBool("isShootingGrapple") && autoAimObjectSpawner.transform.parent.gameObject.activeSelf && autoAimObjectSpawner.activeSelf)
            {
                if (playerState.aim)
                {
                    Vector3 cameraForwardXZ = playerCamera.transform.forward;
                    cameraForwardXZ.y = 0;
                    Vector3 rayOrigin = playerCamera.transform.position;
                    Vector3 rayEnd = (playerCamera.transform.position + playerCamera.transform.forward * (rangeDistance + rangeRadius * 2f));
                    Vector3 direction = (rayEnd - rayOrigin).normalized;
                    bool isRayChecked = Physics.SphereCast(rayOrigin, rangeRadius, direction, out _raycastHit, rangeDistance, layerForAutoAim, QueryTriggerInteraction.Ignore);

                    if (isRayChecked)
                    {
                        direction = (_raycastHit.collider.gameObject.transform.position - rayOrigin).normalized;
                        isRayChecked = Physics.SphereCast(rayOrigin, 0.2f, direction, out _raycastHit, (rangeDistance + rangeRadius * 2f), layerFilterForAutoAim, QueryTriggerInteraction.Ignore);
                        if (isRayChecked)
                        {
                            if (_raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("AutoAimedObject"))
                            {
                                if (Vector3.Angle(playerCamera.transform.forward, (_raycastHit.collider.gameObject.transform.position - rayOrigin)) < rangeAngle)
                                {
                                    autoAimPosition = _raycastHit.collider.gameObject.transform;
                                    steadyInteractionState.isAutoAimObjectFounded = true;
                                    return;
                                }
                            }
                        }
                    }
                }
                steadyInteractionState.isAutoAimObjectFounded = false;
            }
        }

        public void MakeShootPosition()
        {
            if (playerState.isOutOfControl || playerState.isStopped)
                return;

            if(GameManager.Instance.isSideView)
            {
                Vector3 target = Vector3.zero;
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f, LayerMask.NameToLayer("Defalut"), QueryTriggerInteraction.Ignore))
                {
                    target = hit.point;
                }
                Vector3 forward = target - autoAimObjectSpawner.transform.position;
                forward.z = 0;  
                forward = forward.normalized * autoAimObjectRange;
                playerController.photonView.RPC(nameof(SetShoot), RpcTarget.AllViaServer, autoAimObjectSpawner.transform.position + forward);
            }
            else if (steadyInteractionState.isAutoAimObjectFounded)
            {
                // µµÂø À§Ä¡: autoAimPosition
                playerController.photonView.RPC(nameof(SetShoot), RpcTarget.AllViaServer, autoAimPosition.position);
            }
            else
            {
                // µµÂøÀ§Ä¡: È­¸é Áß¾Ó¿¡ ·¹ÀÌ ½÷¼­ µµÂøÇÏ´Â °÷
                RaycastHit hit;
                bool rayCheck = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, autoAimObjectRange, -1, QueryTriggerInteraction.Ignore);
                if (rayCheck && (hit.distance > Vector3.Distance(transform.position, playerCamera.transform.position)))
                {
                    playerController.photonView.RPC(nameof(SetShoot), RpcTarget.AllViaServer, hit.point);
                }
                else
                {
                    playerController.photonView.RPC(nameof(SetShoot), RpcTarget.AllViaServer, playerCamera.transform.position + playerCamera.transform.forward * autoAimObjectRange);
                }
            }
        }

        [PunRPC]
        void SetShoot(Vector3 pos)
        {
            shootPosition = pos;
        }

        protected override void InputFire()
        {
            if (playerState.isOutOfControl || playerState.isStopped)
                return;

            if (autoAimObjectSpawner.transform.parent.gameObject.activeSelf)
            {
                if (!cymbals.gameObject.activeSelf)
                {
                    steadyInteractionState.isSucceededInHittingTaget = false;
                    autoAimObjectSpawner.SetActive(false);
                    cymbals.InitObject(autoAimObjectSpawner.transform.position, shootPosition, autoAimObjectSpeed, autoAimObjectDepartOffset);
                    //if (steadyInteractionState.isAutoAimObjectFounded)
                    //{
                    //    // µµÂø À§Ä¡: autoAimPosition
                    //    cymbals.InitObject(autoAimObjectSpawner.transform.position, autoAimPosition.position, autoAimObjectSpeed, autoAimObjectDepartOffset);
                    //}
                    //else
                    //{
                    //    // µµÂøÀ§Ä¡: È­¸é Áß¾Ó¿¡ ·¹ÀÌ ½÷¼­ µµÂøÇÏ´Â °÷
                    //    RaycastHit hit;
                    //    bool rayCheck = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, autoAimObjectRange, -1, QueryTriggerInteraction.Ignore);
                    //    if (rayCheck && (hit.distance > Vector3.Distance(transform.position, playerCamera.transform.position)))
                    //    {
                    //        cymbals.InitObject(autoAimObjectSpawner.transform.position, hit.point, autoAimObjectSpeed, autoAimObjectDepartOffset);
                    //    }
                    //    else
                    //    {
                    //        cymbals.InitObject(autoAimObjectSpawner.transform.position, (playerCamera.transform.position + playerCamera.transform.forward * autoAimObjectRange), autoAimObjectSpeed, autoAimObjectDepartOffset);
                    //    }
                    //}
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {

            if ((other.gameObject.layer == LayerMask.NameToLayer("UITriggers")) && other.CompareTag("CymbalsTarget"))
            {
                if (autoAimTargetObjects.Contains(other.gameObject))
                    return;
                autoAimTargetObjects.Add(other.gameObject);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if ((other.gameObject.layer == LayerMask.NameToLayer("UITriggers")) && other.CompareTag("CymbalsTarget"))
            {
                other.gameObject.transform.parent.gameObject.GetComponentInChildren<OneIndicator>().SetUI(false);
                autoAimTargetObjects.Remove(other.gameObject);
            }
        }
    }
}
