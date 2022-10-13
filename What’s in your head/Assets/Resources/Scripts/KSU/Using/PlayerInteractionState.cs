using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class PlayerInteractionState : MonoBehaviour
    {
        public bool isRidingRope = false;
        public bool isMoveToRope = false;
        public bool isMoveFromRope = false;

        public bool isRailFounded = false;
        public bool isRailReady = false;
        public bool isRidingRail = false;
        public bool isMoveToRail = false;

        // Steady
        public bool isHookableObjectFounded = false;
        public bool isRidingHookingRope = false;

    }
}