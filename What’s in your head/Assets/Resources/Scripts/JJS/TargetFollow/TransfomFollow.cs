using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS
{
    public class TransfomFollow : MonoBehaviour
    {
        public GameObject target;

        void Update()
        {
            targetPositionFollow();
        }

        void targetPositionFollow()
        {
            transform.position = target.transform.position;
        }
    }
}

