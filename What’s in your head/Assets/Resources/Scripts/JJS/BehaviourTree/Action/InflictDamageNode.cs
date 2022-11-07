using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS;
using KSU;
using Photon.Pun;
namespace JJS.BT
{
    
    public class InflictDamageNode : ActionNode
    {
        public int damage;
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
            if (waitCheck && !objectInfo.delayEnable)
                return State.Success;
            for (int i = 0; i<find.finderSpot.targetObj.Count; i++)
            {
                PlayerController player = find.finderSpot.targetObj[i].GetComponent<PlayerController>();
                
                player.GetDamage(damage, DamageType.Attacked);
                //player.photonView.RPC(nameof(player.GetDamage), RpcTarget.AllViaServer, damage, DamageType.Attacked);
            }
            return State.Running;
        }
    }
}
