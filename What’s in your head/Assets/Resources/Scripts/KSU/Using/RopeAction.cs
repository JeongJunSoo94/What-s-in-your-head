using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Options.InputBindings;

public class RopeAction : MonoBehaviour
{
    public enum Direction { F, FR, R, BR, B, BL, L, FL, Default }
    
    public GameObject player;

    public RopeSpawner spawner; 

    //public float detectingRange = 10f;
    //public float ropeLength = 15f;

    //public float rotationSpeed = 180f;
    //public float swingSpeed = 30f;
    public Direction targetDirection = Direction.Default;
    public float targetAddYRotation;
    public float currentAddYRotation;


    //public float swingAngle = 60f;
    [SerializeField] GameObject ropeAnchor;
    GameObject rope;

    public bool isSwingForward = true;
    public bool isRotating = false;
    bool isRopeExisting = false;


    // Start is called before the first frame update
    void Start()
    {
        spawner = GetComponentInParent<RopeSpawner>();
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
        if(isRopeExisting)
        {
            Swing();
            SetRotate();
            CalculateDistance();
        }
    }

    void SetStartPoints()
    {

    }

    void FindStartPoints()
    {

    }

    void MakeRope()
    {
        ropeAnchor.transform.localScale = new Vector3(1, 1, 1) * (1f / spawner.transform.localScale.x);
        rope = new GameObject("Rope");  // 플레이어가 매달릴 끝 생성
        rope.transform.parent = ropeAnchor.transform;
        Vector3 localPos = Vector3.zero;
        localPos.y = -spawner.ropeLength;
        rope.transform.localPosition = localPos;
        player.GetComponent<PlayerController3D>().enabled = false; // 플레이어를 로프로 이동
        player.transform.parent = rope.transform;
        player.transform.localPosition = Vector3.zero;
        player.transform.LookAt(player.transform.position + rope.transform.forward);

        player.transform.parent = rope.transform;  // 플레이어를 로프에 종속

        Vector3 localRot = Vector3.zero;
        localRot.x = -spawner.swingAngle;
        ropeAnchor.transform.localRotation = Quaternion.Euler(localRot);

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
