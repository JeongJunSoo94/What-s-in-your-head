using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS.BT
{
    public class PhotonSuccessNode : ActionNode
    {
        public bool local;
        protected override void OnStart()
        {

        }


        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (local)
                return State.Success;
            if (objectInfo.photonView.IsMine)
            {
                return State.Success;
            }
            return State.Failure;
        }
    }
}
