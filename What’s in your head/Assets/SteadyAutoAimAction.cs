using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using YC.Camera_;
using YC.Camera_Single;

using JCW.UI.InGame.Indicator;
using JCW.UI.InGame;
using KSU.Monster;
using KSU.AutoAim.Object;

namespace KSU.AutoAim.Player
{
    public enum AutoAimTargetType { GrappledObject, Monster, Null };
    public class SteadyAutoAimAction : MonoBehaviour
    {
        protected SteadyInteractionState steadyInteractionState;
        protected PlayerState playerState;
        protected Animator playerAnimator;

        protected Camera playerCamera;
        [SerializeField] protected GameObject lookAtObj;
        [SerializeField] protected LayerMask layerFilterForAutoAim;
        [SerializeField] protected LayerMask layerForAutoAim;
        protected RaycastHit _raycastHit;

        [SerializeField] protected GameObject autoAimObjectSpawner;
        [SerializeField] protected GameObject autoAimObject;
        protected GameObject hitTarget;

        Transform autoAimPosition;
        [SerializeField] AimUI aimUI;

        [Header("_______���� ���� ��_______")]
        [Header("����ü ���ư��� �ӷ�")]
        public float cymbalsSpeed = 10f;
        [Header("����ü �ӷ��� ������ �� ���� ���� ���̼���")]
        public float cymbalsDepartOffset = 0.5f;
        [Header("���� Ÿ���� Ž�� ����(ĸ��) ������")]
        public float rangeRadius = 5f;
        [Header("���� Ÿ���� Ž�� ����(ĸ��) ����(�Ÿ�)")]
        public float rangeDistance = 15f;
        [Header("����ü ��ô �ִ� �Ÿ�(rangeDistance + rangeRadius * 2 �̻�����)")]
        public float grapplingRange = 30f;
        [Header("���� Ÿ���� Ž�� ���� (����)"), Range(1f, 89f)]
        public float rangeAngle = 30f;

        GameObject autoAimTarget;
        Vector3 targetPosition;

        Vector3 autoAimObjectVec;
        AutoAimTargetType curTargetType = AutoAimTargetType.Null;

        List<GameObject> autoAimTargetObjects = new();

        void Awake()
        {
            steadyInteractionState = GetComponent<SteadyInteractionState>();
            playerCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // ��Ƽ��
            playerAnimator = GetComponent<Animator>();

            autoAimObject = Instantiate(autoAimObject);
            autoAimObject.SetActive(false);
        }

