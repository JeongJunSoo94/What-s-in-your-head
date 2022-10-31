using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YC_OBJ;

public class OBJ_CT_BASE_SMB : StateMachineBehaviour
{
    ContaminatedTree Obj;
    public ContaminatedTree GetObjController(Animator animator)
    {
        if (Obj == null)
        {
            Obj = animator.gameObject.GetComponent<ContaminatedTree>();
        }
        return Obj;
    }
}
