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
            objectInfo.DelayCoroutin(true, duration);
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
