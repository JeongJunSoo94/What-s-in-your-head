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

        public int railTriggerDetectionNum = 0;
        public bool isRailFounded = false;
        public bool isRailTriggered = false;
        public bool isRidingRail = false;
        public bool isMovingToRail = false;
        public bool isRailJumpingUp = false;
        public bool isRailJumping = false;
    }
}