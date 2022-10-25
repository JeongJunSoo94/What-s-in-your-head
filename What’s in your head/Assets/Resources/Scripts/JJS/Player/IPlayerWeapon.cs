using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS
{
    public interface IPlayerWeapon
    {
        void LeftClick();
        void LeftNotClick();
        void RightClick();
        void RightNotClick();
    }
}