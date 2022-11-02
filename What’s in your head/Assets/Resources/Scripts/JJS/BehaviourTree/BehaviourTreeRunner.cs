using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS.BT
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public BehaviourTree tree;

        void Start()
        {
            tree = tree.Clone(GetComponent<ObjectInfo>());
        }

        // Update is called once per frame
        void Update()
        {
            tree.Update();
        }
    }
}
