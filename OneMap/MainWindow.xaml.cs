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

using MahApps.Metro.Controls;

using OneMap.Controls;
using OneMap.OneNote;

using ReactiveUI;

using Splat;

namespace OneMap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Locator.CurrentMutable.Register(() => new NotebookView(), typeof(IViewFor<NotebookTreeItem>));
            Locator.CurrentMutable.Register(() => new SectionGroupView(), typeof(IViewFor<SectionGroupTreeItem>));
            Locator.CurrentMutable.Register(() => new SectionView(), typeof(IViewFor<SectionTreeItem>));
            Locator.CurrentMutable.Register(() => new PageView(), typeof(IViewFor<PageTreeItem>));


            this.Events().Loaded.Subscribe(args =>
            {
                var notebooks = new OneNotePersistence().LoadNotebooks();

                var vms = notebooks.Notebook.Select((x, i) => new NotebookTreeItem(x, i)).ToList();

                this.OneNoteHierarchy.ItemsSource = vms;
            });
        }
    }
}
