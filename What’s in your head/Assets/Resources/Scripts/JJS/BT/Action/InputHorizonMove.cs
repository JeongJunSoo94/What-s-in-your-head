using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            if (ItTakesTwoKeyManager.Instance.GetKey(KeyName.W) || ItTakesTwoKeyManager.Instance.GetKey(KeyName.S)
               || ItTakesTwoKeyManager.Instance.GetKey(KeyName.A) || ItTakesTwoKeyManager.Instance.GetKey(KeyName.D))
            {
                _obj.direction.z = (ItTakesTwoKeyManager.Instance.GetKey(KeyName.W) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.S) ? -1 : 0);
                _obj.direction.x = (ItTakesTwoKeyManager.Instance.GetKey(KeyName.D) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.A) ? -1 : 0);
                return NodeState.SUCCESS;
            }
            else
            {
                return NodeState.FAILURE;
            }
        }
    }
}

