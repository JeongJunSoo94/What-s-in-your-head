using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Task
{
    protected List<Task> nodes = new List<Task>();

    public Sequence(List<Task> nodes)
    {
        this.nodes = nodes;
    }
    public override NodeState Evaluate()
    {
        foreach (var node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    _nodeState = NodeState.RUNNING;
                    return _nodeState;
                case NodeState.SUCCESS:
                    break;
                case NodeState.FAILURE:
                    _nodeState = NodeState.FAILURE;
                    return _nodeState;
                default:
                    break;
            }
        }
        _nodeState = NodeState.SUCCESS;
        return _nodeState;
    }
}
