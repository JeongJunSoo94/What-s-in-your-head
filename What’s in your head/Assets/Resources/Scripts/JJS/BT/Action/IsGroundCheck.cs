using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS
{
    public class IsGroundCheck : Task
    {
        CharacterControlBT _obj;
        RaycastHit groundRaycastHit;

        public IsGroundCheck(CharacterControlBT obj)
        {
            _obj = obj;
            groundRaycastHit = new RaycastHit();
        }
        public override NodeState Evaluate()
        {
            _obj.groundChecked = Physics.SphereCast(_obj.transform.position + Vector3.up * _obj.pCapsuleCollider.radius, _obj.pCapsuleCollider.radius, Vector3.down, out groundRaycastHit, _obj.groundCheckMaxDistance, LayerMask.NameToLayer("Platform"));

            if (_obj.groundChecked)
            {
                Vector3 HorVel = _obj.pRigidbody.velocity;
                HorVel.y = 0;
                _obj.slopeAngle = Vector3.Angle(HorVel, groundRaycastHit.normal) - 90f;
                return NodeState.SUCCESS;
            }
            else
            {
                return NodeState.FAILURE;
            }
        }

        void DrawGizmoUpdate()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(groundRaycastHit.point, 0.07f);
        }
    }
}