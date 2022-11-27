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
using JCW.AudioCtrl;

namespace KSU.AutoAim.Player
{
    //public enum GrappleTargetType { GrappledObject, Monster, Null};
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PhotonView))]
    public class SteadyGrappleAction : SteadyAutoAimAction
    {
        PlayerController playerController;
        Rigidbody playerRigidbody;
        
        SteadyGrapple grapple;
        string grappleDirectory;
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


        [Header("_______변경 가능 값_______")]
        [Header("플레이어 날아가는 속력")]
        public float grappleMoveSpeed = 10f;
        //[Header("갈고리 날아가는 속력")]
        //public float grappleSpeed = 10f;
        //[Header("갈고리 속력이 빠를땐 이 값을 조금 높이세요")]
        //public float grappleDepartOffset = 0.5f;
        [Header("목표지점 도착후 날아가는 속도")]
        public float escapeGrapplePower = 10f;
        //[Header("오토 타겟팅 탐지 범위(캡슐) 반지름")]
        //public float rangeRadius = 5f;
        //[Header("오토 타겟팅 탐지 범위(캡슐) 길이(거리)")]
        //public float rangeDistance = 15f;
        //[Header("갈고리 투척 최대 거리(rangeDistance + rangeRadius * 2 이상으로)")]
        //public float grapplingRange = 30f;
        //[Header("오토 타겟팅 탐지 범위 (각도)"), Range(1f, 89f)]
        //public float rangeAngle = 30f;

        //Vector3 targetPosition;


        //List<GameObject> grappledObjects = new();

        // Start is called before the first frame update
        override protected void Awake()
        {
            base.Awake();
            
            playerAnimator = GetComponent<Animator>();
            //photonView = GetComponent<PhotonView>();

            playerController = GetComponent<PlayerController>();
            playerState = GetComponent<PlayerState>();
            steadyInteractionState = GetComponent<SteadyInteractionState>();
            playerRigidbody = GetComponent<Rigidbody>();
            lookAtObj = this.gameObject.GetComponent<CameraController>().lookatBackObj;

            playerCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // 멀티용

            autoAimObject = Instantiate(autoAimObject);
            grapple = autoAimObject.GetComponent<SteadyGrapple>();
            grapple.player = this.gameObject;
            grapple.spawner = autoAimObjectSpawner;
            autoAimObject.SetActive(false);

            playerAnimator = GetComponent<Animator>();
        }
        //그래플 속의 메이크 로프를 여기서 해보자
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
                            if (_raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("AutoAimedObject") || _raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
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
            Vector3 target = Vector3.zero;
            if (GameManager.Instance.isTopView)
            {
                ///////////////// 선택지 2: 여기에 마우스 거리 만큼의 위치까지 쏘는 갈고리 발사
                Vector3 forward = (playerController.playerMouse.point.transform.position - autoAimObjectSpawner.transform.position);
                forward.y = 0;
                if (forward.magnitude > autoAimObjectRange)
                    forward = forward.normalized * autoAimObjectRange;
                target = (autoAimObjectSpawner.transform.position + forward);

                playerController.photonView.RPC(nameof(SetGrappleShoot), RpcTarget.AllViaServer, target);
            }
            else if (steadyInteractionState.isAutoAimObjectFounded)
            {
                // 도착 위치: autoAimPosition
                playerController.photonView.RPC(nameof(SetGrappleShoot), RpcTarget.AllViaServer, autoAimPosition.position);
            }
            else
            {
                // 도착위치: 화면 중앙에 레이 쏴서 도착하는 곳
                RaycastHit hit;
                bool rayCheck = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, autoAimObjectRange, -1, QueryTriggerInteraction.Ignore);
                if (rayCheck && (hit.distance > Vector3.Distance(transform.position, playerCamera.transform.position)))
                {
                    playerController.photonView.RPC(nameof(SetGrappleShoot), RpcTarget.AllViaServer, hit.point);
                }
                else
                {
                    target = playerCamera.transform.position + playerCamera.transform.forward * autoAimObjectRange;
                    playerController.photonView.RPC(nameof(SetGrappleShoot), RpcTarget.AllViaServer, target);
                }
            }
        }

        [PunRPC]
        void SetGrappleShoot(Vector3 pos)
        {
            shootPosition = pos;
        }

        protected override void InputFire()
        {
            if (playerState.isOutOfControl || playerState.isStopped)
                return;

            if (autoAimObjectSpawner.transform.parent.gameObject.activeSelf)
            {
                if (!grapple.gameObject.activeSelf)
                {
                    PlayGrappleThrowSound();
                    steadyInteractionState.isSucceededInHittingTaget = false;
                    autoAimObjectSpawner.SetActive(false);
                    grapple.InitObject(autoAimObjectSpawner.transform.position, shootPosition, autoAimObjectSpeed, autoAimObjectDepartOffset);                                       
                }
            }
        }

        public override void ShootAtSameTime()
        {
            photonView.RPC(nameof(ShootGrapple), RpcTarget.AllViaServer);
        }

        [PunRPC]
        void ShootGrapple()
        {
            playerAnimator.SetBool("isShootingGrapple", true);
        }

        public override void ResetAutoAimWeapon()
        {
            EndGrab();
            steadyInteractionState.ResetState();
        }

        public void StartGrab()
        {
            steadyInteractionState.isGrabMonster = true;
            playerState.isRiding = true;
            playerController.MoveStop();
            Vector3 lookVec = (shootPosition - transform.position);
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
            if (!playerState.isMine)
                return;
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
            playerController.photonView.RPC(nameof(SetGrappleOff), RpcTarget.AllViaServer);
        }

        [PunRPC]
        void SetGrappleOff()
        {
            grapple.gameObject.SetActive(false);
        }

        void PlayGrappleThrowSound()
        {
            SoundManager.Instance.PlayEffect("S3_SteadyHookFire");
        }
        public void PlayGrappleFlyingSound()
        {
            SoundManager.Instance.PlayEffect("S3_SteadyHookFly");
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
