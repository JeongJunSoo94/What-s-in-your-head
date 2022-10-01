using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS
{
    public class PlayerControllerWIYH : MonoBehaviour
    {
        CharacterController3D charactercontrol;

        public float runSpeed;
        public float walkSpeed;
        public float moveSpeed;
        public float rotationSpeed;

        public bool isMoved;
        public bool isRun;

        public Vector3 dir;

        Animator m_animator;

        Camera mCamera;
        void Awake()
        {
            charactercontrol = GetComponent<CharacterController3D>();
            m_animator = GetComponent<Animator>();
            mCamera = Camera.main;
        }
        // Start is called before the first frame update
        void Start()
        {
            charactercontrol.moveSpeed = moveSpeed;
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void FixedUpdate()
        {
            CamaraMoveUpdate();
        }

        public void InputRun()
        {
            if (ItTakesTwoKeyManager.Instance.GetKey(KeyName.CapsLock))
            {
                charactercontrol.moveSpeed = runSpeed;
                isRun = true;
            }
            else
            {
                charactercontrol.moveSpeed = walkSpeed;
                isRun = false;
            }
        }

        public void CamaraMoveUpdate()
        {
            charactercontrol.worldMoveDir = mCamera.transform.forward * dir.z
            + mCamera.transform.right * dir.x;

            charactercontrol.worldMoveDir.y = 0;
        }

        public void InputMove()
        {
            dir.z = ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.W) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.S) ? -1 : 0));
            dir.x = ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.D) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.A) ? -1 : 0));
            if (dir.z != 0 || dir.x != 0)
            {
                isMoved = true;
            }
            else
            {
                dir = Vector3.zero;
                isMoved = false;
            }
        }

        public void InputJump()
        {
            if (ItTakesTwoKeyManager.Instance.GetKeyDown(KeyName.Space))
            {
                //if (charactercontrol.curJumpCoolTime <= 0f)
                {
                    charactercontrol.IsJumpTriggered = true;
                }
            }
        }

        public void RotationAim()
        {
            Vector3 forward = mCamera.transform.forward.normalized;
            forward.y = 0;
            transform.LookAt(transform.position + forward);
        }

        public void RotationNormal()
        {
            Vector3 forward = Vector3.Slerp(transform.forward, charactercontrol.worldMoveDir.normalized, rotationSpeed * Time.fixedDeltaTime / Vector3.Angle(transform.forward, charactercontrol.worldMoveDir.normalized));
            transform.LookAt(transform.position + forward);
        }
        
    }
}
