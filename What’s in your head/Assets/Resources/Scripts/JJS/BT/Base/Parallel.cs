using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallel : Task
{
    protected List<Task> nodes = new List<Task>();

    public Parallel(List<Task> nodes)
    {
        this.nodes = nodes;
    }
    public override NodeState Evaluate()
    {
        bool isAnyNodeRunning = false;
        bool isAnyNodeFailure = false;
        foreach (var node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    isAnyNodeRunning = true;
                    break;
                case NodeState.SUCCESS:
                    break;
                case NodeState.FAILURE:
                    isAnyNodeFailure = true;
                    break;
                default:
                    break;
            }
        }
        _nodeState = isAnyNodeFailure ? NodeState.FAILURE : isAnyNodeRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return _nodeState;
    }
}
