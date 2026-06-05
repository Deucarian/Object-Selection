using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JorisHoef.ObjectSelection.Tests
{
    public sealed class RaycastSelectionControllerTests
    {
        [Test]
        public void RaycastControllerSelectsResolvedKey()
        {
            var registry = new ObjectSelectionRegistry<string>();
            var selection = new ObjectSelectionService<string>(registry);

            var cameraObject = new GameObject("Selection Camera");
            var controllerObject = new GameObject("Selection Controller");
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            try
            {
                Camera camera = cameraObject.AddComponent<Camera>();
                camera.pixelRect = new Rect(0f, 0f, 100f, 100f);
                camera.transform.position = new Vector3(0f, 0f, -10f);
                camera.transform.rotation = Quaternion.identity;

                cube.transform.position = Vector3.zero;
                registry.Register(new SelectableObject<string>("cube", cube));

                var controller = controllerObject.AddComponent<TestRaycastSelectionController>();
                controller.SelectionCamera = camera;
                controller.Initialize(selection);
                SelectionChangeReason reason = SelectionChangeReason.Programmatic;
                selection.SelectionChanged += (_, args) => reason = args.Reason;

                Physics.SyncTransforms();

                Assert.IsTrue(controller.TrySelectFromScreenPosition(new Vector3(50f, 50f, 0f)));
                Assert.IsTrue(selection.HasSelection);
                Assert.AreEqual("cube", selection.CurrentKey);
                Assert.AreEqual(SelectionChangeReason.Raycast, reason);
            }
            finally
            {
                Object.DestroyImmediate(cube);
                Object.DestroyImmediate(controllerObject);
                Object.DestroyImmediate(cameraObject);
            }
        }

        [Test]
        public void MissingCameraRaycastReturnsFalse()
        {
            var registry = new ObjectSelectionRegistry<string>();
            var selection = new ObjectSelectionService<string>(registry);
            var controllerObject = new GameObject("Selection Controller");

            try
            {
                var controller = controllerObject.AddComponent<TestRaycastSelectionController>();
                controller.SelectionCamera = null;
                controller.Initialize(selection);

                Assert.IsFalse(controller.TrySelectFromScreenPosition(Vector3.zero));
            }
            finally
            {
                Object.DestroyImmediate(controllerObject);
            }
        }

        private sealed class TestRaycastSelectionController : RaycastSelectionController<string>
        {
        }
    }
}
