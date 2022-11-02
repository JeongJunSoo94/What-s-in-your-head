using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS.BT
{
    public class FailNode : ActionNode
    {
        public string message;
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {

            return State.Failure;
        }
    }
}
