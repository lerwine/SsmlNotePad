using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    public class TextLineBackgroundParser
    {
        private object _syncRoot = new object();
        private string _text = "";
        private Task<Model.TextLine[]> _getTextLineInfo;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private List<Tuple<object, GetTextLinesHandler>> _onGetLineInfoComplete = new List<Tuple<object, GetTextLinesHandler>>();

        public TextLineBackgroundParser()
        {
            _getTextLineInfo = Task<Model.TextLine[]>.FromResult<Model.TextLine[]>(new Model.TextLine[] { new Model.TextLine(1, 0, "", "") });
        }

        public void GetTextLines(object state, GetTextLinesHandler onGetLineInfoComplete)
        {
            lock (_syncRoot)
            {
                if (_getTextLineInfo.IsCompleted)
                    Task.Factory.StartNew(o =>
                    {
                        object[] args = o as object[];
                        GetTextLinesHandler h = args[0] as GetTextLinesHandler;
                        h(args[1], args[2] as string, args[3] as Task<Model.TextLine[]>);
                    }, new object[] { onGetLineInfoComplete, state, _text, _getTextLineInfo });
                else
                    _onGetLineInfoComplete.Add(new Tuple<object, GetTextLinesHandler>(state, onGetLineInfoComplete));
            }
        }

        private void GetTextLineInfoCompleted(Task<Model.TextLine[]> task)
        {
            lock (_syncRoot)
            {
                if (task.Id != _getTextLineInfo.Id || task.IsCanceled)
                    return;

                foreach (Tuple<object, GetTextLinesHandler> handler in _onGetLineInfoComplete)
                {
                    task.ContinueWith((t, o) =>
                    {
                        object[] args = o as object[];
                        Tuple<object, GetTextLinesHandler> h = args[0] as Tuple<object, GetTextLinesHandler>;
                        GetTextLinesHandler onGetLineInfoComplete = h.Item2;
                        onGetLineInfoComplete(h.Item1, args[1] as string, t);
                    }, new object[] { handler, _text });
                }
                _onGetLineInfoComplete.Clear();
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                string text = value ?? "";

                lock (_syncRoot)
                {
                    if (text == _text)
                        return;

                    _text = text;

                    if (_getTextLineInfo.IsCompleted)
                    {
                        try { _tokenSource.Dispose(); }
                        catch { }
                    }
                    else
                    {
                        Task.Factory.StartNew(o =>
                        {
                            object[] args = o as object[];
                            CancellationTokenSource cts = args[0] as CancellationTokenSource;
                            if (!cts.IsCancellationRequested)
                                cts.Cancel();
                            Task<Model.TextLine[]> t = args[1] as Task<Model.TextLine[]>;
                            try
                            {
                                if (!t.IsCompleted)
                                    t.Wait(1000);
                            }
                            finally { cts.Dispose(); }
                        }, new object[] { _tokenSource, _getTextLineInfo });
                    }

                    _tokenSource = new CancellationTokenSource();
                    _getTextLineInfo = Model.TextLine.SplitAsync(_text, _tokenSource.Token);
                    _getTextLineInfo.ContinueWith(GetTextLineInfoCompleted);
                }
            }
        }
    }

    public delegate void GetTextLinesHandler(object state, string text, Task<Model.TextLine[]> task);
}
