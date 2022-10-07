using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Options.InputBindings;

public class RopeAction : MonoBehaviour
{
    public GameObject player;

    public enum Direction { F, FR, R, FL, Default }

    public float detectingRange = 10f;
    public float ropeLength = 8f;

    public float rotationSpeed = 180f;
    public float swingSpeed = 30f;
    public Direction targetDirection = Direction.Default;
    public float targetAddYRotation;
    public float currentAddYRotation;


    public float swingAngle = 60f;
    GameObject ropeAnchor;
    GameObject rope;

    public bool isSwingForward = true;
    public bool isRotating = false;

    Camera m_Camera;
    Vector3 forwardVec = Vector3.zero;
    public float currentFowardYRotation;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;
        MakeRope();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRotating)
        {
            InputKey();
        }
    }

    private void FixedUpdate()
    {
        Swing();
        MakeForwardVec();
        SetRotate();
    }

    void MakeRope()
    {
        ropeAnchor = new GameObject("RopeAnchor");
        ropeAnchor.transform.parent = this.transform;
        ropeAnchor.transform.localPosition = Vector3.zero;
        rope = new GameObject("Rope"); // 로프 생성 해야함
        rope.transform.parent = ropeAnchor.transform;
        Vector3 localPos = Vector3.zero;
        localPos.y = -10f;
        rope.transform.localPosition = localPos;
        player.GetComponent<PlayerController3D>().enabled = false;
        player.transform.position = rope.transform.position;
        player.transform.LookAt(player.transform.position + rope.transform.forward);
        player.transform.parent = rope.transform;
        Vector3 localRot = Vector3.zero;
        localRot.x = -swingAngle;
        rope.transform.rotation = Quaternion.Euler(localRot);


    }

    void Swing()
    {
        float rotationX = ropeAnchor.transform.localRotation.eulerAngles.x;
        if (rotationX > 180)
        {
            rotationX = rotationX - 360f;
        }

        if (isSwingForward)
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

        ropeAnchor.transform.localRotation = Quaternion.Euler(Vector3.right * rotationX);
    }

    void InputKey()
    {
        bool F = ITT_KeyManager.Instance.GetKey(PlayerAction.MoveForward);
        bool B = ITT_KeyManager.Instance.GetKey(PlayerAction.MoveBackward);
        bool L = ITT_KeyManager.Instance.GetKey(PlayerAction.MoveLeft);
        bool R = ITT_KeyManager.Instance.GetKey(PlayerAction.MoveRight);

        Direction input = targetDirection;
        if ((F && L) || (B && R))
        {
            input = Direction.FL;
        }
        else if ((F && R) || (B && L))
        {
            input = Direction.FR;
        }
        else if (F || B)
        {
            input = Direction.F;
        }
        else if (L || R)
        {
            input = Direction.R;
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

    void MakeForwardVec()
    {
        forwardVec = m_Camera.transform.forward;
        forwardVec.y = 0;
    }

    void SetRotate()
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        if(forwardVec != forward)
        {
            transform.LookAt(transform.position + forwardVec);
            targetDirection = Direction.F;
            currentAddYRotation = 0f;
            targetAddYRotation = 0f;
            isRotating = false;
        }

        if (isRotating)
        {          
            if (Mathf.Abs(currentAddYRotation - targetAddYRotation) < 3f)
            {
                isRotating = false;
                return;
            }

            if (currentAddYRotation > targetAddYRotation)
            {
                currentAddYRotation -= rotationSpeed * Time.fixedDeltaTime;
            }
            else
            {
                currentAddYRotation += rotationSpeed * Time.fixedDeltaTime;
            }
            Vector3 AddYRotation = Vector3.up * currentAddYRotation;
            Vector3 currentYRotation = transform.rotation.eulerAngles + AddYRotation;
            transform.rotation = Quaternion.Euler(currentYRotation);
        }
    }

    
}
