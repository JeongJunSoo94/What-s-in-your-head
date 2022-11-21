using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.AutoAim.Object
{
    public class AutoAimTargetObject : MonoBehaviour
    {
        [SerializeField] public float detectingUIRange;
        [SerializeField] protected GameObject detectingUITrigger;
        protected virtual void Awake()
        {
            detectingUITrigger.transform.localScale = Vector3.one * detectingUIRange * 2f;
        }
    }
}