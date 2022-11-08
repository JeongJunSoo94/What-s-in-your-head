using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    

    //지면 체크 변수
    #region
    [Tooltip("지면으로 체크할 레이어 설정")]
    public LayerMask groundLayerMask;
    [Range(0.1f, 10.0f), Tooltip("지면 감지 거리")]
    public float groundCheckDistance = 2.0f;
    [Range(0.0f, 1f), Tooltip("지면 인식 허용 최소 거리")]
    public float groundCheckThresholdMin = 0.02f;
    [Range(0.0f, 1f), Tooltip("지면 인식 허용 최대 거리")]
    public float groundCheckThresholdMax = 0.2f;
    [Range(0.0f, 1f), Tooltip("전방 막힘 최소 높이")]
    public float forwardblockingMinHeight = 0.2f;

    //bool isGroundDelayOn = false;
    //public float groundDelayTime = 0.05f;
    public float height = 0f;
    public float forwardHeight = 0f;

    public bool RayCheck;
    //



    RaycastHit groundRaycastHit;
    RaycastHit fowardRaycastHit;
    [Tooltip("캐릭터가 바라보는 방향의 경사각")]
    public float slopeAngle;
    [Tooltip("slopeAngle만큼의 Y축 보정 인자")]
    public float slopeAngleCofacter;

    [Tooltip("지면 위 상태")]
    public bool IsGrounded = false;
    [Tooltip("전방 막힘 상태")]
    public bool isFowardBlock = false;
    [Tooltip("공중 막힘 상태")]
    public bool isAirBlocked = false;
    //[Tooltip("공중 막힘 오브젝트 수")]
    //public int airBlockNum = 0;
    [Tooltip("최대 경사각 초과 상태")]
    public bool isOverAngleForSlope = false;
    #endregion

    // 수평 이동 변수
    #region
    [Tooltip("방향키 입력 상태")]
    public bool isMove = false;
    [Tooltip("달리기 토글 ON/OFF 상태")]
    public bool isRun = false;
    #endregion

    // 점프 변수
    #region
    [Tooltip("점프 쿨이 돌았을 때 TRUE")]
    public bool CanJump = true;
    [Tooltip("대시 후 즉시 점프 불가")]
    public bool isDelayingDashJump = false;
    [Tooltip("지면에서 점프한 상태")]
    public bool IsJumping = false;
    [Tooltip("공중에서 점프한 상태")]
    public bool IsAirJumping = false;
    [Tooltip("특수 점프(트램펄린)한 상태")]
    public bool isCumstomJumping = false;
    #endregion

    // 대시 변수
    #region
    [Tooltip("지면에서 대시한 상태")]
    public bool IsDashing = false;
    [Tooltip("공중에서 대시한 상태")]
    public bool IsAirDashing = false;
    [Tooltip("공중에서 이전에 대시했던 상태")]
    public bool WasAirDashing = false;
    
    #endregion

    //제어불가능 상태 변수
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

    [Header("_______변경 가능 값_______")]
    [Header("캐릭터가 타고 올라갈 수 있는 최대 경사각")]
    public float maxSlopeAngle = 40f;
    [Header("점프 쿨타임")]
    public float jumpCoolTime = 0.2f;
    [Header("지면 대시 지속 시간")]
    public float dashTime = 1f;
    [Header("공중 대시 지속 시간")]
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



    // 지면 체크 함수(지면 각도에 따라서 지면체크 거리 안에 안들어올 수 있기에 보정 필요)
    #region
    public void CheckGround(float sphereRadius)
    {
        RayCheck = Physics.SphereCast(transform.position + Vector3.up * (sphereRadius * 1.5f + Physics.defaultContactOffset), (sphereRadius - Physics.defaultContactOffset), Vector3.down, out groundRaycastHit, groundCheckDistance + sphereRadius * 0.5f + Physics.defaultContactOffset, groundLayerMask, QueryTriggerInteraction.Ignore);
        
        if (RayCheck)
        {
            slopeAngle = Vector3.Angle(Vector3.up, groundRaycastHit.normal);
            if (Mathf.Abs(slopeAngle) >= maxSlopeAngle)
            {
                //Debug.Log("경사각 초과");
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
                    //Debug.Log("FALSE: " + height + " << 높이, 기준 >> " + (groundCheckThresholdMax + Mathf.Abs(uSlopeAngleCofacter)));
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

    //달리기 함수
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

    // 점프 함수
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
        // 지면 위 상태이고 점프 쿨타임()이 돌아있을 때, 점프 중이 아닐 때만
        if (IsGrounded && CanJump && !IsJumping)
        {
            // 점프 쿨타임이 돌아가기 시작, Jumping상태 true
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
        // 공중 상태이고 점프 쿨타임이 돌아있을 때, 공중 점프 중이 아닐 때만
        if (!IsGrounded && CanJump && !IsAirJumping)
        {
            // 점프 쿨타임이 돌아가기 시작, AirJumping상태 true
            IsAirJumping = true;
            StopCoroutine(nameof(JumpCool));
            StartCoroutine(nameof(JumpCool));
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
        if (IsGrounded)
        {
            StartCoroutine(nameof(StartDashTimer));
            //StartCoroutine(nameof(DelayJump));
            IsDashing = true;
        }
    }

    public void CheckAirDash()
    {
        // 공중 상태일 때, 공중 대시 중이 아닐 때만
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