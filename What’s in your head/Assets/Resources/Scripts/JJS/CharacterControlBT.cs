using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControlBT : MonoBehaviour
{
    public float walkSpeed = 4.0f;
    public float runSpeed = 6.0f;
    public float dashSpeed = 4.0f;
    public float addSpeed = 0.0f;
    public float dashTime = 0.5f;
    public float JumpPower = 10.0f;
    public float rotationSpeed = 360.0f;

    public Rigidbody pRigidbody;
    public Camera pCamera;
    public RaycastHit[] raycastHits;

    public Vector3 _moveDir;
    public Vector3 direction;
    public Vector3 dashVec = Vector3.zero;

    public bool onPlatform = true;
    public bool isJumping = false;
    public bool isDash = false;
    public bool isAirDash = false;
    public int jumpcount = 0;
    public int dashcount = 0;

    public Animator animator;
    public string currentState;

    private Task _root;

    void Start()
    {
        ItTakesTwoKeyManager.Instance.GetKeyDown(KeyName.W);
        pRigidbody = gameObject.GetComponent<Rigidbody>();
        pCamera = Camera.main;
        animator = GetComponent<Animator>();
        ConstructBehaviourTree();
    }

    void Update()
    {
        _root.Evaluate();
        //Dash();
        //Jump();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Move();
    }

    private void ConstructBehaviourTree()
    {
        //IsCoverAvaliableNode coverAvaliableNode = new IsCoverAvaliableNode(avaliableCovers, playertransform, this);
        //GoToCoverNode goToCoverNode = new GoToCoverNode(_agent, this);
        //HealthNode healthNode = new HealthNode(this, lowHealthThreshold);
        //IsCoveredNode isCoveredNode = new IsCoveredNode(playertransform, transform);
        //ChaseNode chaseNode = new ChaseNode(playertransform, _agent, this);
        //RangeNode chasingRangeNode = new RangeNode(chasingRange, playertransform, transform);
        //RangeNode shootingRangeNode = new RangeNode(shootingRange, playertransform, transform);
        //ShootNode shootNode = new ShootNode(_agent, this);

        //Sequence chaseSequence = new Sequence(new List<Task> { chasingRangeNode, chaseNode });
        //Sequence shootSequence = new Sequence(new List<Task> { shootingRangeNode, shootNode });

        //Sequence goToCoverSequence = new Sequence(new List<Task> { coverAvaliableNode, goToCoverNode });
        //Selector findCoverSequence = new Selector(new List<Task> { goToCoverSequence, chaseSequence });
        //Selector tryToTakeCoverSelector = new Selector(new List<Task> { isCoveredNode, findCoverSequence });
        //Sequence  = new Sequence(new List<Task> { healthNode, tryToTakeCoverSelector });

        IsInputMoveKey isInputMoveKey = new IsInputMoveKey(this);
        ObjectMove playerMove = new ObjectMove(this);
        ObjectRotation playerRotate = new ObjectRotation(this);
        PlayAnimationBlend playerMoveAnim = new PlayAnimationBlend(this, "Move");
        PlayAnimation playerwaitAnim = new PlayAnimation(this, "WAIT01");
        Sequence move = new (new List<Task> { isInputMoveKey , playerMove, playerRotate, playerMoveAnim });

        //Parallel move = new Parallel(new List<Task> { });

        _root = new Selector(new List<Task> { move, playerwaitAnim });
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

        direction = pCamera.transform.forward * ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.W) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.S) ? -1 : 0))
            + pCamera.transform.right * ((ItTakesTwoKeyManager.Instance.GetKey(KeyName.D) ? 1 : 0) + (ItTakesTwoKeyManager.Instance.GetKey(KeyName.A) ? -1 : 0));
        direction.y = 0;
        direction = direction.normalized;

        Vector3 forward = Vector3.Slerp(transform.forward, direction, rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, direction));
        transform.LookAt(transform.position + forward);

        direction = forward * moveSpeed;
        //(!onPlatform || isJumping) 으로 하면 점프후 대시를 할때 대시중 아래로 계속 떨어짐
        //((onPlatform && !isJumping)) || (isDash && isJumping)) 으로 하면 점프후 대시할때 높이유지는 되지만 대시중 점프가 막힘 대시중 점프랑 점프중 대시를 다르게 구분할수 있게해야함
        if ((onPlatform && !isJumping) || isAirDash) // 으로 하면 점프후 대시후 점프가 막힘 >>> 그래서 점프할때 isAirDash 상태면 isAirDash를 false 로 변경하니 해결
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
            StartCoroutine("CorDash");
        }
        dashVec = transform.forward.normalized * addSpeed;
    }

    IEnumerator CorDash()
    {
        if (!onPlatform)
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
            if (isAirDash)
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
        raycastHits = Physics.SphereCastAll(transform.position, 0.15f, -transform.up, 0.01f, LayerMask.NameToLayer("Platform"));
        Debug.Log("raycastHits.Length: " + raycastHits.Length);
        bool rayCheck = false;
        for (int index = 0; index < raycastHits.Length; ++index)
        {
            if (raycastHits[index].collider.tag == "Platform")
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
            if (RaycheckGround())
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
            if (!RaycheckGround())
            {
                jumpcount = 1;
                onPlatform = false;
                //pRigidbody.useGravity = true;

                if (isDash && isJumping)
                {
                    {
                        //dashcount = 1; 여기는 지면대시 >> 점프 >> 대시를 막는 코드라서 지움
                        StopCoroutine("CorDash"); // 지면대시 >> 점프 >> 대시하면 처음 대시의 코루틴이 작동해서 공중대시가 빨리끝남
                        addSpeed = 0.0f;
                        isDash = false;
                    }
                }
            }
        }
    }
}
