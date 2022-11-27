using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object
{
    public class ActivatedRail : MonoBehaviour
    {
        [SerializeField] Rail rail;

        // Start is called before the first frame update
        private void OnDisable()
        {
            rail.EscapeAll();
        }

    }
}
