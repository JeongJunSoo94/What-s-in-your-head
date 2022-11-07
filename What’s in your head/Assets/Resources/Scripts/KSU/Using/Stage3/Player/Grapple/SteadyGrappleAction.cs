using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using YC.Camera_;
using YC.Camera_Single;


using JCW.UI.InGame.Indicator;
using JCW.UI.InGame;
using KSU.AutoAim.Player.Object;
using KSU.AutoAim.Object;
using KSU.AutoAim.Object.Monster;

namespace KSU.AutoAim.Player
{
    //public enum GrappleTargetType { GrappledObject, Monster, Null};
    public class SteadyGrappleAction : SteadyAutoAimAction
    {
        PlayerController playerController;
        Rigidbody playerRigidbody;
        
        SteadyGrapple grapple;

        //GameObject grappledTarget;

        Vector3 grappleVec;
        //SteadyInteractionState steadyInteractionState;
        //PlayerState playerState;
        //Animator playerAnimator;

        //Camera playerCamera;
        //[SerializeField] GameObject lookAtObj;
        //RaycastHit _raycastHit;
        //[SerializeField] LayerMask layerFilterForGrapple;
        //[SerializeField] LayerMask layerForGrapple;

        //[SerializeField] GameObject grappleSpawner;
        //[SerializeField] GameObject grappleObject;
        //Transform autoAimPosition;
        //[SerializeField] AimUI aimUI;
        //GrappleTargetType curTargetType;


        [Header("_______���� ���� ��_______")]
        [Header("�÷��̾� ���ư��� �ӷ�")]
        public float grappleMoveSpeed = 10f;
        //[Header("���� ���ư��� �ӷ�")]
        //public float grappleSpeed = 10f;
        //[Header("���� �ӷ��� ������ �� ���� ���� ���̼���")]
        //public float grappleDepartOffset = 0.5f;
        [Header("��ǥ���� ������ ���ư��� �ӵ�")]
        public float escapeGrapplePower = 10f;
        //[Header("���� Ÿ���� Ž�� ����(ĸ��) ������")]
        //public float rangeRadius = 5f;
        //[Header("���� Ÿ���� Ž�� ����(ĸ��) ����(�Ÿ�)")]
        //public float rangeDistance = 15f;
        //[Header("���� ��ô �ִ� �Ÿ�(rangeDistance + rangeRadius * 2 �̻�����)")]
        //public float grapplingRange = 30f;
        //[Header("���� Ÿ���� Ž�� ���� (����)"), Range(1f, 89f)]
        //public float rangeAngle = 30f;
        
        //Vector3 targetPosition;


        //List<GameObject> grappledObjects = new();

        // Start is called before the first frame update
        void Awake()
        {
            playerAnimator = GetComponent<Animator>();
            photonView = GetComponent<PhotonView>();

            playerController = GetComponent<PlayerController>();
            playerState = GetComponent<PlayerState>();
            steadyInteractionState = GetComponent<SteadyInteractionState>();
            playerRigidbody = GetComponent<Rigidbody>();
            lookAtObj = this.gameObject.GetComponent<CameraController>().lookatBackObj;

            playerCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // ��Ƽ��

            autoAimObject = Instantiate(autoAimObject);
            grapple = autoAimObject.GetComponent<SteadyGrapple>();
            grapple.player = this.gameObject;
            grapple.spawner = autoAimObjectSpawner;
            autoAimObject.SetActive(false);

            playerAnimator = GetComponent<Animator>();
        }

        void Update()
        {
            SearchAutoAimTargetdObject();
            if(photonView.IsMine)
            {
                SendInfoUI();
                SendInfoAImUI();
            }
        }

