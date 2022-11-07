using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using YC.Camera_;
using YC.Camera_Single;

using JCW.UI.InGame.Indicator;
using JCW.UI.InGame;

namespace KSU.AutoAim.Player
{
    public class SteadyCymbalsAction : AutoAimObject
    {
        SteadyInteractionState steadyInteractionState;
        PlayerController playerController;
        PlayerState playerState;
        Rigidbody playerRigidbody;
        Animator playerAnimator;

        Camera playerCamera;
        [SerializeField] GameObject lookAtObj;
        RaycastHit _raycastHit;
        [SerializeField] LayerMask layerFilterForCymbals;
        [SerializeField] LayerMask layerForCymbals;

        [SerializeField] GameObject cymbalsSpawner;
        [SerializeField] GameObject cymbalsObject;
        SteadyCymbals cymbals;
        Transform autoAimPosition;
        [SerializeField] AimUI aimUI;


        [Header("_______���� ���� ��_______")]
        [Header("�ɹ��� ���ư��� �ӷ�")]
        public float cymbalsSpeed = 10f;
        [Header("���� �ӷ��� ������ �� ���� ���� ���̼���")]
        public float cymbalsDepartOffset = 0.5f;
        [Header("���� Ÿ���� Ž�� ����(ĸ��) ������")]
        public float rangeRadius = 5f;
        [Header("���� Ÿ���� Ž�� ����(ĸ��) ����(�Ÿ�)")]
        public float rangeDistance = 15f;
        [Header("�ɹ��� ��ô �ִ� �Ÿ�(rangeDistance + rangeRadius * 2 �̻�����)")]
        public float grapplingRange = 30f;
        [Header("���� Ÿ���� Ž�� ���� (����)"), Range(1f, 89f)]
        public float rangeAngle = 30f;

        GameObject cymbalsTarget;

        Vector3 cymbalsVec;
        Vector3 targetPosition;


        List<GameObject> cymbalsObjects = new();


        //// Start is called before the first frame update
        //void Awake()
        //{
        //    playerController = GetComponent<PlayerController>();
        //    playerState = GetComponent<PlayerState>();
        //    steadyInteractionState = GetComponent<SteadyInteractionState>();
        //    playerRigidbody = GetComponent<Rigidbody>();


        //    playerCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // ��Ƽ��

        //    cymbalsObject = Instantiate(cymbalsObject);
        //    cymbalsObject.SetActive(false);
        //    cymbals = cymbalsObject.GetComponent<SteadyCymbals>();
        //    cymbals.player = this;
        //    cymbals.spawner = cymbalsSpawner;

        //    playerAnimator = GetComponent<Animator>();
        //}

        //void Update()
        //{
        //    SearchCymbalsdObject();
        //    if (playerState.isMine)
        //    {
        //        SendInfoUI();
        //        SendInfoAImUI();
        //    }
        //}
        //public void SearchCymbalsdObject()
        //{
        //    if (!playerAnimator.GetBool("isShootingCymbals") && cymbalsSpawner.transform.parent.gameObject.activeSelf && cymbalsSpawner.activeSelf)
        //    {
        //        Debug.Log("�˻� ����");
        //        if (playerState.aim)
        //        {
        //            Debug.Log("���� ��");
        //            Vector3 cameraForwardXZ = playerCamera.transform.forward;
        //            cameraForwardXZ.y = 0;
        //            Vector3 rayOrigin = playerCamera.transform.position;
        //            Vector3 rayEnd = (playerCamera.transform.position + playerCamera.transform.forward * (rangeDistance + rangeRadius * 2f));
        //            Vector3 direction = (rayEnd - rayOrigin).normalized;
        //            bool isRayChecked = Physics.SphereCast(rayOrigin, rangeRadius, direction, out _raycastHit, rangeDistance, layerForCymbals, QueryTriggerInteraction.Ignore);

