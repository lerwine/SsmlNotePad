using Erwine.Leonard.T.SsmlNotePad.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class ProcessMessageCollection<TItem> : ObservableCollection<TItem>, Reserved.IProcessMessageViewModelCollection<ReadOnlyObservableCollection<TItem>, TItem>
        where TItem : DependencyObject, Reserved.IProcessMessageViewModel
    {
        private string _message = "";
        private Model.AlertLevel _messageStatus = Model.AlertLevel.None;
        private ObservableCollection<TItem> _verboseItems;
        private ObservableCollection<TItem> _informationItems = new ObservableCollection<TItem>();
        private ObservableCollection<TItem> _alertItems = new ObservableCollection<TItem>();
        private ObservableCollection<TItem> _warningItems = new ObservableCollection<TItem>();
        private ObservableCollection<TItem> _errorItems = new ObservableCollection<TItem>();

        public const string PropertyName_Message = "Message";
        public const string PropertyName_MessageStatus = "MessageStatus";

        public string Message
        {
            get { return _message; }
            set
            {
                string s = value ?? "";
                if (s == _message)
                    return;

                _message = s;
                OnPropertyChanged(new PropertyChangedEventArgs(PropertyName_MessageStatus));
            }
        }

        public Model.AlertLevel MessageStatus
        {
            get { return _messageStatus; }
            set
            {
                if (value == _messageStatus)
                    return;

                _messageStatus = value;
                OnPropertyChanged(new PropertyChangedEventArgs(PropertyName_MessageStatus));
            }
        }

        public ReadOnlyObservableCollection<TItem> VerboseItems { get; private set; }

        public ReadOnlyObservableCollection<TItem> InformationItems { get; private set; }

        public ReadOnlyObservableCollection<TItem> AlertItems { get; private set; }

        public ReadOnlyObservableCollection<TItem> WarningItems { get; private set; }

        public ReadOnlyObservableCollection<TItem> ErrorItems { get; private set; }

        protected override void ClearItems()
        {
            base.ClearItems();
            _verboseItems.Clear();
            _informationItems.Clear();
            _alertItems.Clear();
            _warningItems.Clear();
            _errorItems.Clear();
            Message = "";
            MessageStatus = Model.AlertLevel.None;
        }

        protected override void InsertItem(int index, TItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            base.InsertItem(index, item);

            CheckAdd(item);
        }

        protected override void RemoveItem(int index)
        {
            TItem item = this[index];
            base.RemoveItem(index);
            CheckRemove(item);
        }

        protected override void SetItem(int index, TItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            TItem oldItem = (index < Count) ? this[index] : null;
            base.SetItem(index, item);
            CheckRemove(oldItem);
            CheckAdd(item);
        }

        private void CheckAdd(TItem item)
        {
            if (item.Level == Model.MessageLevel.Diagnostic || _verboseItems.Any(i => ReferenceEquals(i, item)))
                return;

            _verboseItems.Add(item);

            if (item.Level == Model.MessageLevel.Verbose)
                return;

            _informationItems.Add(item);

            if (item.Level == Model.MessageLevel.Information)
                return;

            _alertItems.Add(item);

            if (item.Level == Model.MessageLevel.Alert)
                return;

            _warningItems.Add(item);

            if (item.Level != Model.MessageLevel.Warning)
                _errorItems.Add(item);
        }

        private void CheckRemove(TItem item)
        {
            if (item == null || this.Any(i => ReferenceEquals(i, item)))
                return;

            _verboseItems.Remove(item);
            _informationItems.Remove(item);
            _alertItems.Remove(item);
            _warningItems.Remove(item);
            _errorItems.Remove(item);
        }

        private void UpdateMessage()
        {
            var g = this.GroupBy(i => i.Level).OrderByDescending(i => i.Key).LastOrDefault();
            if (g == null)
            {
                Message = "";
                MessageStatus = Model.AlertLevel.None;
                return;
            }

            TItem item = g.OrderByDescending(a => a.Created).First();
            MessageStatus = item.Level.ToAlertLevel();
            Message = item.Message;
        }

        public ProcessMessageCollection()
        {
            VerboseItems = new ReadOnlyObservableCollection<TItem>(_verboseItems);
            InformationItems = new ReadOnlyObservableCollection<TItem>(_informationItems);
            AlertItems = new ReadOnlyObservableCollection<TItem>(_alertItems);
            WarningItems = new ReadOnlyObservableCollection<TItem>(_warningItems);
            ErrorItems = new ReadOnlyObservableCollection<TItem>(_errorItems);
        }
    }
}