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
        public float attackDelayTime=1;
        Flashlight find;
        Dictionary<GameObject, PlayerController> target = new Dictionary<GameObject, PlayerController>();
        Dictionary<PlayerController, bool> attackDelay = new Dictionary<PlayerController, bool>();
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
            if (objectInfo.photonView.IsMine && !objectInfo.delayEnable)
                return State.Success;
            for (int i = 0; i<find.finderSpot.targetObj.Count; i++)
            {
                PlayerController player;
                if (target.ContainsKey(find.finderSpot.targetObj[i]))
                {
                    player = target[find.finderSpot.targetObj[i]];
                }
                else
                {
                    player = find.finderSpot.targetObj[i].GetComponent<PlayerController>();
                    target.Add(find.finderSpot.targetObj[i], player);
                    attackDelay.Add(player, false);
                }
                if (!attackDelay[player])
                {
                    player.GetDamage(damage, DamageType.Attacked);
                    objectInfo.StartCoroutine(Delay(player, attackDelayTime));
                }
                //player.photonView.RPC(nameof(player.GetDamage), RpcTarget.AllViaServer, damage, DamageType.Attacked);
            }
            return State.Running;
        }

        IEnumerator Delay(PlayerController player, float delayTime)
        {
            attackDelay[player] = true;
            float curCool = 0;
            while (curCool < delayTime)
            {
                curCool += 0.01f;
                yield return objectInfo.wait;
            }
            attackDelay[player] = false;
            yield break;
        }
    }
}
