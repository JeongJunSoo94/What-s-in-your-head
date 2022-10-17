using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS.Weapon
{
    public class BulletOrbit : MonoBehaviour
    {
        public BezierCurve bezierCurveOrbit;

        public Camera mainCamera;

        Ray ray;

        Vector3 dir;
        public float shootMaxDistance;
        public float shootCurDistance;
        public float curveHeight = 1f;
        public float curveWidth;

        public GameObject IK;
        public GameObject Weapon;
        void OnDrawGizmos()
        {
            OnDrawGizmosRay();
        }

        void OnDrawGizmosRay()
        {
            ray.origin = transform.position;
            ray.direction = dir;

            Debug.DrawRay(ray.origin, ray.direction * shootCurDistance, Color.red);
        }
        void FixedUpdate()
        {
            ShootLine();
        }

        void ShootLine()
        {
            RaycastHit hit;
            dir = mainCamera.transform.forward;

            if (Physics.Raycast(transform.position, dir, out hit, shootMaxDistance, -1, QueryTriggerInteraction.Ignore))
            {
                shootCurDistance = Vector3.Distance(transform.position, hit.point);
                bezierCurveOrbit.p1 = transform.position;

                float Height = hit.point.y - transform.position.y;
                Height /= shootMaxDistance;
                float width = curveWidth;
                if (Height > 0)
                {
                    width -= Height * 0.25f;
                }
                else
                {
                    width -= Height * 0.25f;
                    Height *= -1f;
                }

                Vector3 direction = (transform.position - hit.point) * width;

                direction.y += (1 - width) + Height * curveHeight;
                bezierCurveOrbit.p2 = hit.point + direction;
                bezierCurveOrbit.p3 = hit.point;
                bezierCurveOrbit.p4 = hit.point;
            }
            else
            {
                Vector3 maxPos = transform.position + dir * shootMaxDistance;
                shootCurDistance = Vector3.Distance(transform.position, maxPos);
                bezierCurveOrbit.p1 = transform.position;
                bezierCurveOrbit.p3 = maxPos;
                bezierCurveOrbit.p4 = maxPos;

                float Height = maxPos.y - transform.position.y;
                Height /= shootMaxDistance;
                float width = curveWidth;
                if (Height > 0)
                {
                    width -= Height * 0.25f;
                }
                else
                {
                    width -= Height * 0.25f;
                    Height *= -1f;
                }

                Vector3 direction = (transform.position - maxPos) * width;
                direction.y += 1f + Height * curveHeight;
                bezierCurveOrbit.p2 = maxPos + direction;
            }
            IK.transform.position = bezierCurveOrbit.p4;

            Weapon.transform.LookAt(bezierCurveOrbit.p4);
        }
    }

}
