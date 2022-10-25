using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using JCW.UI.Options.InputBindings;
using YC.Camera_;
using YC.Camera_Single;
using JCW.UI.InGame.Indicator;

namespace KSU
{
    public struct Obj_Info
    {
        public Obj_Info(bool isUIAct, bool isInter, float dist)
        {
            isUIActive = isUIAct; isInteractable = isInter; distance = dist;
        }

        public Obj_Info(bool isUIAct, bool isInter)
        {
            isUIActive = isUIAct; isInteractable = isInter; distance = 0;
        }
        public bool isUIActive;
        public bool isInteractable;
        public float distance;
    }

    public class RopeAction : MonoBehaviour
    {
        Camera mainCamera;
        PlayerController playerController;
        PlayerState playerState;
        PlayerInteractionState interactionState;

        LineRenderer rope;
        [SerializeField] GameObject hand;

        RaycastHit _raycastHit;
        [SerializeField] LayerMask layerFilterForRope;

        GameObject currentRidingRope;
        public GameObject interactableRope;

        Dictionary<GameObject, Obj_Info> detectedRopes = new Dictionary<GameObject, Obj_Info>();

        [Header("_______변경 가능 값_______")]
        [Header("로프로 날아가는 속도")] 
        public float moveToRopeSpeed = 6f;
        [Header("로프 해제 시 날아가는")] 
        public float escapingRopeSpeed = 6f;
        [Header("로프 해제 후 딜레이 타임")] 
        public float escapingRopeDelayTime = 1f;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            playerState = GetComponent<PlayerState>();
            interactionState = GetComponent<PlayerInteractionState>();
            mainCamera = GetComponent<CameraController>().FindCamera(); // 멀티용

            if (mainCamera == null)
                Debug.Log("카메라 NULL");

            rope = GetComponentInChildren<LineRenderer>();
        }

        private void Update()
        {
            if(playerState.isMine)
            {
                FindInteractableRope();
                if(GameManager.Instance.characterOwner.Count !=0)
                    SendInfoUI();
            }

            if(interactionState.isRidingRope)
            {
                if(currentRidingRope != null)
                {
                    rope.SetPosition(0, hand.transform.position);
                    rope.SetPosition(1, currentRidingRope.transform.position);
                }
            }
        }

        void FindInteractableRope()
        {
            if ((detectedRopes.Count > 0) && currentRidingRope == null)
            {
                Obj_Info node = new();
                float minDist = 100f;
                GameObject minDistObj = null;
                Dictionary<GameObject, Obj_Info> temp = new();
                if (interactionState.isRailFounded)
                {
                    foreach (var detectedRope in detectedRopes.Keys)
                    {
                        RopeSpawner spawner = detectedRope.GetComponent<RopeSpawner>();
                        bool rayCheck = Physics.Raycast(transform.position, (detectedRope.transform.position - transform.position), out _raycastHit, spawner.detectingRange * 1.5f, layerFilterForRope, QueryTriggerInteraction.Ignore);
                        if (rayCheck)
                        {
                            if (_raycastHit.collider.CompareTag("Rope"))
                            {
                                node.isUIActive = true;
                                node.isInteractable = false;
                                node.distance = Vector3.Distance(transform.position, _raycastHit.collider.gameObject.transform.position);
                                temp.Add(detectedRope, node);
                            }
                            else
                            {
                                node.isUIActive = false;
                                node.isInteractable = false;
                                temp.Add(detectedRope, node);
                            }
                        }
                        else
                        {
                            node.isUIActive = false;
                            node.isInteractable = false;
                            temp.Add(detectedRope, node);
                        }
                    }

                    if (minDistObj != null)
                    {
                        node = temp.GetValueOrDefault(minDistObj);
                        node.isInteractable = true;
                        temp[minDistObj] = node;
                    }
                    detectedRopes = temp;
                    interactableRope = minDistObj;
                }
                else
                {
                    foreach (var detectedRope in detectedRopes.Keys)
                    {
                        RopeSpawner spawner = detectedRope.GetComponent<RopeSpawner>();
                        bool rayCheck = Physics.Raycast(transform.position, (detectedRope.transform.position - transform.position), out _raycastHit, spawner.detectingRange * 1.5f, layerFilterForRope, QueryTriggerInteraction.Ignore);
                        if (rayCheck)
                        {
                            if (_raycastHit.collider.CompareTag("Rope"))
                            {
                                if ((spawner.interactableRange > _raycastHit.distance) && (minDist > _raycastHit.distance) && !interactionState.isRailFounded)
                                {
                                    minDist = _raycastHit.distance;
                                    minDistObj = detectedRope;
                                }
                                node.isUIActive = true;
                                node.isInteractable = false;
                                node.distance = Vector3.Distance(transform.position, _raycastHit.collider.gameObject.transform.position);
                                temp.Add(detectedRope, node);
                            }
                            else
                            {
                                node.isUIActive = false;
                                node.isInteractable = false;
                                temp.Add(detectedRope, node);
                            }
                        }
                        else
                        {
                            node.isUIActive = false;
                            node.isInteractable = false;
                            temp.Add(detectedRope, node);
                        }
                    }

                    if (minDistObj != null)
                    {
                        node = temp.GetValueOrDefault(minDistObj);
                        node.isInteractable = true;
                        temp[minDistObj] = node;
                    }
                    detectedRopes = temp;
                    interactableRope = minDistObj;
                }
            }
        }

