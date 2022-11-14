using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Interaction
{
    public class PairButton : InteractableObject
    {
        bool isOnNella = false;
        bool isOnSteady = false;
        [SerializeField] float pressingSpeed = 1f;
        [SerializeField] float pressingDistance = 1f;
        float unPressedPos;

        private void Start()
        {
            unPressedPos = transform.position.y;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella"))
                isOnNella = true;
            if (collision.gameObject.CompareTag("Steady"))
                isOnSteady = true;

            if (isOnNella && isOnSteady)
                StartInteraction();
        }
        public override void StartInteraction()
        {
            if (isInteractable)
            {
                photonView.RPC(nameof(SetInteractable), RpcTarget.AllViaServer, false);
                photonView.RPC(nameof(SetActivation), RpcTarget.AllViaServer, true);
                photonView.RPC(nameof(SendActivation), RpcTarget.AllViaServer);
            }
        }

        [PunRPC]
        protected override void SetActivation(bool isOn)
        {
            isActivated = isOn;
            StartCoroutine(nameof(PressButton));
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella"))
                isOnNella = false;
            if (collision.gameObject.CompareTag("Steady"))
                isOnSteady = false;
        }

        IEnumerator PressButton()
        {
            Vector3 curPos = transform.position;
            float endPos = unPressedPos - pressingDistance;
            while(true)
            {
                curPos.y -= pressingSpeed * Time.deltaTime;
                transform.position = curPos;
                if (curPos.y < endPos)
                    yield break;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
