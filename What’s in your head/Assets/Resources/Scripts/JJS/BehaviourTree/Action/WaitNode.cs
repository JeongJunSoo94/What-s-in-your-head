using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS.BT
{
    public class WaitNode : ActionNode
    {
        public float duration = 1;
        protected override void OnStart()
        {
            if (objectInfo.delayCheck)
            { 
                if (objectInfo.photonView.IsMine)
                { 
                    if (!objectInfo.delayEnable)
                    {
                        objectInfo.DelayCoroutin(true, duration);
                        objectInfo.delayCheck = false;
                    }
                }
            }
        }

        protected override void OnStop()
        {
            objectInfo.DelayCoroutin(false, duration);
        }

        protected override State OnUpdate()
        {
            if (!objectInfo.delayEnable)
            {
                return State.Success;
            }
            return State.Running;
        }
    }
}
