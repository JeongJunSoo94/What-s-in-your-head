using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Common
{
    public static class GameLayers
    {
        private const string CharacterLayerName = "Character";
        private const string DynamicObstaclesLayerName = "DynamicObstacles";

        public static LayerMask Character
        {
            get { return LayerMask.NameToLayer(CharacterLayerName); }
        }

        public static LayerMask DynamicObstacles
        {
            get { return LayerMask.NameToLayer(DynamicObstaclesLayerName); }
        }

        public static LayerMask CharacterMask
        {
            get { return 1 << Character; }
        }
        public static LayerMask DynamicObstaclesMask
        {
            get { return 1 << DynamicObstacles; }
        }
    }
}