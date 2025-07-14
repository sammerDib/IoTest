using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;

using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.LogViewer
{
    public class LogViewer : TextEditor
    {
        public LogViewer()
        {
            TextArea.DefaultInputHandler.NestedInputHandlers.Add(new TextAreaInputHandler(TextArea));
            DataContextChanged += LogViewer_DataContextChanged;
            Document.Changing += Document_Changing;
            Document.Changed += Document_Changed;
            Document.UndoStack.SizeLimit = 0;
        }

        #region AutoScroll

        private bool _shouldAutoScroll;

        private bool IsScrolledToEnd => VerticalOffset + ViewportHeight >= ExtentHeight;

        private void Document_Changing(object sender, DocumentChangeEventArgs e)
        {
            _shouldAutoScroll = IsScrolledToEnd && e.Offset == Document.TextLength;
        }

        private void Document_Changed(object sender, DocumentChangeEventArgs e)
        {
            if (_shouldAutoScroll)
            {
                ScrollToLine(LineCount);
            }
        }

        #endregion AutoScroll

        private void ResetSyntaxHighlighting()
        {
            SyntaxHighlighting = new Highlighter();

            foreach (var item in Enum.GetNames(typeof(ExecutionState)))
            {
                var rule = new HighlightingRule
                {
                    Regex = new Regex(item),
                    Color = new HighlightingColor
                    {
                        Foreground = new SimpleHighlightingBrush(Brushes.SeverityWarningBrush.Color)
                        //FontWeight = FontWeights.Bold
                    }
                };
                SyntaxHighlighting.MainRuleSet.Rules.Add(rule);
            }

            var trueValue = new HighlightingRule
            {
                Regex = new Regex("true", RegexOptions.IgnoreCase),
                Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Brushes.SeverityInformationBrush.Color) }
            };
            SyntaxHighlighting.MainRuleSet.Rules.Add(trueValue);

            var falseValue = new HighlightingRule
            {
                Regex = new Regex("false", RegexOptions.IgnoreCase),
                Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Brushes.SeverityInformationBrush.Color) }
            };
            SyntaxHighlighting.MainRuleSet.Rules.Add(falseValue);

            if (_viewModel != null)
            {
                foreach (var platformType in ((Package)_viewModel.Device.GetTopContainer()).AllTypes<CSharpType>(t => t.PlatformType.IsEnum).Select(t => t.PlatformType))
                {
                    foreach (var literal in Enum.GetNames(platformType))
                    {
                        var rule = new HighlightingRule
                        {
                            Regex = new Regex(literal),
                            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Brushes.SeverityInformationBrush.Color) }
                        };

                        SyntaxHighlighting.MainRuleSet.Rules.Add(rule);
                    }
                }

                // Sort the list of highlight rules according to the decreasing length of the keyword
                // Fixes a display anomaly if the different values of an enumeration have common substrings
                var reversedSortList = SyntaxHighlighting.MainRuleSet.Rules.ToList();
                reversedSortList.Sort((r1, r2) => r2.Regex.ToString().Length.CompareTo(r1.Regex.ToString().Length));
                SyntaxHighlighting.MainRuleSet.Rules.Clear();
                foreach (var rule in reversedSortList)
                {
                    SyntaxHighlighting.MainRuleSet.Rules.Add(rule);
                }
            }
        }

        private LogViewerViewModel _viewModel;

        private void LogViewer_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.NewTraceAdded -= Logs_NewTraceAdded;
            }
            if (e.NewValue == null)
            {
                return;
            }
            _viewModel = (LogViewerViewModel)e.NewValue;
            _viewModel.NewTraceAdded += Logs_NewTraceAdded;
            _viewModel.LogsCleared += Logs_Cleared;

            ResetSyntaxHighlighting();

            foreach (HighlightingRule rule in _viewModel.SyntaxHighlighting.MainRuleSet.Rules)
            {
                SyntaxHighlighting.MainRuleSet.Rules.Add(rule);
            }

            Dispatcher.BeginInvoke((Action)delegate
            {
                Text = "";
                
                foreach (string log in _viewModel.Logs.ToList())
                {
                    AppendText(log);
                    AppendText(Environment.NewLine);
                }
            });
        }

        private void Logs_Cleared(object sender, EventArgs e)
        {
            Document.Text = "";
        }

        private void Logs_NewTraceAdded(object sender, NewTraceEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                AppendText(e.Trace);
                AppendText(Environment.NewLine);

                if (e.RemoveFirstTrace)
                {
                    // Delete line including the line delimiter so it will be marked as "Deleted" by the LineManager
                    var documentLine = Document.Lines[0];
                    Document.Remove(documentLine.Offset, documentLine.TotalLength);
                }
            });
        }
    }
}
