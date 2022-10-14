using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;

namespace KSU
{
    public class RopeAction : MonoBehaviour
    {
        Camera mainCamera;
        PlayerController3D playerController;
        CharacterState3D playerState;
        PlayerInteractionState interactionState;

        LayerMask layerFilterForRope;

        public GameObject currentRope;
        public GameObject minDistRope;
        public List<GameObject> detectedRopeSpawners = new List<GameObject>();

        public float escapingRopeSpeed = 6f;
        public float escapingRopeDelayTime = 1f;

        private void Awake()
        {
            layerFilterForRope = ((-1) - (1 << LayerMask.NameToLayer("Player")));
        }

        void FindInteractableRope()
        {
            if (minDistRope != null && !interactionState.isRidingRope)
            {
                RaycastHit hit;
                RopeSpawner spawner = minDistRope.GetComponentInChildren<RopeSpawner>();

                Physics.Raycast(transform.position, (minDistRope.transform.position - transform.position), out hit, spawner.detectingRange * 1.5f, layerFilterForRope, QueryTriggerInteraction.Ignore);

                if (!(hit.distance < spawner.interactableRange))
                {
                    minDistRope = null;
                }

                //RaycastHit hit;
                //GameObject key = interactableObj.Key;
                //GameObject value = interactableObj.Value;
                //RopeSpawner spawner = key.GetComponentInChildren<RopeSpawner>();

                //Physics.Raycast(transform.position, (key.transform.position - transform.position), out hit, spawner.detectingRange * 1.5f, layerFilterForRope, QueryTriggerInteraction.Ignore);

                //if (key.gameObject != hit.transform.gameObject)
                //{
                //    value.SetActive(false);
                //}
                //else
                //{
                //    value.SetActive(true);
                //    float amount = 1f - (hit.distance - spawner.interactableRange) / (spawner.detectingRange - spawner.interactableRange);
                //    if (!interactionState.isRailReady && !interactionState.isRidingRope)
                //    {
                //        if (hit.distance < spawner.interactableRange)
                //        {
                //            if (minDist > hit.distance)
                //            {
                //                minDist = hit.distance;
                //                if (minDistRope != null)
                //                {
                //                    detectedRopeSpawners.GetValueOrDefault(minDistObj).GetComponentsInChildren<Image>()[1].sprite = gaugeImage;
                //                }
                //                minDistRope = key;
                //            }
                //        }
                //    }
                //}
            }
        }

        public void RideRope()
        {
            playerState.IsAirJumping = false;
            interactionState.isRidingRope = true;
            interactionState.isMoveToRope = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            minDistRope.GetComponentInChildren<RopeSpawner>().StartRopeAction(this.gameObject);
        }
        public void EscapeRope()
        {
            StartCoroutine("DelayEscape");
            float jumpPower = minDistRope.GetComponentInChildren<RopeSpawner>().EndRopeAction(this.gameObject);
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
            foreach(var ropeSpawner in detectedRopeSpawners)
            {

            }
        }
    }
}
