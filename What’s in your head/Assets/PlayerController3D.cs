using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController3D : MonoBehaviour
{
    //  Scripts Components
    #region
    CharacterState3D characterState;
    #endregion

    // ����Ƽ ���� Components
    #region
    Animator _animator;
    CapsuleCollider _capsuleCollider;
    Camera _camera;
    Rigidbody _rigidbody;
    #endregion

    // ���� Speed
    #region
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
    [Tooltip("���� ���� �ӷ�")]
    public float airDragSpeed = 1f;
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
    public float curGravity; //���� �̵��̳� ���� ��� �߿��� 0���� ������ְ� Ư���� ��쿣 �߷°��� ��ȭ�� �� �ֱ� ������
    [Range(-20f, -1f), Tooltip("���ܼӵ�")]
    public float terminalSpeed;
    #endregion

    // ȸ�� Speed
    #region
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
        CheckKeyInput(); // �̰� animator��  fsm���� �Ѵٰ� ������ ���⿡ ��Ƽ� ����ص�(fsm���� �̵� �� �͵�)
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

    private void MakeinertiaVec() // ���� ���� �� ����� ��������
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
            if (characterState.IsAirDashing) // ���ߴ�� ���� ��
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
            if (characterState.IsDashing) // ��� ���� ��
            {
                moveSpeed = dashSpeed;
                moveVec = transform.forward.normalized * dashSpeed;
            }
            else
            {
                if (characterState.isRun) // �޸� ��
                {
                    moveSpeed = runSpeed;
                }
                else // �޸��� ���� �ƴ� ��
                {
                    moveSpeed = walkSpeed;
                }

                moveVec = moveDir * moveSpeed;
            }

            if (characterState.isMove && characterState.slopeAngle < 0) // �������� �̵��� ��簢�� ���� �����ӵ� ������
                moveVec.y = characterState.slopeAngleCofacter * moveSpeed;
        }


        _rigidbody.velocity = moveVec;
    }
}
