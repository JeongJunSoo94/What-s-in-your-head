using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YC_MODULE
{
    public class Discovery : MonoBehaviour
    {
        Transform objTransform;

        [Header("<기획 편집 사항>")]
        [Space]
        [SerializeField] float ViewRadius;

        public bool isSetTarget { get; private set; }

        public GameObject targetObj { get; private set; }

        LayerMask layerMask;
        
        

        void Awake()
        {
            isSetTarget = false;
            objTransform = this.gameObject.transform;
        }


        void Update()
        {
            DetectTarget();
        }

        void DetectTarget()
        {          
            Collider[] Targets = Physics.OverlapSphere(objTransform.position, ViewRadius);

            if (Targets.Length == 0) return;

            foreach (Collider col in Targets)
            {
                if (col.gameObject.CompareTag("Nella") || col.gameObject.CompareTag("Steady"))
                {
                    targetObj = col.gameObject;
                    isSetTarget = true;
                    Debug.Log("Target!");
                    break;
                }
                else
                {
                    targetObj = null;
                    isSetTarget = false;
                    Debug.Log("NULL!");
                }
            }
        }

        // 오브젝트의 반경 값을 그려준다
        void OnDrawGizmos()
        {
            if (!objTransform) return;

            Gizmos.color = new Color32(255, 255, 255, 100);
            Gizmos.DrawWireSphere(objTransform.position, ViewRadius);

            Gizmos.color = new Color32(255, 0, 255, 100);
            Gizmos.DrawWireSphere(objTransform.position, ViewRadius);

        }
    }

}
