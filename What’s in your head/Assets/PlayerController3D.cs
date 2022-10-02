using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController3D : MonoBehaviour
{
    //  Scripts Components
    #region
    CharacterState3D characterState;
    #endregion

    // 유니티 제공 Components
    #region
    Animator _animator;
    CapsuleCollider _capsuleCollider;
    Camera _camera;
    Rigidbody _rigidbody;
    #endregion

    // 수평 Speed
    #region
    [Tooltip("현재 이동 속력")]
    public float moveSpeed = 0f;
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
    [Tooltip("공기 저항 속력")]
    public float airDragSpeed = 1f;
    [Tooltip("공중 이동 속력")]
    public float airMoveSpeed = 1f;
    #endregion

    // 수직 Speed
    #region
    [Tooltip("점프 속도")]
    public float jumpSpeed = 10f;
    [Tooltip("공중 점프 속도")]
    public float airJumpSpeed = 6f;
    [Range(-20f, 0f), Tooltip("중력")]
    public float gravity = -9.81f;
    public float curGravity; //지면 이동이나 공중 대시 중에는 0으로 만들어주고 특수한 경우엔 중력값이 변화할 수 있기 때문에
    [Range(-20f, -1f), Tooltip("종단속도")]
    public float terminalSpeed;
    #endregion

    // 회전 Speed
    #region
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
    #endregion


    void Awake()
    {
        characterState = GetComponent<CharacterState3D>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _camera = Camera.main;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
        CheckKeyInput(); // 이건 animator의  fsm으로 한다고 했으나 여기에 모아서 사용해둠(fsm으로 이동 될 것들)
    }

    private void FixedUpdate()
    {
        Rotation();
        Move();
    }
    private void CheckState() 
    {
        if(characterState.IsGrounded)
        {
            characterState.CheckGround(_capsuleCollider.radius);
            if(!characterState.IsGrounded)
            {
                MakeinertiaVec();
            }
        }
        else
        {
            characterState.CheckGround(_capsuleCollider.radius);
        }
        characterState.CheckMove(_rigidbody.velocity);
    }

    private void MakeinertiaVec() // 공중 진입 시 생기는 관성벡터
    {
        inertiaSpeed = moveSpeed;
        inertiaNormalVec = moveDir.normalized;
    }
    private void CheckKeyInput()
    {
        InputRun();
        InputMove();
        InputJump();
        InputDash();
    }

    public void InputRun()
    {
        if (ItTakesTwoKeyManager.Instance.GetKeyDown(KeyName.CapsLock))
        {
            characterState.ToggleRun();
        }
    }

    public void InputMove()
    {
        moveDir = 
          _camera.transform.forward * ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.W) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.S) ? -1 : 0))
        + _camera.transform.right * ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.D) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.A) ? -1 : 0));
        moveDir.y = 0;
        moveDir = moveDir.normalized;

        if (moveDir.magnitude > 0)
            characterState.isMove = true;
        else
            characterState.isMove = false;
    }
    public void TopViewInputMove()
    {
        moveDir.z = ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.W) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.S) ? -1 : 0));
        moveDir.x = ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.D) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.A) ? -1 : 0));
    }
    public void InputJump()
    {
        if (ItTakesTwoKeyManager.Instance.GetKeyDown(KeyName.Space))
        {
            Debug.Log("Space Down");
            if(characterState.IsGrounded)
            {
                Debug.Log("Check IsGrounded");
                if (!characterState.IsJumping)
                {
                    Debug.Log("Check not IsJumping");
                    characterState.CheckJump();
                    if (characterState.IsJumping)
                    {
                        Debug.Log("Get Jump Power");
                        moveVec.y = jumpSpeed;
                        return;
                    }
                }
            }

            if(!characterState.IsGrounded && !characterState.IsAirJumping)
            {
                characterState.CheckAirJump();
                if (characterState.IsAirJumping)
                {
                    moveVec.y = airJumpSpeed;
                }
            }
        }
    }
    public void InputDash()
    {
        if (ItTakesTwoKeyManager.Instance.GetKeyDown(KeyName.LeftShift))
        {
            if (characterState.IsGrounded && !characterState.IsDashing)
            {
                characterState.CheckDash();
                if (characterState.IsDashing)
                {
                    moveVec.y = jumpSpeed;
                }
            }

            if (!characterState.IsGrounded && !characterState.IsAirDashing)
            {
                characterState.CheckAirDash();
                if (characterState.IsAirDashing)
                {
                    moveVec.y = airJumpSpeed;
                }
            }
        }
    }

    public void Rotation()
    {
        Vector3 forward = Vector3.Slerp(transform.forward, moveDir.normalized, rotationSpeed * Time.fixedDeltaTime / Vector3.Angle(transform.forward, moveDir.normalized));
        forward.y = 0;
        moveDir = forward;
        transform.LookAt(transform.position + forward);
    }

    private void Move()
    {
        if (!characterState.IsGrounded || characterState.IsJumping || !characterState.CanJump)
        {
            if (characterState.IsAirDashing) // 공중대시 중일 때
            {
                moveVec = transform.forward.normalized * airDashSpeed;
            }
            else
            {
                inertiaSpeed -= airDragSpeed * Time.fixedDeltaTime;
                if (inertiaSpeed < 0)
                    inertiaSpeed = 0;

                moveVec = moveDir * airMoveSpeed + inertiaNormalVec * inertiaSpeed + Vector3.up * (moveVec.y + gravity * Time.fixedDeltaTime);
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
                    moveSpeed = runSpeed;
                }
                else // 달리기 중이 아닐 때
                {
                    moveSpeed = walkSpeed;
                }

                moveVec = moveDir * moveSpeed;
            }

            if (characterState.isMove && characterState.slopeAngle < 0) // 내리막길 이동시 경사각에 따른 수직속도 보정값
                moveVec.y = characterState.slopeAngleCofacter * moveSpeed;
        }


        _rigidbody.velocity = moveVec;
    }
}
