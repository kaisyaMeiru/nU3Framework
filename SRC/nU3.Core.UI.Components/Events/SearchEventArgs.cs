namespace nU3.Core.UI.Components.Controls
{
    public class SearchEventArgs : EventArgs
    {
        public string SearchTerm { get; set; } = "";
        public string SearchType { get; set; } = "";
        public Dictionary<string, object>? Parameters { get; set; }
    }
}
