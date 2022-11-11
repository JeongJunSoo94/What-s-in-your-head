using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YC_OBJ
{
    public class Maze : MonoBehaviour
    {
        [SerializeField] GameObject NoenEscape_DeadZone;
        [SerializeField] GameObject Escape_DeadZone;

        public void EscapeMaze()
        {
            NoenEscape_DeadZone.SetActive(false);
            Escape_DeadZone.SetActive(true);
        }
    }
}

