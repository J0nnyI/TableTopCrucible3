﻿
using ReactiveUI;

using TableTopCrucible.Domain.Settings.Wpf.PageViewModels;

namespace TableTopCrucible.Domain.Settings.Wpf.Pages
{
    public partial class DirectorySetupPageV : ReactiveUserControl<DirectorySetupPageVm>, IActivatableView
    {
        public DirectorySetupPageV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.Bind(ViewModel,
                    vm=>vm.DirectorySetupList,
                    v=>v.DirectorySetupList.ViewModel),
                this.Bind(ViewModel,
                    vm=>vm.Title.Value,
                    v=>v.Title.Text),
            });
        }
    }
}