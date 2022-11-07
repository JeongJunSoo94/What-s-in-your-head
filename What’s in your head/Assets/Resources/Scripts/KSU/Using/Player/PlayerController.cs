using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using Photon.Pun;
using JCW.AudioCtrl;
using Cinemachine;

using YC.Camera_;
using YC.Camera_Single;
using JCW.Object;
using JJS;
using KSU.Monster;

namespace KSU
{
    public class PlayerController : MonoBehaviour
    {
        //  Scripts Components
        #region    
        public PlayerState characterState;
        public PlayerMouseController playerMouse;
        #endregion

        // 유니티 제공 Components
        #region
        Animator playerAnimator;
        CapsuleCollider playerCapsuleCollider;
        public Camera mainCamera;
        public Rigidbody playerRigidbody;
        //[Header("키 설정")] [SerializeField] private GameObject UI_BG;    
        PhotonView photonView;
        #endregion

        // 수평 Speed
        #region
        [Header("움직임")]
        [Tooltip("현재 이동 속력")]
        float moveSpeed = 0f;
        [Tooltip("걷는 속력")]
        public float walkSpeed = 4f;
        [Tooltip("달리는 속력")]
        public float runSpeed = 7f;
        [Tooltip("대시 속력")]
        public float dashSpeed = 10f;
        [Tooltip("공중 대시 속력")]
        public float airDashSpeed = 8f;
        [Tooltip("관성 속력")]
        public float inertiaSpeed = 0f;
        [Tooltip("공중 이동 속력")]
        public float airMoveSpeed = 1f;
        //[Tooltip("넉백 수평 속력")]
        //public float knockBackHorizonSpeed = 6f;
        #endregion

        // 수직 Speed
        #region
        [Tooltip("점프 속도")]
        public float jumpSpeed = 10f;
        [Tooltip("공중 점프 속도")]
        public float airJumpSpeed = 6f;
        [Range(-20f, 0f), Tooltip("중력")]
        public float gravity = -9.81f;
        public float gravityCofactor = 0.8f;
        [Range(-20f, -1f), Tooltip("종단속도")]
        public float terminalSpeed = -10f;
        [Tooltip("넉백 수직 속력")]
        public float knockBackVerticalSpeed = 8f;
        #endregion

        // 회전 Speed
        #region
        [Tooltip("캐릭터 이동시 회전 속도")]
        public float rotationSpeed = 720f;
        #endregion

        // 속도 관련 벡터
        #region
        [Tooltip("현재 이동 벡터")]
        public Vector3 moveVec = Vector3.zero; // rigidbody.velocity에 대입할 최종 속도 벡터
        [Tooltip("키입력에 따른 수평 벡터")]
        public Vector3 moveDir = Vector3.zero; // 키 입력에 따른 수평 속도 벡터
        [Tooltip("대시 벡터")]
        public Vector3 dashVec = Vector3.zero; // dash또는 airdash중에 사용할 벡터
        [Tooltip("관성 노말 벡터")]
        public Vector3 inertiaNormalVec = Vector3.zero; // 공중 상태에 사용할 관성 벡터
        [Tooltip("넉백 벡터")]
        public Vector3 knockBackVec = Vector3.zero;
        #endregion

        [HideInInspector] public bool isOn_HP_UI = false;

        void Awake()
        {
            // >> : YC
            Cursor.lockState = CursorLockMode.Locked;
            Application.targetFrameRate = 120;
            // << :

            characterState = GetComponent<PlayerState>();
            playerAnimator = GetComponent<Animator>();
            playerCapsuleCollider = GetComponent<CapsuleCollider>();
            playerRigidbody = GetComponent<Rigidbody>();
            playerMouse = GetComponent<PlayerMouseController>();


            //====================================================

            photonView = GetComponent<PhotonView>();

            mainCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // 멀티용
            if (mainCamera == null)
                Debug.Log("카메라 NULL");

            characterState.isMine = photonView.IsMine;
            if (!photonView.IsMine)
            {
                GameManager.Instance.otherPlayerTF = this.transform;
                GetComponent<AudioListener>().enabled = false;
            }
            else
                GameManager.Instance.myPlayerTF = this.transform;
            // << : 

            Application.targetFrameRate = 120;
            KeyManager.Instance.GetKeyDown(PlayerAction.MoveBackward);
        }

