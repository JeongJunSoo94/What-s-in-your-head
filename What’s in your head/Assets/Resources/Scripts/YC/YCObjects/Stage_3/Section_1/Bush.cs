using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// 
/// Current Issue       : 
/// 
/// Object Name         : 덤불
/// 
/// Object Description  : 이동을 위한 단순 파괴 오브젝트, 플레이어의 경로를 가로막는 장애물로서 점프나 타기가 불가능 하다("커다란 투명한 충돌체로 만든다")
///                         덤불 오브젝트는 아래 스킬로 피격 당할시, 파괴모션을 출력하고 사라진다. (다시 생성 X)
/// 
/// Script Description  : 첫번째 자식 오브젝트는 이동을 막기위한 단순 투명 콜라이더, 두번째 자식 오브젝트는 실제 플레이어 스킬과 상호작용하는 덤불 오브젝트
///                        자식오브젝트가 충돌을 체크한 뒤, 해당 오브젝트에게 충돌 메시지를 보낸다. (탑 콜라이더가 있어서)
///                         
/// Player Intraction   : 스테디(돋보기 스킬), 넬라(기타)
/// 
/// </summary>

namespace YC_OBJ
{
    public class Bush : MonoBehaviour
    {
        public void Destroy_Cor()
        {
            Debug.Log("Destroy");
            StartCoroutine("DestroyAndEffect");
        }

        IEnumerator DestroyAndEffect()
        {
            yield return new WaitForSeconds(3f);
            Destroy(this.gameObject);
        }
    }
}
