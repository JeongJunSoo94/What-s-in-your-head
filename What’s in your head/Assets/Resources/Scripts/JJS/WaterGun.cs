using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Spawner;

namespace JJS
{
    public class WaterGun : MonoBehaviour
    {
        BezierCurve bezierCurveOrbit;

        Camera mainCamera;

        Spawner spawner;

        Ray ray;
        public float shootMaxDistance;
        public float shootCurDistance;

        Vector3 dir;
        private void Awake()
        {
            mainCamera = Camera.main;
            spawner = gameObject.GetComponent<Spawner>();
        }
        // Start is called before the first frame update
        void Start()
        {
            bezierCurveOrbit = gameObject.GetComponent<BezierCurve>();
        }

        // Update is called once per frame
        void Update()
        {
            ShootLine();
        }

        public void Shoot()
        {
            GameObject bullet=spawner.Respawn();
            if (bullet != null)
            {
                BezierCurve bezier = bullet.GetComponent<BezierCurve>();
                bezier.p1 = bezierCurveOrbit.p1;
                bezier.p2 = bezierCurveOrbit.p2;
                bezier.p3 = bezierCurveOrbit.p3;
                bezier.p4 = bezierCurveOrbit.p4;
            }
        }

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

        void ShootLine()
        {
            RaycastHit hit;
            dir = mainCamera.transform.forward;
            //Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            //pos = (pos - transform.position).normalized;
            Debug.Log(transform.position);
            if (Physics.Raycast(transform.position, dir, out hit, shootMaxDistance,-1,QueryTriggerInteraction.Ignore))
            {
                shootCurDistance = Vector3.Distance(transform.position, hit.point);
                bezierCurveOrbit.p1 = transform.position;
                Vector3 direction = (transform.position - hit.point)*0.5f;
                direction.y += 1f;
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
                Vector3 direction = (transform.position - maxPos) * 0.5f;
                direction.y += 1f;
                bezierCurveOrbit.p2 = maxPos + direction;
            }
        }
    }
}
