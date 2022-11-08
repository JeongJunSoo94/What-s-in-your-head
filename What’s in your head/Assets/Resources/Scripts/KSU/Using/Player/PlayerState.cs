using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    

    //���� üũ ����
    #region
    [Tooltip("�������� üũ�� ���̾� ����")]
    public LayerMask groundLayerMask;
    [Range(0.1f, 10.0f), Tooltip("���� ���� �Ÿ�")]
    public float groundCheckDistance = 2.0f;
    [Range(0.0f, 1f), Tooltip("���� �ν� ��� �ּ� �Ÿ�")]
    public float groundCheckThresholdMin = 0.02f;
    [Range(0.0f, 1f), Tooltip("���� �ν� ��� �ִ� �Ÿ�")]
    public float groundCheckThresholdMax = 0.2f;
    [Range(0.0f, 1f), Tooltip("���� ���� �ּ� ����")]
    public float forwardblockingMinHeight = 0.2f;

    //bool isGroundDelayOn = false;
    //public float groundDelayTime = 0.05f;
    public float height = 0f;
    public float forwardHeight = 0f;

    public bool RayCheck;
    //



    RaycastHit groundRaycastHit;
    RaycastHit fowardRaycastHit;
    [Tooltip("ĳ���Ͱ� �ٶ󺸴� ������ ��簢")]
    public float slopeAngle;
    [Tooltip("slopeAngle��ŭ�� Y�� ���� ����")]
    public float slopeAngleCofacter;

    [Tooltip("���� �� ����")]
    public bool IsGrounded = false;
    [Tooltip("���� ���� ����")]
    public bool isFowardBlock = false;
    [Tooltip("���� ���� ����")]
    public bool isAirBlocked = false;
    //[Tooltip("���� ���� ������Ʈ ��")]
    //public int airBlockNum = 0;
    [Tooltip("�ִ� ��簢 �ʰ� ����")]
    public bool isOverAngleForSlope = false;
    #endregion

    // ���� �̵� ����
    #region
    [Tooltip("����Ű �Է� ����")]
    public bool isMove = false;
    [Tooltip("�޸��� ��� ON/OFF ����")]
    public bool isRun = false;
    #endregion

    // ���� ����
    #region
    [Tooltip("���� ���� ������ �� TRUE")]
    public bool CanJump = true;
    [Tooltip("��� �� ��� ���� �Ұ�")]
    public bool isDelayingDashJump = false;
    [Tooltip("���鿡�� ������ ����")]
    public bool IsJumping = false;
    [Tooltip("���߿��� ������ ����")]
    public bool IsAirJumping = false;
    [Tooltip("Ư�� ����(Ʈ���޸�)�� ����")]
    public bool isCumstomJumping = false;
    #endregion

    // ��� ����
    #region
    [Tooltip("���鿡�� ����� ����")]
    public bool IsDashing = false;
    [Tooltip("���߿��� ����� ����")]
    public bool IsAirDashing = false;
    [Tooltip("���߿��� ������ ����ߴ� ����")]
    public bool WasAirDashing = false;
    
    #endregion

    //����Ұ��� ���� ����
    #region
    public bool isMine = false;
    public bool isOutOfControl = false;
    public bool isStopped = false;
    public bool isRiding = false;
    #endregion

    public bool CanResetKnockBack = true;

    public bool aim = false;
    public bool top = false;
    public bool swap = false;

    [Header("_______���� ���� ��_______")]
    [Header("ĳ���Ͱ� Ÿ�� �ö� �� �ִ� �ִ� ��簢")]
    public float maxSlopeAngle = 40f;
    [Header("���� ��Ÿ��")]
    public float jumpCoolTime = 0.2f;
    [Header("���� ��� ���� �ð�")]
    public float dashTime = 1f;
    [Header("���� ��� ���� �ð�")]
    public float airDashTime = 0.5f;

    public void InitState(bool initMove, bool initTop)
    {
        aim = false;
        swap = false;
        CanResetKnockBack = true;

        if (initMove)
        {
            isFowardBlock = false;
            isOverAngleForSlope = false;
            isMove = false;
            isRun = false;
            CanJump = true;
            IsJumping = false;
            IsAirJumping = false;
            IsDashing = false;
            IsAirDashing = false;
            WasAirDashing = false;
            IsGrounded = false;
            isRiding = false;
        }
        if (initTop)
        {
            top = false;
        }
    }



    // ���� üũ �Լ�(���� ������ ���� ����üũ �Ÿ� �ȿ� �ȵ��� �� �ֱ⿡ ���� �ʿ�)
    #region
    public void CheckGround(float sphereRadius)
    {
        RayCheck = Physics.SphereCast(transform.position + Vector3.up * (sphereRadius * 1.5f + Physics.defaultContactOffset), (sphereRadius - Physics.defaultContactOffset), Vector3.down, out groundRaycastHit, groundCheckDistance + sphereRadius * 0.5f + Physics.defaultContactOffset, groundLayerMask, QueryTriggerInteraction.Ignore);
        
        if (RayCheck)
        {
            slopeAngle = Vector3.Angle(Vector3.up, groundRaycastHit.normal);
            if (Mathf.Abs(slopeAngle) >= maxSlopeAngle)
            {
                //Debug.Log("��簢 �ʰ�");
                IsGrounded = false;
                isRun = false;
                isOverAngleForSlope = true;
            }
            else
            {
                isOverAngleForSlope = false;
                float uSlopeAngleCofacter = Mathf.Tan(slopeAngle * Mathf.PI / 180);
                height = (transform.position - groundRaycastHit.point).y;
                if (height <= (groundCheckThresholdMax + Mathf.Abs(uSlopeAngleCofacter)))
                {
                    IsGrounded = true;
                    slopeAngle = Vector3.Angle(transform.forward, groundRaycastHit.normal) - 90f;
                    slopeAngleCofacter = Mathf.Tan(slopeAngle * Mathf.PI / 180);
                    if (CanJump)
                    {
                        ResetJump();
                        ResetAirDash();
                    }
                }
                else
                {
                    //Debug.Log("FALSE: " + height + " << ����, ���� >> " + (groundCheckThresholdMax + Mathf.Abs(uSlopeAngleCofacter)));
                    slopeAngleCofacter = 0f;
                    IsGrounded = false;
                    isRun = false;
                }
            }

            bool rayChecked = Physics.SphereCast(transform.position + Vector3.up * (sphereRadius + Physics.defaultContactOffset) - transform.forward * Physics.defaultContactOffset, (sphereRadius - Physics.defaultContactOffset), transform.forward, out fowardRaycastHit, 0.2f, groundLayerMask, QueryTriggerInteraction.Ignore);
            if(rayChecked)
            {
                float forwardAngle = Vector3.Angle(Vector3.up, fowardRaycastHit.normal);
                
                if (forwardAngle > maxSlopeAngle)
                {
                    isFowardBlock = true;
                }
                else
                    isFowardBlock = false;

                forwardHeight = Mathf.Abs(fowardRaycastHit.point.y - transform.position.y);
                if (forwardHeight < forwardblockingMinHeight)
                {
                    slopeAngleCofacter = forwardblockingMinHeight;
                    IsGrounded = true;
                    isFowardBlock = false;
                }
            }
            else
            {
                isFowardBlock = false;
            }
        }
        else
        {
            isOverAngleForSlope = false;
            IsGrounded = false;
        }
    }
    #endregion

    //�޸��� �Լ�
    #region
    public void ToggleRun()
    {
        isRun = !isRun;
    }

    public void CheckMove(Vector3 vel)
    {
        if (isMove)
        {
            if (!(vel.magnitude > 0))
                isRun = false;
        }
    }
    #endregion

    // ���� �Լ�
    #region

    public void ResetJump()
    {
        IsJumping = false;
        IsAirJumping = false;
        isCumstomJumping = false;
    }
    public IEnumerator JumpCool()
    {
        CanJump = false;
        yield return new WaitForSeconds(jumpCoolTime);
        CanJump = true;
    }

    public void CheckJump()
    {
        // ���� �� �����̰� ���� ��Ÿ��()�� �������� ��, ���� ���� �ƴ� ����
        if (IsGrounded && CanJump && !IsJumping)
        {
            // ���� ��Ÿ���� ���ư��� ����, Jumping���� true
            IsJumping = true;
            StopCoroutine(nameof(JumpCool));
            StartCoroutine(nameof(JumpCool));
        }
    }

    public void SetCustomJumpState()
    {
        ResetJump();
        ResetAirDash();
        IsJumping = true;
        isCumstomJumping = true;
        StopCoroutine(nameof(JumpCool));
        StartCoroutine(nameof(JumpCool));
    }

    public void CheckAirJump()
    {
        // ���� �����̰� ���� ��Ÿ���� �������� ��, ���� ���� ���� �ƴ� ����
        if (!IsGrounded && CanJump && !IsAirJumping)
        {
            // ���� ��Ÿ���� ���ư��� ����, AirJumping���� true
            IsAirJumping = true;
            StopCoroutine(nameof(JumpCool));
            StartCoroutine(nameof(JumpCool));
        }
    }
    #endregion

    // ��� �Լ�
    #region
    private void ResetAirDash()
    {
        IsAirDashing = false;
        WasAirDashing = false;
    }

    public void CheckDash()
    {
        // ���� �� ������ ��, ��� ���� �ƴ� ����
        if (IsGrounded)
        {
            StartCoroutine(nameof(StartDashTimer));
            //StartCoroutine(nameof(DelayJump));
            IsDashing = true;
        }
    }

    public void CheckAirDash()
    {
        // ���� ������ ��, ���� ��� ���� �ƴ� ����
        if (!IsGrounded && !IsAirDashing)
        {
            StartCoroutine(nameof(StartDashTimer));
            IsAirDashing = true;
            WasAirDashing = true;
        }
    }

    //IEnumerator DelayJump()
    //{
    //    isDelayingDashJump = true;
    //    yield return new WaitForSeconds(0.1f);
    //    isDelayingDashJump = false;
    //}
    private IEnumerator StartDashTimer()
    {
        if (IsGrounded)
        {
            IsDashing = true;
            yield return new WaitForSeconds(dashTime);
            IsDashing = false;
        }
        else
        {
            IsAirDashing = true;
            yield return new WaitForSeconds(airDashTime);
            IsAirDashing = false;
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(fowardRaycastHit.point, 0.1f);
    }

    private void OnCollisionStay(Collision collision)
    {
        if(IsGrounded || !CanJump)
        {
            isAirBlocked = false;
        }
        else
        {
            isAirBlocked = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!IsGrounded)
        {
            isAirBlocked = false;
        }
    }
}