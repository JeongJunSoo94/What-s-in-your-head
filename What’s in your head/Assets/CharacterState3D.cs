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
    [Range(0.0f, 1f), Tooltip("지면 인식 허용 최소 거리")]
    public float groundCheckThresholdMin = 0.1f;
    [Range(0.0f, 1f), Tooltip("지면 인식 허용 최대 거리")]
    public float groundCheckThresholdMax = 0.3f;
    public float height = 0f;

    RaycastHit groundRaycastHit;
    RaycastHit groundHit;
    [Tooltip("캐릭터가 타고 올라갈 수 있는 최대 경사각")]
    public float maxSlopeAngle = 40f;
    [Tooltip("캐릭터가 바라보는 방향의 경사각")]
    public float slopeAngle;
    [Tooltip("slopeAngle만큼의 Y축 보정 인자")]
    public float slopeAngleCofacter;

    [Tooltip("지면 위 상태")]
    public bool IsGrounded = false;
    #endregion

    // 수평 이동 변수
    #region
    [Tooltip("방향키 입력 상태")]
    public bool isMove;
    [Tooltip("달리기 토글 ON/OFF 상태")]
    public bool isRun;
    #endregion

    // 점프 변수
    #region
    [Tooltip("점프 쿨타임")]
    public float jumpCoolTime = 0.2f;
    [Tooltip("점프 쿨이 돌았을 때 TRUE")]
    public bool CanJump = true;
    [Tooltip("지면에서 점프한 상태")]
    public bool IsJumping = false;
    [Tooltip("공중에서 점프한 상태")]
    public bool IsAirJumping = false;
    #endregion

    // 대시 변수
    #region
    [Tooltip("지면에서 대시한 상태")]
    public bool IsDashing = false;
    [Tooltip("공중에서 대시한 상태")]
    public bool IsAirDashing = false;
    [Tooltip("공중에서 이전에 대시했던 상태")]
    public bool WasAirDashing = false;
    [Tooltip("지면 대시 지속 시간")]
    public float dashTime = 1f;
    [Tooltip("공중 대시 지속 시간")]
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
        bool RayCheck = Physics.SphereCast(transform.position + Vector3.up * (sphereRadius + Physics.defaultContactOffset), (sphereRadius - Physics.defaultContactOffset), Vector3.down, out groundRaycastHit, sphereRadius + groundCheckDistance, groundLayerMask, QueryTriggerInteraction.Ignore);

        if (RayCheck)
        {
            slopeAngle = Vector3.Angle(Vector3.up, groundRaycastHit.normal);
            if (Mathf.Abs(slopeAngle) >= maxSlopeAngle)
            {
                IsGrounded = false;
                isRun = false;
                Debug.Log("경사각 초과");
            }
            else
            {
                float uSlopeAngleCofacter = Mathf.Tan(slopeAngle * Mathf.PI / 180);
                height = (transform.position - groundRaycastHit.point).y;
                if (height <= (groundCheckThresholdMax + Mathf.Abs(uSlopeAngleCofacter)))
                {
                    IsGrounded = true;
                    slopeAngle = Vector3.Angle(transform.forward, groundRaycastHit.normal) - 90f;
                    slopeAngleCofacter = Mathf.Tan(slopeAngle * Mathf.PI / 180);
                    ResetJump();
                    ResetAirDash();
                }
                else
                {
                    Debug.Log("FALSE: " + height + " << 높이, 기준 >> " + (groundCheckThresholdMax + Mathf.Abs(uSlopeAngleCofacter)));
                    slopeAngleCofacter = 0f;
                    IsGrounded = false;
                    isRun = false;
                }
            }
        }
        else
        {
            IsGrounded = false;
            Debug.Log("RayCheck 실패");
        }
    }

    //public void CheckGroundDistance(CapsuleCollider _capsuleCollider)
    //{
    //    // radius of the SphereCast
    //    float radius = _capsuleCollider.radius * 0.9f;
    //    var dist = 10f;
    //    float colliderHeight = _capsuleCollider.height;
    
    //    // ray for RayCast
    //    Ray ray2 = new Ray(transform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);
    
    
    //    // raycast for check the ground distance
    //    if (Physics.Raycast(ray2, out groundHit, (colliderHeight / 2) + dist, groundLayerMask) && !groundHit.collider.isTrigger)
    //        dist = transform.position.y - groundHit.point.y;
    
    
    //    // sphere cast around the base of the capsule to check the ground distance
    //    if (dist >= groundMinDistance)
    //    {
    //        Vector3 pos = transform.position + Vector3.up * (_capsuleCollider.radius);
    //        Ray ray = new Ray(pos, -Vector3.up);
    
    
    //        if (Physics.SphereCast(ray, radius, out groundHit, _capsuleCollider.radius + groundMaxDistance, groundLayerMask) && !groundHit.collider.isTrigger)
    //        {
    //            Physics.Linecast(groundHit.point + (Vector3.up * 0.1f), groundHit.point + Vector3.down * 0.15f, out groundHit, groundLayerMask);
    //            float newDist = transform.position.y - groundHit.point.y;
    
    
    //            if (dist > newDist) dist = newDist;
    //        }
    //    }
    
    
    //    groundDistance = (float)System.Math.Round(dist, 2);

    //    {

    //        Physics.SphereCast(this.transform.position +
    //            // here, to make the sphere start position higher by this offset
    //            Vector3.up * (CharCollider.radius + Physics.defaultContactOffset),
    //            // and here to make the sphere radius lower by this offset
    //            CharCollider.radius - Physics.defaultContactOffset,
    //            Vector3.down, out groundHitInfo, maxCheckHeight,
    //                LayersHelper.LayerExceptEnemiesPlayerAndUI, QueryTriggerInteraction.Ignore);
    //    }
    //}
    #endregion

    //달리기 함수
    #region
    public void  ToggleRun()
    {
        isRun = !isRun;
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
            WasAirDashing = true;
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
