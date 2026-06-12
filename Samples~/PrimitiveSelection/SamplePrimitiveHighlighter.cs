using UnityEngine;
using Object = UnityEngine.Object;

namespace Deucarian.ObjectSelection.Samples.PrimitiveSelection
{
    public sealed class SamplePrimitiveHighlighter : MonoBehaviour, IObjectSelectionVisual<string>
    {
        [SerializeField] private Color selectedColor = new Color(1f, 0.72f, 0.18f, 1f);

        private MaterialPropertyBlock _propertyBlock;

        public void ApplySelected(string key, Object target)
        {
            Renderer renderer = ResolveRenderer(target);
            if (renderer == null)
            {
                return;
            }

            if (_propertyBlock == null)
            {
                _propertyBlock = new MaterialPropertyBlock();
            }

            renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_Color", selectedColor);
            _propertyBlock.SetColor("_BaseColor", selectedColor);
            renderer.SetPropertyBlock(_propertyBlock);
        }

        public void ApplyDeselected(string key, Object target)
        {
            Renderer renderer = ResolveRenderer(target);
            if (renderer == null)
            {
                return;
            }

            renderer.SetPropertyBlock(null);
        }

        private static Renderer ResolveRenderer(Object unityObject)
        {
            if (unityObject == null)
            {
                return null;
            }

            Renderer renderer = unityObject as Renderer;
            if (renderer != null)
            {
                return renderer;
            }

            GameObject gameObject = unityObject as GameObject;
            if (gameObject != null)
            {
                return gameObject.GetComponent<Renderer>();
            }

            Component component = unityObject as Component;
            return component != null ? component.GetComponent<Renderer>() : null;
        }
    }
}
