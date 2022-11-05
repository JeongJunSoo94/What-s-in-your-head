using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Character
{
    public class RoyaleCharacterZeroInput : RoyaleCharacterInput
    {
        public override Vector3 GetInputMove()
        {
            return Vector3.zero;
        }

        public override float GetInputJump()
        {
            return 0;
        }

        public override void SetForwardTransform(Transform forward)
        {
        }
    }
}