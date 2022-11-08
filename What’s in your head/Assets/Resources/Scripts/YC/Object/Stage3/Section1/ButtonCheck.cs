using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YC_OBJ
{
    public class ButtonCheck : MonoBehaviour
    {
        string interactionTag1 = "Nella";
        string interactionTag2 = "Steady";

        // 토글 모드처럼 눌려졌다 떼졌다 할 것인지

        [Header("토글 모드 (눌렀다 뗐다")]
        [SerializeField] bool isToggle = false;
        bool isSet = false;

        [SerializeField] WoodButton woodButton;
        int count = 0;

       
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(interactionTag1) || other.gameObject.CompareTag(interactionTag2))
            {
                count++;
                woodButton.SendMessage(nameof(woodButton.SetAnimation), count);

                if (!isToggle)
                    isSet = true;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (!isToggle && isSet)
                return;

            if (other.gameObject.CompareTag(interactionTag1) || other.gameObject.CompareTag(interactionTag2))
            {
                count--;
                woodButton.SendMessage(nameof(woodButton.SetAnimation), count);
            }
        }



    }

}
