using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState3D : MonoBehaviour
{
    //���� üũ ����
    #region
    [Tooltip("�������� üũ�� ���̾� ����")]
    public LayerMask groundLayerMask;
    [Range(0.1f, 10.0f), Tooltip("���� ���� �Ÿ�")]
    public float groundCheckDistance = 2.0f;
    [Range(0.0f, 0.1f), Tooltip("���� �ν� ��� �Ÿ�")]
    public float groundCheckThreshold = 0.01f;

    RaycastHit groundRaycastHit;

    public float maxSlopeAngle = 40f; // ĳ���Ͱ� Ÿ�� �ö� �� �ִ� �ִ� ��簢
    public float slopeAngle; // ĳ���Ͱ� �ٶ󺸴� ������ ��簢
    public float slopeAngleCofacter; // slopeAngle��ŭ�� ���� ����


    public bool IsGrounded = false; // ���� �� ����
    #endregion

    // ���� �̵� ����
    #region
    public bool isMove;
    public bool isRun;
    #endregion

    // ���� ����
    #region
    public float jumpCoolTime = 0.2f;

    public bool CanJump = true;
    public bool IsJumping = false;
    public bool IsAirJumping = false;
    #endregion

    // ��� ����
    #region
    public bool IsDashing = false;
    public bool IsAirDashing = false;
    public bool WasAirDashing = false;
    public float dashTime = 1f;
    public float airDashTime = 0.5f;
    #endregion

    //����Ұ��� ���� ����
    #region
    public bool IsOutOfControl { get; protected set; }
    public float outOfControllDuration;
    #endregion

    // ���� üũ �Լ�(���� ������ ���� ����üũ �Ÿ� �ȿ� �ȵ��� �� �ֱ⿡ ���� �ʿ�)
    #region
    public void CheckGround(float sphereRadius)
    {
        IsGrounded = Physics.SphereCast(transform.position + Vector3.up * sphereRadius, sphereRadius, Vector3.down, out groundRaycastHit, groundCheckDistance, groundLayerMask, QueryTriggerInteraction.Ignore);

        if (IsGrounded)
        {
            slopeAngle = Vector3.Angle(groundRaycastHit.normal, transform.forward) - 90f;

            if (Mathf.Abs(slopeAngle) >= maxSlopeAngle)
            {
                IsGrounded = false;
                isRun = false;
            }
            else
            {
                slopeAngleCofacter = Mathf.Tan(slopeAngle);
                if((transform.position - groundRaycastHit.point).y <= (groundCheckThreshold + Mathf.Abs(slopeAngleCofacter)))
                {
                    ResetJump();
                    ResetAirDash();
                }
                else
                {
                    IsGrounded = false;
                    isRun = false;
                }
            }
        }
    }
    #endregion

    //�޸��� �Լ�
    #region
    public void  ToggleRun()
    {
        if (isRun)
        {
            isRun = false;
        }
        else
        {
            isRun = true;
        }
    }

    public void CheckMove(Vector3 vel)
    {
        if(isMove)
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
        CanJump = true;
        IsJumping = false;
        IsAirJumping = false;
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
            StartCoroutine("JumpCool");
        }
    }
    public void CheckAirJump()
    {
        // ���� �����̰� ���� ��Ÿ���� �������� ��, ���� ���� ���� �ƴ� ����
        if (!IsGrounded && CanJump && !IsAirJumping)
        {
            // ���� ��Ÿ���� ���ư��� ����, AirJumping���� true
            IsAirJumping = true;
            StartCoroutine("JumpCool");
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
        if (IsGrounded && !IsDashing)
        {
            StartCoroutine("StartDashTimer");
            IsDashing = true;
        }
    }

    public void CheckAirDash()
    {
        // ���� ������ ��, ���� ��� ���� �ƴ� ����
        if (!IsGrounded && !IsAirDashing && !WasAirDashing)
        {
            StartCoroutine("StartDashTimer");
            IsAirDashing = true;
        }
    }
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
}
