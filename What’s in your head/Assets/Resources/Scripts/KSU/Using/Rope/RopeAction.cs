using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using JCW.UI.Options.InputBindings;
using YC.Camera_;
using YC.Camera_Single;
using JCW.UI.InGame;

namespace KSU
{
    public struct Obj_Info
    {
        public Obj_Info(bool isUIAct, bool isInter, float dist, Camera pCamera)
        {
            isUIActive = isUIAct; isInteractable = isInter; distance = dist; playerCamera = pCamera;
        }
        public bool isUIActive;
        public bool isInteractable;
        public float distance;
        public Camera playerCamera;
    }

    public class RopeAction : MonoBehaviour
    {
        Camera mainCamera;
        PlayerController3D playerController;
        CharacterState3D playerState;
        PlayerInteractionState interactionState;

        RaycastHit _raycastHit;
        LayerMask layerFilterForRope;

        public GameObject currentRidingRope;
        public GameObject interactableRope;

        public Dictionary<GameObject, Obj_Info> detectedRopes = new Dictionary<GameObject, Obj_Info>();

        [Header("로프로 날아가는 속도")] public float moveToRopeSpeed = 6f;
        [Header("로프 해제 시 날아가는")] public float escapingRopeSpeed = 6f;
        [Header("로프 해제 후 딜레이 타임")] public float escapingRopeDelayTime = 1f;

        private void Awake()
        {
            playerController = GetComponent<PlayerController3D>();
            playerState = GetComponent<CharacterState3D>();
            interactionState = GetComponent<PlayerInteractionState>();
            if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
                mainCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // 멀티용
            else
                mainCamera = this.gameObject.GetComponent<CameraController_Single>().FindCamera(); // 싱글용

            if (mainCamera == null)
                Debug.Log("카메라 NULL");
            else
                Debug.Log(mainCamera);
            layerFilterForRope = ((-1) - (1 << LayerMask.NameToLayer("Player")));
        }

        private void Update()
        {
            FindInteractableRope();
            SendInfoUI();
        }

        void FindInteractableRope()
        {
            if ((detectedRopes.Count > 0) && currentRidingRope == null)
            {
                Obj_Info node = new();
                node.playerCamera = mainCamera;
                float minDist = 100f;
                GameObject minDistObj = null;
                Dictionary<GameObject, Obj_Info> temp = new();                
                foreach (var detectedRope in detectedRopes.Keys)
                {                              
                    RopeSpawner spawner = detectedRope.GetComponent<RopeSpawner>();
                    bool rayCheck = Physics.Raycast(transform.position, (detectedRope.transform.position - transform.position), out _raycastHit, spawner.detectingRange * 1.5f, layerFilterForRope, QueryTriggerInteraction.Ignore);
                    if(rayCheck)
                    {
                        if(_raycastHit.collider.CompareTag("Rope"))
                        {
                            if((spawner.interactableRange > _raycastHit.distance) && (minDist > _raycastHit.distance) && !interactionState.isRailFounded)
                            {
                                minDist = _raycastHit.distance;
                                minDistObj = detectedRope;
                            }
                            node.isUIActive = true;
                            node.isInteractable = false;
                            node.distance = Vector3.Distance(transform.position, _raycastHit.collider.gameObject.transform.position);
                            //temp[detectedRope] = node;
                            temp.Add(detectedRope, node);
                        }
                        else
                        {
                            node.isUIActive = false;
                            node.isInteractable = false;
                            //temp[detectedRope] = node;
                            temp.Add(detectedRope, node);
                        }
                    }
                    else
                    {
                        node.isUIActive = false;
                        node.isInteractable = false;
                        //temp[detectedRope] = node;
                        temp.Add(detectedRope, node);
                    }
                }
                
                if(minDistObj != null)
                {
                    node = temp.GetValueOrDefault(minDistObj);
                    node.isInteractable = true;
                    temp[minDistObj] = node;
                }
                detectedRopes = temp;
                interactableRope = minDistObj;
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

            currentRidingRope.GetComponentInChildren<RopeSpawner>().StartRopeAction(this.gameObject, moveToRopeSpeed);
        }
        public void EscapeRope()
        {
            StartCoroutine("DelayEscape");

            float jumpPower = currentRidingRope.GetComponentInChildren<RopeSpawner>().EndRopeAction(this.gameObject);

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
            playerController.enabled = true;
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
                    //isMine false면 안보냄 / 현재 널
                    //Debug.Log(rope.Key.GetComponentInChildren<TargetIndicator>().gameObject.name);
                    Debug.Log("넘겨줄 카메라" + rope.Value.playerCamera);
                    rope.Key.GetComponentInChildren<TargetIndicator>().SetUI(rope.Value.isUIActive, rope.Value.isInteractable, rope.Value.distance, rope.Value.playerCamera);
                    // UI상태(bool)가 다르면 신호 struct Obj_Info(bool isUIActive,bool isInteractive, float distance)를 보냄
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Rope") && this.gameObject.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log("트리거 엔터 : " + mainCamera);
                detectedRopes.Add(other.gameObject, new Obj_Info(false, false, 100f, mainCamera));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Rope") && this.gameObject.GetComponent<PhotonView>().IsMine)
            {
                detectedRopes.Remove(other.gameObject);
            }
        }
    }
}
