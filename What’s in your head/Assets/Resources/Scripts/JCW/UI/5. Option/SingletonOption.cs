using UnityEngine;

namespace JCW.UI.Options
{
    public class SingletonOption : MonoBehaviour
    {
        public static SingletonOption Instance = null;
        private void Awake()
        {
            if (Instance==null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else if (Instance != this)
                Destroy(this.gameObject);
        }

        private void OnEnable()
        {
            /*
            
            추후 : 현재 게임 중이면 Background 이미지 꺼주고, 메인 메뉴에서의 옵션은 켜주기

            */
        }
    }
}

