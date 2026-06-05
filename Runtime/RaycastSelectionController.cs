using System;
using UnityEngine;

namespace JorisHoef.ObjectSelection
{
    /// <summary>
    /// Converts mouse raycasts into keyed selection commands.
    /// </summary>
    /// <typeparam name="TKey">The stable selection key type.</typeparam>
    public abstract class RaycastSelectionController<TKey> : MonoBehaviour
    {
        [SerializeField] private Camera selectionCamera;
        [SerializeField] private LayerMask layerMask = Physics.DefaultRaycastLayers;
        [SerializeField] private float maxDistance = 1000f;
        [SerializeField] private QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
        [SerializeField] private int mouseButton;
        [SerializeField] private bool selectOnMouseDown = true;
        [SerializeField] private bool clearSelectionOnMiss;

        /// <summary>
        /// Gets or sets the camera used to create screen-point rays.
        /// </summary>
        public Camera SelectionCamera
        {
            get { return selectionCamera; }
            set { selectionCamera = value; }
        }

        /// <summary>
        /// Gets or sets the raycast layer mask.
        /// </summary>
        public LayerMask LayerMask
        {
            get { return layerMask; }
            set { layerMask = value; }
        }

        /// <summary>
        /// Gets or sets the maximum raycast distance.
        /// </summary>
        public float MaxDistance
        {
            get { return maxDistance; }
            set { maxDistance = value; }
        }

        /// <summary>
        /// Gets or sets trigger interaction behavior for physics raycasts.
        /// </summary>
        public QueryTriggerInteraction QueryTriggerInteraction
        {
            get { return queryTriggerInteraction; }
            set { queryTriggerInteraction = value; }
        }

        /// <summary>
        /// Gets or sets the mouse button index used by <see cref="Update"/>.
        /// </summary>
        public int MouseButton
        {
            get { return mouseButton; }
            set { mouseButton = value; }
        }

        /// <summary>
        /// Gets or sets whether selection is performed on mouse down instead of mouse up.
        /// </summary>
        public bool SelectOnMouseDown
        {
            get { return selectOnMouseDown; }
            set { selectOnMouseDown = value; }
        }

        /// <summary>
        /// Gets or sets whether missed raycasts clear the current selection.
        /// </summary>
        public bool ClearSelectionOnMiss
        {
            get { return clearSelectionOnMiss; }
            set { clearSelectionOnMiss = value; }
        }

        /// <summary>
        /// Gets the selection service invoked by this controller.
        /// </summary>
        public ObjectSelectionService<TKey> SelectionService { get; private set; }

        /// <summary>
        /// Gets the registry used to resolve raycast hits to keys.
        /// </summary>
        public ObjectSelectionRegistry<TKey> Registry { get; private set; }

        /// <summary>
        /// Gets or sets a predicate that suppresses input when it returns <c>true</c>.
        /// </summary>
        public Func<bool> ShouldIgnoreInput { get; set; }

        /// <summary>
        /// Initializes the controller from a selection service.
        /// </summary>
        /// <param name="selectionService">The selection service to invoke.</param>
        public void Initialize(ObjectSelectionService<TKey> selectionService)
        {
            if (selectionService == null)
            {
                throw new ArgumentNullException(nameof(selectionService));
            }

            Initialize(selectionService, selectionService.Registry);
        }

        /// <summary>
        /// Initializes the controller with explicit selection and registry dependencies.
        /// </summary>
        /// <param name="selectionService">The selection service to invoke.</param>
        /// <param name="registry">The registry used to resolve raycast hits.</param>
        public void Initialize(
            ObjectSelectionService<TKey> selectionService,
            ObjectSelectionRegistry<TKey> registry)
        {
            if (selectionService == null)
            {
                throw new ArgumentNullException(nameof(selectionService));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            SelectionService = selectionService;
            Registry = registry;
        }

        /// <summary>
        /// Attempts a physics raycast from a screen position and selects the resolved key.
        /// </summary>
        /// <param name="screenPosition">The screen position in pixels.</param>
        /// <returns><c>true</c> when a registered key was selected.</returns>
        public bool TrySelectFromScreenPosition(Vector3 screenPosition)
        {
            RaycastHit hit;
            if (!TryRaycast(screenPosition, out hit))
            {
                ClearIfConfigured();
                return false;
            }

            return TrySelect(hit);
        }

        /// <summary>
        /// Attempts to raycast from a screen position.
        /// </summary>
        /// <param name="screenPosition">The screen position in pixels.</param>
        /// <param name="hit">The resulting physics hit.</param>
        /// <returns><c>true</c> when the physics raycast hit something.</returns>
        public bool TryRaycast(Vector3 screenPosition, out RaycastHit hit)
        {
            hit = default(RaycastHit);

            Camera cameraToUse = GetCamera();
            if (cameraToUse == null)
            {
                return false;
            }

            Ray ray = cameraToUse.ScreenPointToRay(screenPosition);
            return Physics.Raycast(
                ray,
                out hit,
                Mathf.Max(0f, maxDistance),
                layerMask,
                queryTriggerInteraction);
        }

        /// <summary>
        /// Attempts to resolve and select a key from a raycast hit.
        /// </summary>
        /// <param name="hit">The physics hit to resolve.</param>
        /// <returns><c>true</c> when a registered key was selected.</returns>
        public bool TrySelect(RaycastHit hit)
        {
            if (SelectionService == null || Registry == null)
            {
                return false;
            }

            TKey key;
            if (!TryResolveKey(hit, out key))
            {
                ClearIfConfigured();
                return false;
            }

            return SelectionService.TrySelect(key, SelectionChangeReason.Raycast);
        }

        /// <summary>
        /// Resolves a key from a raycast hit.
        /// </summary>
        /// <param name="hit">The physics hit to resolve.</param>
        /// <param name="key">The resolved key.</param>
        /// <returns><c>true</c> when a registered key could be resolved.</returns>
        protected virtual bool TryResolveKey(RaycastHit hit, out TKey key)
        {
            key = default(TKey);

            if (Registry == null)
            {
                return false;
            }

            if (hit.collider != null && Registry.TryGetKey(hit.collider, out key))
            {
                return true;
            }

            if (hit.transform != null && Registry.TryGetKey(hit.transform.gameObject, out key))
            {
                return true;
            }

            if (hit.rigidbody != null && Registry.TryGetKey(hit.rigidbody, out key))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Assigns Camera.main when no explicit selection camera has been configured.
        /// </summary>
        protected virtual void Awake()
        {
            if (selectionCamera == null)
            {
                selectionCamera = Camera.main;
            }
        }

        /// <summary>
        /// Polls mouse input and forwards accepted clicks to the selection service.
        /// </summary>
        protected virtual void Update()
        {
            if (SelectionService == null || Registry == null)
            {
                return;
            }

            if (ShouldIgnoreInput != null && ShouldIgnoreInput())
            {
                return;
            }

            bool shouldSelect = selectOnMouseDown
                ? Input.GetMouseButtonDown(mouseButton)
                : Input.GetMouseButtonUp(mouseButton);

            if (shouldSelect)
            {
                TrySelectFromScreenPosition(Input.mousePosition);
            }
        }

        private Camera GetCamera()
        {
            if (selectionCamera != null)
            {
                return selectionCamera;
            }

            selectionCamera = Camera.main;
            return selectionCamera;
        }

        private void ClearIfConfigured()
        {
            if (clearSelectionOnMiss && SelectionService != null)
            {
                SelectionService.ClearSelection(SelectionChangeReason.Raycast);
            }
        }
    }
}
