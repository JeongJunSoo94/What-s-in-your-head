using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    // Start is called before the first frame update
    public float RotationSpeed;
    public Vector3 RotationAxis;
    void Update()
    {
        transform.Rotate(RotationAxis * RotationSpeed*Time.deltaTime);
    }

   

}
