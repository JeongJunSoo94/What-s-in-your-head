using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS.BT
{
    public class ParallelNode : CompositeNode
    {
        int current;
        bool isFail;
        bool isRun;
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            current = 0;
            isFail = false;
            isRun = false;
            while (current < children.Count)
            {
                var child = children[current];
                switch (child.Update())
                {
                    case State.Running:
                        isRun = true;
                        break;
                    case State.Failure:
                        isFail = true;
                        break;
                    case State.Success:
                        break;
                }
                current++;
            }
            return isFail ? State.Failure : isRun ? State.Running: State.Success;
        }
    }
}
