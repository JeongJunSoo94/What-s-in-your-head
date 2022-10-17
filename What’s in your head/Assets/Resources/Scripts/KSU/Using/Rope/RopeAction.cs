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
        PlayerController playerController;
        PlayerState playerState;
        PlayerInteractionState interactionState;

        RaycastHit _raycastHit;
        LayerMask layerFilterForRope;

        public GameObject currentRidingRope;
        public GameObject interactableRope;

        public Dictionary<GameObject, Obj_Info> detectedRopes = new Dictionary<GameObject, Obj_Info>();

        [Header("������ ���ư��� �ӵ�")] public float moveToRopeSpeed = 6f;
        [Header("���� ���� �� ���ư���")] public float escapingRopeSpeed = 6f;
        [Header("���� ���� �� ������ Ÿ��")] public float escapingRopeDelayTime = 1f;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            playerState = GetComponent<PlayerState>();
            interactionState = GetComponent<PlayerInteractionState>();
            if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
                mainCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // ��Ƽ��
            else
                mainCamera = this.gameObject.GetComponent<CameraController_Single>().FindCamera(); // �̱ۿ�

            if (mainCamera == null)
                Debug.Log("ī�޶� NULL");

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
                if (interactionState.isRailFounded)
                {

                }
                Obj_Info node = new();
                node.playerCamera = mainCamera;
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
                    //isMine false�� �Ⱥ��� / ���� ��
                    //Debug.Log(rope.Key.GetComponentInChildren<TargetIndicator>().gameObject.name);
                    Debug.Log("�Ѱ��� ī�޶�" + rope.Value.playerCamera);
                    rope.Key.GetComponentInChildren<TargetIndicator>().SetUI(rope.Value.isUIActive, rope.Value.isInteractable, rope.Value.distance, rope.Value.playerCamera);
                    // UI����(bool)�� �ٸ��� ��ȣ struct Obj_Info(bool isUIActive,bool isInteractive, float distance)�� ����
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Rope") && this.gameObject.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log("Ʈ���� ���� : " + mainCamera);
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
