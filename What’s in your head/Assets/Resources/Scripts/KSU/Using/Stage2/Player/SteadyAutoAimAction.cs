using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using YC.Camera_;
using YC.Camera_Single;

using JCW.UI.InGame.Indicator;
using JCW.UI.InGame;
using KSU.AutoAim.Object;

namespace KSU.AutoAim.Player
{
    public enum AutoAimTargetType { GrappledObject, Monster, CymbalsTargetObject, Null };
    public class SteadyAutoAimAction : MonoBehaviour
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

        protected Transform autoAimPosition;
        [SerializeField] protected AimUI aimUI;

        [Header("_______���� ���� ��_______")]
        [Header("����ü ���ư��� �ӷ�")]
        public float autoAimObjectSpeed = 10f;
        [Header("����ü �ӷ��� ������ �� ���� ���� ���̼���")]
        public float autoAimObjectDepartOffset = 0.5f;
        [Header("���� Ÿ���� Ž�� ����(ĸ��) ������")]
        public float rangeRadius = 5f;
        [Header("���� Ÿ���� Ž�� ����(ĸ��) ����(�Ÿ�)")]
        public float rangeDistance = 15f;
        [Header("����ü ��ô �ִ� �Ÿ�(rangeDistance + rangeRadius * 2 �̻�����)")]
        public float autoAimObjectRange = 30f;
        [Header("���� Ÿ���� Ž�� ���� (����)"), Range(1f, 89f)]
        public float rangeAngle = 30f;

        protected GameObject autoAimTarget;
        protected Vector3 targetPosition;

        protected AutoAimTargetType curTargetType = AutoAimTargetType.Null;

        protected List<GameObject> autoAimTargetObjects = new();

        protected virtual void SearchAutoAimTargetdObject()
        {
            
        }

        protected virtual void InputFire()
        {

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

        protected void SendInfoUI()
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

        public bool GetWhetherHit(AutoAimTargetType autoAimTargetType)
        {
            if ((autoAimTargetType == curTargetType))
            {
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

