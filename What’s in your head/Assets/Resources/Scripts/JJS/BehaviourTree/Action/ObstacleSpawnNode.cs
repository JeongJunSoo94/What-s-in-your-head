using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS;
namespace JJS.BT
{
    public class ObstacleSpawnNode : ActionNode
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
            find.SpotTargetCheck();
        }

        protected override State OnUpdate()
        {
            if (objectInfo.photonView.IsMine && !objectInfo.delayEnable)
                return State.Success;
            if (find.SpotTargetCheck())
            {
                //return State.Success;
            }
            return State.Running;
        }
    }
}
