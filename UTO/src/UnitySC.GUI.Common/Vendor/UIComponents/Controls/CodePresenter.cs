using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Search;
using UnitySC.GUI.Common.Vendor.UIComponents.Controls.AvalonEdit;

using Brushes = UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared.Brushes;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public enum CodeType
    {
        CSharp,
        Xml
    }

    public class CodePresenter : Control
    {
        #region Static

        private static readonly Dictionary<CodeType, IHighlightingDefinition> Highlightings = new();

        static CodePresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CodePresenter), new FrameworkPropertyMetadata(typeof(CodePresenter)));
        }

        #endregion

        #region Fields

        private readonly SearchResultBackgroundRenderer _searchRenderer = new();
        private TextEditor _textEditor;
        private Button _copyButton;
        private ToggleButton _matchCaseButton;

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textEditor = GetTemplateChild("PART_TextEditor") as TextEditor;

            CustomizeTextEditor();

            if (_copyButton != null)
            {
                _copyButton.Click -= OnCopyButton_Click;
            }

            _copyButton = GetTemplateChild("PART_CopyButton") as Button;

            if (_copyButton != null)
            {
                _copyButton.Click += OnCopyButton_Click;
            }

            UpdateCodeType();
            UpdateCodeText();

            #region Search

            if (GetTemplateChild("PART_NextButton") is Button nextButton)
            {
                nextButton.Click += delegate { FindNext(); };
            }

            if (GetTemplateChild("PART_PreviousButton") is Button previousButton)
            {
                previousButton.Click += delegate { FindPrevious(); };
            }

            _matchCaseButton = GetTemplateChild("PART_MatchCaseButton") as ToggleButton;

            if (_matchCaseButton != null)
            {
                _matchCaseButton.Click += delegate
                {
                    SearchEngine.MatchCase = _matchCaseButton.IsChecked ?? false;
                    OnSearchEngineApplySearch(this, EventArgs.Empty);
                };
            }

            #endregion
        }

        #region Overrides of UIElement

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            _textEditor?.Select(0, 0);
        }

        #endregion

        #region Event handlers

        private void OnCopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (CodeText != null)
            {
                Clipboard.SetText(CodeText);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        e.Handled = true;
                        switch (Keyboard.Modifiers & ModifierKeys.Shift)
                        {
                            case ModifierKeys.Shift:
                                FindPrevious();
                                break;
                            default:
                                FindNext();
                                break;
                        }

                        return;
                    }
            }

            base.OnKeyDown(e);
        }

        #endregion

        #region Private methods

        private void CustomizeTextEditor()
        {
            var textArea = _textEditor?.TextArea;
            if (textArea != null)
            {
                textArea.SelectionBorder = null;
                textArea.SelectionCornerRadius = 0;
            }

            var textView = _textEditor?.TextArea.TextView;
            if (textView != null)
            {
                textView.LinkTextForegroundBrush = Brushes.HighlightBrush;
                textView.BackgroundRenderers.Add(_searchRenderer);
            }
        }

        private void UpdateCodeType()
        {
            if (_textEditor == null) return;

            _textEditor.SyntaxHighlighting = GetHighlightOrInitialize(CodeType);
        }

        private static IHighlightingDefinition GetHighlightOrInitialize(CodeType codeType)
        {
            if (Highlightings.TryGetValue(codeType, out var highlightingDefinition))
            {
                return highlightingDefinition;
            }

            highlightingDefinition = codeType switch
            {
                CodeType.CSharp => HighlightingManager.Instance.GetDefinition("C#"),
                CodeType.Xml => HighlightingManager.Instance.GetDefinition("XML"),
                _ => throw new InvalidOperationException()
            };

            UpdateHighlightDefinition(highlightingDefinition);
            Highlightings.Add(codeType, highlightingDefinition);
            return highlightingDefinition;
        }

        /// <summary>
        /// Change the HighlightDefinition to replace all colors with the closest theme colors.
        /// </summary>
        private static void UpdateHighlightDefinition(IHighlightingDefinition definition)
        {
            foreach (var highlightingColor in definition.NamedHighlightingColors)
            {
                if (highlightingColor.Foreground?.GetBrush(null) is not SolidColorBrush brush)
                {
                    continue;
                }

                var color = brush.Color;
                var closestColor = GetClosestColor(color);
                highlightingColor.Foreground = new SimpleHighlightingBrush(closestColor);
            }
        }

        protected virtual void UpdateCodeText()
        {
            if (_textEditor == null)
            {
                return;
            }

            // Used explicitly because TemplateBinding does not work with this property.
            _textEditor.Text = CodeText;
            OnSearchEngineApplySearch();
        }

        #endregion

        #region Static helpers

        /// <summary>
        /// Gets the closest color contained in the resource from the color passed as a parameter.
        /// </summary>
        private static Color GetClosestColor(Color color)
        {
            var colors = ResourcesHelper.GetAll<Color>();

            var hue1 = GetHue(color);

            // For foreground color
            if (hue1 == null)
            {
                return Brushes.ForegroundBrush.Color;
            }

            List<(Color color, float distance)> distances = colors.Values.Select(c => (c, GetHueDistance(GetHue(c), hue1))).ToList();

            Color? currentColor = null;
            float currentDistance = float.MaxValue;

            foreach (var tuple in distances)
            {
                if (currentColor == null || tuple.distance < currentDistance)
                {
                    currentColor = tuple.color;
                    currentDistance = tuple.distance;
                }
            }

            return currentColor ?? color;
        }

        private static float GetHueDistance(float? hue1, float? hue2)
        {
            if (hue1 == null || hue2 == null)
            {
                return float.MaxValue;
            }

            float abs = Math.Abs(hue1.Value - hue2.Value);
            return abs > 180 ? 360 - abs : abs;
        }

        public static float? GetHue(Color c)
        {
            // No hue (Gray, Black, White...)
            if (c.R == c.G && c.R == c.B)
            {
                return null;
            }

            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B).GetHue();
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty CodeTextProperty = DependencyProperty.Register(
            nameof(CodeText),
            typeof(string),
            typeof(CodePresenter),
            new PropertyMetadata(default(string), OnCodeTextChanged));

        private static void OnCodeTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CodePresenter self)
            {
                self.UpdateCodeText();
            }
        }

        public string CodeText
        {
            get => (string)GetValue(CodeTextProperty);
            set => SetValue(CodeTextProperty, value);
        }

        public static readonly DependencyProperty CodeTypeProperty = DependencyProperty.Register(
            nameof(CodeType),
            typeof(CodeType),
            typeof(CodePresenter),
            new PropertyMetadata(default(CodeType), OnCodeTypeChanged));

        private static void OnCodeTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CodePresenter self)
            {
                self.UpdateCodeType();
            }
        }

        public CodeType CodeType
        {
            get => (CodeType)GetValue(CodeTypeProperty);
            set => SetValue(CodeTypeProperty, value);
        }

        public static readonly DependencyProperty ShowLineNumbersProperty = DependencyProperty.Register(
            nameof(ShowLineNumbers),
            typeof(bool),
            typeof(CodePresenter),
            new PropertyMetadata(default(bool)));

        public bool ShowLineNumbers
        {
            get => (bool)GetValue(ShowLineNumbersProperty);
            set => SetValue(ShowLineNumbersProperty, value);
        }

        public static readonly DependencyProperty EnableCopyToClipboardProperty = DependencyProperty.Register(
            nameof(EnableCopyToClipboard),
            typeof(bool),
            typeof(CodePresenter),
            new PropertyMetadata(default(bool)));

        public bool EnableCopyToClipboard
        {
            get { return (bool)GetValue(EnableCopyToClipboardProperty); }
            set { SetValue(EnableCopyToClipboardProperty, value); }
        }

        #endregion

        #region Search

        public static readonly DependencyPropertyKey SearchEnginePropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(SearchEngine),
                typeof(ISearchEngine),
                typeof(CodePresenter),
                new FrameworkPropertyMetadata(default(ISearchEngine), FrameworkPropertyMetadataOptions.None, OnSearchEngineChanged));

        private static void OnSearchEngineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CodePresenter self)
            {
                self.OnSearchEngineChanged(e.OldValue as ISearchEngine);
            }
        }

        public static readonly DependencyProperty SearchEngineProperty = SearchEnginePropertyKey.DependencyProperty;

        public ISearchEngine SearchEngine
        {
            get => (ISearchEngine)GetValue(SearchEngineProperty);
            protected set => SetValue(SearchEnginePropertyKey, value);
        }

        private void OnSearchEngineChanged(ISearchEngine oldValue)
        {
            if (oldValue != null)
            {
                SearchEngine.ApplySearch -= OnSearchEngineApplySearch;
            }

            if (SearchEngine != null)
            {
                SearchEngine.ApplySearch += OnSearchEngineApplySearch;
            }
        }

        public static readonly DependencyProperty UseSearchEngineProperty = DependencyProperty.Register(
            nameof(UseSearchEngine),
            typeof(bool),
            typeof(CodePresenter),
            new PropertyMetadata(default(bool), OnUseSearchEngineChanged));

        private static void OnUseSearchEngineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CodePresenter self)
            {
                self.SearchEngine = self.UseSearchEngine ? new SearchEngine<string>() : null;
            }
        }

        public bool UseSearchEngine
        {
            get => (bool)GetValue(UseSearchEngineProperty);
            set => SetValue(UseSearchEngineProperty, value);
        }

        #region Event handlers

        private void OnSearchEngineApplySearch(object sender = null, EventArgs e = null)
        {
            var isTextSelected = false;

            var text = _textEditor.Text;
            if (text == null) return;

            _searchRenderer.CurrentResults.Clear();
            _textEditor.TextArea.ClearSelection();

            if (SearchEngine != null && !string.IsNullOrEmpty(text))
            {
                var offset = _textEditor.TextArea.Caret.Offset;

                foreach (var result in SearchEngine.Apply(_textEditor.Document.Text))
                {
                    var searchResult = new SearchResult
                    {
                        StartOffset = result.StartOffset,
                        Length = result.Length,
                        Data = result.Data
                    };

                    if (!isTextSelected && result.StartOffset >= offset)
                    {
                        SelectResult(searchResult);
                        isTextSelected = true;
                    }

                    _searchRenderer.CurrentResults.Add(searchResult);
                }
            }

            _textEditor.TextArea.TextView.InvalidateLayer(KnownLayer.Selection);
        }

        #endregion

        private void SelectResult(SearchResult result)
        {
            _textEditor.TextArea.Caret.Offset = result.StartOffset;
            _textEditor.TextArea.Selection = Selection.Create(_textEditor.TextArea, result.StartOffset, result.EndOffset);
            _textEditor.TextArea.Caret.BringCaretToView();
            // show caret even if the editor does not have the Keyboard Focus
            _textEditor.TextArea.Caret.Show();
        }

        /// <summary>
        /// Moves to the next occurrence in the file.
        /// </summary>
        public void FindNext()
        {
            var result = _searchRenderer.CurrentResults.FindFirstSegmentWithStartAfter(_textEditor.TextArea.Caret.Offset + 1)
                         ?? _searchRenderer.CurrentResults.FirstSegment;
            if (result != null)
            {
                SelectResult(result);
            }
        }

        /// <summary>
        /// Moves to the previous occurrence in the file.
        /// </summary>
        public void FindPrevious()
        {
            var result = _searchRenderer.CurrentResults.FindFirstSegmentWithStartAfter(_textEditor.TextArea.Caret.Offset);
            if (result != null) result = _searchRenderer.CurrentResults.GetPreviousSegment(result);
            if (result == null) result = _searchRenderer.CurrentResults.LastSegment;
            if (result != null)
            {
                SelectResult(result);
            }
        }

        #endregion
    }
}
