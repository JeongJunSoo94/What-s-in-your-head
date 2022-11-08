using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Interaction
{
    public class InteractableObject : MonoBehaviour
    {
        [SerializeField] protected GameObject detectingTrigger;
        public float detectingRange = 20f;
        public float interactableRange = 5f;

        protected bool isInteractable = true;

        // Start is called before the first frame update
        virtual protected void Awake()
        {
            SetDetectingTrigger();
        }

        virtual protected void SetDetectingTrigger()
        {
            detectingTrigger.transform.localScale = Vector3.one * detectingRange * 2f;
        }

        public virtual void StartInteraction()
        {
            if (isInteractable)
            {
                isInteractable = false;
                // 상호작용 시작
            }
        }
    }
}
