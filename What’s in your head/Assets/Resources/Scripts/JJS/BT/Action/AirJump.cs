using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS
{
    public class AirJump : Task
    {
        private CharacterControlBT _obj;
        public AirJump(CharacterControlBT obj)
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
            _obj.direction.y = _obj.airJumpPower;
        }

    }
}