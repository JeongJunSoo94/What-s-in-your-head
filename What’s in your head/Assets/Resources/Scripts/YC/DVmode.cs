using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSU;

public class DVmode : MonoBehaviour
{
    float NellaSpeed = 0;
    float SteadySpeed = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Nella")
        {
            NellaSpeed = collision.gameObject.GetComponent<PlayerController>().walkSpeed;
            collision.gameObject.GetComponent<PlayerController>().walkSpeed = 70;
        }
        if (collision.gameObject.tag == "Steady")
        {
            SteadySpeed = collision.gameObject.GetComponent<PlayerController>().walkSpeed;
            collision.gameObject.GetComponent<PlayerController>().walkSpeed = 70;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Nella")
        {
            collision.gameObject.GetComponent<PlayerController>().walkSpeed = NellaSpeed;
        }
        if (collision.gameObject.tag == "Steady")
        {
            collision.gameObject.GetComponent<PlayerController>().walkSpeed = SteadySpeed;
        }
    }
}
