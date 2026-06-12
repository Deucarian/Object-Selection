using UnityEngine;
using Object = UnityEngine.Object;

namespace Deucarian.ObjectSelection
{
    internal static class UnityObjectUtility
    {
        public static bool IsDestroyed(Object unityObject)
        {
            return ReferenceEquals(unityObject, null) || unityObject == null;
        }

        public static GameObject GetGameObject(Object unityObject)
        {
            if (IsDestroyed(unityObject))
            {
                return null;
            }

            GameObject gameObject = unityObject as GameObject;
            if (gameObject != null)
            {
                return gameObject;
            }

            Component component = unityObject as Component;
            return component != null && !IsDestroyed(component.gameObject)
                ? component.gameObject
                : null;
        }
    }
}
