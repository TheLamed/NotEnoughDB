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

namespace NotEnoughDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Data data => DataContext as Data;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new Data(this);
        }

        private void ButtonGo_Click(object sender, RoutedEventArgs e)
        {
            if (SQLite.IsChecked ?? false)  data.Initialise(DataBases.SQLite);
            if (Neo4j.IsChecked ?? false)   data.Initialise(DataBases.Neo4j);
            if (FireBase.IsChecked ?? false) data.Initialise(DataBases.FireBase);
            if (MongoDB.IsChecked ?? false) data.Initialise(DataBases.MongoDB);

            if (!data.IsController()) return;

            main.Visibility = Visibility.Visible;
            first.Visibility = Visibility.Collapsed;
        }

        private void OpenUsers(object sender, MouseButtonEventArgs e)
        {
            data.ChangeEntity(Entities.User);
        }
        private void OpenServers(object sender, MouseButtonEventArgs e)
        {
            data.ChangeEntity(Entities.Server);
        }
        private void OpenOrders(object sender, MouseButtonEventArgs e)
        {
            data.ChangeEntity(Entities.Order);
        }
    }
}
