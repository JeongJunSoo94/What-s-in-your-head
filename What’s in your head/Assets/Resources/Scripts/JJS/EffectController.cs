using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Spawner;
using JCW.Object;

namespace JJS
{
    public class EffectController : MonoBehaviour
    {
        public ParticleSystem particle;
        public Spawner effectSpawner;
        private void Awake()
        {
            particle = GetComponent<ParticleSystem>();
        }
        // Update is called once per frame
        void Update()
        {
            EffectPlaying();
        }
        void EffectPlaying()
        {
            if (particle.isStopped)
            { 
                effectSpawner.Despawn(gameObject);
            }
        }
    }
}
