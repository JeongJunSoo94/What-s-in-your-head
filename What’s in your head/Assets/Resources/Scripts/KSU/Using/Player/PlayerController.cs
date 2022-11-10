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
using KSU.AutoAim.Object.Monster;
using JCW.Object.Stage1;

namespace KSU
{
    public enum DamageType { Attacked, KnockBack, Dead };
    public class PlayerController : MonoBehaviour
    {
        //  Scripts Components
        #region    
        public PlayerState characterState;
        public PlayerMouseController playerMouse;
        #endregion

        // ����Ƽ ���� Components
        #region
        public Animator playerAnimator;
        CapsuleCollider playerCapsuleCollider;
        public Camera mainCamera;
        public Rigidbody playerRigidbody;
        //[Header("Ű ����")] [SerializeField] private GameObject UI_BG;    
        public PhotonView photonView;
        #endregion

        // ���� Speed
        #region
        [Header("������")]
        [Header("���� �̵� �ӷ�")]
        float moveSpeed = 0f;
        [Header("�ȴ� �ӷ�")]
        public float walkSpeed = 4f;
        [Header("�޸��� �ӷ�")]
        public float runSpeed = 7f;
        [Header("��� �ӷ�")]
        public float dashSpeed = 10f;
        [Header("���� ��� �ӷ�")]
        public float airDashSpeed = 8f;
        [Header("���� �ӷ�")]
        public float inertiaSpeed = 0f;
        [Header("���� �̵� �ӷ�")]
        public float airMoveSpeed = 1f;
        [Header("���� �̵� �ִ� �ӷ�")]
        public float airMoveMaxSpeed = 10f;
        //[Tooltip("�˹� ���� �ӷ�")]
        //public float knockBackHorizonSpeed = 6f;
        #endregion

        // ���� Speed
        #region
        [Tooltip("���� �ӵ�")]
        public float jumpSpeed = 10f;
        [Tooltip("���� ���� �ӵ�")]
        public float airJumpSpeed = 6f;
        [Range(-100f, 0f), Tooltip("�߷�")]
        public float gravity = -9.81f;
        public float gravityCofactor = 0.8f;
        [Range(-100f, -1f), Tooltip("���ܼӵ�")]
        public float terminalSpeed = -10f;
        [Tooltip("�˹� ���� �ӷ�")]
        public float knockBackVerticalSpeed = 8f;
        #endregion

        // ȸ�� Speed
        #region
        [Tooltip("ĳ���� �̵��� ȸ�� �ӵ�")]
        public float rotationSpeed = 720f;
        #endregion

        // �ӵ� ���� ����
        #region
        [Tooltip("���� �̵� ����")]
        public Vector3 moveVec = Vector3.zero; // rigidbody.velocity�� ������ ���� �ӵ� ����
        [Tooltip("Ű�Է¿� ���� ���� ����")]
        public Vector3 moveDir = Vector3.zero; // Ű �Է¿� ���� ���� �ӵ� ����
        [Tooltip("��� ����")]
        public Vector3 dashVec = Vector3.zero; // dash�Ǵ� airdash�߿� ����� ����
        [Tooltip("���� �븻 ����")]
        public Vector3 inertiaNormalVec = Vector3.zero; // ���� ���¿� ����� ���� ����
        [Tooltip("�˹� ����")]
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

            mainCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // ��Ƽ��
            if (mainCamera == null)
                Debug.Log("ī�޶� NULL");

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
        void EscapeInteraction()
        {
            GetComponent<PlayerInteraction>().EscapeInteraction();
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

        public void MakeinertiaVec(float speed, Vector3 nomalVec) // ���� ���� �� ����� ��������
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
            if (photonView.IsMine)
            { 
                GameManager.Instance.curPlayerHP = 12;
            }

            characterState.InitState(true, false);

            string path = Application.dataPath + "/Resources/CheckPointInfo/Stage" + GameManager.Instance.curStageIndex
                + "/" + GameManager.Instance.curStageType + "/Section" + GameManager.Instance.curSection + ".json";
            if (!File.Exists(path))
            {
                Debug.Log("üũ����Ʈ �ҷ����� ���� / �ҷ����� �ߴ� ���� : " + GameManager.Instance.curSection);
                return;
            }

            string jsonString = File.ReadAllText(path);

            SavePosition.PlayerInfo data = JsonUtility.FromJson<SavePosition.PlayerInfo>(jsonString);
            transform.SetPositionAndRotation(new Vector3((float)data.position[0], (float)data.position[1], (float)data.position[2]), new Quaternion((float)data.rotation[0], (float)data.rotation[1], (float)data.rotation[2], (float)data.rotation[3]));
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

        void InputCustomJump(float customJumpSpeed)
        {
            if (characterState.isOutOfControl || characterState.isStopped || characterState.isRiding)
                return;

            characterState.SetCustomJumpState();
            Vector3 horVel = moveDir;
            horVel.y = 0;
            transform.LookAt(transform.position + horVel);
            MakeinertiaVec(0, horVel.normalized);
            moveVec.y = customJumpSpeed;
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

                if (characterState.isAirBlocked)
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

                if (characterState.IsAirDashing) // ���ߴ�� ���� ��
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
                    Vector3 horVec = moveVec;
                    horVec.y = 0;
                    if (horVec.magnitude > airMoveMaxSpeed)
                        moveVec = horVec.normalized * airMoveMaxSpeed + Vector3.up * moveVec.y;

                    if (moveVec.y < terminalSpeed)
                    {
                        moveVec.y = terminalSpeed;
                    }
                }
            }
            else
            {
                if (characterState.IsDashing) // ��� ���� ��
                {
                    moveSpeed = dashSpeed;
                    moveVec = transform.forward.normalized * dashSpeed;
                }
                else
                {
                    if (characterState.isRun) // �޸� ��
                    {
                        moveSpeed += runSpeed * Time.fixedDeltaTime * 20f;
                        if (moveSpeed > runSpeed)
                            moveSpeed = runSpeed;
                    }
                    else if (characterState.isMove)// �޸��� ���� �ƴ� ��
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

                if (characterState.isMove) // �������� �̵��� ��簢�� ���� �����ӵ� ������
                    moveVec.y = characterState.slopeAngleCofacter * moveSpeed;

                if (characterState.height >= characterState.groundCheckThresholdMin)
                    moveVec.y = playerRigidbody.velocity.y + gravity * Time.fixedDeltaTime;
            }

            playerRigidbody.velocity = moveVec;
        }

