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

        [Tooltip("지면으로 체크할 레이어 설정")]
        public LayerMask groundLayerMask = -1;

        [Range(0.1f, 10.0f), Tooltip("지면 감지 거리")]
        public float groundCheckDistance = 2.0f;
        [Range(0.0f, 0.1f), Tooltip("지면 인식 허용 거리")]
        public float groundCheckThreshold = 0.01f;

        //전방체크
        RaycastHit forwardRaycastHit;
        [Range(0.01f, 0.5f), Tooltip("전방 감지 거리")]
        public float forwardCheckDistance = 0.1f;

        [Range(1f, 70f), Tooltip("등반 가능한 경사각")]
        public float maxSlopeAngle = 50f;

        public bool IsForwardBlocked;//{ get; protected set; }//전방장애물

        //전방체크

        //바닥체크
        RaycastHit groundRaycastHit;

        public Vector3 groundNormal;
        public Vector3 groundCross;

        public float groundDistance;
        public float groundSlopeAngle;         // 현재 바닥의 경사각
        public float groundVerticalSlopeAngle; // 수직으로 재측정한 경사각
        public float forwardSlopeAngle; // 캐릭터가 바라보는 방향의 경사각

        public bool IsGrounded;//{ get; protected set; }
        public bool IsOnSteepSlope;// { get; protected set; }//불가능한 경사로
        //바닥체크

        //이동
        public float moveSpeed;
        public Vector3 horizontalVelocity;

        //이동

        //점프
        public float jumpForce = 10f;
        public float jumpCoolTime;
        public float curJumpCoolTime;
        public int jumpCount;
        public float outOfControllDuration;

        public bool IsJumpTriggered { get; set; }
        public bool IsJumping { get; protected set; }

        //점프

        [Range(0f, 4f), Tooltip("경사로 이동속도 변화율(가속/감속)")]
        public float slopeAccel = 1f;

        public float curSlopeAccel = 1f;

        [Range(-9.81f, 0f), Tooltip("중력")]
        public float gravity = -9.81f;
        public float curGravity;
        public bool IsOutOfControl { get; protected set; }//제어불가능


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
            // 회전은 자식 트랜스폼을 통해 직접 제어할 것이기 때문에 리지드바디 회전은 제한
            //m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            //m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            //m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            m_Rigidbody.useGravity = false; // 중력 직접 제어
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
                // 지면 노멀벡터 초기화
                groundNormal = groundRaycastHit.normal;

                // 현재 위치한 지면의 경사각 구하기(캐릭터 이동방향 고려)
                groundSlopeAngle = Vector3.Angle(groundNormal, Vector3.up);
                forwardSlopeAngle = Vector3.Angle(groundNormal, dir) - 90f;

                IsOnSteepSlope = groundSlopeAngle >= maxSlopeAngle;

                groundDistance = Mathf.Max(groundRaycastHit.distance - _capsuleRadiusDiff - groundCheckThreshold, -0f);

                IsGrounded = (groundDistance <= 0.0001f) && !IsOnSteepSlope;
            }

            // 월드 이동벡터 회전축
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
            // 1. 점프
            if (IsJumpTriggered && curJumpCoolTime <= 0f)
            {
                curGravity = jumpForce;

                // 점프 쿨타임, 트리거 초기화
                curJumpCoolTime = jumpCoolTime;
                IsJumpTriggered = false;
                IsJumping = true;
                jumpCount++;
            }
        }

        private void HorizontalMove()
        {
            // 2. XZ 이동속도 계산
            // 공중에서 전방이 막힌 경우 제한 (지상에서는 벽에 붙어서 이동할 수 있도록 허용)
            if (IsForwardBlocked && !IsGrounded)
            {
                horizontalVelocity = Vector3.zero;
            }
            else // 이동 가능한 경우 : 지상 or 전방이 막히지 않음
            {
                horizontalVelocity = worldMoveDir.normalized * moveSpeed;
            }
        }

        private void FrontRotateMove()
        {
            // 3. XZ 벡터 회전
            // 지상이거나 지면에 가까운 높이
            if (IsGrounded || groundDistance < groundCheckDistance && !IsJumping)
            {
                if (worldMoveDir.sqrMagnitude > 0.01f && !IsForwardBlocked)
                {
                    //// 경사로 인한 가속/감속
                    //if (slopeAccel > 0f)
                    //{
                    //    bool isPlus = forwardSlopeAngle >= 0f;
                    //    float absFsAngle = isPlus ? forwardSlopeAngle : -forwardSlopeAngle;
                    //    float accel = slopeAccel * absFsAngle * 0.01111f + 1f;
                    //    curSlopeAccel = !isPlus ? accel : 1.0f / accel;

                    //    horizontalVelocity *= curSlopeAccel;
                    //}

                    // 벡터 회전 (경사로)
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

