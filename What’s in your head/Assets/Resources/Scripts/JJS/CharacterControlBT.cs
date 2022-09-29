using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS
{
    public class CharacterControlBT : MonoBehaviour
    {
        public float slopeAngle = 0.0f;

        [Range(0.01f, 2.0f)]
        public float slopeCofacter = 0.1f;

        [Range(0.01f, 2.0f)]
        public float groundCheckMaxDistance = 0.1f;

        public float walkSpeed = 4.0f;
        public float runSpeed = 6.0f;
        public float moveSpeed = 0;
        public float dashSpeed = 4.0f;
        float addSpeed = 0.0f;
        public float gravity = 9.8f;
        public float terVelocity = 20f;
        public float dashTime = 0.5f;
        public float jumpPower = 10.0f;
        public float airJumpPower = 10.0f;
        public float rotationSpeed = 360.0f;

        public Rigidbody pRigidbody;
        public CapsuleCollider pCapsuleCollider;
        public Camera mCamera;
        public RaycastHit forwardRaycastHit;

        public Vector3 direction;
        public Vector3 dashVec = Vector3.zero;

        public bool onPlatform = true;
        public bool isBlocked = false;
        public bool isJumping = false;
        public bool isAirJump = false;
        public bool jumpTrigger = false;
        public float jumpTriggerTime = 0.1f;
        public bool isDash = false;
        public bool isAirDash = false;
        public int jumpcount = 0;

        public bool groundChecked;

        public Animator animator;
        public string currentState;
        public Task _root;

        private void Awake()
        {
            // >> 게임매니저로 옮겨갈 것들
            ItTakesTwoKeyManager.Instance.GetKeyDown(KeyName.W);
            Application.targetFrameRate = 60;
            // <<
        }
        // Start 와 캐릭터 기본설정 초기화
        #region
        void Start()
        {
            forwardRaycastHit = new RaycastHit();
            mCamera = Camera.main;
            RigidbodyInit();
            CapsuleColliderInit();
            ConstructBehaviourTree();
        }
        void RigidbodyInit()
        {
            TryGetComponent<Rigidbody>(out pRigidbody);
            if (pRigidbody == null)
                pRigidbody = gameObject.AddComponent<Rigidbody>();
            pRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            pRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            pRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            pRigidbody.useGravity = false;
        }

        void CapsuleColliderInit()
        {
            TryGetComponent<CapsuleCollider>(out pCapsuleCollider);
            if (pCapsuleCollider == null)
                pCapsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            float top = -0.1f;
            SkinnedMeshRenderer[] skinMashRenders = GetComponentsInChildren<SkinnedMeshRenderer>();
            if (skinMashRenders.Length > 0)
            {
                foreach (var skinMashRender in skinMashRenders)
                {
                    foreach (var vertex in skinMashRender.sharedMesh.vertices)
                    {
                        if (top < vertex.y)
                            top = vertex.y;
                    }
                }
            }
            else
            {
                MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
                if (meshFilters.Length > 0)
                {
                    foreach (var meshFilter in meshFilters)
                    {
                        foreach (var vertex in meshFilter.mesh.vertices)
                        {
                            if (top < vertex.y)
                                top = vertex.y;
                        }
                    }
                }
            }
            if (top < 0.0f)
                Debug.Log("Cant find meshes");

            pCapsuleCollider.center = Vector3.up * top / 2.0f;
            pCapsuleCollider.height = top;
            pCapsuleCollider.radius = top / 4.0f;
            //float upperSphereCenter = pCapsuleCollider.height - pCapsuleCollider.radius;
        }
        private void ConstructBehaviourTree()
        {

            Sequence action = new(new List<Task> { });

            InputHorizonMove inputHorizonMove = new InputHorizonMove(this);
            VerticalMove inputVerticalMove = new VerticalMove(this);

            IsGroundCheck isGroundCheck = new IsGroundCheck(this);

            ObjectMove playerMove = new ObjectMove(this);
            ObjectRotation playerRotate = new ObjectRotation(this);
            PlayAnimation playerWalkAnim = new PlayAnimation(this, "Move");
            PlayAnimation playerWaitAnim = new PlayAnimation(this, "WAIT01");
            Selector moveAnim = new Selector(new List<Task> { playerWaitAnim, playerWalkAnim });

            Sequence horizonMove = new(new List<Task> { inputHorizonMove, playerRotate });//수평움직임

            AirJump Airjump = new AirJump(this);
            GroundJump groundJump = new GroundJump(this);

            Selector airVertical = new Selector(new List<Task> { Airjump });
            Selector groundVertical = new Selector(new List<Task> { groundJump });

            Selector isJumpCheck = new Selector(new List<Task> { groundVertical, airVertical });

            Sequence verticalMove = new(new List<Task> { isJumpCheck });//수직움직임

            Parallel move = new Parallel(new List<Task> { isGroundCheck, horizonMove, verticalMove , playerMove });

            _root = new Parallel(new List<Task> { move, action });
        }
        #endregion
        void Update()
        {
            _root.Evaluate();
            //RaycheckGround();
            //RaycheckForward();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //MoveVec();
            //JumpVec();
            //DashVec();
            //Move();
        }

        void MoveVec()
        {
            float moveSpeed;

            if (!isDash && !isJumping && ItTakesTwoKeyManager.Instance.GetKey(KeyName.CapsLock))
            {
                moveSpeed = runSpeed;
            }
            else
            {
                moveSpeed = walkSpeed;
            }

            direction = mCamera.transform.forward * ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.W) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.S) ? -1 : 0))
                + mCamera.transform.right * ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.D) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.A) ? -1 : 0));
            direction.y = 0;
            direction = direction.normalized;

            Vector3 forward = Vector3.Slerp(transform.forward, direction, rotationSpeed * Time.fixedDeltaTime / Vector3.Angle(transform.forward, direction));
            transform.LookAt(transform.position + forward);

            direction = forward * moveSpeed;
            direction.y = pRigidbody.velocity.y;

            if (jumpTrigger)
            {
                direction.y -= gravity * Time.fixedDeltaTime;
            }
            else if (onPlatform)
            {
                if (slopeAngle > -50f)
                    direction.y = moveSpeed * Mathf.Tan(slopeAngle * Mathf.PI / 180);
                else
                    direction.y = 0;
            }
            else if (isAirDash)
            {
                direction.y = 0;
            }
            else
            {
                direction.y -= gravity * Time.fixedDeltaTime;
            }
            if (direction.y < -terVelocity)
                direction.y = -terVelocity;
        }

        void DashVec()
        {
            dashVec = Vector3.zero;
            if (!isDash && ItTakesTwoKeyManager.Instance.GetKeyDown(KeyName.LeftShift))
            {
                StartCoroutine("CorDash");
            }
            dashVec = transform.forward.normalized * addSpeed;
        }

        IEnumerator CorDash()
        {
            if (!onPlatform)
            {
                isAirDash = true;
            }
            isDash = true;
            addSpeed = dashSpeed;
            yield return new WaitForSeconds(dashTime);
            isAirDash = false;
            isDash = false;
            addSpeed = 0.0f;
        }

        void JumpVec()
        {
            if (ItTakesTwoKeyManager.Instance.GetKeyDown(KeyName.Space))
            {
                if (isAirDash)
                {
                    isAirDash = false;
                }

                if (!isJumping || jumpcount < 2)
                {
                    StartCoroutine("CorJump");
                    direction.y = jumpPower;
                    jumpcount++;
                    isJumping = true;
                }
            }
        }

        IEnumerator CorJump()
        {
            if (jumpTrigger)
            {
                yield return 0;
            }
            else
            {
                jumpTrigger = true;
                yield return new WaitForSeconds(jumpTriggerTime);
                jumpTrigger = false;
            }
        }

        void Move()
        {
            pRigidbody.velocity = direction + dashVec;
            //Debug.Log("수직속도: " + pRigidbody.velocity.y);
        }
        void RaycheckGround()
        {
            //bool rayChecked = Physics.SphereCast(transform.position + Vector3.up * pCapsuleCollider.radius, pCapsuleCollider.radius, Vector3.down, out groundRaycastHit, groundCheckMaxDistance);
            //Debug.Log(groundRaycastHit.point);
            //Debug.Log("rayCheck: " + rayChecked);

            //if (rayChecked)
            //{
            //    if (groundRaycastHit.collider.tag == "Platform")
            //    {
            //        rayChecked = true;
            //        Vector3 HorVel = pRigidbody.velocity;
            //        HorVel.y = 0;
            //        slopeAngle = Vector3.Angle(HorVel, groundRaycastHit.normal) - 90f;
            //    }
            //    else
            //    {
            //        rayChecked = false;
            //    }
            //}

            //if (rayChecked)
            //{
            //    onPlatform = true;
            //    isJumping = false;
            //    jumpcount = 0;
            //}
            //else
            //{
            //    if (onPlatform)
            //    {
            //        jumpcount = 1;
            //        onPlatform = false;

            //        if (isDash && isJumping)
            //        {
            //            {
            //                //dashcount = 1; 여기는 지면대시 >> 점프 >> 대시를 막는 코드라서 지움
            //                StopCoroutine("CorDash"); // 지면대시 >> 점프 >> 대시하면 처음 대시의 코루틴이 작동해서 공중대시가 빨리끝남
            //                addSpeed = 0.0f;
            //                isDash = false;
            //            }
            //        }
            //    }
            //}
        }


        void DrawGizmoUpdate()
        {
            //if (onPlatform)
            //{
            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawSphere(groundRaycastHit.point, 0.07f);
            //}

            if (isBlocked)
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
   
