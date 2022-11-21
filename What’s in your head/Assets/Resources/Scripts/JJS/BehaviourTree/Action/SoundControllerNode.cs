using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS.BT
{
    public class SoundControllerNode : ActionNode
    {
        public string soundName;
        public bool playSound;
        Flashlight my;
        protected override void OnStart()
        {
            if (my == null)
            {
                my = objectInfo.PrefabObject.GetComponent<Flashlight>();
                if (playSound)
                {
                    my.PlaySound(soundName);
                }
                else
                {
                    my.StopSound();
                }

            }
            else
            {
                if (playSound)
                {
                    my.PlaySound(soundName);
                }
                else
                {
                    my.StopSound();
                }
            }
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            return State.Success;
        }
    }
}