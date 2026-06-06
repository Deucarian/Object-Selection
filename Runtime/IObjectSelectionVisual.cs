using UnityEngine;

namespace JorisHoef.ObjectSelection
{
    /// <summary>
    /// Defines how a selected or deselected object should be represented visually.
    /// </summary>
    /// <typeparam name="TKey">The stable selection key type.</typeparam>
    public interface IObjectSelectionVisual<TKey>
    {
        /// <summary>
        /// Applies selected visuals to the target object for the supplied key.
        /// </summary>
        /// <param name="key">The selected key.</param>
        /// <param name="target">The selected Unity object.</param>
        void ApplySelected(TKey key, Object target);

        /// <summary>
        /// Applies deselected visuals to the target object for the supplied key.
        /// </summary>
        /// <param name="key">The deselected key.</param>
        /// <param name="target">The deselected Unity object.</param>
        void ApplyDeselected(TKey key, Object target);
    }
}
