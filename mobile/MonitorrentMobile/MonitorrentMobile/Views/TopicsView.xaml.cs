﻿using System.ComponentModel;
using MonitorrentMobile.Enums;
using MonitorrentMobile.ViewModel;
using Xamarin.Forms;

namespace MonitorrentMobile.Views
{
    public partial class TopicsView : ContentPage
    {
        public TopicsView()
        {
            InitializeComponent();
        }

        private TopicsViewModel _context;

        protected override void OnBindingContextChanged()
        {
            if (_context != null)
            {
                _context.UpdateProgressAction = null;
                _context.PropertyChanged -= ContextOnPropertyChanged;
            }
            _context = this.BindingContext as TopicsViewModel;
            if (_context != null)
            {
                _context.PropertyChanged += ContextOnPropertyChanged;
                _context.UpdateProgressAction = ProgressBar.ProgressTo;
                ContextOnPropertyChanged(_context, new PropertyChangedEventArgs(nameof(_context.CompletionStatus)));
            }
            
            base.OnBindingContextChanged();
        }

        //TODO couldn't find a better way, binding on BackgroundColor doesn't work for some reason
        private void ContextOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(_context.CompletionStatus))
            {
                switch (_context.CompletionStatus)
                {
                    case CompletionStatus.Executing:
                    case CompletionStatus.Success:
                        Layout.BackgroundColor = Color.White;
                        return;
                    case CompletionStatus.Error:
                        Layout.BackgroundColor = Color.FromHex("FFCDD2");
                        return;
                }
            }
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //Do not allow item selection
            ((ListView)sender).SelectedItem = null;
        }
    }
}