        private void FixedUpdate()
        {
            MoveToTarget();
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

        //public void SendInfoAImUI()
        //{
        //    if(steadyInteractionState.isAutoAimObjectFounded)
        //    {
        //        aimUI.SetTarget(autoAimPosition, rangeAngle);
        //    }
        //    else
        //    {
        //        aimUI.SetTarget(null, rangeAngle);
        //    }
        //}

        protected override void InputFire()
        {
            if (playerState.isOutOfControl || playerState.isStopped)
                return;

            if (autoAimObjectSpawner.transform.parent.gameObject.activeSelf)
            {
                if (!grapple.gameObject.activeSelf)
                {
                    steadyInteractionState.isSucceededInHittingTaget = false;
                    autoAimObjectSpawner.SetActive(false);
                    if(GameManager.Instance.isTopView)
                    {
                        ///////////////// ������ 2: ���⿡ ���콺 �Ÿ� ��ŭ�� ��ġ���� ��� ���� �߻�
                        Vector3 forward = (playerController.playerMouse.point.transform.position - autoAimObjectSpawner.transform.position);
                        forward.y = 0;
                        if (forward.magnitude > autoAimObjectRange)
                            forward = forward.normalized * autoAimObjectRange;
                        grapple.InitObject(autoAimObjectSpawner.transform.position, (autoAimObjectSpawner.transform.position + forward), autoAimObjectSpeed, autoAimObjectDepartOffset);
                    }
                    else if (steadyInteractionState.isAutoAimObjectFounded)
                    {
                        // ���� ��ġ: autoAimPosition
                        grapple.InitObject(autoAimObjectSpawner.transform.position, autoAimPosition.position, autoAimObjectSpeed, autoAimObjectDepartOffset);
                    }
                    else
                    {
                        // ������ġ: ȭ�� �߾ӿ� ���� ���� �����ϴ� ��
                        RaycastHit hit;
                        bool rayCheck = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, autoAimObjectRange, -1,QueryTriggerInteraction.Ignore);
                        if(rayCheck && (hit.distance > Vector3.Distance(transform.position, playerCamera.transform.position)))
                        {
                            grapple.InitObject(autoAimObjectSpawner.transform.position, hit.point, autoAimObjectSpeed, autoAimObjectDepartOffset);
                        }
                        else
                        {
                            grapple.InitObject  (autoAimObjectSpawner.transform.position, (playerCamera.transform.position + playerCamera.transform.forward * autoAimObjectRange), autoAimObjectSpeed, autoAimObjectDepartOffset);
                        }
                    }
                }
            }
        }

        //public void RecieveGrappleInfo(bool isSuceeded, GameObject targetObj, AutoAimTargetType grappleTargetType)
        //{
        //    curTargetType = grappleTargetType;
        //    steadyInteractionState.isSucceededInHittingTaget = isSuceeded;

        //    if (isSuceeded)
        //    {
        //        grappledTarget = targetObj;
        //    }
        //    else
        //    {
        //        autoAimObjectSpawner.SetActive(true);
        //    }
        //}

        //public bool GetWhetherHit(AutoAimTargetType grappleTargetType)
        //{
        //    if((grappleTargetType == curTargetType))
        //    {
        //        return steadyInteractionState.isSucceededInHittingTaget;
        //    }
        //    return false;
        //}

        public void StartGrab()
        {
            steadyInteractionState.isGrabMonster = true;
            playerState.isRiding = true;
            playerController.MoveStop();
            Vector3 lookVec = (autoAimPosition.position - transform.position);
            lookVec.y = 0;
            transform.LookAt(transform.position + lookVec);
        }

        public void EndGrab()
        {
            steadyInteractionState.isGrabMonster = false;
            playerState.isRiding = false;
            autoAimObjectSpawner.SetActive(true);
        }
        public void Hook()
        {
            playerState.isRiding = true;
            playerState.IsAirJumping = false;
            playerState.WasAirDashing = false;
            targetPosition = hitTarget.GetComponent<GrappledObject>().GetOffsetPosition();
            grappleVec = (targetPosition - transform.position).normalized * grappleMoveSpeed;
            Vector3 lookVec = grappleVec;
            lookVec.y = 0;
            transform.LookAt(transform.position + lookVec);
            steadyInteractionState.isGrappling = true;
        }

        void MoveToTarget()
        {
            if (steadyInteractionState.isGrappling)
            {
                if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
                {
                    EscapeMoving();
                }
                playerRigidbody.velocity = grappleVec;
            }
        }