        void Update()
        {
            SearchAutoAimTargetdObject();
            if (playerState.isMine)
            {
                SendInfoUI();
                SendInfoAImUI();
            }
        }
        public void SearchAutoAimTargetdObject()
        {
            if (!playerAnimator.GetBool("isShootingCymbals") && autoAimObjectSpawner.transform.parent.gameObject.activeSelf && autoAimObjectSpawner.activeSelf)
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

        public void SendInfoAImUI()
        {
            if (steadyInteractionState.isAutoAimObjectFounded)
            {
                aimUI.SetTarget(autoAimPosition, rangeAngle);
            }
            else
            {
                aimUI.SetTarget(null, rangeAngle);
            }
        }

        void SendInfoUI()
        {
            if (autoAimTargetObjects.Count > 0)
            {
                foreach (var cymbalsObject in autoAimTargetObjects)
                {
                    Vector3 directoin = (cymbalsObject.transform.parent.position - playerCamera.transform.position).normalized;
                    bool rayCheck = false;
                    RaycastHit hit;
                    rayCheck = Physics.Raycast(playerCamera.transform.position, directoin, out hit, cymbalsObject.GetComponentInParent<AutoAimTargetObject>().detectingUIRange * 1.5f, layerFilterForAutoAim, QueryTriggerInteraction.Ignore);
                    
                    if (rayCheck)
                    {
                        if (hit.collider.CompareTag(cymbalsObject.tag))
                        {
                            rayCheck = true;
                        }
                        else
                        {
                            rayCheck = false;
                        }
                    }

                    if (rayCheck)
                    {
                        cymbalsObject.transform.parent.gameObject.GetComponentInChildren<OneIndicator>().SetUI(true);
                    }
                    else
                    {
                        cymbalsObject.transform.parent.gameObject.GetComponentInChildren<OneIndicator>().SetUI(false);
                    }
                }
            }
        }

        public void RecieveAutoAimObjectInfo(bool isSuceeded, GameObject targetObj, AutoAimTargetType autoAimTargetType)
        {
            curTargetType = autoAimTargetType;
            steadyInteractionState.isSucceededInHittingTaget = isSuceeded;

            if (isSuceeded)
            {
                hitTarget = targetObj;
            }
            else
            {
                autoAimObjectSpawner.SetActive(true);
            }
        }

        //public void InputFire()
        //{
        //    if (playerState.isOutOfControl || playerState.isStopped)
        //        return;

        //    if (cymbalsSpawner.transform.parent.gameObject.activeSelf)
        //    {
        //        if (!cymbals.gameObject.activeSelf)
        //        {
        //            steadyInteractionState.isSucceededInHittingTaget = false;
        //            cymbalsSpawner.SetActive(false);
        //            if (GameManager.Instance.isTopView)
        //            {
        //                ///////////////// ������ 2: ���⿡ ���콺 �Ÿ� ��ŭ�� ��ġ���� ��� ���� �߻�
        //                Vector3 forward = (playerController.playerMouse.point.transform.position - cymbalsSpawner.transform.position);
        //                forward.y = 0;
        //                if (forward.magnitude > grapplingRange)
        //                    forward = forward.normalized * grapplingRange;
        //                cymbals.InitCymbals(cymbalsSpawner.transform.position, (cymbalsSpawner.transform.position + forward), cymbalsSpeed, cymbalsDepartOffset);
        //            }
        //            else if (steadyInteractionState.isAutoAimObjectFounded)
        //            {
        //                // ���� ��ġ: autoAimPosition
        //                cymbals.InitCymbals(cymbalsSpawner.transform.position, autoAimPosition.position, cymbalsSpeed, cymbalsDepartOffset);
        //            }
        //            else
        //            {
        //                // ������ġ: ȭ�� �߾ӿ� ���� ���� �����ϴ� ��
        //                RaycastHit hit;
        //                bool rayCheck = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, grapplingRange, -1, QueryTriggerInteraction.Ignore);
        //                if (rayCheck && (hit.distance > Vector3.Distance(transform.position, playerCamera.transform.position)))
        //                {
        //                    cymbals.InitObject(cymbalsSpawner.transform.position, hit.point, cymbalsSpeed, cymbalsDepartOffset);
        //                }
        //                else
        //                {
        //                    cymbals.InitObject(cymbalsSpawner.transform.position, (playerCamera.transform.position + playerCamera.transform.forward * grapplingRange), cymbalsSpeed, cymbalsDepartOffset);
        //                }
        //            }
        //        }
        //    }
        //}

        //public void RecieveCymbalsInfo(bool isSuceeded, GameObject targetObj)
        //{
        //    steadyInteractionState.isSucceededInHittingTaget = isSuceeded;

        //    if (isSuceeded)
        //    {
        //        cymbalsTarget = targetObj;
        //        //if (targetObj.CompareTag("cymbalsdObjects"))
        //        //    Hook();
        //    }
        //    else
        //    {
        //        cymbalsSpawner.SetActive(true);
        //    }
        //}

        //public bool GetWhetherHit()
        //{
        //    return steadyInteractionState.isSucceededInHittingTaget;
        //}

        //public bool GetWhetherCymbalsActived()
        //{
        //    return cymbals.gameObject.activeSelf;
        //}

        private void OnTriggerEnter(Collider other)
        {

            if ((other.gameObject.layer == LayerMask.NameToLayer("UITriggers")) && (other.CompareTag("CymbalsdObject") || other.CompareTag("PoisonSnake") || other.CompareTag("TrippleHeadSnake")))
            {
                autoAimTargetObjects.Add(other.gameObject);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if ((other.gameObject.layer == LayerMask.NameToLayer("UITriggers")) && (other.CompareTag("CymbalsdObject") || other.CompareTag("PoisonSnake") || other.CompareTag("TrippleHeadSnake")))
            {
                other.gameObject.transform.parent.gameObject.GetComponentInChildren<OneIndicator>().SetUI(false);
                autoAimTargetObjects.Remove(other.gameObject);
            }
        }
    }
}

