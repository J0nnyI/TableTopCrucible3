﻿using System;
using System.Linq;
using System.Windows;
using ReactiveUI;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Domain.Library.Wpf.Pages.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.Pages.Views
{
    /// <summary>
    ///     Interaction logic for LibraryPageV.xaml
    /// </summary>
    public partial class LibraryPageV : ReactiveUserControl<LibraryPageVm>
    {
        public LibraryPageV()
        {
            InitializeComponent();
            this.WhenActivated(() => new IDisposable[]
            {
                this.Bind(ViewModel,
                    vm => vm.ItemList,
                    v => v.ItemList.ViewModel),
                this.Bind(ViewModel,
                    vm => vm.ListHeader,
                    v => v.ListHeader.ViewModel),
                this.Bind(ViewModel,
                    vm => vm.Filter,
                    v => v.Filter.ViewModel),
                this.Bind(ViewModel,
                    vm => vm.ModelViewer,
                    v => v.ModelViewer.ViewModel),
                this.Bind(ViewModel,
                    vm => vm.DataViewer,
                    v => v.DataViewer.ViewModel),
                this.Bind(ViewModel,
                    vm => vm.Actions,
                    v => v.Actions.ViewModel),
                this.Bind(ViewModel,
                    vm => vm.ViewerHeader,
                    v => v.ViewerHeader.ViewModel),
                this.Bind(ViewModel,
                    vm => vm.FileList,
                    v => v.FileList.ViewModel),
                this.Bind(ViewModel,
                    vm => vm.Gallery,
                    v => v.Gallery.ViewModel)
            });
        }

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FileDrop"))
            {
                var paths =
                    (e.Data.GetData("FileDrop") as string[])
                    .Select(FilePath.From).ToArray()
                    .ToArray();
                ViewModel.HandleFileDrop(paths);
            }
        }
    }
}