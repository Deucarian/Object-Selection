using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JorisHoef.ObjectSelection
{
    /// <summary>
    /// Simple immutable implementation of <see cref="ISelectableObject{TKey}"/>.
    /// </summary>
    /// <typeparam name="TKey">The stable selection key type.</typeparam>
    public sealed class SelectableObject<TKey> : ISelectableObject<TKey>
    {
        /// <summary>
        /// Creates a selectable object with an optional source GameObject.
        /// </summary>
        /// <param name="id">The stable selection identity.</param>
        /// <param name="targetObject">The Unity object represented by the key.</param>
        /// <param name="sourceGameObject">The optional GameObject used for scene interaction.</param>
        public SelectableObject(TKey id, Object targetObject, GameObject sourceGameObject = null)
        {
            if (ReferenceEquals(id, null))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (UnityObjectUtility.IsDestroyed(targetObject))
            {
                throw new ArgumentNullException(nameof(targetObject));
            }

            Id = id;
            TargetObject = targetObject;
            SourceGameObject = sourceGameObject != null
                ? sourceGameObject
                : UnityObjectUtility.GetGameObject(targetObject);
        }

        /// <inheritdoc />
        public TKey Id { get; }

        /// <inheritdoc />
        public Object TargetObject { get; }

        /// <inheritdoc />
        public GameObject SourceGameObject { get; }
    }
}
