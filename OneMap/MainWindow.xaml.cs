using System;
using System.Windows;

using MahApps.Metro.Controls;

using ReactiveUI;

namespace OneMap
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IViewFor<MainWindowViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(MainWindowViewModel), typeof(MainWindow),
            new PropertyMetadata(default(MainWindowViewModel)));

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();

            DataContext = ViewModel;

            this.Bind(ViewModel, x => x.SelectedTab, x => x.Tabs.SelectedItem);

            this.BindCommand(ViewModel, x => x.MoveUp, x => x.MoveUp);

            this.BindCommand(ViewModel, x => x.MoveDown, x => x.MoveDown);

            this.BindCommand(ViewModel, x => x.Promote, x => x.Promote);

            this.BindCommand(ViewModel, x => x.Demote, x => x.Demote);

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