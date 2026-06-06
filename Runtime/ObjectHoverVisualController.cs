using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JorisHoef.ObjectSelection
{
    /// <summary>
    /// Applies an object hover visual strategy from hover change events.
    /// </summary>
    /// <typeparam name="TKey">The stable hover key type.</typeparam>
    public sealed class ObjectHoverVisualController<TKey> : IDisposable
    {
        private readonly ObjectHoverService<TKey> _hoverService;
        private readonly IObjectHoverVisual<TKey> _visual;

        private bool _hasAppliedHover;
        private TKey _appliedKey;
        private Object _appliedTarget;
        private bool _isDisposed;

        /// <summary>
        /// Creates a visual controller and subscribes it to an object hover service.
        /// </summary>
        /// <param name="hoverService">The service that owns hover state.</param>
        /// <param name="visual">The visual strategy that decides how hover looks.</param>
        /// <param name="applyCurrentHover">When true, immediately applies the current hover if one exists.</param>
        public ObjectHoverVisualController(
            ObjectHoverService<TKey> hoverService,
            IObjectHoverVisual<TKey> visual,
            bool applyCurrentHover = true)
        {
            if (hoverService == null)
            {
                throw new ArgumentNullException(nameof(hoverService));
            }

            if (visual == null)
            {
                throw new ArgumentNullException(nameof(visual));
            }

            _hoverService = hoverService;
            _visual = visual;

            _hoverService.HoverChanged += OnHoverChanged;

            if (applyCurrentHover)
            {
                Refresh();
            }
        }

        /// <summary>
        /// Gets whether this controller has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        /// <summary>
        /// Reapplies the visual strategy for the current hover without changing hover state.
        /// </summary>
        public void Refresh()
        {
            ThrowIfDisposed();

            if (_hoverService.HasHover)
            {
                ApplyHoveredIfChanged(_hoverService.CurrentHoveredKey, _hoverService.CurrentHoveredObject);
                return;
            }

            ClearAppliedHover();
        }

        /// <summary>
        /// Unsubscribes from hover changes and restores the currently applied visual.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _hoverService.HoverChanged -= OnHoverChanged;
            ClearAppliedHover();
            _isDisposed = true;
        }

        private void OnHoverChanged(object sender, HoverChangedEventArgs<TKey> args)
        {
            if (args.HadPreviousHover &&
                !IsSameHover(args.PreviousKey, args.PreviousObject, args.CurrentKey, args.CurrentObject))
            {
                ApplyUnhovered(args.PreviousKey, args.PreviousObject);
            }

            if (args.HasHover)
            {
                ApplyHoveredIfChanged(args.CurrentKey, args.CurrentObject);
                return;
            }

            ClearAppliedHover();
        }

        private void ApplyHoveredIfChanged(TKey key, Object target)
        {
            if (UnityObjectUtility.IsDestroyed(target))
            {
                return;
            }

            if (_hasAppliedHover && IsSameHover(_appliedKey, _appliedTarget, key, target))
            {
                return;
            }

            if (_hasAppliedHover)
            {
                ApplyUnhovered(_appliedKey, _appliedTarget);
            }

            _visual.ApplyHovered(key, target);
            _appliedKey = key;
            _appliedTarget = target;
            _hasAppliedHover = true;
        }

        private void ApplyUnhovered(TKey key, Object target)
        {
            if (!UnityObjectUtility.IsDestroyed(target))
            {
                _visual.ApplyUnhovered(key, target);
            }

            if (_hasAppliedHover && IsSameHover(_appliedKey, _appliedTarget, key, target))
            {
                _appliedKey = default(TKey);
                _appliedTarget = null;
                _hasAppliedHover = false;
            }
        }

        private void ClearAppliedHover()
        {
            if (!_hasAppliedHover)
            {
                return;
            }

            ApplyUnhovered(_appliedKey, _appliedTarget);
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private static bool IsSameHover(TKey leftKey, Object leftTarget, TKey rightKey, Object rightTarget)
        {
            return EqualityComparer<TKey>.Default.Equals(leftKey, rightKey) &&
                IsSameTarget(leftTarget, rightTarget);
        }

        private static bool IsSameTarget(Object leftTarget, Object rightTarget)
        {
            if (UnityObjectUtility.IsDestroyed(leftTarget) && UnityObjectUtility.IsDestroyed(rightTarget))
            {
                return true;
            }

            return leftTarget == rightTarget;
        }
    }
}
