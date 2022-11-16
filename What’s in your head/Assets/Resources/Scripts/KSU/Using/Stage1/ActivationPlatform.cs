using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Interaction
{
    public class ActivationPlatform : InteractingTargetObject
    {
        public override void RecieveActivation(InteractableObject sender, bool isActive)
        {
            if (isActive)
            {
                this.gameObject.SetActive(true);
            }
        }
    }
}