        //void SendInfoUI()
        //{
        //    if (autoAimTargetObjects.Count > 0)
        //    {
        //        foreach (var grappledObject in autoAimTargetObjects)
        //        {
        //            Vector3 directoin = (grappledObject.transform.parent.position - playerCamera.transform.position).normalized;
        //            bool rayCheck = false;
        //            RaycastHit hit;
        //            switch (grappledObject.tag)
        //            {
        //                case "GrappledObject":
        //                    {
        //                        rayCheck = Physics.Raycast(playerCamera.transform.position, directoin, out hit, grappledObject.GetComponentInParent<GrappledObject>().detectingUIRange * 1.5f, layerFilterForAutoAim, QueryTriggerInteraction.Ignore);
        //                        if (rayCheck)
        //                        {
        //                            if (hit.collider.CompareTag("GrappledObject"))
        //                            {
        //                                rayCheck = true;
        //                            }
        //                            else
        //                            {
        //                                rayCheck = false;
        //                            }
        //                        }
        //                    }
        //                    break;
        //                case "PoisonSnake":
        //                    {
        //                        rayCheck = Physics.Raycast(playerCamera.transform.position, directoin, out hit, grappledObject.GetComponentInParent<DefenseMonster>().detectingUIRange * 1.5f, layerFilterForAutoAim, QueryTriggerInteraction.Ignore);
        //                        if (rayCheck)
        //                        {
        //                            if (hit.collider.CompareTag("PoisonSnake"))
        //                            {
        //                                rayCheck = true;
        //                            }
        //                            else
        //                            {
        //                                rayCheck = false;
        //                            }
        //                        }
        //                    }
        //                    break;
        //                case "TrippleHeadSnake":
        //                    {
        //                        rayCheck = Physics.Raycast(playerCamera.transform.position, directoin, out hit, grappledObject.GetComponentInParent<DefenseMonster>().detectingUIRange * 1.5f, layerFilterForAutoAim, QueryTriggerInteraction.Ignore);
        //                        if (rayCheck)
        //                        {
        //                            if (hit.collider.CompareTag("TrippleHeadSnake"))
        //                            {
        //                                rayCheck = true;
        //                            }
        //                            else
        //                            {
        //                                rayCheck = false;
        //                            }
        //                        }
        //                    }
        //                    break;
        //            }

        //            if (rayCheck)
        //            {
        //                grappledObject.transform.parent.gameObject.GetComponentInChildren<OneIndicator>().SetUI(true);
        //            }
        //            else
        //            {
        //                grappledObject.transform.parent.gameObject.GetComponentInChildren<OneIndicator>().SetUI(false);
        //            }
        //        }
        //    }
        //}

        public void EscapeMoving()
        {
            hitTarget = null;
            steadyInteractionState.isSucceededInHittingTaget = false;
            autoAimObjectSpawner.SetActive(true);
            steadyInteractionState.isGrappling = false;
            Vector3 inertiaVec = grappleVec;
            float jumpPower = inertiaVec.y;
            inertiaVec.y = 0;

            playerController.MakeinertiaVec(escapeGrapplePower, transform.forward.normalized);
            playerController.moveVec = Vector3.up * jumpPower;
            playerController.characterState.isRiding = false;
            grapple.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {

            if ((other.gameObject.layer == LayerMask.NameToLayer("UITriggers")) && (other.CompareTag("GrappledObject") || other.CompareTag("PoisonSnake") || other.CompareTag("TrippleHeadSnake")))
            {
                autoAimTargetObjects.Add(other.gameObject);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if ((other.gameObject.layer == LayerMask.NameToLayer("UITriggers")) && (other.CompareTag("GrappledObject") || other.CompareTag("PoisonSnake") || other.CompareTag("TrippleHeadSnake")))
            {
                other.gameObject.transform.parent.gameObject.GetComponentInChildren<OneIndicator>().SetUI(false);
                autoAimTargetObjects.Remove(other.gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (steadyInteractionState.isGrappling)
            {
                playerRigidbody.velocity = Vector3.zero;
                grappleVec = Vector3.zero;
                EscapeMoving();
            }
        }
    }
}
