using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
	//ParticleSystem particle;
	public Vector3 particlePosition;

    //private void Awake()
    //   {
    //	particle = GetComponent<ParticleSystem>();
    //   }

    private void Update()
    {
        transform.position = particlePosition;
    }


    //   void OnEnable()
    //{
    //	StartCoroutine("CheckIfAlive");
    //}

    //IEnumerator CheckIfAlive()
    //{
    //	while (true)
    //	{
    //		yield return new WaitForSeconds(0.5f);
    //		if (particle.isStopped)
    //		{
    //			this.gameObject.SetActive(false);
    //			yield break;
    //		}
    //	}
    //}
}
