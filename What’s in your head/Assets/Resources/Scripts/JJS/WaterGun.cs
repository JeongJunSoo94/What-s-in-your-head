using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Spawner;
using JCW.UI.Options.InputBindings;
using YC.Camera_;
using YC.Camera_Single;
using JCW.AudioCtrl;

namespace JJS.Weapon
{
    [RequireComponent(typeof(AudioSource))]
    public class WaterGun : MonoBehaviour
    {

        Ray ray;
        Vector3 dir;

        [Header("BulletInfo")]
        public float speed = 1;

        [Header("WaterGunInfo")]
        public float shootMaxDistance;
        public float shootMinDistrace;
        public float shootCurDistance;
        public float curveHeight = 1f;
        public float curveWidth = 0.5f;
        public float shootSpeed = 0.5f;
        public int bulletCount = 0;
        public LayerMask layerMask;

        [Header("Don't Touch")]
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
        public bool shootEnable;
        public float curShootCool;
        GameObject bulletSpawner;
        public int bulletCurCount = 0;
        public Rigidbody rigid;
        AudioSource audioSource;

        public Spawner effectSpawner;
        public GameObject effect;
        public GameObject effectSpawnerObj;
        public int effectCurCount = 0;

        public Spawner effectCircleSpawner;
        public GameObject effectCircle;
        public GameObject effectCircleSpawnerObj;
        public int effectCircleCurCount = 0;

        private void Awake()
        {
            bezierCurveOrbit = gameObject.GetComponent<BezierCurve>();
            bezierCurveOrbit.targetObj = startPos;
            InitSpawner();
            shootEnable = true;
            audioSource = GetComponent<AudioSource>();
            JCW.AudioCtrl.AudioSettings.SetAudio(audioSource, 1f, 40f);
            //rigid = transform.parent.gameObject.GetComponent<Rigidbody>();
        }

        public void InitSpawner()
        {
            if (effectSpawnerObj == null)
            {
                effectSpawnerObj = new GameObject("EffectSpawner");
                effectSpawnerObj.AddComponent<Spawner>();
            }
            effectSpawner = effectSpawnerObj.GetComponent<Spawner>();
            effect.GetComponent<EffectController>().effectSpawner = effectSpawner;
            effectSpawner.obj = effect;
            effectSpawner.count = effectCurCount;
            effectSpawner.spawnCount = 0;

            if (effectCircleSpawnerObj == null)
            {
                effectCircleSpawnerObj = new GameObject("EffectCircleSpawner");
                effectCircleSpawnerObj.AddComponent<Spawner>();
            }
            effectCircleSpawner = effectCircleSpawnerObj.GetComponent<Spawner>();
            effectCircle.GetComponent<EffectController>().effectSpawner = effectCircleSpawner;
            effectCircleSpawner.obj = effectCircle;
            effectCircleSpawner.count = effectCircleCurCount;
            effectCircleSpawner.spawnCount = 0;

            if (bulletSpawner == null)
            { 
                bulletSpawner = new GameObject("BulletSpawner");
                bulletSpawner.AddComponent<Spawner>();
            }
            spawner = bulletSpawner.GetComponent<Spawner>();
            bullet.GetComponent<Bullet>().effectWaterSpawner = effectSpawner;
            bullet.GetComponent<Bullet>().effectWaterCircleSpawner = effectCircleSpawner;
            spawner.obj = bullet;
            spawner.count = bulletCount;
            spawner.spawnCount = 0;
        }

        public void ShootStart()
        {
            SoundManager.Instance.Play3D_RPC("S3_Watergun", audioSource);
            Shoot();
            bulletCurCount++;
        }

        public void ShootCoroutineEnable()
        {
            if (shootEnable)
                StartCoroutine(ShootCoolTime());
        }

        IEnumerator ShootCoolTime()
        {
            shootEnable = false;
            while (curShootCool < shootSpeed)
            {
                curShootCool += 0.01f;
                yield return new WaitForSeconds(0.01f);
            }
            curShootCool = 0;
            shootEnable = true;
            yield break;
        }

