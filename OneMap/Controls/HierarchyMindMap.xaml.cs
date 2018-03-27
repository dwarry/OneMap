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

using Splat;

namespace OneMap.Controls
{
    /// <summary>
    /// Interaction logic for HierarchyMindMap.xaml
    /// </summary>
    public partial class HierarchyMindMap : UserControl, IViewFor<OneNoteHierarchyMindMapViewModel>
    {
        public HierarchyMindMap()
        {
            InitializeComponent();

            ViewModel = Locator.Current.GetService<OneNoteHierarchyMindMapViewModel>();

            DataContext = ViewModel;

            this.OneWayBind(ViewModel, x => x.Title, x => x.Title.Text);

        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(OneNoteHierarchyMindMapViewModel), typeof(HierarchyMindMap), new PropertyMetadata(default(OneNoteHierarchyMindMapViewModel)));


        public OneNoteHierarchyMindMapViewModel ViewModel
        {
            get { return (OneNoteHierarchyMindMapViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }


        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (OneNoteHierarchyMindMapViewModel)value;
        }
    }
}
