using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Spawner;
using JCW.Object;

namespace JJS
{
    public class Bullet : MonoBehaviour
    {
        float bulletRange;
        public float speed= 1f;
        public BezierCurve curve;
        public Spawner spawner;
        public TrailRenderer trail;
        public Spawner effectWaterSpawner;
        public Spawner effectWaterCircleSpawner;

        private void Awake()
        {
            spawner = this.transform.parent.gameObject.GetComponent<Spawner>();
            curve = gameObject.GetComponent<BezierCurve>();
            bulletRange = 0f;
            trail = GetComponentInChildren<TrailRenderer>();
        }
        private void OnEnable()
        {
            trail.Clear();
            bulletRange = 0f;
            curve.range = 0;
        }

        //private void OnDisable()
        //{
        //    trail.time = 0f;
        //    //trail.enabled = false;
        //}
        // >> : 찬, 총알 발사 관련 수정 
        void Update()
        {
            if (bulletRange < 1f)
            {
                bulletRange += speed / Vector3.Distance(curve.p1, curve.p4) * Time.deltaTime;
            }
            else
            {
                bulletRange = 1f;
                spawner.Despawn(this.gameObject);
            }
            Vector3 direction = curve.BezierCurveUpdate(curve.p1, curve.p2, curve.p3, curve.p4, bulletRange + (speed / Vector3.Distance(curve.p1, curve.p4)) * Time.deltaTime);

            transform.LookAt(direction);
            curve.range = bulletRange;
        }
        // << :
        private void OnTriggerEnter(Collider other)
        {
            //if (other.gameObject.CompareTag("ContaminationField"))
            //    other.gameObject.GetComponent<HostField>().GetDamaged();

            //Debug.Log(other.gameObject.name);
            if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                effectWaterCircleSpawner.Respawn(transform.position, Quaternion.LookRotation(other.transform.up));
                spawner.Despawn(this.gameObject);
                return;
            }

            if (!(other.gameObject.layer == LayerMask.NameToLayer("UITriggers")))
            {
                effectWaterSpawner.Respawn(transform.position,Quaternion.LookRotation(-transform.forward));
                spawner.Despawn(this.gameObject);
            }
        }

    }
}