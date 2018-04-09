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
    /// Interaction logic for HeadingView.xaml
    /// </summary>
    public partial class HeadingView : UserControl, IViewFor<HeadingTreeItem>
    {
        public HeadingView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, x => x.Title, x => x.Title.Text);
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(HeadingTreeItem), typeof(HeadingView), new PropertyMetadata(default(HeadingTreeItem)));


        public HeadingTreeItem ViewModel
        {
            get { return (HeadingTreeItem) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (HeadingTreeItem) value;
        }
    }
}
