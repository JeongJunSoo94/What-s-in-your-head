using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using YC.Camera_;
using YC.Camera_Single;
using JCW.UI.Options.InputBindings;
using JCW.UI.InGame.Indicator;
using KSU.Object.Interaction;

namespace KSU
{    
    public class PlayerInteraction : MonoBehaviour
    {
        PlayerInteractionState interactionState;
        PlayerState playerState;
        public RailAction railAction;
        public RopeAction ropeAction;
        Animator animator;
        RaycastHit _raycastHit;
        GameObject interactableObject;
        [SerializeField] LayerMask layerFilter;

        Dictionary<GameObject, Obj_Info> detectedInteractableObjects = new();

        // Start is called before the first frame update
        void Awake()
        {
            interactionState = GetComponent<PlayerInteractionState>();
            playerState = GetComponent<PlayerState>();
            railAction = GetComponent<RailAction>();
            ropeAction = GetComponent<RopeAction>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            FindInteractableObject();
            SendInfoUI();
        }

        public void InputInteract()
        {
            if (!playerState.isMine)
                return;
            if (KeyManager.Instance.GetKeyDown(PlayerAction.Interaction))
            {
                if (interactableObject != null)
                {
                    interactableObject.GetComponent<InteractableObject>().StartInteraction();
                    return;
                }

                if (interactionState.isRailFounded && !animator.GetBool("isMoveToRail") && !interactionState.GetWhetherHasParent())
                {
                    animator.SetFloat("moveToRailSpeed", 0.833f / railAction.StartRailAction());
                    animator.SetBool("isMoveToRail", true);
                    return;
                }
                else
                {
                    if (ropeAction.interactableRope != null && !interactionState.isMoveFromRope && !interactionState.isRopeEscapeDelayOn)
                    {
                        animator.SetBool("isMoveToRope", ropeAction.RideRope());
                        return;
                    }
                }
            }
        }

        void FindInteractableObject()
        {
            if ((detectedInteractableObjects.Count > 0))
            {
                Obj_Info node = new();
                float minDist = 100f;
                GameObject minDistObj = null;
                Dictionary<GameObject, Obj_Info> temp = new();

                foreach (var detectedObject in detectedInteractableObjects.Keys)
                {
                    InteractableObject obj = detectedObject.GetComponent<InteractableObject>();
                    bool rayCheck = Physics.Raycast(transform.position + Vector3.up * 0.2f, (detectedObject.transform.position - transform.position), out _raycastHit, obj.detectingRange * 1.5f, layerFilter, QueryTriggerInteraction.Ignore);
                    if (rayCheck)
                    {
                        if (_raycastHit.collider.CompareTag("InteractableObject"))
                        {
                            if ((obj.interactableRange > _raycastHit.distance) && (minDist > _raycastHit.distance))
                            {
                                minDist = _raycastHit.distance;
                                minDistObj = detectedObject;
                            }
                            node.isUIActive = true;
                            node.isInteractable = false;
                            node.distance = Vector3.Distance(transform.position, _raycastHit.collider.gameObject.transform.position);
                            temp.Add(detectedObject, node);
                        }
                        else
                        {
                            node.isUIActive = false;
                            node.isInteractable = false;
                            temp.Add(detectedObject, node);
                        }
                    }
                    else
                    {
                        node.isUIActive = false;
                        node.isInteractable = false;
                        temp.Add(detectedObject, node);
                    }
                }

                if (minDistObj != null)
                {
                    node = temp.GetValueOrDefault(minDistObj);
                    node.isInteractable = true;
                    temp[minDistObj] = node;
                }

                detectedInteractableObjects = temp;
                interactableObject = minDistObj;
            }
        }

        void SendInfoUI()
        {
            if (!playerState.isMine)
                return;

            if (detectedInteractableObjects.Count > 0)
            {
                foreach (var obj in detectedInteractableObjects)
                {
                    Debug.Log("SendInfoUI");
                    obj.Key.GetComponentInChildren<ConvertIndicator>().SetUI(obj.Value.isUIActive, obj.Value.isInteractable, obj.Value.distance);
                }
            }
        }

        void SendUIOff()
        {
            if (!playerState.isMine)
                return;

            if (detectedInteractableObjects.Count > 0)
            {
                foreach (var obj in detectedInteractableObjects)
                {
                    obj.Key.transform.GetComponentInChildren<ConvertIndicator>().SetUI(false, false, 0f);
                }
            }
        }

        void InitDictionary()
        {
            detectedInteractableObjects.Clear();
        }

        public void InitInteraction()
        {
            SendUIOff();
            InitDictionary();
            railAction.SendUIOff();
            railAction.InitDictionary();
            ropeAction.SendUIOff();
            ropeAction.InitDictionary();
            interactionState.InitInteractionState();
        }

        public void EscapeInteraction()
        {
            railAction.EscapeRailAction();
            ropeAction.EscapeRope();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("InteractableObject"))
            {
                Debug.Log("트리거 엔터");
                //if (detectedInteractableObjects.ContainsKey(other.gameObject.transform.parent.gameObject))
                //    return;
                detectedInteractableObjects.Add(other.gameObject.transform.parent.gameObject, new Obj_Info(false, false, 100f));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("InteractableObject"))
            {
                other.gameObject.transform.parent.gameObject.GetComponentInChildren<ConvertIndicator>().SetUI(false, false, 100f);
                detectedInteractableObjects.Remove(other.gameObject.transform.parent.gameObject);

            }
        }
    }
}
