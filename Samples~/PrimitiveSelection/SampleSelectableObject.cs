using UnityEngine;
using Object = UnityEngine.Object;

namespace JorisHoef.ObjectSelection.Samples.PrimitiveSelection
{
    public sealed class SampleSelectableObject : MonoBehaviour, ISelectableObject<string>
    {
        [SerializeField] private string id;

        public string Id
        {
            get { return id; }
        }

        public Object TargetObject
        {
            get { return gameObject; }
        }

        public GameObject SourceGameObject
        {
            get { return gameObject; }
        }

        public void SetId(string value)
        {
            id = value;
        }
    }
}
