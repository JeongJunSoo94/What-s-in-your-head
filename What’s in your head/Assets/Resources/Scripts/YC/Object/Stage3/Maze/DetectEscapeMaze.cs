using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YC_OBJ
{
    public class DetectEscapeMaze : MonoBehaviour
    {
        [SerializeField] Maze Maze;
        string tag1 = "Nella";
        string tag2 = "Steady";

        BoxCollider col;

        private void Awake()
        {
            col = this.gameObject.GetComponent<BoxCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(tag1) ||
                other.gameObject.CompareTag(tag2))
            {
                Maze.EscapeMaze();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(tag1) ||
                other.gameObject.CompareTag(tag2))
            {
                col.isTrigger = false;
                Maze.EscapeMaze();
            }
        }
    }
}
