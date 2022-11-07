using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object.Stage1
{
    public class ColorBlock : MonoBehaviour
    {
        [Header("���� �÷�")]
        [Header("0~6 : ����~����")] [SerializeField] [Range(0,6)] int colorNum;
                
        private void Awake()
        {
            for (int i =0 ; i<7 ; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(i == colorNum);
            }
            
        }

        public void SetActiveCheck(bool isAcitve)
        {
            transform.GetChild(colorNum).gameObject.SetActive(isAcitve);
        }
    }
}