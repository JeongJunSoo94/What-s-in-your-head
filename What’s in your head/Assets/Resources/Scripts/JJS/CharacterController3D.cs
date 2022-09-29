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
        public LayerMask groundLayerMask = -1;

        [Range(0.1f, 10.0f), Tooltip("���� ���� �Ÿ�")]
        public float groundCheckDistance = 2.0f;
        [Range(0.0f, 0.1f), Tooltip("���� �ν� ��� �Ÿ�")]
        public float groundCheckThreshold = 0.01f;

        //����üũ
        RaycastHit forwardRaycastHit;
        [Range(0.01f, 0.5f), Tooltip("���� ���� �Ÿ�")]
        public float forwardCheckDistance = 0.1f;

        [Range(1f, 70f), Tooltip("��� ������ ��簢")]
        public float maxSlopeAngle = 50f;

        public bool IsForwardBlocked;//{ get; protected set; }//������ֹ�

        //����üũ

        //�ٴ�üũ
        RaycastHit groundRaycastHit;

        public Vector3 groundNormal;
        public Vector3 groundCross;

        public float groundDistance;
        public float groundSlopeAngle;         // ���� �ٴ��� ��簢
        public float groundVerticalSlopeAngle; // �������� �������� ��簢
        public float forwardSlopeAngle; // ĳ���Ͱ� �ٶ󺸴� ������ ��簢

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

        public float _castRadius=1f;


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
            InitRigidbody();
        }

        void FixedUpdate()
        {
            _fixedDeltaTime = Time.fixedDeltaTime;

            CheckGround(worldMoveDir);
            CheckForward(worldMoveDir);

            UpdatePhysics();
            UpdateValues();

            CalculateMovements();
            ApplyMovementsToRigidbody();
        }

        private void InitRigidbody()
        {
            // ȸ���� �ڽ� Ʈ�������� ���� ���� ������ ���̱� ������ ������ٵ� ȸ���� ����
            //m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            //m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            //m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            m_Rigidbody.useGravity = false; // �߷� ���� ����
        }

        private void CheckForward(Vector3 dir)
        {
            bool cast = Physics.CapsuleCast(transform.position + Vector3.up * m_Capsule.radius, transform.position + Vector3.up * (m_Capsule.height- m_Capsule.radius), _castRadius, dir + Vector3.down * 0.1f,
                    out forwardRaycastHit, forwardCheckDistance, -1, QueryTriggerInteraction.Ignore);

            IsForwardBlocked = false;
            if (cast)
            {
                float forwardObstacleAngle = Vector3.Angle(forwardRaycastHit.normal, Vector3.up);
                IsForwardBlocked = forwardObstacleAngle >= maxSlopeAngle;
            }
        }

        private void CheckGround(Vector3 dir)
        {
            groundDistance = float.MaxValue;
            groundNormal = Vector3.up;
            groundSlopeAngle = 0f;
            forwardSlopeAngle = 0f;

            bool cast = Physics.SphereCast(transform.position + Vector3.up * m_Capsule.radius, _castRadius, Vector3.down, out groundRaycastHit, groundCheckDistance, groundLayerMask, QueryTriggerInteraction.Ignore);

            IsGrounded = false;

            if (cast)
            {
                // ���� ��ֺ��� �ʱ�ȭ
                groundNormal = groundRaycastHit.normal;

                // ���� ��ġ�� ������ ��簢 ���ϱ�(ĳ���� �̵����� ���)
                groundSlopeAngle = Vector3.Angle(groundNormal, Vector3.up);
                forwardSlopeAngle = Vector3.Angle(groundNormal, dir) - 90f;

                IsOnSteepSlope = groundSlopeAngle >= maxSlopeAngle;

                groundDistance = Mathf.Max(groundRaycastHit.distance - _capsuleRadiusDiff - groundCheckThreshold, -0f);

                IsGrounded = (groundDistance <= 0.0001f) && !IsOnSteepSlope;
            }

            // ���� �̵����� ȸ����
            groundCross = Vector3.Cross(groundNormal, Vector3.up);
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
            FrontRotateMove();
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
            // 2. XZ �̵��ӵ� ���
            // ���߿��� ������ ���� ��� ���� (���󿡼��� ���� �پ �̵��� �� �ֵ��� ���)
            if (IsForwardBlocked && !IsGrounded)
            {
                horizontalVelocity = Vector3.zero;
            }
            else // �̵� ������ ��� : ���� or ������ ������ ����
            {
                horizontalVelocity = worldMoveDir.normalized * moveSpeed;
            }
        }

        private void FrontRotateMove()
        {
            // 3. XZ ���� ȸ��
            // �����̰ų� ���鿡 ����� ����
            if (IsGrounded || groundDistance < groundCheckDistance && !IsJumping)
            {
                if (worldMoveDir.sqrMagnitude > 0.01f && !IsForwardBlocked)
                {
                    //// ���� ���� ����/����
                    //if (slopeAccel > 0f)
                    //{
                    //    bool isPlus = forwardSlopeAngle >= 0f;
                    //    float absFsAngle = isPlus ? forwardSlopeAngle : -forwardSlopeAngle;
                    //    float accel = slopeAccel * absFsAngle * 0.01111f + 1f;
                    //    curSlopeAccel = !isPlus ? accel : 1.0f / accel;

                    //    horizontalVelocity *= curSlopeAccel;
                    //}

                    // ���� ȸ�� (����)
                    horizontalVelocity = Quaternion.AngleAxis(-groundSlopeAngle, groundCross) * horizontalVelocity;
                }
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

            if (IsForwardBlocked)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(forwardRaycastHit.point, 0.07f);
            }
        }

        private void OnDrawGizmos()
        {
            DrawGizmoUpdate();
        }
    }
}

