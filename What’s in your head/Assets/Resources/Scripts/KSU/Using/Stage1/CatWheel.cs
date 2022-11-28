using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace KSU.Object.Interaction.Stage1
{
    public class CatWheel : InteractingTargetObject
    {
        [SerializeField] List<InteractableObject> forwardInteractionSenders;
        [SerializeField] List<InteractableObject> backwardInteractionSenders;
        [SerializeField] Transform cylinder;
        public float rotatingSpeed = 20f;
        public float pushPower = 80f;
        Dictionary<InteractableObject, bool> forwardSenderPair = new();
        Dictionary<InteractableObject, bool> backwardSenderPair = new();

        Rigidbody objRigidbody;

        Vector3 curRot;

        public int power = 0;

        PhotonView photonView;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            objRigidbody = GetComponent<Rigidbody>();
            curRot = cylinder.rotation.eulerAngles;
            photonView = GetComponent<PhotonView>();
            foreach (var sender in forwardInteractionSenders)
            {
                (sender as PushablePlate).standardVector = transform.forward;
                forwardSenderPair.Add(sender, false);
            }

            foreach (var sender in backwardInteractionSenders)
            {
                (sender as PushablePlate).standardVector = -transform.forward;
                backwardSenderPair.Add(sender, false);
            }
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                power = 0;
                foreach (var interactionSender in forwardInteractionSenders)
                {
                    power += (interactionSender as PushablePlate).pushingNum;
                }

                foreach (var interactionSender in backwardInteractionSenders)
                {
                    power -= (interactionSender as PushablePlate).pushingNum;
                }
                objRigidbody.AddForce(transform.forward * power * pushPower);
                Roll();
            }
        }

        public override void RecieveActivation(InteractableObject sender, bool isActive)
        {
            power = 0;
            if (forwardInteractionSenders.Contains(sender))
            {
                forwardSenderPair[sender] = isActive;
            }
            else if (backwardInteractionSenders.Contains(sender))
            {
                backwardSenderPair[sender] = isActive;
            }
        }

        void Roll()
        {
            if (objRigidbody.velocity.magnitude > 0.05f)
            {
                if (Vector3.Angle(objRigidbody.velocity, transform.forward) < 90f)
                {
                    curRot.x += rotatingSpeed * Time.deltaTime;
                    if (curRot.x > 360f)
                        curRot.x -= 360f;
                    cylinder.rotation = Quaternion.Euler(curRot);
                }
                else
                {
                    curRot.x -= rotatingSpeed * Time.deltaTime;
                    if (curRot.x < 0f)
                        curRot.x = 360f;
                    cylinder.rotation = Quaternion.Euler(curRot);
                }
            }
        }
    }
}
