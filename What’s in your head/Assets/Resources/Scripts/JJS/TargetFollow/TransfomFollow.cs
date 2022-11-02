using KSU.Monster;
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
            if(target!=null)
                transform.position = target.transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "PoisonSnake":
                case "TrippleHeadSnake":
                    {
                        other.gameObject.GetComponent<DefenseMonster>().GetDamage(30);
                    }
                    break;
            }
        }
    }
}