        public void RideRope()
        {
            playerState.IsAirJumping = false;
            playerState.WasAirDashing = false;
            interactionState.isRidingRope = true;
            interactionState.isMoveToRope = true;

            GetComponent<Rigidbody>().velocity = Vector3.zero;

            currentRidingRope = interactableRope;
            interactableRope = null;

            Obj_Info node = detectedRopes.GetValueOrDefault(currentRidingRope);
            node.isUIActive = false;
            node.isInteractable = false;
            detectedRopes[currentRidingRope] = node;

            currentRidingRope.GetComponent<RopeSpawner>().StartRopeAction(this.gameObject, moveToRopeSpeed);

            rope.SetPosition(0, hand.transform.position);
            rope.SetPosition(1, hand.transform.position);
            rope.enabled = true;
        }
        public void EscapeRope()
        {
            StartCoroutine("DelayEscape");

            float jumpPower = currentRidingRope.GetComponent<RopeSpawner>().EndRopeAction(this.gameObject);

            Obj_Info node = detectedRopes.GetValueOrDefault(currentRidingRope);
            node.isUIActive = true;
            node.isInteractable = false;
            detectedRopes[currentRidingRope] = node;

            currentRidingRope = null;
            Vector3 inertiaVec = mainCamera.transform.forward;
            inertiaVec.y = 0;

            transform.LookAt(transform.position + inertiaVec);
            playerController.MakeinertiaVec(escapingRopeSpeed, inertiaVec.normalized);
            playerController.moveVec = Vector3.up * playerController.jumpSpeed * jumpPower;
            playerController.characterState.isOutOfControl = false;
            rope.enabled = false;
        }

        IEnumerator DelayEscape()
        {
            interactionState.isMoveFromRope = true;
            yield return new WaitForSeconds(escapingRopeDelayTime);
            interactionState.isMoveFromRope = false;
            interactionState.isRidingRope = false;
        }

        void SendInfoUI()
        {
            if (detectedRopes.Count > 0)
            {
                foreach (var rope in detectedRopes)
                {
                    rope.Key.transform.GetComponentInChildren<ConvertIndicator>().SetUI(rope.Value.isUIActive, rope.Value.isInteractable, rope.Value.distance);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Rope") && GetComponent<PhotonView>().IsMine)
            {
                Debug.Log("트리거 엔터 : " + other.gameObject.transform.parent.gameObject);
                detectedRopes.Add(other.gameObject.transform.parent.gameObject, new Obj_Info(false, false, 100f));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Rope") && GetComponent<PhotonView>().IsMine)
            {
                Debug.Log("트리거 탈출 : " + other.gameObject.transform.parent.gameObject);
                other.gameObject.transform.parent.gameObject.GetComponentInChildren<ConvertIndicator>().SetUI(false, false, 100f);                
                detectedRopes.Remove(other.gameObject.transform.parent.gameObject);
            }
        }
    }
}