        void Shoot()
        {
            GameObject bullet = spawner.Respawn(CorrectionPos(startPos.transform.position), Quaternion.LookRotation(bezierCurveOrbit.p2 - CorrectionPos(startPos.transform.position)));
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

        Vector3 CorrectionPos(Vector3 changePos)
        {
            //if (KeyManager.Instance.GetKey(PlayerAction.MoveForward) && !KeyManager.Instance.GetKey(PlayerAction.MoveBackward)
            //    &&!KeyManager.Instance.GetKey(PlayerAction.MoveLeft)&& !KeyManager.Instance.GetKey(PlayerAction.MoveRight))
            //{
            //    changePos += rigid.velocity * Time.deltaTime * 15;
            //}
            return changePos;
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
                PhisicsShootLine(CorrectionPos(startPos.transform.position), dir);
            }
            else if (type == 1)
            {
                RaycastHit hit;
                if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, shootMaxDistance, layerMask, QueryTriggerInteraction.Ignore))
                {
                    if (hit.distance > Vector3.Distance(CorrectionPos(startPos.transform.position), mainCamera.transform.position))
                    {
                        dir = (hit.point - CorrectionPos(startPos.transform.position)).normalized;
                        PhisicsShootLine(CorrectionPos(startPos.transform.position), dir);
                    }
                    else
                    {
                        MaxPhysicsLine(CorrectionPos(startPos.transform.position), mainCamera.transform.forward);
                    }
                }
                else
                {
                    MaxPhysicsLine(CorrectionPos(startPos.transform.position), mainCamera.transform.forward);
                }
            }
            else if (type == 2)
            {
                //if (shootMinDistrace < Vector3.Distance(mousePoint.transform.position, startPos.transform.position))
                //{
                    dir = (mousePoint.transform.position - CorrectionPos(startPos.transform.position)).normalized;
                    PhisicsShootLine(CorrectionPos(startPos.transform.position), dir);
                //}
                //else
                //{
                //    Vector3 direction = transform.forward;
                //    direction.y += (mousePoint.transform.position.y - startPos.transform.position.y);
                //    dir = direction.normalized;
                //    PhisicsShootLine(startPos.transform.position, dir);
                //}
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
            if (!CurPhysicsLine(startPosition, rayDirection))
            {
                MaxPhysicsLine(startPosition, rayDirection);
            }
        }

        bool CurPhysicsLine(Vector3 startPosition, Vector3 rayDirection)
        {
            RaycastHit hit;
            if (Physics.Raycast(startPosition, rayDirection, out hit, shootMaxDistance, -1, QueryTriggerInteraction.Ignore))
            {
                shootCurDistance = Vector3.Distance(startPosition, hit.point);

                bezierCurveOrbit.p1 = startPosition;

                float Height = hit.point.y - startPosition.y;//위쪽값 양수 아래값 음수
                shootCurDistance = Vector3.Distance(startPosition, hit.point);
                Height /= shootCurDistance;
                float width = curveWidth;
                Vector3 direction;
                if (Height > 0)
                {
                    width -= Height * 0.25f;
                    direction = (startPosition - hit.point) * width;
                    direction.y += (curveHeight + Height * curveHeight);
                }
                else
                {
                    width -= Height * 0.25f;
                    direction = (startPosition - hit.point) * width;
                    direction.y += (curveHeight + Height * 2 * curveHeight);
                }

                bezierCurveOrbit.p2 = hit.point + direction;
                bezierCurveOrbit.p3 = hit.point;
                bezierCurveOrbit.p4 = hit.point;
                return true;
            }
            return false;
        }

        void MaxPhysicsLine(Vector3 startPosition, Vector3 rayDirection)
        {
            Vector3 maxPos = mainCamera.transform.position + rayDirection * shootMaxDistance;
            shootCurDistance = Vector3.Distance(startPosition, maxPos);
            bezierCurveOrbit.p1 = startPosition;
            bezierCurveOrbit.p3 = maxPos;
            bezierCurveOrbit.p4 = maxPos;

            float Height = maxPos.y - startPosition.y;
            Height /= shootCurDistance;
            float width = curveWidth;
            if (Height > 0)
            {
                width -= Height * 0.25f;
            }
            else
            {
                width -= Height * 0.25f;
            }

            Vector3 direction = (startPosition - maxPos) * width;
            direction.y += (curveHeight*3 + Height * curveHeight*3);
            bezierCurveOrbit.p2 = maxPos + direction;
        }
    }
}
