using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSpawner : MonoBehaviour
{
    //public List<GameObject> playerList;

    [SerializeField] GameObject ropeAction;

    [SerializeField] SphereCollider detectingCollider;
    [SerializeField] SphereCollider interactingCollider;

    public float detectingRange = 30f;
    public float interactableRange = 20f;

    public float ropeLength = 15f;

    public float rotationSpeed = 180f;

    public float swingSpeed = 30f;
    public float swingAngle = 60f;

    // Start is called before the first frame update
    void Start()
    {
        InitCollider();
    }

    void InitCollider()
    {
        interactingCollider.gameObject.transform.localScale = new Vector3(1, 1, 1) * interactableRange;
        detectingCollider.gameObject.transform.localScale = new Vector3(1, 1, 1) * detectingRange;
    }

    void SetRadius(SphereCollider collider, float range)
    {
        collider.center = Vector3.zero;
        collider.radius = range;
    }

    public void StartRopeAction(GameObject player)
    {
        GameObject obj = Instantiate<GameObject>(ropeAction, transform);
        obj.GetComponent<RopeAction>().spawner = this;
        obj.GetComponent<RopeAction>().player = player;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.forward * 5f);
    }
}
