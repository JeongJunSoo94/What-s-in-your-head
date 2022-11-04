using UnityEngine;

namespace JCW.Object
{
    public class PlayerShadow : ShadowCaster
    {
        PlayerState playerState;

        override protected void Awake()
        {
            base.Awake();

            playerState = transform.parent.GetComponent<PlayerState>();

            // 1.9�� playerState�� �ִ밪 - �ּҰ��� ���Ƿ� �ص� ��.
            scaleOffset = (maxShadowValue - defaultShadowValue) / 1.9f;
        }

        override protected void Update()
        {
            if (!playerState.IsGrounded)
            {
                projector.enabled = true;
                float value = playerState.height * scaleOffset + defaultShadowValue;
                projector.size = new Vector3(value, value, maxShadowDepth);
                projector.fadeFactor = ((value - defaultShadowValue) / maxShadowValue) * shadowTransparent;
            }
            else
                projector.enabled = false;
        }
    }
}