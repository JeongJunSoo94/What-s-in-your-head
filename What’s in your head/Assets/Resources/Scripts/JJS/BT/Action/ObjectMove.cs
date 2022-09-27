using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMove : Task
{
    private CharacterControlBT _obj;
    private Vector3 direction;
    public ObjectMove(CharacterControlBT obj)
    {
        _obj = obj;
    }

    public override NodeState Evaluate()
    {
        Move();
        return NodeState.SUCCESS;
    }
    
    void Move()
    {
        float moveSpeed = _obj.walkSpeed;

        _obj.pRigidbody.velocity = _obj._moveDir;
    }

}
