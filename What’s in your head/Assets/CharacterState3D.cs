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
    [Range(0.0f, 1f), Tooltip("���� �ν� ��� �ּ� �Ÿ�")]
    public float groundCheckThresholdMin = 0.1f;
    [Range(0.0f, 1f), Tooltip("���� �ν� ��� �ִ� �Ÿ�")]
    public float groundCheckThresholdMax = 0.3f;
    public float height = 0f;

    RaycastHit groundRaycastHit;
    RaycastHit groundHit;
    [Tooltip("ĳ���Ͱ� Ÿ�� �ö� �� �ִ� �ִ� ��簢")]
    public float maxSlopeAngle = 40f;
    [Tooltip("ĳ���Ͱ� �ٶ󺸴� ������ ��簢")]
    public float slopeAngle;
    [Tooltip("slopeAngle��ŭ�� Y�� ���� ����")]
    public float slopeAngleCofacter;

    [Tooltip("���� �� ����")]
    public bool IsGrounded = false;
    #endregion

    // ���� �̵� ����
    #region
    [Tooltip("����Ű �Է� ����")]
    public bool isMove;
    [Tooltip("�޸��� ��� ON/OFF ����")]
    public bool isRun;
    #endregion

    // ���� ����
    #region
    [Tooltip("���� ��Ÿ��")]
    public float jumpCoolTime = 0.2f;
    [Tooltip("���� ���� ������ �� TRUE")]
    public bool CanJump = true;
    [Tooltip("���鿡�� ������ ����")]
    public bool IsJumping = false;
    [Tooltip("���߿��� ������ ����")]
    public bool IsAirJumping = false;
    #endregion

    // ��� ����
    #region
    [Tooltip("���鿡�� ����� ����")]
    public bool IsDashing = false;
    [Tooltip("���߿��� ����� ����")]
    public bool IsAirDashing = false;
    [Tooltip("���߿��� ������ ����ߴ� ����")]
    public bool WasAirDashing = false;
    [Tooltip("���� ��� ���� �ð�")]
    public float dashTime = 1f;
    [Tooltip("���� ��� ���� �ð�")]
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
        bool RayCheck = Physics.SphereCast(transform.position + Vector3.up * (sphereRadius + Physics.defaultContactOffset), (sphereRadius - Physics.defaultContactOffset), Vector3.down, out groundRaycastHit, sphereRadius + groundCheckDistance, groundLayerMask, QueryTriggerInteraction.Ignore);

        if (RayCheck)
        {
            slopeAngle = Vector3.Angle(Vector3.up, groundRaycastHit.normal);
            if (Mathf.Abs(slopeAngle) >= maxSlopeAngle)
            {
                IsGrounded = false;
                isRun = false;
                Debug.Log("��簢 �ʰ�");
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
                    Debug.Log("FALSE: " + height + " << ����, ���� >> " + (groundCheckThresholdMax + Mathf.Abs(uSlopeAngleCofacter)));
                    slopeAngleCofacter = 0f;
                    IsGrounded = false;
                    isRun = false;
                }
            }
        }
        else
        {
            IsGrounded = false;
            Debug.Log("RayCheck ����");
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

    //�޸��� �Լ�
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

    // ���� �Լ�
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
