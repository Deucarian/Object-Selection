using System.Collections.Generic;
using UnityEngine;

namespace JorisHoef.ObjectSelection.Samples.PrimitiveSelection
{
    public sealed class PrimitiveSelectionSample : MonoBehaviour
    {
        private readonly string[] _keys = { "cube", "sphere", "capsule", "cylinder" };
        private readonly Dictionary<string, GameObject> _objects = new Dictionary<string, GameObject>();

        private ObjectSelectionRegistry<string> _registry;
        private ObjectSelectionService<string> _selection;
        private SamplePrimitiveHighlighter _highlighter;
        private PrimitiveRaycastSelectionController _raycastController;
        private string _lastEvent = "No selection events yet.";

        private void Awake()
        {
            _registry = new ObjectSelectionRegistry<string>();
            _selection = new ObjectSelectionService<string>(_registry);
            _selection.SelectionChanged += OnSelectionChanged;

            EnsureCamera();
            EnsureLight();
            EnsurePrimitives();
            RegisterSceneSelectables();
            EnsureHighlighter();
            EnsureRaycastController();
        }

        private void OnDestroy()
        {
            if (_selection != null)
            {
                _selection.SelectionChanged -= OnSelectionChanged;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectKey("cube");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectKey("sphere");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectKey("capsule");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SelectKey("cylinder");
            }
            else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                _selection.ClearSelection();
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(16f, 16f, 340f, 220f), GUI.skin.box);
            GUILayout.Label("JorisHoef Object Selection");
            GUILayout.Label("Current: " + (_selection.HasSelection ? _selection.CurrentKey : "(none)"));
            GUILayout.Label("Previous: " + (_selection.HasPreviousSelection ? _selection.PreviousKey : "(none)"));
            GUILayout.Label(_lastEvent);

            GUILayout.Space(8f);
            GUILayout.BeginHorizontal();
            DrawSelectButton("1 Cube", "cube");
            DrawSelectButton("2 Sphere", "sphere");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            DrawSelectButton("3 Capsule", "capsule");
            DrawSelectButton("4 Cylinder", "cylinder");
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Clear"))
            {
                _selection.ClearSelection();
            }

            GUILayout.EndArea();
        }

        private void DrawSelectButton(string label, string key)
        {
            if (GUILayout.Button(label))
            {
                SelectKey(key);
            }
        }

        private void SelectKey(string key)
        {
            _selection.Select(key, SelectionChangeReason.Programmatic);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs<string> args)
        {
            if (_highlighter != null)
            {
                _highlighter.OnSelectionChanged(args);
            }

            string previous = args.HadPreviousSelection ? args.PreviousKey : "(none)";
            string current = args.HasSelection ? args.CurrentKey : "(none)";
            _lastEvent = "Event: " + previous + " -> " + current + " (" + args.Reason + ")";
            Debug.Log(_lastEvent);
        }

        private void EnsureCamera()
        {
            if (Camera.main != null)
            {
                return;
            }

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.transform.position = new Vector3(0f, 4f, -8f);
            camera.transform.rotation = Quaternion.Euler(25f, 0f, 0f);
        }

        private void EnsureLight()
        {
            if (FindObjectOfType<Light>() != null)
            {
                return;
            }

            var lightObject = new GameObject("Directional Light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private void EnsurePrimitives()
        {
            CreatePrimitive("cube", PrimitiveType.Cube, new Vector3(-3f, 0f, 0f));
            CreatePrimitive("sphere", PrimitiveType.Sphere, new Vector3(-1f, 0f, 0f));
            CreatePrimitive("capsule", PrimitiveType.Capsule, new Vector3(1f, 0f, 0f));
            CreatePrimitive("cylinder", PrimitiveType.Cylinder, new Vector3(3f, 0f, 0f));
        }

        private void CreatePrimitive(string key, PrimitiveType type, Vector3 position)
        {
            if (_objects.ContainsKey(key))
            {
                return;
            }

            GameObject existing = GameObject.Find(key);
            GameObject primitive = existing != null ? existing : GameObject.CreatePrimitive(type);
            primitive.name = key;
            primitive.transform.position = position;

            var selectable = primitive.GetComponent<SampleSelectableObject>();
            if (selectable == null)
            {
                selectable = primitive.AddComponent<SampleSelectableObject>();
            }

            selectable.SetId(key);
            _objects.Add(key, primitive);
        }

        private void RegisterSceneSelectables()
        {
            SampleSelectableObject[] selectables = FindObjectsOfType<SampleSelectableObject>();
            for (int i = 0; i < selectables.Length; i++)
            {
                _registry.Register(selectables[i]);
            }
        }

        private void EnsureHighlighter()
        {
            _highlighter = GetComponent<SamplePrimitiveHighlighter>();
            if (_highlighter == null)
            {
                _highlighter = gameObject.AddComponent<SamplePrimitiveHighlighter>();
            }

            for (int i = 0; i < _keys.Length; i++)
            {
                GameObject target;
                if (_objects.TryGetValue(_keys[i], out target))
                {
                    _highlighter.Track(target);
                }
            }
        }

        private void EnsureRaycastController()
        {
            _raycastController = GetComponent<PrimitiveRaycastSelectionController>();
            if (_raycastController == null)
            {
                _raycastController = gameObject.AddComponent<PrimitiveRaycastSelectionController>();
            }

            _raycastController.SelectionCamera = Camera.main;
            _raycastController.Initialize(_selection);
        }
    }
}
