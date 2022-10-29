using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS
{ 
    public class CharacterBuilder : MonoBehaviour
    {
        public GameObject nella;
        public GameObject steady;

        public void FindCharacter()
        {
            nella = GameObject.FindWithTag("Nella");
            steady = GameObject.FindWithTag("Steady");
        }

        public virtual void SetCharacterComponent()
        {
        }
        public void SetCharacterGameObject(GameObject findObject,out GameObject discoverObject, string findName)
        {
            discoverObject = null;
            Transform[] allChildren = findObject.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (child.name == findName)
                {
                    discoverObject = child.gameObject;
                    return;
                }
            }
        }
        public virtual void SetCharacter()
        {
        
        }
    }
}
