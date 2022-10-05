using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Options.InputBindings;


namespace JJS
{
    public class InputHorizonMove : Task
    {
        CharacterControlBT _obj;
        public InputHorizonMove(CharacterControlBT obj)
        {
            _obj = obj;
        }
        public override NodeState Evaluate()
        {
            if (ITT_KeyManager.Instance.GetKeyDown(PlayerAction.MoveForward) || ITT_KeyManager.Instance.GetKeyDown(PlayerAction.MoveBackward)
               || ITT_KeyManager.Instance.GetKeyDown(PlayerAction.MoveRight) || ITT_KeyManager.Instance.GetKeyDown(PlayerAction.MoveLeft))
            {
                _obj.direction.z = ((ITT_KeyManager.Instance.GetKeyDown(PlayerAction.MoveForward) ? 1 : 0) + (ITT_KeyManager.Instance.GetKeyDown(PlayerAction.MoveBackward) ? -1 : 0));
                _obj.direction.x = ((ITT_KeyManager.Instance.GetKeyDown(PlayerAction.MoveRight) ? 1 : 0) + (ITT_KeyManager.Instance.GetKeyDown(PlayerAction.MoveLeft) ? -1 : 0));
                return NodeState.SUCCESS;
            }
            else
            {
                return NodeState.FAILURE;
            }
        }
    }
}

