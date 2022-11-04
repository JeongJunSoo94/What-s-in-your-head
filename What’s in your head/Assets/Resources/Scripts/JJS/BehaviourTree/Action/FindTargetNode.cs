using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS;
namespace JJS.BT
{
    public class FindTargetNode : ActionNode
    {
        Flashlight find;
        protected override void OnStart()
        {
            if (find==null)
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
                return State.Success;
            }
            return State.Failure;
        }
    }
}
