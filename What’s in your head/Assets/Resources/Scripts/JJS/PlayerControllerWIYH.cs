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

        public bool isRun;

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
            Debug.Log("Update");
        }

        private void FixedUpdate()
        {
            Debug.Log("FixedUpdate");
        }

        public void InputRun()
        {
            Debug.Log("AnimationUpdate");
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

        public void InputMove()
        {
            charactercontrol.worldMoveDir = mCamera.transform.forward * ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.W) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.S) ? -1 : 0))
            + mCamera.transform.right * ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.D) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.A) ? -1 : 0));

            charactercontrol.worldMoveDir.y = 0;
        }
        public void TopViewInputMove()
        {
            charactercontrol.worldMoveDir.z = ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.W) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.S) ? -1 : 0));
            charactercontrol.worldMoveDir.x = ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.D) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.A) ? -1 : 0));
            charactercontrol.worldMoveDir.y = 0;
        }
        public void InputJump()
        {
            if (ItTakesTwoKeyManager.Instance.GetKeyDown(KeyName.Space))
            {
                if (charactercontrol.curJumpCoolTime <= 0f)
                {
                    charactercontrol.IsJumpTriggered = true;
                }
            }
        }

        public void Rotation()
        {
            Vector3 forward = Vector3.Slerp(transform.forward, charactercontrol.worldMoveDir.normalized, rotationSpeed * Time.fixedDeltaTime / Vector3.Angle(transform.forward, charactercontrol.worldMoveDir.normalized));
            transform.LookAt(transform.position + forward);
        }
        
    }
}
