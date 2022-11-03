using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS.BT
{
    public class LightControllerNode : ActionNode
    {
        Flashlight find;
        public bool fake=false;
        public bool enable =false;
        public Color color;
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
            if(fake)
                enable = !enable;
            find.SetLightColor(color);
            find.LightEnable(enable);
            return State.Success;
        }
    }
}
