using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discovery3D : MonoBehaviour
{
    [Range(0f, 180f)]
    [SerializeField] private float _viewAngle;

    [SerializeField]
    private float _discoveryRadius;

    [Range(-180f, 0f)]
    [SerializeField] private float _viewDirection;

    [SerializeField] private LayerMask viewTargetMask;

    //[SerializeField]
    Collider[] hitColliders;

    private RaycastHit[] rayHits;
    private Ray[] ray = new Ray[3];

    public Collider[] HitColliders { get { return hitColliders; } }

    public bool gizmoOn;

    public bool targetOn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (DiscoveryTargetBool3D())
        {
            targetOn=true;
        }
        else
        { 
            targetOn = false;
        }
    }
    void OnDrawGizmos()
    {   
        if(gizmoOn)
            OnDrawGizmosRay();
    }

    void OnDrawGizmosRay()
    {
        Vector3 originPos = transform.position;
        Vector3 RightDir = AngleToDirX(-_viewAngle + _viewDirection);
        Vector3 LeftDir = AngleToDirX(_viewAngle + _viewDirection);
        Vector3 lookDir = transform.forward;

        ray[0].origin = transform.position;
        ray[0].direction = LeftDir;
        ray[1].origin = transform.position;
        ray[1].direction = RightDir;
        ray[2].origin = transform.position;
        ray[2].direction = lookDir;

        Gizmos.DrawWireSphere(transform.position, _discoveryRadius);
        Debug.DrawRay(ray[0].origin, ray[0].direction * _discoveryRadius, Color.red);
        Debug.DrawRay(ray[1].origin, ray[1].direction * _discoveryRadius, Color.red);
        Debug.DrawRay(ray[2].origin, ray[2].direction * _discoveryRadius, Color.green);
    }

    private Vector3 AngleToDirX(float angleInDegree)
    {
        float radian = (angleInDegree - transform.eulerAngles.y) * Mathf.Deg2Rad;//호도법 계산식
        return new Vector3(Mathf.Sin(-radian), 0, Mathf.Cos(-radian));
    }

    public bool DiscoveryTargetBool3D()
    {
        Vector3 originPos = transform.position;
        int count = 0;
        hitColliders = Physics.OverlapSphere(transform.position, _discoveryRadius, viewTargetMask);

        for (int index = 0; index < hitColliders.Length; index++)
        {
            if (hitColliders[index] != null)
            {
                Vector3 targetPos = hitColliders[index].transform.position;
                Vector3 dir = (targetPos - originPos).normalized;//(타겟의 위치값 - 오브젝트의 위치값).백터값
                Vector3 lookDir = AngleToDirX(_viewDirection);

                // float angle = Vector3.Angle(lookDir, dir)
                // 아래 두 줄은 위의 코드와 동일하게 동작함. 내부 구현도 동일
                float dot = Vector3.Dot(lookDir, dir);
                float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

                if (angle <= _viewAngle)
                {
                    count++;
                    {
                        Debug.DrawLine(originPos, targetPos, Color.red);
                    }
                }
            }
        }

        if (count == 0)
        {
            return false;
        }
        return true;
    }
}
