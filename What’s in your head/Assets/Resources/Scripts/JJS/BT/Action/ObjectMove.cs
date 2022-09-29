using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS
{
    public class ObjectMove : Task
    {
        CharacterControlBT _obj;
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
            _obj.pRigidbody.velocity = _obj.direction + _obj.dashVec;
        }

    }
}

