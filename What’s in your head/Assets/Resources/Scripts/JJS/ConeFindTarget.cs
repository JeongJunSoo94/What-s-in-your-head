using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS
{
    public class ConeFindTarget : MonoBehaviour
    {
        [SerializeField]
        [Range(0f, 50f)]
        private float _discoveryRadius;

        [Range(0f, 90f)]
        [SerializeField] private float _viewAngle;

        //[Range(-180f, 180f)]
        //[SerializeField] private float _viewDirection;

        [SerializeField] private LayerMask viewTargetMask;
        [SerializeField] private LayerMask viewObstacleMask;

        private Collider[] hitColliders;
        private RaycastHit[] rayHits;
        private Ray[] ray = new Ray[5];

        public Collider[] HitColliders { get { return hitColliders; } }

        public List<GameObject> targetObj;

        enum type
        {
            x,
            y,
            z
        }
        private Vector3 AngleToDirection(Transform transform, float angle, type type)
        {
            Vector3 newDirection = Vector3.zero;
            switch (type)
            {
                case type.x:
                    {
                        newDirection = (transform.forward + transform.right * Mathf.Tan(angle * Mathf.Deg2Rad)).normalized;
                    }
                    break;
                case type.y:
                    {
                        newDirection = (transform.forward + transform.up * Mathf.Tan(angle * Mathf.Deg2Rad)).normalized;
                    }
                    break;
                case type.z:
                    {
                        newDirection = (transform.forward + transform.forward * Mathf.Tan(angle * Mathf.Deg2Rad)).normalized;
                    }
                    break;
            }
            return newDirection;
        }
        void Update()
        {
            //DiscoveryTargetBool();
        }
        void OnDrawGizmos()
        {
            OnDrawGizmosRay();
        }

        void OnDrawGizmosRay()
        {

            Gizmos.DrawWireSphere(transform.position, _discoveryRadius);

            Vector3 originPos = transform.position;
            Vector3 lookDir = transform.forward;
            Vector3 rightDir = AngleToDirection(transform, _viewAngle, type.x);
            Vector3 leftDir = AngleToDirection(transform, -_viewAngle, type.x);
            Vector3 upDir = AngleToDirection(transform, _viewAngle, type.y);
            Vector3 downDir = AngleToDirection(transform, -_viewAngle, type.y);

            ray[0].origin = transform.position;
            ray[0].direction = rightDir;
            ray[1].origin = transform.position;
            ray[1].direction = leftDir;
            ray[2].origin = transform.position;
            ray[2].direction = upDir;
            ray[3].origin = transform.position;
            ray[3].direction = downDir;
            ray[4].origin = transform.position;
            ray[4].direction = lookDir;


            for (int i = 0; i < 2; i++)
            {
                Debug.DrawLine(ray[i].origin, ray[i].origin + ray[i].direction * _discoveryRadius, Color.red);
            }
            for (int i = 2; i < 4; i++)
            {
                Debug.DrawLine(ray[i].origin, ray[i].origin + ray[i].direction * _discoveryRadius, Color.blue);
            }

            Debug.DrawRay(ray[4].origin, ray[4].direction * _discoveryRadius, Color.green);
        }
        public bool DiscoveryTargetBool()
        {
            Vector3 originPos = transform.position;
            int count = 0;
            hitColliders = Physics.OverlapSphere(transform.position, _discoveryRadius, viewTargetMask);
            if (hitColliders.Length != 0)
            {
                targetObj.Clear();
            }
            for (int index = 0; index < hitColliders.Length; index++)
            {
                if (hitColliders[index] != null)
                {
                    Vector3 targetPos = hitColliders[index].transform.position;
                    Vector3 dir = (targetPos - originPos).normalized;//(타겟의 위치값 - 오브젝트의 위치값).백터값
                    Vector3 lookDir = transform.forward;

                    // float angle = Vector3.Angle(lookDir, dir)
                    // 아래 두 줄은 위의 코드와 동일하게 동작함. 내부 구현도 동일
                    float dot = Vector3.Dot(lookDir, dir);
                    float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

                    if (angle <= _viewAngle)
                    {
                        if (Physics.Raycast(originPos, dir, _discoveryRadius, viewObstacleMask, QueryTriggerInteraction.Ignore))
                        {
                            continue;
                        }
                        count++;
                        targetObj.Add(hitColliders[index].gameObject);
                        Debug.DrawLine(originPos, targetPos, Color.red);
                    }
                }
            }

            if (count == 0)
            {
                return false;
            }
            return true;
        }
    }

}
