using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class SteadyInteractionState : MonoBehaviour
    {
        public bool isAutoAimObjectFounded = false;
        public bool isSucceededInHittingTaget = false;

        public bool isGrappling = false;
        public bool isGrabMonster = false;

        public void ResetState()
        {
            isAutoAimObjectFounded = false;
            isSucceededInHittingTaget = false;

            isGrappling = false;
            isGrabMonster = false;

        }
    }

}
