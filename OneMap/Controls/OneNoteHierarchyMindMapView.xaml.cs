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
using ReactiveUI.Events;

using Splat;

namespace OneMap.Controls
{
    /// <summary>
    /// Interaction logic for OneNoteHierarchyMindMapView.xaml
    /// </summary>
    public partial class OneNoteHierarchyMindMapView : ReactiveUserControl<OneNoteHierarchyMindMapViewModel>
    {
        public OneNoteHierarchyMindMapView()
        {
            InitializeComponent();


            ViewModel = Locator.Current.GetService<OneNoteHierarchyMindMapViewModel>();

            DataContext = ViewModel;
            LeftTree.ItemsSource = ViewModel.LeftTreeItems;
            RightTree.ItemsSource = ViewModel.RightTreeItems;

            this.Events().Loaded.Subscribe(args =>
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(LeftTree);

                var leftAdorner = new MindMapLinkAdorner(LeftTree, Title, true);

                adornerLayer.Add(leftAdorner);

                adornerLayer = AdornerLayer.GetAdornerLayer(RightTree);

                adornerLayer.Add(new MindMapLinkAdorner(RightTree, Title, false));

            });

            DataContext = ViewModel;

            this.WhenActivated(disposable =>
            {
                ViewModel.Refresh();
                this.Bind(ViewModel, x => x.LeftSelection, x => x.LeftTree.SelectedItem).DisposeWith(disposable);
                this.Bind(ViewModel, x => x.RightSelection, x => x.RightTree.SelectedItem).DisposeWith(disposable);
                this.OneWayBind(ViewModel, x => x.Title, x => x.Title.Text).DisposeWith(disposable);
                //                this.OneWayBind(ViewModel, x => x.LeftTreeItems, x => x.LeftTree.ItemsSource).DisposeWith(disposable);
                //                this.OneWayBind(ViewModel, x => x.RightTreeItems, x => x.RightTree.ItemsSource).DisposeWith(disposable);
                LeftTree.ItemsSource = ViewModel.LeftTreeItems;
                RightTree.ItemsSource = ViewModel.RightTreeItems;

            });



        }

    }
}