        // Update is called once per frame
        void Update()
        {
            CheckState();
        }

        private void FixedUpdate()
        {
            if (!photonView.IsMine || characterState.isStopped )
            {
                playerRigidbody.velocity = Vector3.zero;
                return;
            }
            if (characterState.isRiding)
            {
                return;
            }
            TakeRotation();
            Move();
        }

        public void InitController()
        {
            moveSpeed = 0f;
            moveVec = Vector3.zero;
            inertiaNormalVec = Vector3.zero;
            inertiaSpeed = 0f;
            playerRigidbody.velocity = Vector3.zero;
        }

        public void InitAnimatorParam(bool initTop)
        {
            playerAnimator.SetBool("isAir", false);
            playerAnimator.SetBool("isAirDash", false);
            playerAnimator.SetBool("isAirJump", false);
            playerAnimator.SetBool("WasAirJump", false);
            playerAnimator.SetBool("isAttack", false);
            playerAnimator.SetBool("isAttackNext", false);
            playerAnimator.SetBool("Aim", false);
            playerAnimator.SetBool("AimAttack", false);
            playerAnimator.SetBool("WeaponSwap", false);
            playerAnimator.SetBool("isMoveToRope", false);
            playerAnimator.SetBool("isRidingRope", false);
            playerAnimator.SetBool("isMoveToRail", false);
            playerAnimator.SetBool("isRidingRail", false);
            playerAnimator.SetBool("isRailJump", false);
            playerAnimator.SetBool("isTransferRail", false);
            playerAnimator.SetBool("isAttacked", false);
            playerAnimator.SetBool("AttackedTrigger", false);
            playerAnimator.SetBool("isKnockBack", false);
            playerAnimator.SetBool("KnockBackTrigger", false);
            playerAnimator.SetFloat("MoveX", 0f);
            playerAnimator.SetFloat("MoveZ", 0f);
            playerAnimator.SetFloat("HorizonVelocity", 0f);
            playerAnimator.SetFloat("DistY", 0f);
            playerAnimator.SetFloat("moveToRailSpeed", 0f);
            if (initTop)
                playerAnimator.SetBool("Top", false);
            switch (this.gameObject.tag)
            {
                case "Nella":
                    playerAnimator.SetBool("isSinging", false);
                    break;
                case "Steady":
                    {
                        playerAnimator.SetBool("isShootingGrapple", false);
                        playerAnimator.SetBool("isGrappleMoving", false);
                        playerAnimator.SetBool("isGrabMonster", false);
                    }
                    break;
            }
        }

        void InitInteraction()
        {
            
            GetComponent<PlayerInteraction>().InitInteraction();
        }

        private void CheckState()
        {
            if (characterState.IsGrounded)
            {
                characterState.CheckGround(playerCapsuleCollider.radius);
                if (!characterState.IsGrounded && characterState.CanJump)
                {
                    Vector3 horVel = playerRigidbody.velocity;
                    horVel.y = 0;
                    MakeinertiaVec(horVel.magnitude, moveDir);
                }
            }
            else
            {
                characterState.CheckGround(playerCapsuleCollider.radius);
            }
            characterState.CheckMove(playerRigidbody.velocity);
        }
        

        public void ResetLocalPosition()
        {
            transform.localPosition = Vector3.zero;
        }

        public void MakeinertiaVec(float speed, Vector3 nomalVec) // 공중 진입 시 생기는 관성벡터
        {
            inertiaSpeed = speed;
            inertiaNormalVec = nomalVec;
        }



