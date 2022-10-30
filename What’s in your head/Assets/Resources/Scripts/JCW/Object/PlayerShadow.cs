using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace JCW.Object
{
    public class PlayerShadow : MonoBehaviour
    {
        [Header("�⺻ �׸��� ũ�� (�⺻�� 0.5)")] [SerializeField] [Range(0f, 2.5f)] float defaultShadowValue = 0.5f;
        [Header("�ִ� �׸��� ũ�� (�⺻�� 2)")] [SerializeField] [Range(0f, 10f)] float maxShadowValue = 2f;
        [Header("�׸��� �׷��� �Ÿ� (�⺻�� 20)")] [SerializeField] float maxShadowDepth = 20f;
        [Header("�׸��� ���� ����")] [SerializeField] [Range(0f, 1f)] float shadowTransparent = 1f;


        DecalProjector projector;

        PlayerState playerState;

        float scaleOffset;

        private void Awake()
        {
            projector = GetComponent<DecalProjector>();
            projector.size = new Vector3(defaultShadowValue, defaultShadowValue, defaultShadowValue);          

            playerState = transform.parent.GetComponent<PlayerState>();

            // 1.9�� playerState�� �ִ밪 - �ּҰ��� ���Ƿ� �ص� ��.
            scaleOffset = (maxShadowValue - defaultShadowValue) / 1.9f;
        }

        private void OnEnable()
        {
            projector.pivot = new Vector3(0, 0, maxShadowDepth / 2f);
        }

        void Update()
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