using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.YC_Camera;

public class PlayerState : MonoBehaviour
{
    

    //지면 체크 변수
    #region
    [Tooltip("지면으로 체크할 레이어 설정")]
    public LayerMask groundLayerMask;
    [Range(0.1f, 10.0f), Tooltip("지면에서 지면 감지 거리")]
    public float groundCheckDistance = 1f;
    [Range(0.1f, 10.0f), Tooltip("공중에서 지면 감지 거리")]
    public float groundCheckAirDistance = 0.1f;
    [Range(0.0f, 1f), Tooltip("지면 인식 허용 최소 거리")]
    public float groundCheckThresholdMin = 0.02f;
    [Range(0.0f, 1f), Tooltip("지면 인식 허용 최대 거리")]
    public float groundCheckThresholdMax = 0.05f;
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
    [Tooltip("지면에서 이전에 대시했던 상태")]
    public bool WasDashing = false;
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


    // 스테이지3 미로 관련 변수
    #region
    public bool isInMaze = false;
    string mazeColTag = "MazeCameraCollider";
    CameraController CameraController;
    bool isExitMaze = false;
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
            isOutOfControl = false;
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

        if(IsGrounded)
            RayCheck = Physics.SphereCast(transform.position + Vector3.up * (sphereRadius * 1.5f + Physics.defaultContactOffset), (sphereRadius - Physics.defaultContactOffset), Vector3.down, out groundRaycastHit, groundCheckDistance + sphereRadius * 0.5f + Physics.defaultContactOffset, groundLayerMask, QueryTriggerInteraction.Ignore);
        else
            RayCheck = Physics.SphereCast(transform.position + Vector3.up * (sphereRadius * 1.5f + Physics.defaultContactOffset), (sphereRadius - Physics.defaultContactOffset), Vector3.down, out groundRaycastHit, groundCheckAirDistance + sphereRadius * 0.5f + Physics.defaultContactOffset, groundLayerMask, QueryTriggerInteraction.Ignore);

        if (RayCheck)
        {
            slopeAngle = Vector3.Angle(Vector3.up, groundRaycastHit.normal);
            if (Mathf.Abs(slopeAngle) >= maxSlopeAngle)
            {
                IsGrounded = false;
                isRun = false;
                isOverAngleForSlope = true;
            }
            else
            {
                isOverAngleForSlope = false;
                float uSlopeAngleCofacter = Mathf.Tan(slopeAngle * Mathf.Deg2Rad);
                height = (transform.position - groundRaycastHit.point).y;

                IsGrounded = true;
                slopeAngle = Vector3.Angle(transform.forward, groundRaycastHit.normal) - 90f;
                slopeAngleCofacter = Mathf.Tan(slopeAngle * Mathf.Deg2Rad);
                if (CanJump)
                {
                    ResetJump();
                    ResetDash();
                }
            }

            bool rayChecked1 = Physics.Raycast(transform.position, transform.forward, forwardblockingMinHeight / Mathf.Cos((90f - maxSlopeAngle) * Mathf.Deg2Rad), groundLayerMask, QueryTriggerInteraction.Ignore);
            bool rayChecked2 = Physics.Raycast(transform.position + Vector3.up * forwardblockingMinHeight, transform.forward, forwardblockingMinHeight / Mathf.Cos((90f - maxSlopeAngle) * Mathf.Deg2Rad), groundLayerMask, QueryTriggerInteraction.Ignore);
            if (rayChecked1)
            {
                if (!rayChecked2)
                {
                    isFowardBlock = rayChecked2;
                    IsGrounded = true;
                    slopeAngleCofacter = forwardblockingMinHeight;
                    isFowardBlock = false;
                }
                else
                {
                    isFowardBlock = true;
                }
            }
            else
            {
                isFowardBlock = false;
                if (isOverAngleForSlope)
                    IsGrounded = true;
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
            WasDashing = false;
            StopCoroutine(nameof(JumpCool));
            StartCoroutine(nameof(JumpCool));
        }
    }

    public void SetCustomJumpState()
    {
        ResetJump();
        ResetDash();
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
    private void ResetDash()
    {
        IsAirDashing = false;
        WasAirDashing = false;
        if(!IsDashing)
            WasDashing = false;
    }

    public void CheckDash()
    {
        // 지면 위 상태일 때, 대시 중이 아닐 때만
        StartCoroutine(nameof(StartDashTimer));
    }

    public void CheckAirDash()
    {
        // 공중 상태일 때, 공중 대시 중이 아닐 때만
        if (!IsAirDashing)
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
            WasDashing = true;
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
        if(IsGrounded || !CanJump || IsDashing)
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

    private void OnTriggerEnter(Collider other)
    {
        // << : 스테이지3 미로에 입장하거나 퇴장할 경우, 플레이어의 State와 Camera의 세팅을 바꿔준다.

        if(other.gameObject.tag.Equals("MazeCameraExitCollider"))
        {
            isExitMaze = true;
        }

        if (!other.gameObject.tag.Equals(mazeColTag))
            return;

        if (!CameraController)
            CameraController = this.gameObject.GetComponent<CameraController>();
        
        isInMaze = true;
        CameraController.SetMazeMode(isInMaze, false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.tag.Equals(mazeColTag))
            return;

        isInMaze = false;

        if(isExitMaze)
            CameraController.SetMazeMode(isInMaze, true);
        else
            CameraController.SetMazeMode(isInMaze, false);
    }
}