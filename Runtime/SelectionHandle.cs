using System;

namespace Deucarian.ObjectSelection
{
    /// <summary>Issued by a selection host for a registered object. Reuse it across callers in that scope.</summary>
    public sealed class SelectionHandle : IDisposable
    {
        private readonly SelectionHost owner;
        internal SelectionHandle(SelectionHost owner) { this.owner = owner; }
        internal bool BelongsTo(SelectionHost host) => ReferenceEquals(owner, host);
        public bool IsValid => owner != null && owner.Contains(this);
        public void Dispose()
        {
            if (owner != null) owner.Unregister(this);
        }
    }

    public enum SelectionRequestResult { Selected, Unavailable, ForeignScope }
}
