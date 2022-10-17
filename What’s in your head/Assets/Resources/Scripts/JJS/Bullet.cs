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

        public Vector3 vecDistance;
        private void Awake()
        {
            spawner = this.transform.parent.gameObject.GetComponent<Spawner>();
            curve = gameObject.GetComponent<BezierCurve>();
        }
        void Start()
        {
            bulletRange = 0f;
        }
        private void OnEnable()
        {
            bulletRange = 0f;
            vecDistance = curve.p1 - curve.p4;
        }

        // >> : 찬, 총알 발사 관련 수정 
        void Update()
        {
            if (bulletRange < 1f)
            {
                bulletRange += speed * Time.deltaTime;
            }
            else
            {
                bulletRange = 1f;
                spawner.Despawn(this.gameObject);
            }
            curve.range = bulletRange;
        }
        // << :

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("ContaminationField"))
            {
                Debug.Log("나가라");
                other.gameObject.SendMessage("GetDamaged");
            }

            Debug.Log(other.gameObject.name);
            spawner.Despawn(this.gameObject);
        }

    }
}