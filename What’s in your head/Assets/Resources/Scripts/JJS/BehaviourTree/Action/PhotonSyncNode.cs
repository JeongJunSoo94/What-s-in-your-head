using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS;
using Photon.Pun;

namespace JJS.BT
{
    public class PhotonSyncNode : ActionNode
    {
        public bool local;
        public int syncChange;
        protected override void OnStart()
        {

            if (objectInfo.photonView.IsMine)
            {
                if (local)
                {
                    objectInfo.photonView.RPC(nameof(objectInfo.SyncCheck), RpcTarget.AllViaServer, syncChange);
                    objectInfo.localSync = false;
                }
            }
        }
   
        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (objectInfo.localSync)
            { 
                if (objectInfo.syncIndex== syncChange)
                {
                    return State.Success;
                }
            }
            if (local)
            {
                return State.Success;
            }
            return State.Failure;
        }

    }
}
