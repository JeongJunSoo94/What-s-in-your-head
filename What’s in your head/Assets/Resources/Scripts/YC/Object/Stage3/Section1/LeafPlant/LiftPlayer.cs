using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YC_OBJ
{
    public class LiftPlayer : MonoBehaviour
    {

        string interactionTag1 = "Nella";
        string interactionTag2 = "Steady";


        public GameObject player;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(interactionTag1) ||
                collision.gameObject.CompareTag(interactionTag2))
            {
                collision.gameObject.transform.parent = this.gameObject.transform;
               
                player = collision.gameObject;
            }
        }
     

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag(interactionTag1) ||
                collision.gameObject.CompareTag(interactionTag2))
            {
                collision.gameObject.transform.parent = null;

                player = null;
            }
        }
    }
}
