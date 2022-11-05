using UnityEngine;

public class OrbitingCameraInput : MonoBehaviour
{
    public Vector2 GetCameraInput()
    {
        var x = Input.GetAxis("Mouse X");
        var y = Input.GetAxis("Mouse Y");

        return new Vector2(x, y);
    }
}
