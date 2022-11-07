using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS.BT
{
    public class SelectorNode : CompositeNode
    {
        int current;
        protected override void OnStart()
        {
            current = 0;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            for (int i = current; i < children.Count; ++i)
            {
                current = i; 
                var child = children[current];
                switch (child.Update())
                {
                    case State.Running:
                        return State.Running;
                    case State.Failure:
                        continue;
                    case State.Success:
                        return State.Success;
                }
            }
            return State.Failure;
        }
    }
}
