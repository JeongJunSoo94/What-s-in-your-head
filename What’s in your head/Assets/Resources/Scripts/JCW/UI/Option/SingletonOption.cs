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
    }
}

