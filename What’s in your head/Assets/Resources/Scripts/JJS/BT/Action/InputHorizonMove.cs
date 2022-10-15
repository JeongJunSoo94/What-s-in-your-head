using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;


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
            if (KeyManager.Instance.GetKeyDown(PlayerAction.MoveForward) || KeyManager.Instance.GetKeyDown(PlayerAction.MoveBackward)
               || KeyManager.Instance.GetKeyDown(PlayerAction.MoveRight) || KeyManager.Instance.GetKeyDown(PlayerAction.MoveLeft))
            {
                _obj.direction.z = ((KeyManager.Instance.GetKeyDown(PlayerAction.MoveForward) ? 1 : 0) + (KeyManager.Instance.GetKeyDown(PlayerAction.MoveBackward) ? -1 : 0));
                _obj.direction.x = ((KeyManager.Instance.GetKeyDown(PlayerAction.MoveRight) ? 1 : 0) + (KeyManager.Instance.GetKeyDown(PlayerAction.MoveLeft) ? -1 : 0));
                return NodeState.SUCCESS;
            }
            else
            {
                return NodeState.FAILURE;
            }
        }
    }
}

