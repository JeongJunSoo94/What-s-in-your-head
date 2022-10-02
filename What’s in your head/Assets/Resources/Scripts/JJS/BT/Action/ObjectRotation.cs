using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS
{
    public class ObjectRotation : Task
    {
        private CharacterControlBT _obj;
        public ObjectRotation(CharacterControlBT obj)
        {
            _obj = obj;
        }

        public override NodeState Evaluate()
        {
            Rotate();
            return NodeState.SUCCESS;
        }

        void Rotate()
        {
            Vector3 forward = Vector3.Slerp(_obj.transform.forward, _obj.direction, _obj.rotationSpeed * Time.deltaTime / Vector3.Angle(_obj.transform.forward, _obj.direction));
            _obj.transform.LookAt(_obj.transform.position + forward);
        }
    }
}

