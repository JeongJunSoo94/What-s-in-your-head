using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Spawner;

using YC.Camera_;
using YC.Camera_Single;
using Photon.Pun;

namespace JJS.Weapon
{
    public class WaterGun : MonoBehaviour
    {

        Ray ray;

        Vector3 dir;

        [Header("BulletInfo")]
        public float speed = 1;

        [Header("WaterGunInfo")]
        public float shootMaxDistance;
        public float shootCurDistance;
        public float curveHeight=1f;
        public float curveWidth=0.5f;
        public float shootSpeed = 0.5f;
        public int bulletCount = 0;

        [Header("Don't touch")]
        public BezierCurve bezierCurveOrbit;
        public Camera mainCamera;
        public Spawner spawner;
        public GameObject targetIK;
        public GameObject gunDirection;
        public GameObject hitPos;
        public GameObject mousePoint;
        public GameObject weapon;
        public GameObject startPos;
        public GameObject bullet;
        public bool startShoot;
        public float curShootCool;
        GameObject bulletSpawner;
        public int bulletCurCount = 0;
        private void Awake()
        {
            bezierCurveOrbit = gameObject.GetComponent<BezierCurve>();
            bezierCurveOrbit.targetObj = startPos;
            bulletSpawner = new GameObject("BulletSpawner");
            bulletSpawner.AddComponent<Spawner>();
            spawner = bulletSpawner.GetComponent<Spawner>();
            spawner.obj = bullet;
            spawner.count = bulletCount;
            spawner.spawnCount = 0;
        }
        //void FixedUpdate()
        //{
        //}

        public void ShootStart()
        {
            startShoot = true;
            StartCoroutine(ShootCoroutine());
        }

        public void ShootStop()
        {
            startShoot = false;
            StopCoroutine(ShootCoroutine());
        }

        IEnumerator ShootCoroutine()
        {
            while (startShoot)
            {
                if (curShootCool == 0)
                {
                    Shoot();
                    bulletCurCount++;
                    StartCoroutine(ShootCoolTime());
                }
                yield return new WaitForSeconds(shootSpeed -curShootCool);
            }
            yield break;
        }

        IEnumerator ShootCoolTime()
        {
            while (curShootCool< shootSpeed)
            {
                curShootCool+= 0.01f;
                yield return new WaitForSeconds(0.01f);
            }
            curShootCool = 0;
            yield break;
        }

        void Shoot()
        {
            GameObject bullet=spawner.Respawn(startPos.transform.position, Quaternion.LookRotation(bezierCurveOrbit.p2 - startPos.transform.position));
            if (bullet != null)
            {
                Bullet bulletInfo = bullet.GetComponent<Bullet>();
                bulletInfo.speed = speed;
                BezierCurve bezier = bullet.GetComponent<BezierCurve>();

                bezier.p1 = bezierCurveOrbit.p1;
                bezier.p2 = bezierCurveOrbit.p2;
                bezier.p3 = bezierCurveOrbit.p3;
                bezier.p4 = bezierCurveOrbit.p4;
            }
        }

        //void OnDrawGizmos()
        //{
        //    OnDrawGizmosRay();
        //}

        //void OnDrawGizmosRay()
        //{
        //    ray.origin = startPos.transform.position;
        //    ray.direction = dir;

        //    Debug.DrawRay(ray.origin, ray.direction * shootCurDistance, Color.red);

        //    //Debug.DrawRay(Weapon.transform.position, ray.direction * shootCurDistance, Color.green);
        //}

        public void ShootLine(int type=0)
        {
            if (type == 0)
            {
                dir = transform.forward;
                PhisicsShootLine(startPos.transform.position, dir);
            }
            else if (type == 1)
            {
                dir = mainCamera.transform.forward;
                PhisicsShootLine(startPos.transform.position, dir);
            }
            else if (type == 2)
            {
                dir = (mousePoint.transform.position - startPos.transform.position).normalized;
                PhisicsShootLine(startPos.transform.position, dir);
            }

            switch (type)
            {
                case 0:
                    {
                        weapon.transform.LookAt(bezierCurveOrbit.p4);
                        hitPos.transform.position = bezierCurveOrbit.p4;
                        targetIK.transform.LookAt(bezierCurveOrbit.p4);
                    }
                    break;
                case 1:
                    {
                        weapon.transform.LookAt(bezierCurveOrbit.p2);
                        gunDirection.transform.position = bezierCurveOrbit.p2;
                        hitPos.transform.position = bezierCurveOrbit.p4;
                        targetIK.transform.LookAt(bezierCurveOrbit.p2);
                    }
                    break;
                case 2:
                    {
                        weapon.transform.LookAt(bezierCurveOrbit.p2);
                        gunDirection.transform.position = bezierCurveOrbit.p2;
                        hitPos.transform.position = bezierCurveOrbit.p4;
                        targetIK.transform.LookAt(bezierCurveOrbit.p2);
                    }
                    break;
            }
        }

        void PhisicsShootLine(Vector3 startPosition,Vector3 rayDirection)
        {
            RaycastHit hit;
            if (Physics.Raycast(startPosition, rayDirection, out hit, shootMaxDistance, -1, QueryTriggerInteraction.Ignore))
            {
                shootCurDistance = Vector3.Distance(startPosition, hit.point);
                bezierCurveOrbit.p1 = startPosition;

                float Height = hit.point.y - startPosition.y;
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

                Vector3 direction = (startPosition - hit.point) * width;

                direction.y += (1 - width) + Height * curveHeight;
                bezierCurveOrbit.p2 = hit.point + direction;
                bezierCurveOrbit.p3 = hit.point;
                bezierCurveOrbit.p4 = hit.point;
            }
            else
            {
                Vector3 maxPos = startPosition + rayDirection * shootMaxDistance;
                shootCurDistance = Vector3.Distance(startPosition, maxPos);
                bezierCurveOrbit.p1 = startPosition;
                bezierCurveOrbit.p3 = maxPos;
                bezierCurveOrbit.p4 = maxPos;

                float Height = maxPos.y - startPosition.y;
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

                Vector3 direction = (startPosition - maxPos) * width;
                direction.y += 1f + Height * curveHeight;
                bezierCurveOrbit.p2 = maxPos + direction;
            }
        }
    }
}
