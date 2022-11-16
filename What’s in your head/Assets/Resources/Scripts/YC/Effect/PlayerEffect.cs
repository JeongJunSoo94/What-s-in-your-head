using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YC.Effect
{
    public class PlayerEffect : MonoBehaviour
    {
        public GameObject dash;

        public DashEffect dashEffect { get; private set; }

        void Awake()
        {
            dashEffect = dash.GetComponent<DashEffect>();
        }
    
    }

}
