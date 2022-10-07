using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAction : MonoBehaviour
{
    //public enum Direction { F, FR, R, FL, Default }

    //public float detectingRange = 10f;
    //public float ropeLength = 8f;

    //public float rotationSpeed = 180f;
    //public float swingSpeed = 30f;
    //public Direction targetDirection = Direction.Default;
    //public float targetAddYRotation;
    //public float currentAddYRotation;


    //public float swingAngle = 60f;
    //GameObject rope;

    //public bool isSwingForward = true;
    //public bool isRotating = false;

    //Camera m_Camera;
    //Vector3 forwardVec = Vector3.zero;
    //public float currentFowardYRotation;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    ItTakesTwoKeyManager.Instance.GetKey(KeyName.W);
    //    m_Camera = Camera.main;
    //    MakeRope();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (!isRotating)
    //    {
    //        InputKey();
    //    }
    //}

    //private void FixedUpdate()
    //{
    //    Swing();
    //    MakeForwardVec();
    //    SetRotate();
    //}

    //void MakeRope()
    //{
    //    rope = new GameObject(); // 로프 생성 해야함
    //}

    //void Swing()
    //{
    //    float rotationX = rope.transform.localRotation.eulerAngles.x;
    //    if (rotationX > 180)
    //    {
    //        rotationX = rotationX - 360f;
    //    }

    //    if (isSwingForward)
    //    {
    //        rotationX += swingSpeed * Time.fixedDeltaTime;
    //        if (rotationX > swingAngle)
    //        {
    //            isSwingForward = false;
    //        }
    //    }
    //    else
    //    {
    //        rotationX -= swingSpeed * Time.fixedDeltaTime;
    //        if (rotationX < -swingAngle)
    //        {
    //            isSwingForward = true;
    //        }
    //    }

    //    rope.transform.localRotation = Quaternion.Euler(Vector3.right * rotationX);
    //}

    //void InputKey()
    //{
    //    bool F = ItTakesTwoKeyManager.Instance.GetKey(KeyName.W);
    //    bool B = ItTakesTwoKeyManager.Instance.GetKey(KeyName.S);
    //    bool L = ItTakesTwoKeyManager.Instance.GetKey(KeyName.A);
    //    bool R = ItTakesTwoKeyManager.Instance.GetKey(KeyName.D);

    //    Direction input = targetDirection;
    //    if ((F && L) || (B && R))
    //    {
    //        input = Direction.FL;
    //    }
    //    else if ((F && R) || (B && L))
    //    {
    //        input = Direction.FR;
    //    }
    //    else if (F || B)
    //    {
    //        input = Direction.F;
    //    }
    //    else if (L || R)
    //    {
    //        input = Direction.R;
    //    }

    //    if (input != targetDirection)
    //    {
    //        targetDirection = input;
    //        targetAddYRotation = (int)targetDirection * 45;
    //        Debug.Log("(int)targetDirection = " + (int)targetDirection);
    //        Debug.Log("targetYRotation = " + (int)targetDirection * 45);
    //        isRotating = true;
    //    }
    //}

    //void MakeForwardVec()
    //{
    //    forwardVec = m_Camera.transform.forward;
    //    forwardVec.y = 0;
    //}

    //void SetRotate()
    //{
    //    Vector3 forward = transform.forward;
    //    forward.y = 0;
    //    if(forwardVec != forward)
    //    {
    //        transform.LookAt(transform.position + forwardVec);
    //        targetDirection = Direction.F;
    //        currentAddYRotation = 0f;
    //        targetAddYRotation = 0f;
    //        isRotating = false;
    //    }

    //    if (isRotating)
    //    {          
    //        if (Mathf.Abs(currentAddYRotation - targetAddYRotation) < 3f)
    //        {
    //            isRotating = false;
    //            return;
    //        }

    //        if (currentAddYRotation > targetAddYRotation)
    //        {
    //            currentAddYRotation -= rotationSpeed * Time.fixedDeltaTime;
    //        }
    //        else
    //        {
    //            currentAddYRotation += rotationSpeed * Time.fixedDeltaTime;
    //        }
    //        Vector3 AddYRotation = Vector3.up * currentAddYRotation;
    //        Vector3 currentYRotation = transform.rotation.eulerAngles + AddYRotation;
    //        transform.rotation = Quaternion.Euler(currentYRotation);
    //    }
    //}

    
}
