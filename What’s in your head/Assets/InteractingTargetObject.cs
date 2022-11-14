using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Interaction
{
    public class InteractingTargetObject : MonoBehaviour
    {
        protected bool isActivated = true;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public virtual void RecieveActivation(bool isActive)
        {
            isActivated = isActive;
        }
    }
}
