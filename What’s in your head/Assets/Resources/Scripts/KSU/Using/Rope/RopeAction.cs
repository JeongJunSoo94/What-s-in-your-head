using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using JCW.UI.Options.InputBindings;
using YC.YC_Camera;
using YC.YC_CameraSingle;
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
        Animator animator;

        LineRenderer rope;
        [SerializeField] GameObject hand;

        RaycastHit _raycastHit;
        [SerializeField] LayerMask layerFilterForRope;

        public GameObject currentRidingRope;
        Vector3 currentRidingRopePosition;
        public GameObject interactableRope;
        public float ridingRopeDelayTime = 0.5f;

        bool isRopeOn = false;


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
            animator = GetComponent<Animator>();
            mainCamera = GetComponent<CameraController>().FindCamera(); // 멀티용
            rope = GetComponentInChildren<LineRenderer>();
        }

        private void Update()
        {
            FindInteractableRope();
            SendInfoUI();

            if (isRopeOn)
            {
                rope.SetPosition(0, hand.transform.position);
                rope.SetPosition(1, currentRidingRopePosition);
            }
        }

        [PunRPC]
        void SetRope(Vector3 ropePos, bool isOn)
        {
            currentRidingRopePosition = ropePos;
            isRopeOn = isOn;
            rope.enabled = isOn;
        }
        
        public void InitDictionary()
        {
            detectedRopes.Clear();
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

        public bool RideRope()
        {
            if(currentRidingRope != null)
            {
                Debug.Log("currentRidingRope != null");
                return false;
            }
            playerState.IsAirJumping = false;
            playerState.WasAirDashing = false;
            //interactionState.isRidingRope = true;
            interactionState.isMoveToRope = true;

            GetComponent<Rigidbody>().velocity = Vector3.zero;

            currentRidingRope = interactableRope;
            playerController.photonView.RPC(nameof(SetRope), RpcTarget.AllViaServer, currentRidingRope.transform.position, true);
            interactableRope = null;

            Obj_Info node = detectedRopes.GetValueOrDefault(currentRidingRope);
            node.isUIActive = false;
            node.isInteractable = false;
            detectedRopes[currentRidingRope] = node;

            if (currentRidingRope.GetComponent<RopeSpawner>().StartRopeAction(this.gameObject, moveToRopeSpeed))
            {
                rope.SetPosition(0, hand.transform.position);
                rope.SetPosition(1, hand.transform.position);
                //Debug.Log("StartRopeAction == true");
                StartCoroutine(nameof(DelayRide));
                return true;
            }
            else
            {
                //Debug.Log("StartRopeAction == false");
                return false;
            }
        }
        public bool GetWhetherMovingToRope()
        {
            return !interactionState.isMoveToRope;
        }

        IEnumerator DelayRide()
        {
            interactionState.isRopeEscapeDelayOn = true;
            yield return new WaitForSeconds(ridingRopeDelayTime);
            interactionState.isRopeEscapeDelayOn = false;
        }

        public void RecieveDirection(float threshHold)
        {
            animator.SetFloat("RopeSwing",threshHold);
        }
        public void EscapeRope()
        {
            StopCoroutine(nameof(DelayEscape));
            StartCoroutine(nameof(DelayEscape));
            if(currentRidingRope == null)
            {
                interactableRope = null;
                return;
            }
            animator.SetBool("isRidingRope", false);
            //Debug.Log("파라미터 변화");
            //Debug.Log("로프 해제 진입");
            float jumpPower = currentRidingRope.GetComponent<RopeSpawner>().EndRopeAction(this.gameObject);
            //Debug.Log("jumpPower: " + jumpPower);
            Obj_Info node = detectedRopes.GetValueOrDefault(currentRidingRope);
            node.isUIActive = true;
            node.isInteractable = false;
            detectedRopes[currentRidingRope] = node;
            currentRidingRope = null;
            playerController.photonView.RPC(nameof(SetRope), RpcTarget.AllViaServer, Vector3.zero, false);

            Vector3 inertiaVec = mainCamera.transform.forward;
            inertiaVec.y = 0;

            transform.LookAt(transform.position + inertiaVec);
            playerController.MakeinertiaVec(escapingRopeSpeed, inertiaVec.normalized);
            playerController.moveVec = Vector3.up * playerController.jumpSpeed * jumpPower;
            playerState.WasAirDashing = false;
            playerState.IsAirJumping = false;
            playerController.characterState.isRiding = false;
        }

        IEnumerator DelayEscape()
        {
            interactionState.isMoveFromRope = true;
            yield return new WaitForSeconds(escapingRopeDelayTime);
            interactionState.isMoveFromRope = false;
            interactionState.isRidingRope = false;
        }

        public bool GetWhetherBeEscapeFromRope()
        {
            return interactionState.isMoveFromRope;
        }

        void SendInfoUI()
        {
            if (!playerState.isMine)
                return;

            if (detectedRopes.Count > 0)
            {
                foreach (var rope in detectedRopes)
                {
                    rope.Key.transform.GetComponentInChildren<ConvertIndicator>().SetUI(rope.Value.isUIActive, rope.Value.isInteractable, rope.Value.distance);
                }
            }
        }

        public void SendUIOff()
        {
            if (!playerState.isMine)
                return;

            if (detectedRopes.Count > 0)
            {
                foreach (var rope in detectedRopes)
                {
                    rope.Key.GetComponentInChildren<ConvertIndicator>().SetUI(false, false, 0f);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Rope"))
            {
                if(detectedRopes.ContainsKey(other.gameObject.transform.parent.gameObject))
                    return;
                detectedRopes.Add(other.gameObject.transform.parent.gameObject, new Obj_Info(false, false, 100f));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Rope"))
            {
                other?.gameObject.transform.parent.gameObject.GetComponentInChildren<ConvertIndicator>().SetUI(false, false, 100f);
                detectedRopes.Remove(other.gameObject.transform.parent.gameObject);
                
            }
        }
    }
}
