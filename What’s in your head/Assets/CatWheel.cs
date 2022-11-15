using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace KSU.Object.Interaction.Stage1
{
    public class CatWheel : InteractingTargetObject
    {
        //[SerializeField] List<InteractableObject> forwardInteractionSenders;
        //[SerializeField] List<InteractableObject> backwardInteractionSenders;
        [SerializeField] Transform cylinder;
        public float rotatingSpeed = 20f;

        //Dictionary<InteractableObject, bool> forwardSenderPair = new();
        //Dictionary<InteractableObject, bool> backwardSenderPair = new();

        Rigidbody objRigidbody;

        Vector3 curRot;
        //bool isMoving = false;
        //int power = 0;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            objRigidbody = GetComponent<Rigidbody>();
            curRot = cylinder.rotation.eulerAngles;
            //foreach (var sender in forwardInteractionSenders)
            //{
            //    forwardSenderPair.Add(sender, false);
            //}

            //foreach (var sender in backwardInteractionSenders)
            //{
            //    backwardSenderPair.Add(sender, false);
            //}
        }

        private void Update()
        {
            Roll();
        }

        //public override void RecieveActivation(InteractableObject sender, bool isActive)
        //{
        //    if(forwardInteractionSenders.Contains(sender))
        //    {
        //        forwardSenderPair[sender] = isActive;
        //    }
        //    else if(backwardInteractionSenders.Contains(sender))
        //    {
        //        backwardSenderPair[sender] = isActive;
        //    }

        //    foreach (var interactionSender in forwardInteractionSenders)
        //    {
        //        if (forwardSenderPair[interactionSender])
        //        {
        //            power++;
        //        }
        //    }

        //    foreach (var interactionSender in backwardInteractionSenders)
        //    {
        //        if (backwardSenderPair[interactionSender])
        //        {
        //            power--;
        //        }
        //    }

        //    if (power == 0)
        //        isMoving = false;
        //    else
        //        isMoving = true;
        //}

        void Roll()
        {
            if(objRigidbody.velocity.magnitude > 0.2f)
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
