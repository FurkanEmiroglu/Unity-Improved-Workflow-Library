using UnityEngine;

namespace IW.ExtensionMethods
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Swaps the layer of a gameobject, including all of its children.
        /// </summary>
        /// <param name="obj">Target Object</param>
        /// <param name="layer">Target Layer</param>
        public static void SwapLayer(this GameObject obj, int layer)
        {
            SwapLayer(obj, layer, true);
        }
        
        
        /// <summary>
        /// Swaps the layer of a gameobject, including all of its children.
        /// </summary>
        /// <param name="obj">Target Object</param>
        /// <param name="layerName">Target Layer Name</param>
        public static void SwapLayer(this GameObject obj, string layerName)
        {
            SwapLayer(obj, layerName, true);
        }

        /// <summary>
        /// Swaps the layer of a gameObject, based on the given layer name & includes the children if the includeChildren is true.
        /// </summary>
        /// <param name="obj">target gameObject</param>
        /// <param name="layerName">target layerName</param>
        /// <param name="includeChildren">should the children layers change</param>
        public static void SwapLayer(this GameObject obj, string layerName, bool includeChildren)
        {
            obj.layer = LayerMask.NameToLayer(layerName);
            
            if (includeChildren)
            {
                foreach (Transform t in obj.transform)
                {
                    SwapLayer(t.gameObject, layerName, true);
                }
            }
        }
        
        /// <summary>
        /// Swaps the layer of a gameObject, based on the given layer name & includes the children if the includeChildren is true.
        /// </summary>
        /// <param name="obj">target gameObject</param>
        /// <param name="layer">target layerName</param>
        /// <param name="includeChildren">should the children layers change</param>
        public static void SwapLayer(this GameObject obj, int layer, bool includeChildren)
        {
            obj.layer = layer;
            
            if (includeChildren)
            {
                foreach (Transform t in obj.transform)
                {
                    SwapLayer(t.gameObject, layer, true);
                }
            }
        }
    }
}