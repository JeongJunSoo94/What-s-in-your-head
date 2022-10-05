using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController3D : MonoBehaviour
{
    public Rigidbody _rigidbody;
    public Vector3 worldMoveDir;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
       Move();
    }

    private void Move()
    {
        _rigidbody.velocity = worldMoveDir;
    }
}
