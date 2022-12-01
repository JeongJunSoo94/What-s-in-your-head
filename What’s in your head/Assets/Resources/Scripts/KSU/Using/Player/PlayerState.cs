using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.YC_Camera;

public class PlayerState : MonoBehaviour
{
    

    //���� üũ ����
    #region
    [Tooltip("�������� üũ�� ���̾� ����")]
    public LayerMask groundLayerMask;
    [Range(0.1f, 10.0f), Tooltip("���鿡�� ���� ���� �Ÿ�")]
    public float groundCheckDistance = 1f;
    [Range(0.1f, 10.0f), Tooltip("���߿��� ���� ���� �Ÿ�")]
    public float groundCheckAirDistance = 0.1f;
    [Range(0.0f, 1f), Tooltip("���� �ν� ��� �ּ� �Ÿ�")]
    public float groundCheckThresholdMin = 0.02f;
    [Range(0.0f, 1f), Tooltip("���� �ν� ��� �ִ� �Ÿ�")]
    public float groundCheckThresholdMax = 0.05f;
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
    [Tooltip("���鿡�� ������ ����ߴ� ����")]
    public bool WasDashing = false;
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


    // ��������3 �̷� ���� ����
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
            isOutOfControl = false;
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
    private void ResetDash()
    {
        IsAirDashing = false;
        WasAirDashing = false;
        if(!IsDashing)
            WasDashing = false;
    }

    public void CheckDash()
    {
        // ���� �� ������ ��, ��� ���� �ƴ� ����
        StartCoroutine(nameof(StartDashTimer));
    }

    public void CheckAirDash()
    {
        // ���� ������ ��, ���� ��� ���� �ƴ� ����
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
        // << : ��������3 �̷ο� �����ϰų� ������ ���, �÷��̾��� State�� Camera�� ������ �ٲ��ش�.

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