using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;

public class RopeAction : MonoBehaviour
{
    public enum Direction { F, FR, R, BR, B, BL, L, FL, Default }
    
    public RopeSpawner spawner; 
    [SerializeField] GameObject ropeAnchor;
    GameObject rope;
    public GameObject player;

    float startXAngle = 0f;
    public Direction targetDirection = Direction.Default;

    public float rotationX;
    float startYRotation = 0f;
    public float targetAddYRotation;
    public float currentAddYRotation;
    public float rotationTolerance = 10f;

    bool isReadyToRide = false;
    bool isRopeExisting = false;

    public bool isSwingForward = true;

    public bool isRotating = true;



    void Start()
    {
        spawner = GetComponentInParent<RopeSpawner>();
        MakeRope();
    }

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
            SetRotation();
            Swing();
            CalculateDistance();
        }
    }

    void FindStartPoints()
    {
        Vector3 playerFowardVec = player.transform.forward;
        playerFowardVec.y = 0;

        transform.rotation = Quaternion.Euler(Vector3.up * startYRotation);
        Vector3 playerPosVec = player.transform.position - transform.position;
        startXAngle = (Vector3.Angle(playerFowardVec, playerPosVec) - 90f);

        float angle = Vector3.Angle(transform.forward, playerFowardVec);
        if (angle > 90f)
        {
            startYRotation = 180f;
            startXAngle = -startXAngle;
        }
        else
        {
            startYRotation = 0f;
        }
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
        rope.transform.localRotation = Quaternion.Euler(Vector3.zero);
        player.transform.parent = rope.transform;
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.Euler(Vector3.zero);

        isRopeExisting = true;
    }

    public float DestroyRope()
    {
        isRopeExisting = false;
        player.transform.parent = null;
        Destroy(this.gameObject);
        return -rotationX / spawner.swingAngle;
    }

    float FitInHalfDegree(float Angle)
    {
        return (Angle > 180) ? (Angle - 360f) : Angle;
    }

    void Swing()
    {
        Debug.Log("vel: " + player.GetComponent<Rigidbody>().velocity);
        player.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        // -180 < rot X <= 180 사이로 고정 
        rotationX = FitInHalfDegree(ropeAnchor.transform.localRotation.eulerAngles.x);

        if (isSwingForward) // 앞으로 갈지 뒤로갈지 결정
        {
            rotationX -= spawner.swingSpeed * Time.fixedDeltaTime;
            if (rotationX < -spawner.swingAngle)
            {
                isSwingForward = false;
            }
        }
        else
        {
            rotationX += spawner.swingSpeed * Time.fixedDeltaTime;
            if (rotationX > spawner.swingAngle)
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
            isRotating = true;
        }
    }

    void SetRotation()
    {
        if (isRotating)
        {
            // 목표 방향으로 도달하면 회전 멈춤
            if (Mathf.Abs(currentAddYRotation - targetAddYRotation) < rotationTolerance)
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

            currentAddYRotation = FitInRange(currentAddYRotation, 0f, 360f);

            float totalYRotaion = startYRotation + currentAddYRotation;

            totalYRotaion = FitInRange(totalYRotaion, 0f, 360f);

            // rotation 변경
            transform.rotation = Quaternion.Euler(Vector3.up * totalYRotaion);
        }
    }

    float FitInRange(float num, float min, float max)
    {
        if (num < min)
        {
            num += max;
        }
        else if (num > max)
        {
            num -= max;
        }
        return num;
    }

    void CalculateDistance()
    {
        Debug.Log(Vector3.Distance(player.transform.position, this.transform.position));
    }
}
