using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItTakesTwoPlayerControler : MonoBehaviour
{
    public float walkSpeed = 4.0f;
    public float runSpeed = 6.0f;
    public float dashSpeed = 4.0f;
    float addSpeed = 0.0f;
    public float dashTime = 0.5f;
    public float JumpPower = 10.0f;
    public float rotationSpeed = 360.0f;

    Rigidbody pRigidbody;
    Camera pCamera;
    RaycastHit[] raycastHits;

    Vector3 direction;
    Vector3 dashVec = Vector3.zero;

    public bool onPlatform = true;
    public bool isJumping = false;
    public bool isDash = false;
    public bool isAirDash = false;
    public int jumpcount = 0;
    public int dashcount = 0;


    // Start is called before the first frame update
    void Start()
    {
        ItTakesTwoKeyManager.Instance.GetKeyDown(KeyName.W);
        pRigidbody = gameObject.GetComponent<Rigidbody>();
        pCamera = Camera.main;
    }

    void Update()
    {
        Dash();
        Jump();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        float moveSpeed;

        if (!isDash && ItTakesTwoKeyManager.Instance.GetKey(KeyName.CapsLock))
        {
            moveSpeed = runSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }

        direction = pCamera.transform.forward * ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.W) ? 1:0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.S) ? -1:0))
            + pCamera.transform.right * ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.D) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.A) ? -1 : 0));
        direction.y = 0;
        direction = direction.normalized;

        Vector3 forward = Vector3.Slerp(transform.forward, direction, rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, direction));
        transform.LookAt(transform.position + forward);

        direction = forward * moveSpeed;
        //(!onPlatform || isJumping) ���� �ϸ� ������ ��ø� �Ҷ� ����� �Ʒ��� ��� ������
        //((onPlatform && !isJumping)) || (isDash && isJumping)) ���� �ϸ� ������ ����Ҷ� ���������� ������ ����� ������ ���� ����� ������ ������ ��ø� �ٸ��� �����Ҽ� �ְ��ؾ���
        if ((onPlatform && !isJumping) || isAirDash) // ���� �ϸ� ������ ����� ������ ���� >>> �׷��� �����Ҷ� isAirDash ���¸� isAirDash�� false �� �����ϴ� �ذ�
        {
            direction.y = 0;
        }
        else
        {
            direction.y = pRigidbody.velocity.y;
        }

        pRigidbody.velocity = direction + dashVec;
    }

    void Dash()
    {
        dashVec = Vector3.zero;
        if (!isDash && !(dashcount > 0) && ItTakesTwoKeyManager.Instance.GetKeyDown(KeyName.LeftShift))
        {
            StartCoroutine(nameof(CorDash));
        }
        dashVec = transform.forward.normalized * addSpeed;
    }

    IEnumerator CorDash()
    {
        if(!onPlatform)
        {
            isAirDash = true;
        }
        isDash = true;
        addSpeed = dashSpeed;
        dashcount++;
        yield return new WaitForSeconds(dashTime);
        isAirDash = false;
        isDash = false;
        addSpeed = 0.0f;
    }

    void Jump()
    {
        if (ItTakesTwoKeyManager.Instance.GetKeyDown(KeyName.Space))
        {
            if(isAirDash)
            {
                isAirDash = false;
            }

            if (!isJumping || jumpcount < 2)
            {
                pRigidbody.velocity = new Vector3(pRigidbody.velocity.x, JumpPower, pRigidbody.velocity.z);
                jumpcount++;
                isJumping = true;
            }
        }
    }

    bool RaycheckGround()
    {
        raycastHits = Physics.SphereCastAll(transform.position, 0.15f, -transform.up,0.01f,LayerMask.NameToLayer("Platform")); 
        Debug.Log("raycastHits.Length: " + raycastHits.Length);
        bool rayCheck = false;
        for (int index = 0; index < raycastHits.Length; ++index)
        {
            if (raycastHits[index].collider.CompareTag("Platform"))
            {
                rayCheck = true;
            }
        }
        Debug.Log("rayCheck: " + rayCheck);
        return rayCheck;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            if(RaycheckGround())
            {
                onPlatform = true;
                isJumping = false;
                jumpcount = 0;
                dashcount = 0;
                //pRigidbody.useGravity = false
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        dashcount = 0;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            if(!RaycheckGround())
            {
                jumpcount = 1;
                onPlatform = false;
                //pRigidbody.useGravity = true;

                if (isDash && isJumping)
                {
                    {
                        //dashcount = 1; ����� ������ >> ���� >> ��ø� ���� �ڵ�� ����
                        StopCoroutine(nameof(CorDash)); // ������ >> ���� >> ����ϸ� ó�� ����� �ڷ�ƾ�� �۵��ؼ� ���ߴ�ð� ��������
                        addSpeed = 0.0f;
                        isDash = false;
                    }
                }
            }
        }
    }
}