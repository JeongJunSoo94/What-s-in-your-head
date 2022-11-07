using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS.BT
{
    public class PhotonFailNode : ActionNode
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
            if(local)
                return State.Failure;
            if (objectInfo.photonView.IsMine)
            {
                return State.Success;
            }
            return State.Failure;
        }
    }
}
