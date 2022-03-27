using HandyControl.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Media;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Input;

namespace Автомир
{
    public partial class MainWindow : System.Windows.Window
    {
        bool checkedValue = false;
        int manufid = -1;
        int[] manids = new int[100];
        System.Windows.Controls.ComboBox search_man;
        System.Windows.Controls.TextBox search;
        CheckBox onlyActive;
        Grid gridmain;
        public MainWindow()
        {
            InitializeComponent();

            /*Login login = new Login();
            this.Hide();
            login.ShowDialog();
            this.Show();*/
            gridmain = new Grid();
            gridmain.ColumnDefinitions.Add(new ColumnDefinition());
            gridmain.ColumnDefinitions.Add(new ColumnDefinition());
            gridmain.ColumnDefinitions.Add(new ColumnDefinition());

            header.Children.Add(gridmain);
            Grid.SetColumn(gridmain, 0);
            Grid.SetRow(gridmain, 1);

            Grid ngrid = new Grid();
            ngrid.ColumnDefinitions.Add(new ColumnDefinition());
            ngrid.ColumnDefinitions.Add(new ColumnDefinition());
            ngrid.ColumnDefinitions.Add(new ColumnDefinition());
            ngrid.ColumnDefinitions.Add(new ColumnDefinition());

            var rowDefinitionFirst = new RowDefinition();
            rowDefinitionFirst.Height = new GridLength(35, GridUnitType.Pixel);
            ngrid.RowDefinitions.Add(rowDefinitionFirst);

            search = new System.Windows.Controls.TextBox();
            search.TextChanged += Search_TextChanged;
            ngrid.Children.Add(search);
            Grid.SetRow(search, 0);
            Grid.SetColumn(search, 0);

            Button addGood = new Button();
            addGood.Content = "Новый товар";
            addGood.Click += AddGood_Click;
            ngrid.Children.Add(addGood);
            Grid.SetRow(addGood, 0);
            Grid.SetColumn(addGood, 2);

            search_man = new System.Windows.Controls.ComboBox();
            search_man.SelectionChanged += Search_man_SelectionChanged;
            ngrid.Children.Add(search_man);
            Grid.SetRow(search_man, 0);
            Grid.SetColumn(search_man, 1);

            onlyActive = new CheckBox();
            onlyActive.Click += OnlyActive_Click;
            onlyActive.Content = "Только недоступный товар";
            ngrid.Children.Add(onlyActive);
            Grid.SetRow(onlyActive, 0);
            Grid.SetColumn(onlyActive, 3);

            header.Children.Add(ngrid);
            Grid.SetRow(ngrid, 0);
            Grid.SetColumn(ngrid, 0);

            Loaded += OnLoad;

            String connectionString = Properties.Settings.Default.mainConnection;
            SqlConnection sql = new SqlConnection(connectionString);
            sql.Open();

            String query = "SELECT * FROM Product";
            SqlCommand command = new SqlCommand(query);
            command.Connection = sql;

            command = new SqlCommand("SELECT * FROM Manufacturer");
            command.Connection = sql;
            SqlDataReader reader = command.ExecuteReader();
            search_man.Items.Add("Все элементы");
            while (reader.Read())
            {
                int index = search_man.Items.Add(reader["Name"].ToString());
                manids[index] = int.Parse(reader["ID"].ToString());
            }

        }

        private void OnlyActive_Click(object sender, RoutedEventArgs e)
        {
            checkedValue = (bool)onlyActive.IsChecked;
        }
        public void OnLoad(object sender, RoutedEventArgs e)
        {
            String connectionString = Properties.Settings.Default.mainConnection;
            SqlConnection sql = new SqlConnection(connectionString);
            sql.Open();

            String query = "SELECT COUNT(*) FROM Product";
            SqlCommand command = new SqlCommand(query);
            command.Connection = sql;

            int values = (int)command.ExecuteScalar();
            int rows = values / 3 + 1;
            for (int i = 0; i < rows+1; i++)
            {
                var rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                gridmain.RowDefinitions.Add(rowDefinition);
            }


            UpdateThis(null);
        }
        private void Search_man_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int pos = search_man.SelectedIndex;
            if (pos == -1) return;
            if (pos != 0) manufid = manids[pos];
            else manufid = -1;
            UpdateThis(search.Text);
        }

        public void UpdateThis(String searchText)
        {
            gridmain.Children.Clear();

            String connectionString = Properties.Settings.Default.mainConnection;
            SqlConnection sql = new SqlConnection(connectionString);
            sql.Open();

            string query = "SELECT * FROM Product WHERE Title LIKE '%" + searchText + "%'";
            if (searchText == null && search.Text.Length == 0 && manufid == -1)
            {
                query = "SELECT * FROM Product";
                if (!checkedValue) query += " WHERE IsActive = 'False'";
            }
            else
            {
                if (manufid != -1) query += " AND ManufacturerID = " + manufid;
                if (!checkedValue) query += " WHERE IsActive = 'False'";
            }


            SqlCommand command = new SqlCommand(query);
            command.Connection = sql;

            int z = 0;
            int r = 1;

           SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                StackPanel ngrid = new StackPanel();

                ngrid.Width = 200;
                ngrid.HorizontalAlignment = HorizontalAlignment.Center;

                String photo = reader["MainImagePath"].ToString();

                if (photo.Contains(" Товары автосервиса"))
                {
                    photo = photo.Replace(" Товары автосервиса", "");
                    photo = $"{AppDomain.CurrentDomain.BaseDirectory}/images/{photo}";

                }
                if (photo.Contains("Товары автосервиса"))
                {
                    photo = photo.Replace("Товары автосервиса", "");
                    photo = $"{AppDomain.CurrentDomain.BaseDirectory}/images/{photo}";
                }

                int id = int.Parse(reader["ID"].ToString());

                Image img = new Image();
                img.Source = new BitmapImage(new Uri(photo));
                img.Height = 200;
                img.Width = 100;
                img.MouseUp += (s, e) =>
                {
                    Product product = new Product(id);
                    this.Hide(); 
                    product.ShowDialog();
                    this.Show();
                };


                ngrid.Children.Add(img);

                Label name = new Label();
                name.Content = reader["Title"].ToString();
                name.HorizontalAlignment = HorizontalAlignment.Center;
                ngrid.Children.Add(name);


                Label price = new Label();
                price.Content = reader["Cost"].ToString() + " руб";
                price.HorizontalAlignment = HorizontalAlignment.Center;
                ngrid.Children.Add(price);

                bool isActive = bool.Parse(reader["IsActive"].ToString());
                if (!isActive)
                {
                    ngrid.Background = new SolidColorBrush(Colors.Gray);
                    Label neactive = new Label();
                    neactive.Content = "Неактивно";
                    neactive.HorizontalAlignment = HorizontalAlignment.Center;
                    ngrid.Children.Add(neactive);
                }
                else ngrid.Background = Brushes.White;

                gridmain.Children.Add(ngrid);
                Grid.SetRow(ngrid, r);
                Grid.SetColumn(ngrid, z);
                gridmain.RowDefinitions[r].Height = GridLength.Auto;
                z++;
                if (z == 3)
                {
                    z = 0;
                    r++;
                }
            }
        }
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateThis(search.Text);
        }

        private void AddGood_Click(object sender, RoutedEventArgs e)
        {
            NewProduct np = new NewProduct();
            this.Hide();
            np.ShowDialog();
            this.Show();
        }
    }
}
