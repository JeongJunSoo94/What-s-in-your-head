using JJS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.Camera_;

public class CameraTrigger : MonoBehaviour
{
    public float lerpTime;
    public float height;
    public string layerText;
    public bool enable = false;
    public CharacterBuilder builder;
    private void OnTriggerEnter(Collider other)
    {
        if (!enable && other.gameObject.layer == LayerMask.NameToLayer(layerText))
        {
            builder.nella.GetComponent<CameraController>().LerpPlatformHeight_Cor(lerpTime, height);
            builder.steady.GetComponent<CameraController>().LerpPlatformHeight_Cor(lerpTime, height);
            enable = true;
            //other.GetComponent<CameraController>().LerpPlatformHeight_Cor(lerpTime, height);
        }
    }

}
