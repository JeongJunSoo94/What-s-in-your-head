using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS;
namespace JJS.BT
{
    public class TransformLookatNode : ActionNode
    {
        Flashlight find;
        protected override void OnStart()
        {
            if (find == null)
            {
                find = objectInfo.PrefabObject.GetComponent<Flashlight>();
            }
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (!objectInfo.delayEnable)
                return State.Success;
            if (find.TargetCheck())
            {
                if (find.finder.targetObj.Count == 2)
                {
                    Vector3 pos = (find.finder.targetObj[0].transform.position - find.finder.targetObj[1].transform.position) * 0.5f;
                    objectInfo.PrefabObject.transform.LookAt(find.finder.targetObj[1].transform.position + pos);
                }
                else if(find.finder.targetObj.Count !=0)
                {
                    objectInfo.PrefabObject.transform.LookAt(find.finder.targetObj[0].transform.position);
                }
            }
            return State.Running;
        }
    }
}
