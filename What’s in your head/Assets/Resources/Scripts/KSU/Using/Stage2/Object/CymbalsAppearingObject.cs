using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Stage2
{
    public class CymbalsAppearingObject : LinkedObjectWithReciever
    {
        public override void RecieveInfo(bool isActive)
        {
            this.gameObject.SetActive(isActivated);
        }
    }
}
