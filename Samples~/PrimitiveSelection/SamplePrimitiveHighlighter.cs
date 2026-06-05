using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JorisHoef.ObjectSelection.Samples.PrimitiveSelection
{
    public sealed class SamplePrimitiveHighlighter : MonoBehaviour, IObjectSelectionHighlighter<string>
    {
        [SerializeField] private Color selectedColor = new Color(1f, 0.72f, 0.18f, 1f);

        private readonly Dictionary<Renderer, Color> _originalColors = new Dictionary<Renderer, Color>();
        private Renderer _currentRenderer;

        public void Track(GameObject target)
        {
            if (target == null)
            {
                return;
            }

            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer == null || _originalColors.ContainsKey(renderer))
            {
                return;
            }

            _originalColors.Add(renderer, renderer.material.color);
        }

        public void OnSelectionChanged(SelectionChangedEventArgs<string> args)
        {
            RestoreCurrent();

            GameObject currentGameObject = ResolveGameObject(args.CurrentObject);
            if (currentGameObject == null)
            {
                return;
            }

            Renderer renderer = currentGameObject.GetComponent<Renderer>();
            if (renderer == null)
            {
                return;
            }

            Track(currentGameObject);
            _currentRenderer = renderer;
            renderer.material.color = selectedColor;
        }

        private void OnDestroy()
        {
            RestoreCurrent();
        }

        private void RestoreCurrent()
        {
            if (_currentRenderer == null)
            {
                return;
            }

            Color originalColor;
            if (_originalColors.TryGetValue(_currentRenderer, out originalColor))
            {
                _currentRenderer.material.color = originalColor;
            }

            _currentRenderer = null;
        }

        private static GameObject ResolveGameObject(Object unityObject)
        {
            if (unityObject == null)
            {
                return null;
            }

            GameObject gameObject = unityObject as GameObject;
            if (gameObject != null)
            {
                return gameObject;
            }

            Component component = unityObject as Component;
            return component != null ? component.gameObject : null;
        }
    }
}
