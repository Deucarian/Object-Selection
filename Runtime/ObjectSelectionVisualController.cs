using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JorisHoef.ObjectSelection
{
    /// <summary>
    /// Applies an object selection visual strategy from selection change events.
    /// </summary>
    /// <typeparam name="TKey">The stable selection key type.</typeparam>
    public sealed class ObjectSelectionVisualController<TKey> : IDisposable
    {
        private readonly ObjectSelectionService<TKey> _selectionService;
        private readonly IObjectSelectionVisual<TKey> _visual;

        private bool _hasAppliedSelection;
        private TKey _appliedKey;
        private Object _appliedTarget;
        private bool _isDisposed;

        /// <summary>
        /// Creates a visual controller and subscribes it to an object selection service.
        /// </summary>
        /// <param name="selectionService">The service that owns selection state.</param>
        /// <param name="visual">The visual strategy that decides how selection looks.</param>
        /// <param name="applyCurrentSelection">When true, immediately applies the current selection if one exists.</param>
        public ObjectSelectionVisualController(
            ObjectSelectionService<TKey> selectionService,
            IObjectSelectionVisual<TKey> visual,
            bool applyCurrentSelection = true)
        {
            if (selectionService == null)
            {
                throw new ArgumentNullException(nameof(selectionService));
            }

            if (visual == null)
            {
                throw new ArgumentNullException(nameof(visual));
            }

            _selectionService = selectionService;
            _visual = visual;

            _selectionService.SelectionChanged += OnSelectionChanged;

            if (applyCurrentSelection)
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
        /// Reapplies the visual strategy for the current selection without changing selection state.
        /// </summary>
        public void Refresh()
        {
            ThrowIfDisposed();

            if (_selectionService.HasSelection)
            {
                ApplySelectedIfChanged(_selectionService.CurrentKey, _selectionService.CurrentObject);
                return;
            }

            ClearAppliedSelection();
        }

        /// <summary>
        /// Unsubscribes from selection changes and restores the currently applied visual.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _selectionService.SelectionChanged -= OnSelectionChanged;
            ClearAppliedSelection();
            _isDisposed = true;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs<TKey> args)
        {
            if (args.HadPreviousSelection &&
                !IsSameSelection(args.PreviousKey, args.PreviousObject, args.CurrentKey, args.CurrentObject))
            {
                ApplyDeselected(args.PreviousKey, args.PreviousObject);
            }

            if (args.HasSelection)
            {
                ApplySelectedIfChanged(args.CurrentKey, args.CurrentObject);
                return;
            }

            ClearAppliedSelection();
        }

        private void ApplySelectedIfChanged(TKey key, Object target)
        {
            if (UnityObjectUtility.IsDestroyed(target))
            {
                return;
            }

            if (_hasAppliedSelection && IsSameSelection(_appliedKey, _appliedTarget, key, target))
            {
                return;
            }

            if (_hasAppliedSelection)
            {
                ApplyDeselected(_appliedKey, _appliedTarget);
            }

            _visual.ApplySelected(key, target);
            _appliedKey = key;
            _appliedTarget = target;
            _hasAppliedSelection = true;
        }

        private void ApplyDeselected(TKey key, Object target)
        {
            if (!UnityObjectUtility.IsDestroyed(target))
            {
                _visual.ApplyDeselected(key, target);
            }

            if (_hasAppliedSelection && IsSameSelection(_appliedKey, _appliedTarget, key, target))
            {
                _appliedKey = default(TKey);
                _appliedTarget = null;
                _hasAppliedSelection = false;
            }
        }

        private void ClearAppliedSelection()
        {
            if (!_hasAppliedSelection)
            {
                return;
            }

            ApplyDeselected(_appliedKey, _appliedTarget);
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private static bool IsSameSelection(TKey leftKey, Object leftTarget, TKey rightKey, Object rightTarget)
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
