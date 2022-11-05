using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Character
{
    public abstract class RoyaleCharacterInput : MonoBehaviour
    {
        public abstract Vector3 GetInputMove();
        public abstract float GetInputJump();
        public abstract void SetForwardTransform(Transform forward);
    }
}