        public void SetOnCollider()
        {
            playerCapsuleCollider.enabled = true;
        }

        public void SetOffCollider()
        {
            playerCapsuleCollider.enabled = false;
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
            InitInteraction();
            EscapeInteraction();
        }

        public void EndDeath()
        {
            playerCapsuleCollider.enabled = true;
            characterState.isStopped = false;
            Resurrect();
        }

        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "MonsterAttack":
                    {
                        GetDamage(other.GetComponentInParent<DefenseMonster>().attackDamage, DamageType.Attacked);
                    }
                    break;
                case "MonsterRush":
                    {
                        GetDamage(other.GetComponentInParent<TrippleHeadSnake>().rushDamage, DamageType.KnockBack, other.transform.position, other.GetComponentInParent<TrippleHeadSnake>().rushSpeed);
                    }
                    break;
                case "DeadZone":
                    {
                        GetDamage(12, DamageType.Dead);
                    }
                    break;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Trampolin":
                    {
                        InputCustomJump(collision.gameObject.GetComponentInParent<JumpingPlatform>().jumpSpeed);
                    }
                    break;
                case "DeadZone":
                    {
                        GetDamage(12, DamageType.Dead);
                    }
                    break;
            }
        }
        [PunRPC]
        public void GetDamage(int damage, DamageType type, Vector3 colliderPos, float knockBackSpeed)
        {
            string damageTirgger = "DeadTrigger";
            switch(type)
            {
                case DamageType.Attacked:
                    damageTirgger = "AttackedTrigger";
                    break;
                case DamageType.KnockBack:
                    damageTirgger = "KnockBackTrigger";
                    break;
                case DamageType.Dead:
                    damageTirgger = "DeadTrigger";
                    break;
            }

            GameManager.Instance.curPlayerHP -= damage;
            if (GameManager.Instance.curPlayerHP < 0)
            {
                GameManager.Instance.curPlayerHP = 0;
                playerAnimator.SetBool("DeadTrigger", true);
            }
            else
            {
                if(damageTirgger == "KnockBackTrigger")
                {
                    Vector3 knockBackHorVec = (transform.position - colliderPos);
                    knockBackHorVec.y = 0;
                    transform.LookAt(transform.position - knockBackHorVec);
                    MakeKnockBackVec(knockBackHorVec, knockBackSpeed);
                    StartCoroutine(nameof(DelayResetKnockBack));
                }

                playerAnimator.SetBool(damageTirgger, true);
            }
        }
        
        [PunRPC]
        public void GetDamage(int damage, DamageType type)
        {
            if (!photonView.IsMine)
                return;
            string damageTirgger = "DeadTrigger";
            switch (type)
            {
                case DamageType.Attacked:
                    damageTirgger = "AttackedTrigger";
                    break;
                case DamageType.KnockBack:
                    damageTirgger = "KnockBackTrigger";
                    break;
                case DamageType.Dead:
                    damageTirgger = "DeadTrigger";
                    break;
            }
            if (!playerAnimator.GetBool("isAttacked") && !playerAnimator.GetBool("isKnockBack") && !playerAnimator.GetBool("isDead"))
            {
                GameManager.Instance.curPlayerHP -= damage;
                if (GameManager.Instance.curPlayerHP <= 0)
                {
                    GameManager.Instance.curPlayerHP = 0;
                    //playerAnimator.SetBool("DeadTrigger", true);
                    photonView.RPC(nameof(SetAnimatorBool), RpcTarget.AllViaServer, "DeadTrigger", true);
                }
                else
                {
                    photonView.RPC(nameof(SetAnimatorBool), RpcTarget.AllViaServer, damageTirgger, true);
                    //playerAnimator.SetBool(damageTirgger, true);
                }
            }
        }

        [PunRPC]
        public void SetAnimatorBool(string name, bool enable)
        {
            playerAnimator.SetBool(name, enable);
        }
    }
}
