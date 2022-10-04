using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.InputBindings;
public class NellaMouseController : PlayerMouseController
{
    public List<GameObject> hitObj;
    private void Start()
    {
    }

    public override void CheckLeftClick()
    {
        if (ITT_KeyManager.Instance.GetKey(PlayerAction.Fire))
        {
            leftOn = true;
        }
        else
        {
            leftOn = false;
        }
    }

    public override void CheckRightClick()
    {
        if (ITT_KeyManager.Instance.GetKey(PlayerAction.Aim))
        {
            rightOn = true;
        }
        else
        {
            rightOn = false;
        }
    }

    public override void CheckLeftDownClick()
    {
        if (ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Fire))
        {
            leftDown = true;
        }
        else
        {
            leftDown = false;
        }
    }

    public override void CheckRightDownClick()
    {
        if (ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Aim))
        {
            rightDown = true;
        }
        else
        {
            rightDown = false;
        }
    }

    public void OnEnableObject(int index)
    {
        hitObj[index].SetActive(true);
    }

    public void OnDisableObject(int index)
    {
        hitObj[index].SetActive(false);
    }


    public void AttackTime()
    {
        ableToLeft= !ableToLeft;
    }
}
