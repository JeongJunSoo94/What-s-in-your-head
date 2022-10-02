using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Task
{
    protected List<Task> nodes = new List<Task>();

    public Selector(List<Task> nodes)
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
                    _nodeState = NodeState.SUCCESS;
                    return _nodeState;
                case NodeState.FAILURE:
                    break;
                default:
                    break;
            }
        }
        _nodeState = NodeState.FAILURE;
        return _nodeState;
    }
}