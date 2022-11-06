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
            if (find.TargetCheck())
            {
                if (find.finder.targetObj.Count != 0)
                {
                    objectInfo.PrefabObject.transform.LookAt(find.finder.targetObj[0].transform);
                }
                return State.Success;
            }
            else
            {
                return State.Failure;
            }
        }
    }
}
