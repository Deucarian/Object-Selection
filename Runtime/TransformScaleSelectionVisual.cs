using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Deucarian.ObjectSelection
{
    /// <summary>
    /// Selection visual that scales the selected target transform and restores it on deselection.
    /// </summary>
    /// <typeparam name="TKey">The stable selection key type.</typeparam>
    public sealed class TransformScaleSelectionVisual<TKey> : IObjectSelectionVisual<TKey>
    {
        private readonly Dictionary<Transform, Vector3> _originalScales =
            new Dictionary<Transform, Vector3>();
        private readonly Vector3 _selectedScaleMultiplier;

        /// <summary>
        /// Creates a transform scale visual with a subtle default multiplier.
        /// </summary>
        public TransformScaleSelectionVisual()
            : this(1.1f)
        {
        }

        /// <summary>
        /// Creates a transform scale visual using a uniform multiplier.
        /// </summary>
        /// <param name="selectedScaleMultiplier">The multiplier applied while selected.</param>
        public TransformScaleSelectionVisual(float selectedScaleMultiplier)
            : this(new Vector3(selectedScaleMultiplier, selectedScaleMultiplier, selectedScaleMultiplier))
        {
        }

        /// <summary>
        /// Creates a transform scale visual.
        /// </summary>
        /// <param name="selectedScaleMultiplier">The multiplier applied while selected.</param>
        public TransformScaleSelectionVisual(Vector3 selectedScaleMultiplier)
        {
            if (selectedScaleMultiplier.x <= 0f ||
                selectedScaleMultiplier.y <= 0f ||
                selectedScaleMultiplier.z <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(selectedScaleMultiplier),
                    "Scale multipliers must be greater than zero.");
            }

            _selectedScaleMultiplier = selectedScaleMultiplier;
        }

        /// <inheritdoc />
        public void ApplySelected(TKey key, Object target)
        {
            Transform transform = ResolveTransform(target);
            if (transform == null)
            {
                return;
            }

            if (!_originalScales.ContainsKey(transform))
            {
                _originalScales.Add(transform, transform.localScale);
            }

            Vector3 originalScale = _originalScales[transform];
            transform.localScale = Vector3.Scale(originalScale, _selectedScaleMultiplier);
        }

        /// <inheritdoc />
        public void ApplyDeselected(TKey key, Object target)
        {
            Transform transform = ResolveTransform(target);
            if (transform == null)
            {
                return;
            }

            Vector3 originalScale;
            if (_originalScales.TryGetValue(transform, out originalScale))
            {
                transform.localScale = originalScale;
                _originalScales.Remove(transform);
            }
        }

        private static Transform ResolveTransform(Object target)
        {
            if (UnityObjectUtility.IsDestroyed(target))
            {
                return null;
            }

            Transform transform = target as Transform;
            if (transform != null)
            {
                return transform;
            }

            GameObject gameObject = target as GameObject;
            if (gameObject != null)
            {
                return gameObject.transform;
            }

            Component component = target as Component;
            return component != null ? component.transform : null;
        }
    }
}
