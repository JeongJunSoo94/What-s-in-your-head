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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(tag1) ||
                other.gameObject.CompareTag(tag2))
            {
                Maze.EscapeMaze();
            }
        }
    }
}
