using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Common
{
    [DefaultExecutionOrder(2500)]
    public class RoyalePhysicsUpdater : MonoBehaviour
    {
        private const float updateThreshold = 1E-05f;

        private void Update()
        {
            if (Physics.autoSimulation)
            {
                return;
            }
            if (Time.timeScale == 0f)
            {
                return;
            }

            var deltaTime = Mathf.Max(Time.deltaTime, updateThreshold);
            Physics.Simulate(deltaTime);
        }
    }
}