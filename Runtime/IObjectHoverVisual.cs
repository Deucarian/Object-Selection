using UnityEngine;

namespace Deucarian.ObjectSelection
{
    /// <summary>
    /// Defines how a hovered or unhovered object should be represented visually.
    /// </summary>
    /// <typeparam name="TKey">The stable hover key type.</typeparam>
    public interface IObjectHoverVisual<TKey>
    {
        /// <summary>
        /// Applies hovered visuals to the target object for the supplied key.
        /// </summary>
        /// <param name="key">The hovered key.</param>
        /// <param name="target">The hovered Unity object.</param>
        void ApplyHovered(TKey key, Object target);

        /// <summary>
        /// Applies unhovered visuals to the target object for the supplied key.
        /// </summary>
        /// <param name="key">The unhovered key.</param>
        /// <param name="target">The unhovered Unity object.</param>
        void ApplyUnhovered(TKey key, Object target);
    }
}
