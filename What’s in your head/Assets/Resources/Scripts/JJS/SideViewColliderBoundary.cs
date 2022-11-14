using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.Camera_;

public class SideViewColliderBoundary : MonoBehaviour
{
    public Camera cameraMain;
    protected Ray ray;
    protected RaycastHit hit;
    public LayerMask mouseSideLayer;
    public bool left;

    void Update()
    {
        if(cameraMain!=null)
            SideViewUpdate();
    }

    public virtual void SideViewUpdate()
    {
        Vector3 pos;
        if (left)
        {
            pos = new Vector3(0, Screen.height*0.5f, 0);
        }
        else
        {
            pos = new Vector3(Screen.width, Screen.height * 0.5f, 0);
        }
        ray = cameraMain.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out hit, 1000f, mouseSideLayer, QueryTriggerInteraction.Ignore))
        {
            transform.position = hit.point;
        }
    }
}
