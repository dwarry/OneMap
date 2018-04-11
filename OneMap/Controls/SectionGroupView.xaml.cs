using System;
using System.Collections.Generic;
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
    /// Interaction logic for SectionGroupView.xaml
    /// </summary>
    public partial class SectionGroupView : UserControl, IViewFor<SectionGroupTreeItem>
    {
        public SectionGroupView()
        {
            InitializeComponent();

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
            "ViewModel", typeof(SectionGroupTreeItem), typeof(SectionGroupView), new PropertyMetadata(default(SectionGroupTreeItem)));


        public SectionGroupTreeItem ViewModel
        {
            get => (SectionGroupTreeItem)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }


        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (SectionGroupTreeItem)value;
        }

    }
}
