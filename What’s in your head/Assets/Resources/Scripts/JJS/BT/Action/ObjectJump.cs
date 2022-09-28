using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS
{
    public class ObjectJump : Task
    {
        private CharacterControlBT _obj;
        public ObjectJump(CharacterControlBT obj)
        {
            _obj = obj;
        }

        public override NodeState Evaluate()
        {
            Jump();
            return NodeState.SUCCESS;
        }

        void Jump()
        {
            _obj.direction.y = _obj.jumpPower;
        }

    }
}
