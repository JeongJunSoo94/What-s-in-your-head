using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimJJS : MonoBehaviour
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

    Animator animator;
    private string currentState;

    private Task _root;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        _root.Evaluate();
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

        Parallel move = new Parallel(new List<Task> { });

        _root = new Selector(new List<Task> {  });
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);

        currentState = newState;
    }
}
