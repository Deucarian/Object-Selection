using UnityEngine;
using Object = UnityEngine.Object;

namespace JorisHoef.ObjectSelection
{
    /// <summary>
    /// Defines the stable key and Unity payload for an object that can be selected.
    /// </summary>
    /// <typeparam name="TKey">The stable selection key type.</typeparam>
    public interface ISelectableObject<TKey>
    {
        /// <summary>
        /// Gets the stable identity used by selection services.
        /// </summary>
        TKey Id { get; }

        /// <summary>
        /// Gets the Unity object represented by this selection entry.
        /// </summary>
        Object TargetObject { get; }

        /// <summary>
        /// Gets the optional source GameObject used for scene interaction.
        /// </summary>
        GameObject SourceGameObject { get; }
    }
}
