using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS
{
    public class PlayerControllerWIYH : MonoBehaviour
    {
        CharacterController3D charactercontrol;

        public float moveSpeed;
        public float rotationSpeed;



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
            AnimatorInit();
        }

        // Update is called once per frame
        void Update()
        {
            //InputMove();
            InputJump();
        }

        private void FixedUpdate()
        {
            Rotation(charactercontrol.worldMoveDir.normalized);
        }

        void AnimatorInit()
        {
        }

        public void InputMove()
        {
            charactercontrol.worldMoveDir = mCamera.transform.forward * ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.W) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.S) ? -1 : 0))
            + mCamera.transform.right * ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.D) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.A) ? -1 : 0));

            charactercontrol.worldMoveDir.y = 0;
        }

        void InputJump()
        {
            if (ItTakesTwoKeyManager.Instance.GetKeyDown(KeyName.Space))
            {
                if (charactercontrol.curJumpCoolTime <= 0f)
                {
                    charactercontrol.IsJumpTriggered = true;
                }
            }
        }

        void Rotation(Vector3 direction)
        {
            Vector3 forward = Vector3.Slerp(transform.forward, direction, rotationSpeed * Time.fixedDeltaTime / Vector3.Angle(transform.forward, direction));
            transform.LookAt(transform.position + forward);
        }
        
    }
}
