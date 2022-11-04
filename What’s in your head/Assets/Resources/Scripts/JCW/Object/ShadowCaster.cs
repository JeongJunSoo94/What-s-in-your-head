using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace JCW.Object
{
    [RequireComponent(typeof(DecalProjector))]
    abstract public class ShadowCaster : MonoBehaviour
    {
        [Header("기본 그림자 크기 (기본값 0.5)")] [SerializeField] [Range(0f, 25f)] protected float defaultShadowValue = 0.5f;
        [Header("최대 그림자 크기 (기본값 2)")] [SerializeField] [Range(0f, 100f)] protected float maxShadowValue = 2f;
        [Header("그림자 그려줄 거리 (기본값 20)")] [SerializeField] protected float maxShadowDepth = 20f;
        [Header("그림자 투명도 배율")] [SerializeField] [Range(0f, 1f)] protected float shadowTransparent = 1f;

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
