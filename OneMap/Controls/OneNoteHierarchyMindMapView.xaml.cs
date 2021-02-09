using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using ReactiveUI;

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

                if (adornerLayer == null)
                {
                    return;
                }

                var leftAdorner = new MindMapLinkAdorner(LeftTree, Title, true);

                adornerLayer.Add(leftAdorner);

                adornerLayer = AdornerLayer.GetAdornerLayer(RightTree);

                adornerLayer.Add(new MindMapLinkAdorner(RightTree, Title, false));

            });

            DataContext = ViewModel;

            this.WhenActivated(disposable =>
            {
                ViewModel.Refresh();

                this.WhenAnyValue(x => x.LeftTree.SelectedItem).BindTo(this, x => x.ViewModel.LeftSelection)
                    .DisposeWith(disposable);

                this.WhenAnyValue(x => x.RightTree.SelectedItem).BindTo(this, x => x.ViewModel.RightSelection)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel, x => x.Title, x => x.Title.Text).DisposeWith(disposable);

                LeftTree.ItemsSource = ViewModel.LeftTreeItems;

                RightTree.ItemsSource = ViewModel.RightTreeItems;

            });

        }

    }
}
