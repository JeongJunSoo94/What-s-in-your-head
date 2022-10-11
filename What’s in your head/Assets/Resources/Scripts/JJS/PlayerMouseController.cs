using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
public class PlayerMouseController : MonoBehaviour
{
    public bool ableToLeft;
    public bool ableToRight;
    private void Start()
    {
        ableToLeft = false;
        ableToRight = false;
    }

    public virtual void CheckLeftClick(bool enable)
    {
    }

    public virtual void CheckRightClick(bool enable)
    {
    }
}
