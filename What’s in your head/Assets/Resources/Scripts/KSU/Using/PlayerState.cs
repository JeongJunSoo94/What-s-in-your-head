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
    //[Range(0.0f, 1f), Tooltip("���� �ν� ��� �Ÿ�")]
    //public float groundCheckThreshold = 0.2f;


    //
    [Range(0.0f, 1f), Tooltip("���� �ν� ��� �ּ� �Ÿ�")]
    public float groundCheckThresholdMin = 0.1f;
    [Range(0.0f, 1f), Tooltip("���� �ν� ��� �ִ� �Ÿ�")]
    public float groundCheckThresholdMax = 0.3f;
    public float height = 0f;

    public bool RayCheck;
    //



    RaycastHit groundRaycastHit;
    RaycastHit fowardRaycastHit;
    [Tooltip("ĳ���Ͱ� Ÿ�� �ö� �� �ִ� �ִ� ��簢")]
    public float maxSlopeAngle = 40f;
    [Tooltip("ĳ���Ͱ� �ٶ󺸴� ������ ��簢")]
    public float slopeAngle;
    [Tooltip("slopeAngle��ŭ�� Y�� ���� ����")]
    public float slopeAngleCofacter;

    [Tooltip("���� �� ����")]
    public bool IsGrounded = false;
    [Tooltip("���� ���� ����")]
    public bool isFowardBlock = false;
    [Tooltip("�ִ� ��簢 �ʰ� ����")]
    public bool isOverAngleForSlope = false;
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
    public bool IsOutOfControl = false;
    public float outOfControllDuration;
    #endregion

    public bool aim=false;

    // ���� üũ �Լ�(���� ������ ���� ����üũ �Ÿ� �ȿ� �ȵ��� �� �ֱ⿡ ���� �ʿ�)
    #region
    //public void CheckGround(float sphereRadius)
    //{
    //    IsGrounded = Physics.SphereCast(transform.position + Vector3.up * sphereRadius, sphereRadius, Vector3.down, out groundRaycastHit, sphereRadius + groundCheckDistance, groundLayerMask, QueryTriggerInteraction.Ignore);

    //    if (IsGrounded)
    //    {
    //        //Vector3 lookVec = transform.forward;
    //        //lookVec.y = 0;
    //        //lookVec = lookVec.normalized;
    //        slopeAngle = Vector3.Angle(Vector3.up, groundRaycastHit.normal);
    //        Debug.Log("slopeAngle " + slopeAngle);
    //        if (Mathf.Abs(slopeAngle) >= maxSlopeAngle)
    //        {
    //            IsGrounded = false;
    //            isRun = false;
    //        }
    //        else
    //        {
    //            float uSlopeAngleCofacter = Mathf.Tan(slopeAngle * Mathf.PI / 180);
    //            float height = (transform.position - groundRaycastHit.point).y;
    //            //Debug.Log(height + " " + (groundCheckThreshold + Mathf.Abs(uSlopeAngleCofacter)));
    //            //if (height < Mathf.Epsilon)
    //            //    height = 0.01f;

    //            if (height <= (groundCheckThreshold + Mathf.Abs(uSlopeAngleCofacter)))
    //            {
    //                slopeAngle = Vector3.Angle(transform.forward, groundRaycastHit.normal) - 90f;
    //                slopeAngleCofacter = Mathf.Tan(slopeAngle * Mathf.PI / 180);
    //                ResetJump();
    //                ResetAirDash();
    //            }
    //            else
    //            {
    //                slopeAngleCofacter = 0f;
    //                IsGrounded = false;
    //                isRun = false;
    //            }
    //        }
    //    }
    //}

    public void CheckGround(float sphereRadius)
    {
        RayCheck = Physics.SphereCast(transform.position + Vector3.up * (sphereRadius * 1.5f + Physics.defaultContactOffset), (sphereRadius - Physics.defaultContactOffset), Vector3.down, out groundRaycastHit, groundCheckDistance + sphereRadius * 0.5f + Physics.defaultContactOffset, groundLayerMask, QueryTriggerInteraction.Ignore);
        
        if (RayCheck)
        {
            slopeAngle = Vector3.Angle(Vector3.up, groundRaycastHit.normal);
            if (Mathf.Abs(slopeAngle) >= maxSlopeAngle)
            {
                IsGrounded = false;
                isRun = false;
                isOverAngleForSlope = true;
                //Debug.Log("��簢 �ʰ�");
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
                //Debug.Log("fowardRaycastHit.point: " + fowardRaycastHit.point);
                //Debug.Log("groundRaycastHit.point: " + groundRaycastHit.point);
                float forwardAngle = Vector3.Angle(Vector3.up, fowardRaycastHit.normal);
                if (forwardAngle > maxSlopeAngle)
                    isFowardBlock = true;
                else
                    isFowardBlock = false;
                Debug.Log("isFowardBlockt: " + isFowardBlock);
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
            //Debug.Log("RayCheck ����");
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
            StartCoroutine(nameof(JumpCool));
        }
    }
    public void CheckAirJump()
    {
        // ���� �����̰� ���� ��Ÿ���� �������� ��, ���� ���� ���� �ƴ� ����
        if (!IsGrounded && CanJump && !IsAirJumping)
        {
            // ���� ��Ÿ���� ���ư��� ����, AirJumping���� true
            IsAirJumping = true;
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
        Gizmos.DrawSphere(groundRaycastHit.point, 0.1f);
    }
}