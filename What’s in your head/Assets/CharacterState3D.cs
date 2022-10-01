using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState3D : MonoBehaviour
{
    //지면 체크 변수
    #region
    [Tooltip("지면으로 체크할 레이어 설정")]
    public LayerMask groundLayerMask;
    [Range(0.1f, 10.0f), Tooltip("지면 감지 거리")]
    public float groundCheckDistance = 2.0f;
    [Range(0.0f, 0.1f), Tooltip("지면 인식 허용 거리")]
    public float groundCheckThreshold = 0.01f;

    RaycastHit groundRaycastHit;

    public float maxSlopeAngle = 40f; // 캐릭터가 타고 올라갈 수 있는 최대 경사각
    public float slopeAngle; // 캐릭터가 바라보는 방향의 경사각
    public float slopeAngleCofacter; // slopeAngle만큼의 보정 인자


    public bool IsGrounded = false; // 지면 위 상태
    #endregion

    // 수평 이동 변수
    #region
    public bool isMove;
    public bool isRun;
    #endregion

    // 점프 변수
    #region
    public float jumpCoolTime = 0.2f;

    public bool CanJump = true;
    public bool IsJumping = false;
    public bool IsAirJumping = false;
    #endregion

    // 대시 변수
    #region
    public bool IsDashing = false;
    public bool IsAirDashing = false;
    public bool WasAirDashing = false;
    public float dashTime = 1f;
    public float airDashTime = 0.5f;
    #endregion

    //제어불가능 상태 변수
    #region
    public bool IsOutOfControl { get; protected set; }
    public float outOfControllDuration;
    #endregion

    // 지면 체크 함수(지면 각도에 따라서 지면체크 거리 안에 안들어올 수 있기에 보정 필요)
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

    //달리기 함수
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

    // 점프 함수
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
        // 지면 위 상태이고 점프 쿨타임()이 돌아있을 때, 점프 중이 아닐 때만
        if (IsGrounded && CanJump && !IsJumping)
        {
            // 점프 쿨타임이 돌아가기 시작, Jumping상태 true
            IsJumping = true;
            StartCoroutine("JumpCool");
        }
    }
    public void CheckAirJump()
    {
        // 공중 상태이고 점프 쿨타임이 돌아있을 때, 공중 점프 중이 아닐 때만
        if (!IsGrounded && CanJump && !IsAirJumping)
        {
            // 점프 쿨타임이 돌아가기 시작, AirJumping상태 true
            IsAirJumping = true;
            StartCoroutine("JumpCool");
        }
    }
    #endregion

    // 대시 함수
    #region
    private void ResetAirDash()
    {
        IsAirDashing = false;
        WasAirDashing = false;
    }

    public void CheckDash()
    {
        // 지면 위 상태일 때, 대시 중이 아닐 때만
        if (IsGrounded && !IsDashing)
        {
            StartCoroutine("StartDashTimer");
            IsDashing = true;
        }
    }

    public void CheckAirDash()
    {
        // 공중 상태일 때, 공중 대시 중이 아닐 때만
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
