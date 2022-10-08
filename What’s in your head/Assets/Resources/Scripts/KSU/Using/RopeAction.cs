using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Options.InputBindings;

public class RopeAction : MonoBehaviour
{
    public GameObject player;


    public enum Direction { F, FR, R, BR, B, BL, L, FL, Default }

    public float detectingRange = 10f;
    public float ropeLength = 15f;

    public float rotationSpeed = 180f;
    public float swingSpeed = 30f;
    public Direction targetDirection = Direction.Default;
    public float targetAddYRotation;
    public float currentAddYRotation;


    public float swingAngle = 60f;
    [SerializeField] GameObject ropeAnchor;
    GameObject rope;

    public bool isSwingForward = true;
    public bool isRotating = false;
    bool isRopeExisting = false;


    // Start is called before the first frame update
    void Start()
    {
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
        }
    }

    void MakeRope()
    {
        ropeAnchor = new GameObject("RopeAnchor"); // swing � �� ����
        ropeAnchor.transform.parent = this.transform;
        ropeAnchor.transform.localPosition = Vector3.zero;

        rope = new GameObject("Rope");  // �÷��̾ �Ŵ޸� �� ����
        rope.transform.parent = ropeAnchor.transform;
        Vector3 localPos = Vector3.zero;
        localPos.y = -ropeLength;
        rope.transform.localPosition = localPos;

        player.GetComponent<PlayerController3D>().enabled = false; // �÷��̾ ������ �̵�
        player.transform.position = rope.transform.position;
        player.transform.LookAt(player.transform.position + rope.transform.forward);

        player.transform.parent = rope.transform;  // �÷��̾ ������ ����

        Vector3 localRot = Vector3.zero;
        localRot.x = -swingAngle;
        ropeAnchor.transform.localRotation = Quaternion.Euler(localRot);

        isRopeExisting = true;
    }

    float FitInHalfDegree(float Angle)
    {
        return (Angle > 180) ? (Angle - 360f) : Angle;
    }

    void Swing()
    {
        // -180 < rot X <= 180 ���̷� ���� 
        float rotationX = FitInHalfDegree(ropeAnchor.transform.localRotation.eulerAngles.x);

        if (isSwingForward) // ������ ���� �ڷΰ��� ����
        {
            rotationX += swingSpeed * Time.fixedDeltaTime;
            if (rotationX > swingAngle)
            {
                isSwingForward = false;
            }
        }
        else
        {
            rotationX -= swingSpeed * Time.fixedDeltaTime;
            if (rotationX < -swingAngle)
            {
                isSwingForward = true;
            }
        }

        // rotation�� ����
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
            // ��ǥ �������� �����ϸ� ȸ�� ����
            if (Mathf.Abs(currentAddYRotation - targetAddYRotation) < 3f)
            {
                isRotating = false;
                return;
            }

            //  ��ǥ �������� �ð�, �ݽð� �� ����� �������� ȸ��
            if (currentAddYRotation > targetAddYRotation)
            {
                if ((currentAddYRotation - targetAddYRotation) > (targetAddYRotation + 360f - currentAddYRotation))
                {
                    currentAddYRotation += rotationSpeed * Time.fixedDeltaTime;
                }
                else
                {
                    currentAddYRotation -= rotationSpeed * Time.fixedDeltaTime;
                }
            }
            else
            {
                if ((currentAddYRotation + 360f - targetAddYRotation) > (targetAddYRotation - currentAddYRotation))
                {
                    currentAddYRotation += rotationSpeed * Time.fixedDeltaTime;
                }
                else
                {
                    currentAddYRotation -= rotationSpeed * Time.fixedDeltaTime;
                }
            }

            // 0 <= Y rotation <= 360 ���̷� ����
            if (currentAddYRotation < 0)
            {
                currentAddYRotation += 360f;
            }    
            else if(currentAddYRotation > 360f)
            {
                currentAddYRotation -= 360f;
            }

            // rotation ����
            transform.rotation = Quaternion.Euler(Vector3.up * currentAddYRotation);
        }
    }

    
}
