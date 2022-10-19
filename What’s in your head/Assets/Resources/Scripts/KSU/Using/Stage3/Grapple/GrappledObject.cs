using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class GrappledObject : MonoBehaviour
    {
        [SerializeField] GameObject detectingTrriger;
        public GameObject offset;

        public float detectingRange;

        // Start is called before the first frame update
        void Start()
        {
            detectingTrriger.transform.localScale = new Vector3(1, 1, 1) * (detectingRange * 2f);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public Vector3 GetOffsetPosition()
        {
            return offset.transform.position;
        }
    }
}
