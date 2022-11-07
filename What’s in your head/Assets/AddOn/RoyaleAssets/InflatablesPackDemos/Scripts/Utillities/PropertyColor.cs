using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Utillities
{
    [ExecuteInEditMode]
    public class PropertyColor : MonoBehaviour
    {
        [SerializeField] Color32 color = new Color32(0, 0, 0, 1);

        private Color32 currentColor;

        private Renderer rendererComp;
        private MaterialPropertyBlock propBlock;

        void Awake()
        {
            propBlock = new MaterialPropertyBlock();
            rendererComp = GetComponent<Renderer>();

            currentColor = new Color32(0, 0, 0, 0);
        }

        private void Update()
        {
            if (currentColor.GetHashCode() != color.GetHashCode())
            {
                UpdateVertexColors();
                currentColor = color;
            }
        }

        private void UpdateVertexColors()
        {
            rendererComp.GetPropertyBlock(propBlock);
            propBlock.SetColor("_ColorProp", color);
            rendererComp.SetPropertyBlock(propBlock);
        }
    }
}