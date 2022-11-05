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

            // 1.9은 playerState의 최대값 - 최소값을 임의로 해둔 값.
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