using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Spawner;
namespace JJS
{
    public class Bullet : MonoBehaviour
    {
        float bulletRange;
        public BezierCurve curve;
        public Spawner spawner;
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
        }

        void Update()
        {
            if (bulletRange < 1f)
            {
                bulletRange += Time.deltaTime;
            }
            else
            {
                bulletRange = 1f;
                spawner.Despawn(this.gameObject);
            }
            curve.range = bulletRange;
        }
    }
}

