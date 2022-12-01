using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using YC.Camera_;
using YC.Camera_Single;

using JCW.UI.InGame.Indicator;
using JCW.UI.InGame;
using KSU.AutoAim.Object;
using JCW.AudioCtrl;

namespace KSU.AutoAim.Player
{
    public enum AutoAimTargetType { GrappledObject, Monster, CymbalsTargetObject, Null };
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PhotonView))]
    abstract public class SteadyAutoAimAction : MonoBehaviour
    {
        protected SteadyInteractionState steadyInteractionState;
        protected PlayerState playerState;
        protected Animator playerAnimator;
        protected PhotonView photonView;

        protected Camera playerCamera;
        protected Transform lookAtObj;
        [SerializeField] protected LayerMask layerFilterForAutoAim;
        [SerializeField] protected LayerMask layerForAutoAim;
        protected RaycastHit _raycastHit;

        [SerializeField] protected GameObject autoAimObjectSpawner;
        [SerializeField] protected GameObject autoAimObject;
        protected GameObject hitTarget;

        protected Vector3 shootPosition;
        protected Transform autoAimPosition;
        protected AimUI aimUI;
        protected AudioSource audioSource;

        [Header("_______변경 가능 값_______")]
        [Header("투사체 날아가는 속력")]
        public float autoAimObjectSpeed = 10f;
        [Header("투사체 속력이 빠를땐 이 값을 조금 높이세요")]
        public float autoAimObjectDepartOffset = 0.5f;
        [Header("오토 타겟팅 탐지 범위(캡슐) 반지름")]
        public float rangeRadius = 5f;
        [Header("오토 타겟팅 탐지 범위(캡슐) 길이(거리)")]
        public float rangeDistance = 15f;
        [Header("투사체 투척 최대 거리(rangeDistance + rangeRadius * 2 이상으로)")]
        public float autoAimObjectRange = 30f;
        [Header("오토 타겟팅 탐지 범위 (각도)"), Range(1f, 89f)]
        public float rangeAngle = 30f;

        protected GameObject autoAimTarget;
        protected Vector3 targetPosition;

        protected AutoAimTargetType curTargetType = AutoAimTargetType.Null;

        protected List<GameObject> autoAimTargetObjects = new();

        virtual protected void Awake()
        {
            aimUI = GetComponent<CameraController>().aimUI.GetComponent<AimUI>();
            audioSource = GetComponent<AudioSource>();
            photonView = GetComponent<PhotonView>();            
            SoundManager.Set3DAudio(photonView.ViewID, audioSource, 1, 50f);
        }

        abstract protected void SearchAutoAimTargetdObject();

        abstract protected void InputFire();

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

        protected void SendInfoUI()
        {
            if (autoAimTargetObjects.Count > 0)
            {
                foreach (var cymbalsObject in autoAimTargetObjects)
                {
                    Vector3 direction = (cymbalsObject.transform.parent.position - playerCamera.transform.position).normalized;
                    bool rayCheck = false;
                    RaycastHit hit;
                    rayCheck = Physics.Raycast(playerCamera.transform.position, direction, out hit, cymbalsObject.GetComponentInParent<AutoAimTargetObject>().detectingUIRange * 1.5f, layerFilterForAutoAim, QueryTriggerInteraction.Ignore);
                    
                    if (rayCheck)
                        rayCheck = hit.collider.CompareTag(cymbalsObject.tag);
                    OneIndicator indicator = cymbalsObject.transform.parent.GetComponentInChildren<OneIndicator>();
                    if (indicator != null)
                        indicator.SetUI(rayCheck);


                }
            }
        }

        public virtual void ResetAutoAimWeapon()
        {
            Debug.Log("오토에임으로 들어옴");
        }

        public virtual void ShootAtSameTime()
        {
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

        public bool GetWhetherHit(AutoAimTargetType autoAimTargetType)
        {
            if ((autoAimTargetType == curTargetType))
            {
                curTargetType = AutoAimTargetType.Null;
                return steadyInteractionState.isSucceededInHittingTaget;
            }
            return false;
        }

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

        public bool GetWhetherautoAimObjectActived()
        {
            return autoAimObject.activeSelf;
        }

    }
}

