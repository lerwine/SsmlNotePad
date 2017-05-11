using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Erwine.Leonard.T.SsmlNotePad.Model.Workers
{
    public class TextChangeArgs
    {
        private object _syncRoot = new object();
        private string _sourceText;
        private int _firstVisibleLineIndex;
        private Collection<int> _innerVisibleLineStartIndexes = new Collection<int>();
        private ReadOnlyCollection<int> _visibleLineStartIndexes = null;
        private Collection<Rect> _innerVisibleLineStartRects = new Collection<Rect>();
        private ReadOnlyCollection<Rect> _visibleLineStartRects = null;
        private int _selectionLength;
        private int _selectionStart;
        private Task<TextLine[]> _parseLinesTask;
        private Task<XmlValidationResult[]> _validateXmlTask;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public Task<TextLine[]> ParseLinesTask { get { return _parseLinesTask; } }

        public Task<XmlValidationResult[]> ValidateXmlTask { get { return _validateXmlTask; } }

        public void CancelTasks()
        {
            lock (_syncRoot)
            {
                if (_tokenSource != null)
                    _tokenSource.Cancel();
            }
        }
        /// <summary>
        /// The zero-based index of for the first visible line of <see cref="Text"/>.
        /// </summary>
        public int FirstVisibleLineIndex { get { return _firstVisibleLineIndex; } }

        /// <summary>
        /// Represents the zero-based index for the characters within <see cref="Text"/> corresponding to the start of each displayed line.
        /// </summary>
        public ReadOnlyCollection<int> VisibleLineStartIndexes
        {
            get
            {
                if (_visibleLineStartIndexes == null)
                    _visibleLineStartIndexes = new ReadOnlyCollection<int>(_innerVisibleLineStartIndexes);
                return _visibleLineStartIndexes;
            }
        }

        /// <summary>
        /// Represents the leading edge for the characters within <see cref="Text"/> corresponding to the start of each displayed line,
        /// or <seealso cref="Rect.Empty"/> if a bounding rectangle could not be determined.
        /// </summary>
        public ReadOnlyCollection<Rect> VisibleLineStartRects
        {
            get
            {
                if (_visibleLineStartRects == null)
                    _visibleLineStartRects = new ReadOnlyCollection<Rect>(_innerVisibleLineStartRects);
                return _visibleLineStartRects;
            }
        }

        /// <summary>
        /// The selected text within <see cref="Text"/>.
        /// </summary>
        public string SelectedText { get { return (_selectionLength < 0) ? null : ((_selectionLength == 0) ? "" : SourceText.Substring(_selectionStart, _selectionLength)); } }

        /// <summary>
        /// The number of characters in the selected text within <see cref="Text"/>.
        /// </summary>
        public int SelectionLength { get { return _selectionLength; } }

        /// <summary>
        /// The character index for the beginning of the selected text within <see cref="Text"/>.
        /// </summary>
        public int SelectionStart { get { return _selectionStart; } }

        public bool IsLayoutUpdated { get { return _firstVisibleLineIndex > -1; } }

        public string SourceText { get { return _sourceText; } }

        public bool TryCreateNewTaskArgs(TextBox ssmlTextBox, bool isLayoutUpdated, out TextChangeArgs newTaskArgs)
        {
            string sourceText = ssmlTextBox.Text;
            if (sourceText != _sourceText)
            {
                newTaskArgs = new TextChangeArgs(ssmlTextBox, isLayoutUpdated);
                return true;
            }

            if (!isLayoutUpdated)
            {
                newTaskArgs = null;
                return false;
            }

            if (_firstVisibleLineIndex == ssmlTextBox.GetFirstVisibleLineIndex() && _selectionStart == ssmlTextBox.SelectionStart && _selectionLength == ssmlTextBox.SelectionLength && _innerVisibleLineStartIndexes.Count == (ssmlTextBox.GetLastVisibleLineIndex() - _firstVisibleLineIndex) + 1)
            {
                newTaskArgs = null;
                return false;
            }

            lock (_syncRoot)
            {
                if (_tokenSource == null)
                    newTaskArgs = new TextChangeArgs(ssmlTextBox, isLayoutUpdated);
                else
                {
                    newTaskArgs = new TextChangeArgs(ssmlTextBox, _parseLinesTask, _validateXmlTask, _tokenSource);
                    _parseLinesTask = null;
                    _validateXmlTask = null;
                    _tokenSource = null;
                }
            }
            return true;
        }

        private TextChangeArgs(TextBox ssmlTextBox, Task<TextLine[]> parseLinesTask, Task<XmlValidationResult[]> validateXmlTask, CancellationTokenSource tokenSource)
        {
            _sourceText = ssmlTextBox.Text;
            _tokenSource = tokenSource;
            _parseLinesTask = parseLinesTask;
            _validateXmlTask = validateXmlTask;
            InitializeLayoutUpdated(ssmlTextBox);
        }

        public TextChangeArgs(TextBox ssmlTextBox, bool isLayoutUpdated)
        {
            _sourceText = ssmlTextBox.Text;
            _tokenSource = new CancellationTokenSource();
            _parseLinesTask = TextLine.SplitAsync(_sourceText, _tokenSource.Token);
            _validateXmlTask = XmlValidationResult.ValidateAsync(_parseLinesTask, _tokenSource.Token);
            if (isLayoutUpdated)
                InitializeLayoutUpdated(ssmlTextBox);
            else
            {
                _selectionStart = -1;
                _selectionLength = -1;
                _firstVisibleLineIndex = -1;
            }
        }

        private void InitializeLayoutUpdated(TextBox ssmlTextBox)
        {
            _selectionStart = ssmlTextBox.SelectionStart;
            _selectionLength = ssmlTextBox.SelectionLength;
            _firstVisibleLineIndex = ssmlTextBox.GetFirstVisibleLineIndex();
            int visibleLineCount = (ssmlTextBox.GetLastVisibleLineIndex() - _firstVisibleLineIndex) + 1;
            for (int i = 0; i < visibleLineCount; i++)
            {
                int characterIndex = ssmlTextBox.GetCharacterIndexFromLineIndex(_firstVisibleLineIndex + i);
                if (characterIndex > -1 && characterIndex < ssmlTextBox.Text.Length)
                {
                    _innerVisibleLineStartIndexes.Add(characterIndex);
                    _innerVisibleLineStartRects.Add(ssmlTextBox.GetRectFromCharacterIndex(characterIndex));
                }
            }
        }
    }
}