        //            if (isRayChecked)
        //            {
        //                Debug.Log("�ֳ�");
        //                direction = (_raycastHit.collider.gameObject.transform.position - rayOrigin).normalized;
        //                isRayChecked = Physics.SphereCast(rayOrigin, 0.2f, direction, out _raycastHit, (rangeDistance + rangeRadius * 2f), layerFilterForCymbals, QueryTriggerInteraction.Ignore);
        //                if (isRayChecked)
        //                {
        //                    if (_raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("AutoAimedObject"))
        //                    {
        //                        if (Vector3.Angle(playerCamera.transform.forward, (_raycastHit.collider.gameObject.transform.position - rayOrigin)) < rangeAngle)
        //                        {
        //                            Debug.Log("���� Ÿ��");
        //                            //aimUI.SetTarget(_raycastHit.collider.gameObject.transform, rangeAngle);
        //                            autoAimPosition = _raycastHit.collider.gameObject.transform;
        //                            steadyInteractionState.isAutoAimObjectFounded = true;
        //                            return;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        //aimUI.SetTarget(null, rangeAngle);
        //        steadyInteractionState.isAutoAimObjectFounded = false;
        //    }
        //}

        //public void SendInfoAImUI()
        //{
        //    if (steadyInteractionState.isAutoAimObjectFounded)
        //    {
        //        aimUI.SetTarget(autoAimPosition, rangeAngle);
        //    }
        //    else
        //    {
        //        aimUI.SetTarget(null, rangeAngle);
        //    }
        //}

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
        //                    cymbals.InitCymbals(cymbalsSpawner.transform.position, hit.point, cymbalsSpeed, cymbalsDepartOffset);
        //                }
        //                else
        //                {
        //                    cymbals.InitCymbals(cymbalsSpawner.transform.position, (playerCamera.transform.position + playerCamera.transform.forward * grapplingRange), cymbalsSpeed, cymbalsDepartOffset);
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

        //void SendInfoUI()
        //{
        //    if (cymbalsObjects.Count > 0)
        //    {
        //        foreach (var cymbalsObject in cymbalsObjects)
        //        {
        //            Vector3 directoin = (cymbalsObject.transform.parent.position - playerCamera.transform.position).normalized;
        //            bool rayCheck = false;
        //            RaycastHit hit;
        //            switch (cymbalsObject.tag)
        //            {
        //                case "CymbalsdObject":
        //                    {
        //                        rayCheck = Physics.Raycast(playerCamera.transform.position, directoin, out hit, cymbalsObject.GetComponentInParent<CymbalsObject>().detectingRange * 1.5f, layerFilterForCymbals, QueryTriggerInteraction.Ignore);
        //                        if (rayCheck)
        //                        {
        //                            if (hit.collider.CompareTag("CymbalsdObject"))
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
        //                        rayCheck = Physics.Raycast(playerCamera.transform.position, directoin, out hit, cymbalsObject.GetComponentInParent<DefenseMonster>().detectingUIRange * 1.5f, layerFilterForCymbals, QueryTriggerInteraction.Ignore);
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
        //                        rayCheck = Physics.Raycast(playerCamera.transform.position, directoin, out hit, cymbalsObject.GetComponentInParent<DefenseMonster>().detectingUIRange * 1.5f, layerFilterForCymbals, QueryTriggerInteraction.Ignore);
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
        //                cymbalsObject.transform.parent.gameObject.GetComponentInChildren<OneIndicator>().SetUI(true);
        //            }
        //            else
        //            {
        //                cymbalsObject.transform.parent.gameObject.GetComponentInChildren<OneIndicator>().SetUI(false);
        //            }
        //        }
        //    }
        //}

        //private void OnTriggerEnter(Collider other)
        //{

        //    if ((other.gameObject.layer == LayerMask.NameToLayer("UITriggers")) && (other.CompareTag("CymbalsdObject") || other.CompareTag("PoisonSnake") || other.CompareTag("TrippleHeadSnake")))
        //    {
        //        cymbalsObjects.Add(other.gameObject);
        //    }
        //}
        //private void OnTriggerExit(Collider other)
        //{
        //    if ((other.gameObject.layer == LayerMask.NameToLayer("UITriggers")) && (other.CompareTag("CymbalsdObject") || other.CompareTag("PoisonSnake") || other.CompareTag("TrippleHeadSnake")))
        //    {
        //        other.gameObject.transform.parent.gameObject.GetComponentInChildren<OneIndicator>().SetUI(false);
        //        cymbalsObjects.Remove(other.gameObject);
        //    }
        //}
    }
}
