using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JorisHoef.ObjectSelection
{
    /// <summary>
    /// Selection visual that tints a target renderer with a MaterialPropertyBlock.
    /// </summary>
    /// <typeparam name="TKey">The stable selection key type.</typeparam>
    public sealed class RendererTintSelectionVisual<TKey> : IObjectSelectionVisual<TKey>
    {
        private readonly Dictionary<Renderer, MaterialPropertyBlock> _originalBlocks =
            new Dictionary<Renderer, MaterialPropertyBlock>();
        private readonly Color _selectedColor;

        /// <summary>
        /// Creates a renderer tint visual using a warm default selected color.
        /// </summary>
        public RendererTintSelectionVisual()
            : this(new Color(1f, 0.72f, 0.18f, 1f))
        {
        }

        /// <summary>
        /// Creates a renderer tint visual.
        /// </summary>
        /// <param name="selectedColor">The color applied while selected.</param>
        public RendererTintSelectionVisual(Color selectedColor)
        {
            _selectedColor = selectedColor;
        }

        /// <inheritdoc />
        public void ApplySelected(TKey key, Object target)
        {
            Renderer renderer = ResolveRenderer(target);
            if (renderer == null)
            {
                return;
            }

            if (!_originalBlocks.ContainsKey(renderer))
            {
                MaterialPropertyBlock originalBlock = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(originalBlock);
                _originalBlocks.Add(renderer, originalBlock);
            }

            MaterialPropertyBlock selectedBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(selectedBlock);
            selectedBlock.SetColor("_Color", _selectedColor);
            selectedBlock.SetColor("_BaseColor", _selectedColor);
            renderer.SetPropertyBlock(selectedBlock);
        }

        /// <inheritdoc />
        public void ApplyDeselected(TKey key, Object target)
        {
            Renderer renderer = ResolveRenderer(target);
            if (renderer == null)
            {
                return;
            }

            MaterialPropertyBlock originalBlock;
            if (_originalBlocks.TryGetValue(renderer, out originalBlock))
            {
                renderer.SetPropertyBlock(originalBlock);
                _originalBlocks.Remove(renderer);
            }
        }

        private static Renderer ResolveRenderer(Object target)
        {
            if (UnityObjectUtility.IsDestroyed(target))
            {
                return null;
            }

            Renderer renderer = target as Renderer;
            if (renderer != null)
            {
                return renderer;
            }

            GameObject gameObject = target as GameObject;
            if (gameObject != null)
            {
                return gameObject.GetComponent<Renderer>();
            }

            Component component = target as Component;
            return component != null ? component.GetComponent<Renderer>() : null;
        }
    }
}
