using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS.BT
{
    public class ObjectInfo : MonoBehaviour
    {
        GameObject prefabObject;

        private void Awake()
        {
            prefabObject = gameObject;
        }
        public GameObject PrefabObject
        {
            get
            {
                return prefabObject;
            }
            set
            {
                prefabObject = value;
            }
        }
    }
}
