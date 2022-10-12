using UnityEngine;

public class SpacebarToFlicker : MonoBehaviour
{
   public LightFlickering lf;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            lf.flicker();
        }
    }
}
