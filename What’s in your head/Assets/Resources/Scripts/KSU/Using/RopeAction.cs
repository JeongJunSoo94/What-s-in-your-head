using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Options.InputBindings;

public class RopeAction : MonoBehaviour
{
    public enum Direction { F, FR, R, BR, B, BL, L, FL, Default }

    Vector3[] directions;
    
    public GameObject player;

    public RopeSpawner spawner; 

    //public float detectingRange = 10f;
    //public float ropeLength = 15f;

    //public float rotationSpeed = 180f;
    //public float swingSpeed = 30f;
    public Direction targetDirection = Direction.Default;
    public float targetAddYRotation;
    public float currentAddYRotation;

    float startXAngle = 0f;
    int startDirIndex = 0;

    //public float swingAngle = 60f;
    [SerializeField] GameObject ropeAnchor;
    GameObject rope;

    public bool isSwingForward = true;
    public bool isRotating = false;

    bool isReadyToRide = false;
    bool isRopeExisting = false;


    // Start is called before the first frame update
    void Start()
    {
        spawner = GetComponentInParent<RopeSpawner>();
        SetStartPoints();
        MakeRope();
    }

    // Update is called once per frame
    void Update()
    {
        if(isRopeExisting)
        {
            InputKey();
        }
    }

    private void FixedUpdate()
    {
        if(isReadyToRide)
        {
            MovePlayerToRope();
            if(!isReadyToRide)
            {
                AcceptPlayer();
            }
        }

        if(isRopeExisting)
        {
            Swing();
            SetRotate();
            CalculateDistance();
        }
    }

    void SetStartPoints()
    {
        directions = new Vector3[8];
        directions[0] = spawner.transform.forward.normalized;
        directions[2] = spawner.transform.right.normalized;
        directions[4] = -spawner.transform.forward.normalized;
        directions[1] = Vector3.Lerp(directions[0], directions[2], 0.5f).normalized;
        directions[3] = Vector3.Lerp(directions[2], directions[4], 0.5f).normalized;
        for (int i = 0; i < 4; ++i)
        {
            directions[i+4] = -directions[i];
        }
    }

    void FindStartPoints()
    {
        Vector3 playerFowardVec = player.transform.forward;
        playerFowardVec.y = 0;
        float minAngle = 360f;
        for(int i = 0; i < 8; ++i)
        {
            float angle = Vector3.Angle(playerFowardVec, directions[i]);
            if(angle < minAngle)
            {
                minAngle = angle;
                startDirIndex = i;
            }
        }
        //Vector3[] swapedDirections = new Vector3[8];
        //for(int j = 0; j < 8; ++j)
        //{
        //    if((startDir + j) < 8)
        //    {
        //        swapedDirections[j] = directions[startDir + j];
        //    }
        //    else
        //    {
        //        swapedDirections[j] = directions[startDir + j - 8];
        //    }
        //}
        //directions = swapedDirections;
        Vector3 playerPosVec = player.transform.position - transform.position;
        startXAngle = (90f - Vector3.Angle(directions[startDirIndex], playerPosVec));
    }

    void MakeRope()
    {
        FindStartPoints();
        ropeAnchor.transform.localScale = new Vector3(1, 1, 1) * (1f / spawner.transform.localScale.x);
        rope = new GameObject("Rope");  // 플레이어가 매달릴 끝 생성
        rope.transform.parent = ropeAnchor.transform;
        Vector3 localPos = Vector3.zero;
        localPos.y = -spawner.ropeLength;
        rope.transform.localPosition = localPos;
        player.GetComponent<PlayerController3D>().enabled = false;
        FindStartPoints();
        Vector3 localRot = Vector3.zero;
        localRot.x = startXAngle;
        ropeAnchor.transform.localRotation = Quaternion.Euler(localRot);

        isReadyToRide = true;
    }

    void MovePlayerToRope()
    {
        player.transform.position = Vector3.MoveTowards(player.transform.position, rope.transform.position, Time.fixedDeltaTime * 4f);
        if (Vector3.Distance(player.transform.position, rope.transform.position) < 1f)
        {
            isReadyToRide = false;
        }
    }

    void AcceptPlayer()
    {
        player.transform.parent = rope.transform;
        player.transform.localPosition = Vector3.zero;
        player.transform.LookAt(player.transform.position + directions[startDirIndex]);

        player.transform.parent = rope.transform;  // 플레이어를 로프에 종속

        isRopeExisting = true;
    }