        public void InputRun()
        {
            if (characterState.isOutOfControl || characterState.isStopped)
                return;

            if (KeyManager.Instance.GetKeyDown(PlayerAction.ToggleRun))
            {
                characterState.ToggleRun();
            }
        }
        public void Resurrect()
        {
            InitInteraction();
            InitController();
            GameManager.Instance.curPlayerHP = 12;
            characterState.InitState(true, false);
            if (!File.Exists(Application.dataPath + "/Resources/CheckPointInfo/Stage" +
                GameManager.Instance.curStageIndex + "/Section" + GameManager.Instance.curSection + ".json"))
            {
                Debug.Log(GameManager.Instance.curSection);
                Debug.Log("체크포인트 불러오기 실패");
                return;
            }

            string jsonString = File.ReadAllText(Application.dataPath + "/Resources/CheckPointInfo/Stage" +
                GameManager.Instance.curStageIndex + "/Section" + GameManager.Instance.curSection + ".json");

            SavePosition.PlayerInfo data = JsonUtility.FromJson<SavePosition.PlayerInfo>(jsonString);
            transform.SetPositionAndRotation(new Vector3((float)data.position[0], (float)data.position[1], (float)data.position[2]), new Quaternion((float)data.rotation[0], (float)data.rotation[1], (float)data.rotation[2], (float)data.rotation[3]));
        }

