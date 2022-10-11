using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
public class PlayerMouseController : MonoBehaviour
{
    public bool leftDown;
    public bool rightDown;
    public bool leftOn;
    public bool rightOn;

    public bool ableToLeft;
    public bool ableToRight;
    private void Start()
    {
        leftDown = false;
        rightDown = false;

        leftOn = false;
        rightOn = false;

        ableToLeft = false;
        ableToRight = false;
    }

    public virtual void CheckLeftClick()
    {
    }

    public virtual void CheckRightClick()
    {
    }

    public virtual void CheckLeftDownClick()
    {
    }

    public virtual void CheckRightDownClick()
    {
    }
}
