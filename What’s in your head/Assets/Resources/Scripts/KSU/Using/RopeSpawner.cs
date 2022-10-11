using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSpawner : MonoBehaviour
{
    //public List<GameObject> playerList;

    [SerializeField] GameObject ropeAction;

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
        transform.localScale = new Vector3(1, 1, 1) * (detectingRange * 2f);
    }

    public void StartRopeAction(GameObject player)
    {
        ropeAction.GetComponent<RopeAction>().player = player;
        ropeAction.SetActive(true);
        //GameObject obj = Instantiate<GameObject>(ropeAction, transform);
        //obj.GetComponent<RopeAction>().spawner = this;
        //obj.GetComponent<RopeAction>().player = player;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.forward * 5f);
    }
}
