using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Effect
{
    public class MotionTrail : MonoBehaviour
    {
        [Header("잔상 개수")] [SerializeField] int trailCount = 50;
        [Header("잔상 남아있는 시간")] [SerializeField] float trailRemainTime = 1f;
        [Header("잔상 소환 시간 간격")] [SerializeField] float trailSpawnTime = 0.01f;
        [Header("잔상 메쉬 렌더러")] [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
        [Header("적용할 메테리얼")] [SerializeField] Material mat;

        GameObject TrailContainer;

        Queue<GameObject> objQueue = new();

        float curTime = 0f;
        int curSpawnIndex = 0;

        

        private void Awake()
        {
            TrailContainer = new("TrailContainer");
            for (int i= 0 ; i<trailCount ; ++i)
            {
                GameObject go = new("Trail" + i);
                go.transform.parent = TrailContainer.transform;
                go.SetActive(false);
                go.AddComponent<MeshRenderer>();
                go.GetComponent<MeshRenderer>().material = mat;
                go.AddComponent<MeshFilter>();
                objQueue.Enqueue(go);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(nameof(MakeTrail));
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            objQueue.Clear();
            for (int i=0 ; i<trailCount ; ++i)
            {
                GameObject go = TrailContainer.transform.GetChild(i).gameObject;
                go.SetActive(false);
                objQueue.Enqueue(go);
            }
        }

        // 잔상 생성
        IEnumerator MakeTrail()
        {
            while (curSpawnIndex < trailCount)
            {
                if (objQueue.Count != 0)
                {
                    GameObject go = objQueue.Dequeue();
                    go.transform.SetPositionAndRotation(transform.position, transform.rotation);
                    go.SetActive(true);

                    Mesh mesh = new Mesh();
                    skinnedMeshRenderer.BakeMesh(mesh);
                    go.GetComponent<MeshFilter>().mesh = mesh;
                    StartCoroutine(DeleteTrail(go));
                    yield return new WaitForSeconds(trailSpawnTime);
                    if (++curSpawnIndex >= trailCount)
                        curSpawnIndex = 0;
                }
                else
                    yield return null;
            }
        }

        // 잔상 제거
        IEnumerator DeleteTrail(GameObject trail)
        {
            yield return new WaitForSeconds(trailRemainTime);
            trail.SetActive(false);
            objQueue.Enqueue(trail);
            yield break;
        }
    }
}
