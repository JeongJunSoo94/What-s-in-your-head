using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace JCW.Object
{
    [RequireComponent(typeof(DecalProjector))]
    abstract public class ShadowCaster : MonoBehaviour
    {
        [Header("�⺻ �׸��� ũ�� (�⺻�� 0.5)")] [SerializeField] [Range(0f, 25f)] protected float defaultShadowValue = 0.5f;
        [Header("�ִ� �׸��� ũ�� (�⺻�� 2)")] [SerializeField] [Range(0f, 100f)] protected float maxShadowValue = 2f;
        [Header("�׸��� �׷��� �Ÿ� (�⺻�� 20)")] [SerializeField] protected float maxShadowDepth = 20f;
        [Header("�׸��� ���� ����")] [SerializeField] [Range(0f, 1f)] protected float shadowTransparent = 1f;

        protected DecalProjector projector;      
        protected float scaleOffset;

        virtual protected void Awake()
        {
            projector = GetComponent<DecalProjector>();
            projector.size = new Vector3(defaultShadowValue, defaultShadowValue, defaultShadowValue);
        }

        private void OnEnable()
        {
            projector.pivot = new Vector3(0, 0, maxShadowDepth / 2f);
        }

        abstract protected void Update();
    }
}
