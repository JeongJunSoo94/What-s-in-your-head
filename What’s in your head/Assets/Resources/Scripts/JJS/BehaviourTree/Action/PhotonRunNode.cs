using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS.BT
{
    public class PhotonRunNode : ActionNode
    {
        int syncCheck;
        protected override void OnStart()
        {
            syncCheck = objectInfo.syncIndex;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (objectInfo.photonView.IsMine)
            {
                return State.Success;
            }
            if (objectInfo.syncIndex == syncCheck)
            {
                return State.Running;
            } 
            return State.Failure;
        }
    }
}
