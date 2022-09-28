using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS
{
    public class VerticalMove : Task
    {
        CharacterControlBT _obj;
        public VerticalMove(CharacterControlBT obj)
        {
            _obj = obj;
        }
        public override NodeState Evaluate()
        {
            if (_obj.onPlatform)
            {
                if (_obj.slopeAngle > -50f)
                    _obj.direction.y = _obj.moveSpeed * Mathf.Tan(_obj.slopeAngle * Mathf.PI / 180);
                else
                    _obj.direction.y = 0;
            }
            //else if (isAirDash)
            //{
            //    direction.y = 0;
            //}
            else
            {
                _obj.direction.y -= _obj.gravity * Time.fixedDeltaTime;
            }
            if (_obj.direction.y < -_obj.terVelocity)
                _obj.direction.y = -_obj.terVelocity;

            return NodeState.SUCCESS;
        }
    }
}