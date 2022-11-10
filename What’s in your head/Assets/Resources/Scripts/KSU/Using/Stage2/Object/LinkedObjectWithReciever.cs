using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Stage2
{
    public class LinkedObjectWithReciever : MonoBehaviour
    {
        protected bool isActivated = false;

        public virtual void RecieveInfo(bool isActive)
        {
            isActivated = isActive;
        }
    }
}
