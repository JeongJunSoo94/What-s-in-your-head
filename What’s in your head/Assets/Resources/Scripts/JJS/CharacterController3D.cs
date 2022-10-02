using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class CharacterController3D : MonoBehaviour
    {
        Rigidbody m_Rigidbody;
        CapsuleCollider m_Capsule;

        [Tooltip("�������� üũ�� ���̾� ����")]
        public LayerMask groundLayerMask;

        [Range(0.1f, 10.0f), Tooltip("���� ���� �Ÿ�")]
        public float groundCheckDistance = 2.0f;
        [Range(0.0f, 0.1f), Tooltip("���� �ν� ��� �Ÿ�")]
        public float groundCheckThreshold = 0.01f;

        //�ٴ�üũ
        RaycastHit groundRaycastHit;

        public Vector3 groundNormal;
        public Vector3 groundCross;

        public float groundDistance;
        public float slopeAngle; // ĳ���Ͱ� �ٶ󺸴� ������ ��簢
        public float maxSlopeAngle = 40f; // ĳ���Ͱ� Ÿ�� �ö� �� �ִ� �ִ� ��簢

        public bool IsGrounded;//{ get; protected set; }
        public bool IsOnSteepSlope;// { get; protected set; }//�Ұ����� ����
        //�ٴ�üũ

        //�̵�
        public float moveSpeed;
        public Vector3 horizontalVelocity;

        //�̵�

        //����
        public float jumpForce = 10f;
        public float jumpCoolTime;
        public float curJumpCoolTime;
        public int jumpCount;
        public float outOfControllDuration;

        public bool IsJumpTriggered { get; set; }
        public bool IsJumping { get; protected set; }

        //����

        [Range(0f, 4f), Tooltip("���� �̵��ӵ� ��ȭ��(����/����)")]
        public float slopeAccel = 1f;

        public float curSlopeAccel = 1f;

        [Range(-9.81f, 0f), Tooltip("�߷�")]
        public float gravity = -9.81f;
        public float curGravity;
        public bool IsOutOfControl { get; protected set; }//����Ұ���


        private float _capsuleRadiusDiff;
        private float _fixedDeltaTime;


        public Vector3 Velocity { get; protected set; }
        public Rigidbody Rigidbody { get { return m_Rigidbody; } }


        public Vector3 worldMoveDir= Vector3.zero;

        public Vector3 specialVector;

        void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
        }

        private void Start()
        {
            
        }

        void FixedUpdate()
        {
            _fixedDeltaTime = Time.fixedDeltaTime;

            CheckGround(worldMoveDir);

            UpdatePhysics();
            UpdateValues();

            CalculateMovements();
            ApplyMovementsToRigidbody();
        }

        private void CheckGround(Vector3 dir)
        {
            IsGrounded = Physics.SphereCast(transform.position + Vector3.up * m_Capsule.radius, m_Capsule.radius, Vector3.down, out groundRaycastHit, groundCheckDistance, groundLayerMask, QueryTriggerInteraction.Ignore);

            if (IsGrounded)
            {
                // ���� ��ġ�� ������ ��簢 ���ϱ�(ĳ���� �̵����� ���)
                slopeAngle = Vector3.Angle(groundRaycastHit.normal, dir) - 90f;

                IsOnSteepSlope = Mathf.Abs(slopeAngle) >= maxSlopeAngle;

                IsGrounded = !IsOnSteepSlope;
            }
        }

        private void CalculateMovements()
        {
            if (IsOutOfControl)
            {
                horizontalVelocity = Vector3.zero;
                return;
            }
            JumpCheck();
            HorizontalMove();
        }
        private void ResetJump()
        {
            curJumpCoolTime = 0f;
            jumpCount = 0;
            IsJumping = false;
            IsJumpTriggered = false;
        }
        private void JumpCheck()
        {
            // 1. ����
            if (IsJumpTriggered && curJumpCoolTime <= 0f)
            {
                curGravity = jumpForce;

                // ���� ��Ÿ��, Ʈ���� �ʱ�ȭ
                curJumpCoolTime = jumpCoolTime;
                IsJumpTriggered = false;
                IsJumping = true;
                jumpCount++;
            }
        }
        private void HorizontalMove()
        {
            //// 2. XZ �̵��ӵ� ���
            //// ���߿��� ������ ���� ��� ���� (���󿡼��� ���� �پ �̵��� �� �ֵ��� ���)
            if (!IsGrounded)
            {
                horizontalVelocity = Vector3.zero;
            }
            else // �̵� ������ ��� : ���� or ������ ������ ����
            {
                horizontalVelocity = worldMoveDir.normalized * moveSpeed;
            }
        }

        private void UpdatePhysics()
        {
            // Custom Gravity, Jumping State
            if (IsGrounded)
            {
                curGravity = 0f;

                jumpCount = 0;
                IsJumping = false;
            }
            else
            {
                curGravity += _fixedDeltaTime * gravity;
            }
        }

        private void UpdateValues()
        {
            // Calculate Jump Cooldown
            if (curJumpCoolTime > 0f)
                curJumpCoolTime -= _fixedDeltaTime;

            // Out Of Control
            IsOutOfControl = outOfControllDuration > 0f;

            if (IsOutOfControl)
            {
                outOfControllDuration -= _fixedDeltaTime;
                worldMoveDir = Vector3.zero;
            }
        }

        private void ApplyMovementsToRigidbody()
        {
            //if (curSlopeAccel > -50f)
            //    gravity = moveSpeed * Mathf.Tan(curSlopeAccel * Mathf.PI / 180);
            //else
            //    gravity = 0;

            if (IsOutOfControl)
            {
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, curGravity, m_Rigidbody.velocity.z);
                return;
            }
            Velocity = horizontalVelocity + Vector3.up * (curGravity) + specialVector;
            m_Rigidbody.velocity = Velocity;
        }

        void DrawGizmoUpdate()
        {
            //Gizmos.color = Color.green;
            //Gizmos.DrawSphere(transform.position+ Vector3.up * m_Capsule.radius, _castRadius);//(groundRaycastHit.point- transform.position).normalized);

            if (IsGrounded)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(groundRaycastHit.point, 0.07f);
            }

            //if (IsForwardBlocked)
            //{
            //    Gizmos.color = Color.red;
            //    Gizmos.DrawSphere(forwardRaycastHit.point, 0.07f);
            //}
        }

        private void OnDrawGizmos()
        {
            DrawGizmoUpdate();
        }
    }
}

