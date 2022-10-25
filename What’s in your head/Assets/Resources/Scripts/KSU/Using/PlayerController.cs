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


namespace KSU
{
    public class PlayerController : MonoBehaviour
    {
        //  Scripts Components
        #region    
        public PlayerState characterState;
        public PlayerMouseController playerMouse;
        #endregion

        // ����Ƽ ���� Components
        #region
        //Animator _animator;
        CapsuleCollider playerCapsuleCollider;
        public Camera mainCamera;
        Rigidbody playerRigidbody;
        //[Header("Ű ����")] [SerializeField] private GameObject UI_BG;    
        PhotonView photonView;
        #endregion

        // ���� Speed
        #region
        [Header("������")]
        [Tooltip("���� �̵� �ӷ�")]
        public float moveSpeed = 0f;
        [Tooltip("�ȴ� �ӷ�")]
        public float walkSpeed = 4f;
        [Tooltip("�޸��� �ӷ�")]
        public float runSpeed = 7f;
        [Tooltip("��� �ӷ�")]
        public float dashSpeed = 10f;
        [Tooltip("���� ��� �ӷ�")]
        public float airDashSpeed = 8f;
        [Tooltip("���� �ӷ�")]
        public float inertiaSpeed = 0f;
        [Tooltip("���� �̵� �ӷ�")]
        public float airMoveSpeed = 1f;
        #endregion

        // ���� Speed
        #region
        [Tooltip("���� �ӵ�")]
        public float jumpSpeed = 10f;
        [Tooltip("���� ���� �ӵ�")]
        public float airJumpSpeed = 6f;
        [Range(-20f, 0f), Tooltip("�߷�")]
        public float gravity = -9.81f;
        public float gravityCofactor = 0.8f;
        [Range(-20f, -1f), Tooltip("���ܼӵ�")]
        public float terminalSpeed = -10f;
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
        #endregion

        // üũī��Ʈ - JCW

        void Awake()
        {
            // >> : YC
            Cursor.lockState = CursorLockMode.Locked;
            Application.targetFrameRate = 120;
            // << :

            characterState = GetComponent<PlayerState>();
            //_animator = GetComponent<Animator>();
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
                mainCamera.GetComponent<AudioListener>().enabled = false;
            }
            else if (GameManager.Instance.isTest)
            {
                GameManager.Instance.characterOwner.Add(PhotonNetwork.IsMasterClient, gameObject.name.Contains("Nella"));
                GameManager.Instance.isAlive.Add(GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient], true);
            }
            // << : 

            Application.targetFrameRate = 120;
            KeyManager.Instance.GetKeyDown(PlayerAction.MoveBackward);
        }

        // Update is called once per frame
        void Update()
        {
            CheckState();
            //if (!photonView.IsMine)
            //    return;
            //CheckKeyInput(); // �̰� animator��  fsm���� �Ѵٰ� ������ ���⿡ ��Ƽ� ����ص�(fsm���� �̵� �� �͵�)
        }

        private void FixedUpdate()
        {
            if (!photonView.IsMine)
            {
                if(transform.parent != null)
                    //playerRigidbody.velocity = Vector3.zero;
                return;
            }else if(characterState.isOutOfControl)
            {
                return;
            }
            TakeRotation();
            Move();
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

        public void MakeinertiaVec(float speed, Vector3 nomalVec) // ���� ���� �� ����� ��������
        {
            inertiaSpeed = speed;
            inertiaNormalVec = nomalVec;
        }
        private void CheckKeyInput()
        {
            InputRun();
            InputMove();
            InputJump();
            InputDash();
            InputSoundTest();
        }



        public void InputRun()
        {
            if (KeyManager.Instance.GetKeyDown(PlayerAction.ToggleRun))
            {
                characterState.ToggleRun();
            }
        }
        public void Resurrect()
        {
            if (!File.Exists(Application.dataPath + "/Resources/CheckPointInfo/Stage" +
                GameManager.Instance.curStageIndex + "/Section" + GameManager.Instance.curSection + ".json"))
            {
                Debug.Log(GameManager.Instance.curSection);
                Debug.Log("üũ����Ʈ �ҷ����� ����");
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
        }

        public void AimViewInputMove()
        {
            moveDir.z = ((KeyManager.Instance.GetKey(PlayerAction.MoveForward) ? 1 : 0) + (KeyManager.Instance.GetKey(PlayerAction.MoveBackward) ? -1 : 0));
            moveDir.x = ((KeyManager.Instance.GetKey(PlayerAction.MoveRight) ? 1 : 0) + (KeyManager.Instance.GetKey(PlayerAction.MoveLeft) ? -1 : 0));
        }

        public void InputJump()
        {
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
            if (characterState.top)
            {
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
            if (!characterState.IsGrounded || !characterState.CanJump)
            {
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
                    moveVec += Vector3.up * (gravity * gravityCofactor * Time.fixedDeltaTime);
            }

            playerRigidbody.velocity = moveVec;


        }

    }
}
