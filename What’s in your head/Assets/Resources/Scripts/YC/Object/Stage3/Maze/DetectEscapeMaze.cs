using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YC_OBJ
{
    public class DetectEscapeMaze : MonoBehaviour
    {
        [SerializeField] Maze Maze;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Nella" ||
                other.gameObject.tag == "Steady")
            {
                Maze.EscapeMaze();
            }
        }
    }
}
