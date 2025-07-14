using System.Text.RegularExpressions;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Search;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.AvalonEdit
{
    public class SearchResult : TextSegment, ISearchResult
    {
        public Match Data { get; set; }

        public string ReplaceWith(string replacement)
        {
            return Data.Result(replacement);
        }
    }
}