        public void InputSoundTest()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                Resurrect();
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                SoundManager.Instance.PlayBGM_RPC("POP");
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                SoundManager.Instance.PlayBGM_RPC("Tomboy");
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                SoundManager.Instance.PauseResumeBGM_RPC();
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                SoundManager.Instance.PlayEffect_RPC("Explosion");
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                //SoundManager.Instance.PlayEffect_RPC(SoundManager.Instance.GetEffectClips("Fireball"));
                SoundManager.Instance.PlayEffect_RPC("Fireball");
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                SoundManager.Instance.PlayEffect_RPC("GetItem");
            }
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                SoundManager.Instance.PlayEffect_RPC("WaterBall");
            }
        }

        public void InputMove()
        {
            if (characterState.isOutOfControl || characterState.isStopped || characterState.isRiding)
                return;
            moveDir =
              mainCamera.transform.forward * ((KeyManager.Instance.GetKey(PlayerAction.MoveForward) ? 1 : 0) + (KeyManager.Instance.GetKey(PlayerAction.MoveBackward) ? -1 : 0))
            + mainCamera.transform.right * ((KeyManager.Instance.GetKey(PlayerAction.MoveRight) ? 1 : 0) + (KeyManager.Instance.GetKey(PlayerAction.MoveLeft) ? -1 : 0));
            moveDir.y = 0;
            moveDir = moveDir.normalized;

            if (moveDir.magnitude > 0)
                characterState.isMove = true;
            else
            {
                characterState.isMove = false;
            }
        }

        public void MoveStop()
        {
            moveDir = Vector3.zero;
            playerRigidbody.velocity = Vector3.zero;
        }

        public void AimViewInputMove()
        {
            if (characterState.isOutOfControl || characterState.isStopped || characterState.isRiding)
                return;
            moveDir.z = ((KeyManager.Instance.GetKey(PlayerAction.MoveForward) ? 1 : 0) + (KeyManager.Instance.GetKey(PlayerAction.MoveBackward) ? -1 : 0));
            moveDir.x = ((KeyManager.Instance.GetKey(PlayerAction.MoveRight) ? 1 : 0) + (KeyManager.Instance.GetKey(PlayerAction.MoveLeft) ? -1 : 0));
        }

        public void InputJump()
        {
            if (characterState.isOutOfControl || characterState.isStopped || characterState.isRiding)
                return;
            if (KeyManager.Instance.GetKeyDown(PlayerAction.Jump))
            {
                if (!characterState.IsJumping)
                {
                    characterState.CheckJump();
                    if (characterState.IsJumping)
                    {
                        Vector3 horVel = playerRigidbody.velocity;
                        horVel.y = 0;
                        transform.LookAt(transform.position + horVel);
                        MakeinertiaVec(horVel.magnitude, horVel.normalized);
                        moveVec.y = jumpSpeed;
                        return;
                    }
                }

                if (!characterState.IsAirJumping)
                {
                    characterState.CheckAirJump();
                    if (characterState.IsAirJumping)
                    {
                        characterState.IsAirDashing = false;
                        Rotate();
                        if (characterState.isMove)
                        {
                            MakeinertiaVec(walkSpeed, moveDir);
                        }
                        else
                        {
                            Vector3 horVel = playerRigidbody.velocity;
                            horVel.y = 0;
                            MakeinertiaVec(walkSpeed, horVel.normalized);
                        }
                        moveVec.y = airJumpSpeed;
                    }
                }
            }
        }
        public void InputDash()
        {
            if (characterState.isOutOfControl || characterState.isStopped || characterState.isRiding || !characterState.CanJump)
                return;
            if (KeyManager.Instance.GetKeyDown(PlayerAction.Dash))
            {
                if (!characterState.IsDashing)
                {
                    characterState.CheckDash();
                }

                if (!characterState.WasAirDashing)
                {
                    characterState.CheckAirDash();
                    Rotate();
                    if (characterState.isMove)
                    {
                        MakeinertiaVec(walkSpeed, moveDir);
                    }
                    else
                    {
                        Vector3 forward = transform.forward;
                        forward.y = 0f;
                        forward = forward.normalized;
                        MakeinertiaVec(walkSpeed, forward);
                    }
                }
            }
        }

        public void TakeRotation()
        {
            if (characterState.isOutOfControl)
            {
                return;
            }

            if (characterState.top)
            {
                if(!playerMouse.notRotatoin)
                    RotateTop();
            }
            else if (characterState.aim)
            {
                RotateAim();
            }
            else
            {
                RotateSlerp();
            }
        }
        public void RotateTop()
        {
            if (!characterState.IsDashing && !characterState.IsAirDashing)
            {
                Vector3 forward = (playerMouse.point.transform.position - transform.position);
                forward.y = 0;
                transform.LookAt(transform.position + forward);
            }
        }

        public void Rotate()
        {
            transform.LookAt(transform.position + moveDir);
        }

        public void RotateSlerp()
        {

            if (characterState.IsGrounded && characterState.CanJump)
            {
                if (!characterState.IsDashing)
                {
                    Vector3 forward = Vector3.Slerp(transform.forward, moveDir.normalized, rotationSpeed * Time.fixedDeltaTime / Vector3.Angle(transform.forward, moveDir.normalized));
                    forward.y = 0;
                    moveDir = forward;
                    transform.LookAt(transform.position + forward);
                }
            }
        }

        public void RotateAim()
        {
            if (!characterState.IsDashing && !characterState.IsAirDashing)
            {
                Vector3 forward = mainCamera.transform.forward.normalized;
                forward.y = 0;
                transform.LookAt(transform.position + forward);
            }
        }

        private void Move()
        {
            if (characterState.isOutOfControl)
            {
                moveVec.y += gravity * Time.fixedDeltaTime;
                if (moveVec.y < terminalSpeed)
                {
                    moveVec.y = terminalSpeed;
                }

                if(characterState.isAirBlocked)
                {
                    playerRigidbody.velocity = Vector3.up * moveVec.y;
                }
                else
                {
                    playerRigidbody.velocity = moveVec;
                }
                playerRigidbody.velocity = moveVec;
                return;
            }

            if (!characterState.IsGrounded || !characterState.CanJump)
            {
                if (characterState.isAirBlocked)
                {
                    moveVec.y += gravity * Time.fixedDeltaTime;
                    if (moveVec.y < terminalSpeed)
                    {
                        moveVec.y = terminalSpeed;
                    }
                    playerRigidbody.velocity = Vector3.up * moveVec.y;
                    return;
                }

                if (characterState.IsAirDashing) // 공중대시 중일 때
                {
                    moveVec = transform.forward.normalized * airDashSpeed;
                }
                else
                {
                    moveVec = moveDir * airMoveSpeed + Vector3.up * (moveVec.y + gravity * Time.fixedDeltaTime);

                    if (!characterState.isOverAngleForSlope)
                    {
                        moveVec += inertiaNormalVec * inertiaSpeed;
                    }

                    if (moveVec.y < terminalSpeed)
                    {
                        moveVec.y = terminalSpeed;
                    }
                }
            }
            else
            {
                if (characterState.IsDashing) // 대시 중일 때
                {
                    moveSpeed = dashSpeed;
                    moveVec = transform.forward.normalized * dashSpeed;
                }
                else
                {
                    if (characterState.isRun) // 달릴 때
                    {
                        moveSpeed += runSpeed * Time.fixedDeltaTime * 20f;
                        if (moveSpeed > runSpeed)
                            moveSpeed = runSpeed;
                    }
                    else if (characterState.isMove)// 달리기 중이 아닐 때
                    {
                        moveSpeed += walkSpeed * Time.fixedDeltaTime * 20f;
                        if (moveSpeed > walkSpeed)
                            moveSpeed = walkSpeed;
                    }
                    else
                    {
                        moveSpeed -= runSpeed * Time.fixedDeltaTime * 20f;
                        if (moveSpeed < 0)
                            moveSpeed = 0f;
                    }

                    if (characterState.isFowardBlock)
                    {
                        moveVec = Vector3.zero;
                    }
                    else
                    {
                        moveVec = moveDir * moveSpeed;
                    }
                }

                if (characterState.isMove) // 내리막길 이동시 경사각에 따른 수직속도 보정값
                    moveVec.y = characterState.slopeAngleCofacter * moveSpeed;

                if (characterState.height >= characterState.groundCheckThresholdMin)
                    moveVec += Vector3.up * (gravity * gravityCofactor * Time.fixedDeltaTime);
            }

            playerRigidbody.velocity = moveVec;
        }

        public void MakeKnockBackVec(Vector3 horVec, float rushSpeed)
        {
            knockBackVec = horVec.normalized * rushSpeed * 1.3f + Vector3.up * knockBackVerticalSpeed;
        }

        public void StartKnockBack()
        {
            InitController();
            moveVec = knockBackVec;
            characterState.IsJumping = true;
            characterState.isOutOfControl = true;
        }
        public IEnumerator DelayResetKnockBack()
        {
            characterState.CanResetKnockBack = false;
            yield return new WaitForSeconds(0.2f);
            characterState.CanResetKnockBack = true;
        }

        public void EndKnockBack()
        {
            characterState.isOutOfControl = false;
        }

        public void StartAttacked()
        {
            characterState.isOutOfControl = true;
        }

        public void EndAttacked()
        {
            characterState.isOutOfControl = false;
        }

        public void StartDeath()
        {
            characterState.isStopped = true;
            playerCapsuleCollider.enabled = false;
        }

        public void EndDeath()
        {
            playerCapsuleCollider.enabled = true;
            characterState.isStopped = false;
            Resurrect();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!playerAnimator.GetBool("isAttacked") && !playerAnimator.GetBool("isKnockBack") && !playerAnimator.GetBool("isDead"))
            {
                switch (other.tag)
                {
                    case "MonsterAttack":
                        {
                            GameManager.Instance.curPlayerHP -= other.GetComponentInParent<DefenseMonster>().attackDamage;
                            if (GameManager.Instance.curPlayerHP < 0)
                            {
                                GameManager.Instance.curPlayerHP = 0;
                                playerAnimator.SetBool("DeadTrigger", true);
                            }
                            else
                            {
                                GetComponent<Animator>().SetBool("AttackedTrigger", true);
                            }
                        }
                        break;
                    case "MonsterRush":
                        {
                            GameManager.Instance.curPlayerHP -= other.GetComponentInParent<TrippleHeadSnake>().rushDamage;
                            if (GameManager.Instance.curPlayerHP < 0)
                            {
                                GameManager.Instance.curPlayerHP = 0;
                                playerAnimator.SetBool("DeadTrigger", true);
                            }
                            else
                            {
                                Vector3 knockBackHorVec = (transform.position - other.transform.position);
                                knockBackHorVec.y = 0;
                                transform.LookAt(transform.position - knockBackHorVec);
                                MakeKnockBackVec(knockBackHorVec, other.GetComponentInParent<TrippleHeadSnake>().rushSpeed);
                                StartCoroutine(nameof(DelayResetKnockBack));
                                playerAnimator.SetBool("KnockBackTrigger", true);
                            }
                        }
                        break;
                    case "DeadZone":
                        {
                            GameManager.Instance.curPlayerHP = 0;
                            playerAnimator.SetBool("DeadTrigger", true);
                        }
                        break;
                }
            }
        }
    }
}
