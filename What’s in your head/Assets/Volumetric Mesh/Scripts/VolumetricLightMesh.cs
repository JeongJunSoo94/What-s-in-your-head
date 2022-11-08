using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class VolumetricLightMesh : MonoBehaviour
{
    //public Light lightSource;
    //public int quality = 10;
    //public float startOffset = .2f;
    //public float range = 2f;
    //public float spotAngle = 30f;
    //public bool useLightVariables = false;
    //public bool debug = false;

    //private void OnEnable()
    //{
    //    Camera.main.depthTextureMode = DepthTextureMode.Depth;
    //}

    //private void OnValidate() => GenerateMesh();

    //Vector3[] vertices;

    //public void GenerateMesh()
    //{
    //    UnityEngine.Debug.unityLogger.logEnabled = debug;
        
    //    float spotAngle = this.spotAngle;
    //    float range = this.range;

    //    if (useLightVariables)
    //    {
    //        if (lightSource == null) lightSource = GetComponent<Light>();

    //        spotAngle = lightSource.spotAngle;
    //        range = lightSource.range;
    //    }

    //    Mesh mesh = new Mesh();
    //    Bounds bounds = mesh.bounds;
    //    MeshFilter meshFilter = GetComponent<MeshFilter>();
    //    mesh.name = "Volumetric Light Mesh";

    //    float angle = 0f;
    //    float angleIncrease = 360f / quality;

    //    vertices = new Vector3[quality * 2 + 2];
    //    Vector2[] uv = new Vector2[vertices.Length];
    //    int[] triangles = new int[quality * 2 * 3];

    //    for (int i = 0; i < quality + 1; i++)
    //    {
    //        Vector3 vertex = GetVectorInConeFromAngle(angle, startOffset, spotAngle);
    //        vertices[i] = vertex;

    //        angle -= angleIncrease;
    //    }

    //    angle = 0f;

    //    for (int i = 0; i < quality + 1; i++)
    //    {
    //        Vector3 vertex = GetVectorInConeFromAngle(angle, range, spotAngle);
    //        vertices[i + quality + 1] = vertex;

    //        angle -= angleIncrease;
    //    }

    //    for (int i = 0; i <= quality; i++)
    //    {
    //        uv[i] = new Vector2(1f - 1f / (quality) * i, 1);
    //        uv[i + quality + 1] = new Vector2(1f - 1f / (quality) * i, 0);

    //        Debug.Log(uv[i]);
    //    }

    //    for (int tris = 0, i = 0; i < quality; i++)
    //    {
    //        triangles[tris + 0] = i + 0;
    //        triangles[tris + 1] = i + quality + 1;
    //        triangles[tris + 2] = i + quality + 2;
    //        triangles[tris + 3] = i + quality + 2;
    //        triangles[tris + 4] = i + 1;
    //        triangles[tris + 5] = i + 0;

    //        tris += 6;
    //    }

    //    mesh.vertices = vertices;
    //    mesh.uv = uv;
    //    mesh.triangles = triangles;
    //    mesh.RecalculateNormals();

    //    meshFilter.mesh = mesh;
    //}

    //private Vector3 GetVectorInConeFromAngle(float angle, float range, float spotAngle)
    //{
    //    float angleRad = angle * Mathf.Deg2Rad;
    //    return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) *
    //        range * Mathf.Tan(spotAngle * Mathf.Deg2Rad / 2) +
    //        Vector3.forward * range;
    //}

    //private void OnDrawGizmosSelected()
    //{
    //    if (!debug) return;
    //    Gizmos.color = Color.yellow;
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        Gizmos.DrawSphere(transform.rotation * vertices[i] + transform.position, .025f);
    //    }
    //}
}
