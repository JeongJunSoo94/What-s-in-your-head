using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Camera m_Camera;
    public float rangeRadius = 5f;
    public float rangeDistance = 5f;
    public LayerMask targetLayer;
    public Ray ray;
    RaycastHit _raycastHit;
    public Vector3 hVision;

    public bool isFound = false;
    bool isRidingRope = false;

    Vector3 right;

    Vector3 startCenter;
    Vector3 startUp;
    Vector3 startDown;
    Vector3 startLeft;
    Vector3 startRight;

    Vector3 endCenter;
    Vector3 endUp;
    Vector3 endDown;
    Vector3 endLeft;
    Vector3 endRight;


    public List<GameObject> detectedRopeSpawners;
    public List<GameObject> interactableRopeSpawners;

    GameObject minDistObj = null;


    // Start is called before the first frame update
    void Start()
    {
        //m_Camera = Camera.main;
        //ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        //SearchRail();
        RideOnOffRope();
    }

    void SearchRail()
    {
        MakeGizmoVecs();
        SearchWithSphereCast();
    }

    void MakeGizmoVecs()
    {
        right = m_Camera.transform.right;
    
        hVision = m_Camera.transform.forward;
    
        startCenter = m_Camera.transform.position;
        startCenter.y = 0;
        startUp = startCenter + m_Camera.transform.up * rangeRadius;
        startDown = startCenter - m_Camera.transform.up * rangeRadius;
        startLeft = startCenter - right * rangeRadius;
        startRight = startCenter + right * rangeRadius;
    
        endCenter = startCenter + hVision * rangeDistance;
        endUp = endCenter + Vector3.up * rangeRadius;
        endDown = endCenter - Vector3.up * rangeRadius;
        endLeft = endCenter - right * rangeRadius;
        endRight = endCenter + right * rangeRadius;
    }
    
    public GameObject SearchWithSphereCast()
    {
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    
        isFound = Physics.SphereCast(ray.origin, rangeRadius, ray.direction, out _raycastHit, rangeDistance, targetLayer, QueryTriggerInteraction.Ignore);
    
        //Debug.Log(_raycastHit.point);
        //Debug.Log(m_Camera.transform.position - hVision * Physics.defaultContactOffset);
        if (isFound)
        {
            Debug.Log("origin: " + ray.origin + ", distance: " + Vector3.Distance(ray.origin, _raycastHit.point));
        }
        //if(isFound)
        //{
        //    if(tag == _raycastHit.collider.tag)
        //    {
        //        return _raycastHit.collider.gameObject;
        //    }
        //}
    
        return null;
    }
    
    void RideOnOffRope()
    {
        if (!isRidingRope)
        {
            float minDist = 500f;
            minDistObj = null;
            if (interactableRopeSpawners.Count > 0)
            {
                if (JCW.Options.InputBindings.ITT_KeyManager.Instance.GetKeyDown(JCW.Options.InputBindings.PlayerAction.Interaction))
                {
                    foreach (var ropeSpawner in interactableRopeSpawners)
                    {
                        float dist = Vector3.Distance(transform.position, ropeSpawner.transform.position);
                        if (minDist > dist)
                        {
                            minDist = dist;
                            minDistObj = ropeSpawner;
                        }
                    }

                    if (minDistObj != null)
                    {
                        isRidingRope = true;
                        minDistObj.GetComponentInChildren<RopeSpawner>().StartRopeAction(this.gameObject);
                    }
                }
            }
        }
        else
        {
            if (JCW.Options.InputBindings.ITT_KeyManager.Instance.GetKeyDown(JCW.Options.InputBindings.PlayerAction.Interaction))
            {
                isRidingRope = false;
                minDistObj.GetComponentInChildren<RopeAction>().DestroyRope();
            }
        }
        
    }

    void GetOffRope()
    {

    }
    
    private void OnDrawGizmos()
    {
        if (isFound)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_raycastHit.point, 1f);
        }
    
        //{
        //    Gizmos.DrawLine(startUp, endUp);
        //    Gizmos.DrawLine(startDown, endDown);
        //    Gizmos.DrawLine(startRight, endRight);
        //    Gizmos.DrawLine(startLeft, endLeft);
    
        //    Gizmos.DrawWireSphere(startCenter, rangeRadius);
        //    Gizmos.DrawWireSphere(endCenter, rangeRadius);
        //}
    }
    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "RopeDetector":
                detectedRopeSpawners.Add(other.gameObject.transform.parent.gameObject);
                break;
            case "RopeSpawner":
                detectedRopeSpawners.Remove(other.gameObject.transform.parent.gameObject);
                interactableRopeSpawners.Add(other.gameObject.transform.parent.gameObject);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "RopeDetector":
                detectedRopeSpawners.Remove(other.gameObject.transform.parent.gameObject);
                break;
            case "RopeSpawner":
                interactableRopeSpawners.Remove(other.gameObject.transform.parent.gameObject);
                detectedRopeSpawners.Add(other.gameObject.transform.parent.gameObject);
                break;
        }
    }
}
