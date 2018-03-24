using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class NotebookView : UserControl, IViewFor<NotebookTreeItem>
    {
        public NotebookView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, x => x.Title, x => x.Title.Text);
            this.OneWayBind(ViewModel, x => x.Color, x => x.Title.Foreground);
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(SectionTreeItem), typeof(SectionView), new PropertyMetadata(default(SectionTreeItem)));


        public NotebookTreeItem ViewModel
        {
            get { return (NotebookTreeItem)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }


        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (NotebookTreeItem)value;
        }

    }
}
