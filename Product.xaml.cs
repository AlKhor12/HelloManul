using System;
using System.Windows;
using System.Data.SqlClient;
using System.Windows.Media.Imaging;

namespace Автомир
{
    public partial class Product : Window
    {
        int id;
        bool isActivate;
        public Product(int id)
        {
            InitializeComponent();
            this.id = id;
            Loaded += OnLoad;
        }
        public void OnLoad(object sender, RoutedEventArgs e)
        {
            String connectionString = Properties.Settings.Default.mainConnection;
            SqlConnection sql = new SqlConnection(connectionString);
            sql.Open();

            String query = "SELECT * FROM Product WHERE ID = " + id;
            SqlCommand command = new SqlCommand(query);
            command.Connection = sql;

            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            name_product.Text = reader["Title"].ToString();
            price_product.Text = reader["Cost"].ToString();
            String photo = reader["MainImagePath"].ToString();

            if (photo.Contains(" Товары автосервиса"))
            {
                photo = photo.Replace(" Товары автосервиса", "");
                photo = AppDomain.CurrentDomain.BaseDirectory + "/images/" + photo;
            }
            if (photo.Contains("Товары автосервиса"))
            {
                photo = photo.Replace("Товары автосервиса", "");
                photo = AppDomain.CurrentDomain.BaseDirectory + "/images/" + photo;
            }
            isActivate = bool.Parse(reader["IsActive"].ToString());
            if (isActivate) checkBox.Content = "Активно";
            else checkBox.Content = "Неактивно";
            mainimage.Source = new BitmapImage(new Uri(photo));

            reader.Close();
            sql.Close();
         }
        public void updateActive(object sender, RoutedEventArgs e)
        {
            if (isActivate == true) isActivate = false;
            else isActivate = true;

            if (isActivate) checkBox.Content = "Активно";
            else checkBox.Content = "Неактивно";
        }
        public void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void DeleteProduct(object sender, RoutedEventArgs e)
        {
            String connectionString = Properties.Settings.Default.mainConnection;
            SqlConnection sql = new SqlConnection(connectionString);
            sql.Open();
            String query = "DELETE FROM Product WHERE ID = " + id;
            SqlCommand command = new SqlCommand(query);
            command.Connection = sql;
            command.ExecuteNonQuery();
            this.Close();
        }
        public void saveProduct(object sender, RoutedEventArgs e)
        {
            string temp = price_product.Text;
            int pos = temp.IndexOf(",");
            temp = temp.Substring(0, pos);
            int cost = int.Parse(temp);

            String query = "UPDATE Product SET ";
            query += "Title = '" + name_product.Text + "', ";
            query += "Cost = '" + cost + "', ";
            query += "IsActive = '" + isActivate + "' ";
            query += " WHERE ID = " + id;

            String connectionString = Properties.Settings.Default.mainConnection; SqlConnection sql = new SqlConnection(connectionString);
            sql.Open();
            SqlCommand command = new SqlCommand(query);
            command.Connection = sql;
            command.ExecuteNonQuery();
            this.Close();
        }
        public void createProduct(object sender, RoutedEventArgs e)
        {
            NewProduct product = new NewProduct();
            this.Hide();
            product.ShowDialog();
            this.Show();
        }
    }
}
