using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YC_OBJ;

public class Base : StateMachineBehaviour
{
    LeafPlant obj;

    public LeafPlant GetLeafPlant(Animator animator)
    {
        if (obj == null)
        {
            obj = animator.gameObject.GetComponent<LeafPlant>();
        }
        return obj;
    }
}
