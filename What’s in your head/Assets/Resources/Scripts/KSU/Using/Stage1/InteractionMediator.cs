using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Interaction
{
    public class InteractionMediator : InteractingTargetObject
    {
        [SerializeField] List<InteractableObject> interactionSenders;
        [SerializeField] List<InteractingTargetObject> interactionRecievers;

        Dictionary<InteractableObject, bool> senderPair = new();
        // Start is called before the first frame update
        void Start()
        {
            foreach(var sender in interactionSenders)
            {
                senderPair.Add(sender, false);
            }
        }

        public override void RecieveActivation(InteractableObject sender, bool isActive)
        {
            isActivated = true;
            senderPair[sender] = isActive;
            foreach(var interactionSender in interactionSenders)
            {
                if (!isActivated || !senderPair[interactionSender])
                {
                    isActivated = false;
                    return;
                }
            }

            foreach (var interactionReciever in interactionRecievers)
            {
                interactionReciever.RecieveActivation(isActivated);
            }
        }
    }

}