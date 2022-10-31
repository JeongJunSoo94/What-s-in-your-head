using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YC_OBJ
{
    public class ButtonCheck : MonoBehaviour
    {
        string interactionTag1 = "Nella";
        string interactionTag2 = "Steady";

        WoodButton Parent;
        int count = 0;

        void Start()
        {
            Parent = transform.root.gameObject.GetComponent<WoodButton>();
        }
   
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(interactionTag1) || other.gameObject.CompareTag(interactionTag2))
            {
                count++;
                Parent.SendMessage(nameof(Parent.SetAnimation), count);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(interactionTag1) || other.gameObject.CompareTag(interactionTag2))
            {
                count--;
                Parent.SendMessage(nameof(Parent.SetAnimation), count);
            }
        }



    }

}
