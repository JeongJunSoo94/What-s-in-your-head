using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Spawner;
namespace JJS
{
    public class Bullet : MonoBehaviour
    {
        float bulletRange;
        public float speed= 1f;
        public BezierCurve curve;
        public Spawner spawner;

        private void Awake()
        {
            spawner = this.transform.parent.gameObject.GetComponent<Spawner>();
            curve = gameObject.GetComponent<BezierCurve>();
            bulletRange = 0f;
        }
        private void OnEnable()
        {
            bulletRange = 0f;
            curve.range = 0;
        }


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
            Vector3 direction = curve.BezierCurveUpdate(curve.p1, curve.p2, curve.p3, curve.p4, bulletRange+ (speed / Vector3.Distance(curve.p1, curve.p4))* Time.deltaTime);

            transform.LookAt(direction);
            curve.range = bulletRange;
        }
        // << :

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("ContaminationField"))
                other.gameObject.SendMessage("GetDamaged");

            Debug.Log(other.gameObject.name);
            if(!(other.gameObject.layer == LayerMask.NameToLayer("UITriggers")))
            {
                spawner.Despawn(this.gameObject);
            }
        }

    }
}