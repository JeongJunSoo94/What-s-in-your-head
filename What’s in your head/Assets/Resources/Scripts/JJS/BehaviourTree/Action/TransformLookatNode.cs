using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS;
namespace JJS.BT
{
    public class TransformLookatNode : ActionNode
    {
        Flashlight find;
        bool waitCheck = true;
        protected override void OnStart()
        {
            if (find == null)
            {
                find = objectInfo.PrefabObject.GetComponent<Flashlight>();
            }
            waitCheck = false;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (!objectInfo.delayCheck)
            {
                waitCheck = true;
            }
            if(waitCheck&& !objectInfo.delayEnable)
                return State.Success;
            if (find.TargetCheck())
            {
                if (find.finder.targetObj.Count != 0)
                {
                    objectInfo.PrefabObject.transform.LookAt(find.finder.targetObj[0].transform);
                }
            }
            return State.Running;
        }
    }
}
