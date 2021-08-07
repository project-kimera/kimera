﻿using Kimera.Common.Commands;
using Kimera.Data.Entities;
using System.Collections.ObjectModel;
using System.Windows;

namespace Kimera.ViewModels
{
    public class EditCategoryNameViewModel : ViewModelBase
    {
        #region ::Variables & Properties::

        private string _caption = string.Empty;

        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                RaisePropertyChanged();
            }
        }


        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();

        public ObservableCollection<Category> Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
                RaisePropertyChanged();
            }
        }

        private Category _selectedCategory;

        public Category SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }
            set
            {
                _selectedCategory = value;

                Text = value?.Name;

                RaisePropertyChanged();
            }
        }

        private string _text = string.Empty;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ::Commands::

        public RelayCommand<Window> CancelCommand { get; }

        public RelayCommand<Window> ConfirmCommand { get; }

        #endregion

        #region ::Constructors::

        public EditCategoryNameViewModel()
        {
            CancelCommand = new RelayCommand<Window>(Cancel);
            ConfirmCommand = new RelayCommand<Window>(Confirm);
        }

        #endregion

        #region ::Command Actions::

        private void Cancel(Window window)
        {
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
        }

        private void Confirm(Window window)
        {
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        #endregion
    }
}
