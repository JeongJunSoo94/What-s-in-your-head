using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS.BT
{
    public class FailNode : ActionNode
    {

        protected override void OnStart()
        {
           
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
            return State.Failure;
        }
    }
}
