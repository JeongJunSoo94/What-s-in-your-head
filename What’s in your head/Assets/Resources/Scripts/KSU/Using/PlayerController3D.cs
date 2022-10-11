using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using JCW.Options.InputBindings;
using Photon.Pun;
using JCW.AudioCtrl;
using Cinemachine;

using YC.Camera_;
using YC.Camera_Single;

public class PlayerController3D : MonoBehaviour
{
    //  Scripts Components
    #region    
    public CharacterState3D characterState;
    public PlayerMouseController playerMouse;
    #endregion

    // ����Ƽ ���� Components
    #region
    //Animator _animator;
    CapsuleCollider _capsuleCollider;
    Camera _camera;
    Rigidbody _rigidbody;
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
    public int CPcount = 0;
    private int life = 3;


    void Awake()
    {
        // >> : YC
        //Cursor.lockState = CursorLockMode.Locked;
        // << :

        characterState = GetComponent<CharacterState3D>();
        //_animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        playerMouse = GetComponent<PlayerMouseController>();

        // >> :
        //_camera = Camera.main;
        // << :
    }
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        // >> : YC - ī�޶� ������ CameraController.cs���� �ϰڽ��ϴ�. �Ʒ� �ڵ� ����� ��Ȱ ȭ���� �Ұ����մϴ�.
        //if (!photonView.IsMine)
        //{
        //    GetComponentInChildren<Camera>().gameObject.SetActive(false);
        //    GetComponentInChildren<CinemachineFreeLook>().gameObject.SetActive(false);
        //    Destroy(this);
        //}
        //else
        //    _camera = Camera.main;

        _camera = this.gameObject.GetComponent<CameraController_Single>().mainCam; // �̱ۿ�
        //_camera = this.gameObject.GetComponent<CameraController>().mainCam; // ��Ƽ��

        if (!photonView.IsMine) Destroy(this);
        // << : 


        if (WIYH_Manager.Instance.player1 == null)
            WIYH_Manager.Instance.player1 = this.gameObject;
        else if (WIYH_Manager.Instance.player2 == null)
            WIYH_Manager.Instance.player2 = this.gameObject;
        ITT_KeyManager.Instance.GetKeyDown(PlayerAction.MoveBackward);
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
            return;
        CheckState();
        //CheckKeyInput(); // �̰� animator��  fsm���� �Ѵٰ� ������ ���⿡ ��Ƽ� ����ص�(fsm���� �̵� �� �͵�)
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;
        TakeRotation();
        Move();
    }
    private void CheckState()
    {
        if (characterState.IsGrounded)
        {
            characterState.CheckGround(_capsuleCollider.radius);
            if (!characterState.IsGrounded)
            {
                Vector3 horVel = _rigidbody.velocity;
                horVel.y = 0;
                MakeinertiaVec(horVel.magnitude, moveDir);
            }
        }
        else
        {
            characterState.CheckGround(_capsuleCollider.radius);
        }
        characterState.CheckMove(_rigidbody.velocity);
    }

    private void MakeinertiaVec(float speed, Vector3 nomalVec) // ���� ���� �� ����� ��������
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
        if (ITT_KeyManager.Instance.GetKeyDown(PlayerAction.ToggleRun))
        {
            characterState.ToggleRun();
        }
    }
    void Resurrect()
    {
        if (!File.Exists(Application.dataPath + "/Resources/CheckPointInfo/" + this.name + "TF" + (CPcount - 1).ToString() + ".json"))
        {
            Debug.Log("üũ����Ʈ �ҷ����� ����");
            return;
        }

        string jsonString = File.ReadAllText(Application.dataPath + "/Resources/CheckPointInfo/" + this.name + "TF" + (CPcount - 1).ToString() + ".json");
        Debug.Log(jsonString);

        SavePosition.PlayerInfo data = JsonUtility.FromJson<SavePosition.PlayerInfo>(jsonString);
        transform.SetPositionAndRotation(new Vector3((float)data.position[0], (float)data.position[1], (float)data.position[2]), new Quaternion((float)data.rotation[0], (float)data.rotation[1], (float)data.rotation[2], (float)data.rotation[3]));
    }

    public void InputSoundTest()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("���� �÷��̾� ��� : " + --life);
            Resurrect();
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SoundManager.instance.PlayBGM_RPC("POP");
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SoundManager.instance.PlayBGM_RPC("Tomboy");
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SoundManager.instance.PauseResumeBGM_RPC();
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            SoundManager.instance.PlayEffect_RPC("Explosion");
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            //SoundManager.instance.PlayEffect_RPC(SoundManager.instance.GetEffectClips("Fireball"));
            SoundManager.instance.PlayEffect_RPC("Fireball");
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            SoundManager.instance.PlayEffect_RPC("GetItem");
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            SoundManager.instance.PlayEffect_RPC("WaterBall");
        }
    }

    public void InputMove()
    {
        moveDir =
          _camera.transform.forward * ((ITT_KeyManager.Instance.GetKey(PlayerAction.MoveForward) ? 1 : 0) + (ITT_KeyManager.Instance.GetKey(PlayerAction.MoveBackward) ? -1 : 0))
        + _camera.transform.right * ((ITT_KeyManager.Instance.GetKey(PlayerAction.MoveRight) ? 1 : 0) + (ITT_KeyManager.Instance.GetKey(PlayerAction.MoveLeft) ? -1 : 0));
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
        moveDir.z = ((ITT_KeyManager.Instance.GetKey(PlayerAction.MoveForward) ? 1 : 0) + (ITT_KeyManager.Instance.GetKey(PlayerAction.MoveBackward) ? -1 : 0));
        moveDir.x = ((ITT_KeyManager.Instance.GetKey(PlayerAction.MoveRight) ? 1 : 0) + (ITT_KeyManager.Instance.GetKey(PlayerAction.MoveLeft) ? -1 : 0));
    }
    public void InputJump()
    {
        if (ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Jump))
        {
            if (!characterState.IsJumping)
            {
                characterState.CheckJump();
                if (characterState.IsJumping)
                {
                    Vector3 horVel = _rigidbody.velocity;
                    horVel.y = 0;
                    MakeinertiaVec(horVel.magnitude, moveDir);
                    moveVec.y = jumpSpeed;
                    return;
                }
            }

            if (!characterState.IsAirJumping)
            {
                characterState.CheckAirJump();
                if (characterState.IsAirJumping)
                {
                    Rotate();
                    if(characterState.isMove)
                    {
                        MakeinertiaVec(walkSpeed, moveDir);
                    }
                    else
                    {
                        Vector3 horVel = _rigidbody.velocity;
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
        if (ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Dash))
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

    void  TakeRotation()
    {
        if (characterState.aim)
        {
            RotateAim();
        }
        else
        {
            RotateSlerp();
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
            Vector3 forward = _camera.transform.forward.normalized;
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
                moveVec = moveDir * airMoveSpeed + inertiaNormalVec * inertiaSpeed + Vector3.up * (moveVec.y + gravity * Time.fixedDeltaTime);

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

                moveVec = moveDir * moveSpeed;
            }

            if (characterState.isMove) // �������� �̵��� ��簢�� ���� �����ӵ� ������
                moveVec.y = characterState.slopeAngleCofacter * moveSpeed;

            if (characterState.height >= characterState.groundCheckThresholdMin)
                moveVec += Vector3.up * (gravity * gravityCofactor * Time.fixedDeltaTime);
        }

        _rigidbody.velocity = moveVec;

       
    }
}
