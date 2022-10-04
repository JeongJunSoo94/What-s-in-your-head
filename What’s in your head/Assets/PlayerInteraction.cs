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
    RaycastHit[] _raycastHits;
    public Vector3 hVision;

    public bool isFound = false;

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






    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;
        MakeVecs();
        ray = new Ray(startCenter, hVision);
    }

    // Update is called once per frame
    void Update()
    {
        MakeVecs();
        FindInteractableObj();
    }

    void MakeVecs()
    {
        right = m_Camera.transform.right;
        right.y = 0;
        right = right.normalized;

        hVision = m_Camera.transform.forward;
        hVision.y = 0;
        hVision = hVision.normalized;

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

    public GameObject FindInteractableObj()
    {
        ray.origin = startCenter;
        
        hVision = m_Camera.transform.forward;
        hVision.y = 0;
        hVision = hVision.normalized;
        
        ray.direction = hVision;

        _raycastHits = Physics.SphereCastAll(ray, rangeRadius, rangeDistance, targetLayer, QueryTriggerInteraction.Ignore);

        //Debug.Log(_raycastHit.point);
        //Debug.Log(m_Camera.transform.position - hVision * Physics.defaultContactOffset);
        if(_raycastHits.Length > 0)
        {
            Debug.Log("origin: " + ray.origin + ", distance: " + Vector3.Distance(ray.origin, _raycastHits[0].point));
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


    private void OnDrawGizmos()
    {
        if (_raycastHits.Length > 0)
        {
            foreach(var rayHit in _raycastHits)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(rayHit.point, 1f);
            }
        }

        {
            Gizmos.DrawLine(startUp, endUp);
            Gizmos.DrawLine(startDown, endDown);
            Gizmos.DrawLine(startRight, endRight);
            Gizmos.DrawLine(startLeft, endLeft);

            Gizmos.DrawWireSphere(startCenter, rangeRadius);
            Gizmos.DrawWireSphere(endCenter, rangeRadius);
        }
    }
}
