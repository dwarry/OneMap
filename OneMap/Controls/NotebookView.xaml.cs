using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ReactiveUI;

namespace OneMap.Controls
{
    /// <summary>
    /// Interaction logic for NotebookView.xaml
    /// </summary>
    public partial class NotebookView : IViewFor<NotebookTreeItem>//, ISupportsActivation
    {
        public NotebookView()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, x => x.Title, x => x.Title.Text).DisposeWith(d);

                this.OneWayBind(ViewModel, x => x.ForegroundColor, x => x.Foreground, c => new SolidColorBrush(c))
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, x => x.BackgroundColor, x => x.Background, c => new SolidColorBrush(c))
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, x => x.BorderColor, x => x.Bd.BorderBrush, c => new SolidColorBrush(c))
                    .DisposeWith(d);
            });

        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(NotebookTreeItem), typeof(NotebookView), new PropertyMetadata(default(NotebookTreeItem)));


        public NotebookTreeItem ViewModel
        {
            get => (NotebookTreeItem) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (NotebookTreeItem)value;
        }


//        ViewModelActivator ISupportsActivation.Activator { get; } = new ViewModelActivator();
    }
}