    public void DestroyRope()
    {
        isRopeExisting = false;

        player.transform.parent = null;

        PlayerController3D playerController = player.GetComponent<PlayerController3D>();
        Vector3 inertiaVec = player.transform.forward;
        inertiaVec.y = 0;
        player.transform.LookAt(player.transform.position + inertiaVec);
        playerController.enabled = true;
        playerController.MakeinertiaVec(playerController.walkSpeed, inertiaVec.normalized);
        playerController.moveVec = Vector3.up * player.GetComponent<PlayerController3D>().jumpSpeed;
        Destroy(this.gameObject);
    }

    float FitInHalfDegree(float Angle)
    {
        return (Angle > 180) ? (Angle - 360f) : Angle;
    }

    void Swing()
    {
        player.transform.localPosition = Vector3.zero;
        // -180 < rot X <= 180 사이로 고정 
        float rotationX = FitInHalfDegree(ropeAnchor.transform.localRotation.eulerAngles.x);

        if (isSwingForward) // 앞으로 갈지 뒤로갈지 결정
        {
            rotationX += spawner.swingSpeed * Time.fixedDeltaTime;
            if (rotationX > spawner.swingAngle)
            {
                isSwingForward = false;
            }
        }
        else
        {
            rotationX -= spawner.swingSpeed * Time.fixedDeltaTime;
            if (rotationX < -spawner.swingAngle)
            {
                isSwingForward = true;
            }
        }

        // rotation에 대입
        ropeAnchor.transform.localRotation = Quaternion.Euler(Vector3.right * rotationX);
    }

    void InputKey()
    {
        if (isRotating)
        {
            return;
        }

        bool F = ITT_KeyManager.Instance.GetKey(PlayerAction.MoveForward);
        bool B = ITT_KeyManager.Instance.GetKey(PlayerAction.MoveBackward);
        bool L = ITT_KeyManager.Instance.GetKey(PlayerAction.MoveLeft);
        bool R = ITT_KeyManager.Instance.GetKey(PlayerAction.MoveRight);

        Direction input = targetDirection;
        
        if(F)
        {
            if(R)
            {
                input = Direction.FR;
            }
            else if(L)
            {
                input = Direction.FL;
            }
            else
            {
                input = Direction.F;
            }
        }
        else if(B)
        {
            if (R)
            {
                input = Direction.BR;
            }
            else if (L)
            {
                input = Direction.BL;
            }
            else
            {
                input = Direction.B;
            }
        }
        else if(R)
        {
            input = Direction.R;
        }
        else if(L)
        {
            input = Direction.L;
        }

        if (input != targetDirection)
        {
            targetDirection = input;
            targetAddYRotation = (int)targetDirection * 45;
            Debug.Log("(int)targetDirection = " + (int)targetDirection);
            Debug.Log("targetYRotation = " + (int)targetDirection * 45);
            isRotating = true;
        }
    }

    void SetRotate()
    {
        if (isRotating)
        {          
            // 목표 방향으로 도달하면 회전 멈춤
            if (Mathf.Abs(currentAddYRotation - targetAddYRotation) < 5f)
            {
                isRotating = false;
                return;
            }

            //  목표 방향으로 시계, 반시계 중 가까운 방향으로 회전
            if (currentAddYRotation > targetAddYRotation)
            {
                if ((currentAddYRotation - targetAddYRotation) > (targetAddYRotation + 360f - currentAddYRotation))
                {
                    currentAddYRotation += spawner.rotationSpeed * Time.fixedDeltaTime;
                }
                else
                {
                    currentAddYRotation -= spawner.rotationSpeed * Time.fixedDeltaTime;
                }
            }
            else
            {
                if ((currentAddYRotation + 360f - targetAddYRotation) > (targetAddYRotation - currentAddYRotation))
                {
                    currentAddYRotation += spawner.rotationSpeed * Time.fixedDeltaTime;
                }
                else
                {
                    currentAddYRotation -= spawner.rotationSpeed * Time.fixedDeltaTime;
                }
            }

            // 0 <= Y rotation <= 360 사이로 고정
            if (currentAddYRotation < 0)
            {
                currentAddYRotation += 360f;
            }    
            else if(currentAddYRotation > 360f)
            {
                currentAddYRotation -= 360f;
            }

            // rotation 변경
            transform.rotation = Quaternion.Euler(Vector3.up * currentAddYRotation);
        }
    }

    void CalculateDistance()
    {
        Debug.Log(Vector3.Distance(player.transform.position, this.transform.position));
    }
}
