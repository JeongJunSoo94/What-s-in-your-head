using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// 
/// Current Issue       : 없음
/// 
/// Object Name         : 덩굴
/// 
/// Object Description  : 오염된 나무 오브젝트를 전부 정화할시, 열리게 되는 오브젝트이다. (출입문이라고 생각)
/// 
/// Script Description  : 스타트에서 오염된 나무들을 찾아 온 다음, 업데이트에서 오염된 나무들의 정화여부를 체크한다. 
///                         모든 나무가 정화된다면 문이 열리는 코루틴 함수를 호출한다.
///                         
/// Player Intraction   : 없음
/// 
/// </summary>

namespace YC_OBJ
{
    public class Vine : MonoBehaviour
    {
        [Header("<기획 편집 사항>")]
        [Space]

        [Tooltip("회전 속도")]
        [SerializeField] float rotSpeed;

        [Tooltip("문이 열리기까지의 딜레이 시간")]
        [SerializeField] float openDelay;

        Transform leftAxis;     // 왼쪽 덩굴
        Transform rightAxis;    // 오른쪽 덩굴

        float MaxAxis = 90;

        float leftAsixMax;
        float rightAsixMax;


        [HideInInspector] public bool isOpen { get; private set; }
        [HideInInspector] public List<ContaminatedTree> contaminatedTrees { get; private set; }

        string findObjTag = "ContTree";

        void Awake()
        {
            leftAxis = transform.GetChild(0).gameObject.GetComponent<Transform>();
            rightAxis = transform.GetChild(1).gameObject.GetComponent<Transform>();

            isOpen = false;

            contaminatedTrees = new List<ContaminatedTree>();

            leftAsixMax = 360 - MaxAxis;
            rightAsixMax = MaxAxis;
        }

        void Start()
        {
            FindTrees();
        }

        void Update()
        {
            CheckIsPure();

            if (isOpen)
            {
                StartCoroutine("OpenPassage");
                isOpen = false;
            }
        }

        void FindTrees()
        {
            GameObject[] temp = GameObject.FindGameObjectsWithTag("ContTree");

            foreach (GameObject obj in temp)
            {
                contaminatedTrees.Add(obj.GetComponent<ContaminatedTree>());
            }
        }

        void CheckIsPure()
        {
            if (isOpen) return; // 중복 실행 방지

            foreach (ContaminatedTree tree in contaminatedTrees)
            {
                if (!tree.isPure)
                    return;
            }

            isOpen = true;
        }


        IEnumerator OpenPassage()
        {
            yield return new WaitForSeconds(openDelay);

            while (rightAxis.rotation.eulerAngles.z < rightAsixMax)
            {
                leftAxis.transform.Rotate(new Vector3(0, 0, -rotSpeed * Time.deltaTime));
                rightAxis.transform.Rotate(new Vector3(0, 0, rotSpeed * Time.deltaTime));

                if (rightAxis.rotation.eulerAngles.z > rightAsixMax) 
                    rightAxis.rotation = Quaternion.Euler(0, 0, rightAsixMax);
              
                yield return null;
            }
        }
    }
}
