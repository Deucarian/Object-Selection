using Deucarian.Logging;

namespace Deucarian.ObjectSelection
{
    /// <summary>
    /// Package-level log categories for Object Selection.
    /// </summary>
    public static class SelectionLog
    {
        public static readonly DLog General = DLog.For("Selection");
        public static readonly DLog Samples = DLog.For("Selection.Samples");
    }
}
