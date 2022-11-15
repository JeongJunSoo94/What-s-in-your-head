using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Interaction
{
    public class InteractingTargetObject : MonoBehaviour
    {
        protected bool isActivated = false;

        public virtual void RecieveActivation(bool isActive)
        {
            isActivated = isActive;
        }

        public virtual void RecieveActivation(InteractableObject sender, bool isActive)
        {
            isActivated = isActive;
        }
    }
}
