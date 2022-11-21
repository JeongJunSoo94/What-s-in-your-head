using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace JCW.Object
{
    public class SandSackShadow : ShadowCaster
    {
        [HideInInspector] public Transform groundPlatform;

        float initHeight;
        float groundHeight;

        [HideInInspector] public float interpolationHeight=5f;
        override protected void Awake()
        {
            base.Awake();
            initHeight = transform.position.y;
            groundPlatform = this.transform;
        }

        private void Start()
        {
            groundHeight = groundPlatform.position.y;
            // �ִ� ũ��� �ּ� ũ���� ���̸� ���� ���ָӴ��� ���̷� ���� ��.
            scaleOffset = (maxShadowValue - defaultShadowValue) / (initHeight - groundHeight);
        }

        override protected void Update()
        {
            float value = transform.position.y * scaleOffset + defaultShadowValue;
            projector.size = new Vector3(value, value, maxShadowDepth);

            projector.fadeFactor = ((initHeight + interpolationHeight - transform.position.y) / (initHeight - groundHeight)) * shadowTransparent;
        }        

        public void SetGroundPlatform(Transform tf)
        {
            groundPlatform = tf;
            groundHeight = groundPlatform.position.y;
            scaleOffset = (maxShadowValue - defaultShadowValue) / (initHeight - groundHeight);
        }
    }

}

