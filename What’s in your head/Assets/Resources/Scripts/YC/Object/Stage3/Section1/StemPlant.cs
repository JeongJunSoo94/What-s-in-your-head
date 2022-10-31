using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// 
/// Current Issue       : 기획 수정 예정 
/// 
/// Object Name         : 줄기 식물
/// 
/// Object Description  : 넬라의 물총을 맞게 된다면 '로프 혹은 레일' 오브젝트가 서서히 내려온다.
/// 
/// Script Description  : 이 오브젝트는 자식 오브젝트로 로프 혹은 레일을 갖고 있다(Section_1에서는 로프, Section_2에서는 레일을 갖고 있다.)
///                         OnTriggerEnter() 호출시 자식 오브젝트를 SetActive(true)해줄 뿐이다.
///                         
/// Player Intraction   : 넬라(물총 스킬)
/// 
/// </summary>

namespace YC_OBJ
{
    public class StemPlant : MonoBehaviour
    {
        string interactionObjTag = "NellaWater";

        GameObject Obj;

        void Awake()
        {
            Obj = transform.GetChild(1).gameObject;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(interactionObjTag))
            {
                Obj.SetActive(true);
            }
        }
    }
}
