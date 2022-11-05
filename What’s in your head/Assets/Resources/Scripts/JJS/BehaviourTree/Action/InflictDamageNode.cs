using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS;
using KSU;
namespace JJS.BT
{
    
    public class InflictDamageNode : ActionNode
    {
        public int damage;
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
            for (int i = 0; i<find.finder.targetObj.Count; i++)
            {
                find.finder.targetObj[i].GetComponent<PlayerController>().GetDamage(damage, DamageType.Attacked);
            }
            return State.Success;
        }
    }
}
