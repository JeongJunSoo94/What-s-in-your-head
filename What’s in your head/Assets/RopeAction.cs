using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAction : MonoBehaviour
{
    public enum Direction { F, FR, R, FL, Default }

    public float rotationSpeed = 180f;
    public float swingSpeed = 30f;
    public Direction targetDirection = Direction.Default;
    public float targetYRotation;
    public float currentYRotation;

    public float swingAngle = 60f;

    public GameObject rope;

    public bool isSwingForward = true;
    public bool isRotating = false;

    // Start is called before the first frame update
    void Start()
    {
        ItTakesTwoKeyManager.Instance.GetKey(KeyName.W);
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
        SetRotate();
    }

    void Swing()
    {
        float rotationX = rope.transform.localRotation.eulerAngles.x;
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

        rope.transform.localRotation = Quaternion.Euler(Vector3.right * rotationX);
    }

    void InputKey()
    {
        bool F = ItTakesTwoKeyManager.Instance.GetKey(KeyName.W);
        bool B = ItTakesTwoKeyManager.Instance.GetKey(KeyName.S);
        bool L = ItTakesTwoKeyManager.Instance.GetKey(KeyName.A);
        bool R = ItTakesTwoKeyManager.Instance.GetKey(KeyName.D);

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
            targetYRotation = (int)targetDirection * 45;
            Debug.Log("(int)targetDirection = " + (int)targetDirection);
            Debug.Log("targetYRotation = " + (int)targetDirection * 45);
            isRotating = true;
        }
    }

    void SetRotate()
    {
        if (isRotating)
        {
            if (Mathf.Abs(currentYRotation - targetYRotation) < 3f)
            {
                isRotating = false;
                return;
            }

            if (currentYRotation > targetYRotation)
            {
                currentYRotation -= rotationSpeed * Time.fixedDeltaTime;
            }
            else
            {
                currentYRotation += rotationSpeed * Time.fixedDeltaTime;
            }
            Vector3 rotation = Vector3.up * currentYRotation;
            transform.rotation = Quaternion.Euler(rotation);
        }
    }
}
