using System;
using System.Windows;

using MahApps.Metro.Controls;

using OneMap.Controls;

using ReactiveUI;

using Splat;

namespace OneMap
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IViewFor<MainWindowViewModel>, IEnableLogger
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(MainWindowViewModel), typeof(MainWindow),
            new PropertyMetadata(default(MainWindowViewModel)));

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();

            DataContext = ViewModel;

        //    this.OneWayBind(ViewModel, x => x.Tabs, x => x.Tabs.ItemsSource);

            this.Bind(ViewModel, x => x.SelectedTab, x => x.Tabs.SelectedItem, x => (object)x , x => x as MindMapViewModel);

            this.BindCommand(ViewModel, x => x.MoveUp, x => x.MoveUp);

            this.BindCommand(ViewModel, x => x.MoveDown, x => x.MoveDown);

            this.BindCommand(ViewModel, x => x.Promote, x => x.Promote);

            this.BindCommand(ViewModel, x => x.Demote, x => x.Demote);

            this.BindCommand(ViewModel, x => x.ViewPage, x => x.ViewPage);

            this.Events().Loaded.Subscribe(x => { ViewModel.SelectFirstTab(); });

        }




        public MainWindowViewModel ViewModel
        {
            get => (MainWindowViewModel) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }


        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainWindowViewModel) value;
        }
    }
}