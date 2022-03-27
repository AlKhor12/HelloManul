using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
using System.Windows.Shapes;

namespace Автомир
{
    public partial class NewProduct : Window
    {
        String photo;
        Boolean isActivate = true;
        int manid = -1;
        int[] manids = new int[100];
        public NewProduct()
        {
            InitializeComponent();
            Loaded += NewProduct_Loaded;

        }

        private void NewProduct_Loaded(object sender, RoutedEventArgs e)
        {
            string query = "SELECT * FROM Manufacturer";

            String connectionString = Properties.Settings.Default.mainConnection;
            SqlConnection sql = new SqlConnection(connectionString);
            sql.Open();
            SqlCommand command = new SqlCommand(query);
            command.Connection = sql;
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                int index = select_man.Items.Add(reader["Name"].ToString());
                manids[index] = Convert.ToInt32(reader["ID"].ToString());
            }
        }

        public void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void SelectPhoto(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "C:\\";
            openFileDialog1.Filter = "image files (*.jpg)|*.jpg|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == true)
            {
                photo = GenerateName();
                MessageBox.Show(photo);

                File.Copy(openFileDialog1.FileName, AppDomain.CurrentDomain.BaseDirectory + "/images/" + photo + ".jpg");
                mainimage.Source = new BitmapImage(new Uri(openFileDialog1.FileName));
            }
        }
        private string GenerateName()
        {
            string iPass = "";
            string[] arr = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "B", "C", "D", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "V", "W", "X", "Z", "b", "c", "d", "f", "g", "h", "j", "k", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "z", "A", "E", "U", "Y" };
            Random rnd = new Random();
            for (int i = 0; i < 8; i = i + 1)
            {
                iPass = iPass + arr[rnd.Next(0, arr.Length)];
            }
            return iPass;
        }
        public void saveProduct(object sender, RoutedEventArgs e)
        {
            if (manid == -1) return;

            string title = name_product.Text;
            string cost = price_product.Text;
            photo = "Товары автосервиса\\" + photo + ".jpg";


            string query = "INSERT INTO Product (Title, Cost, MainImagePath, IsActive, ManufacturerID) VALUES ('" + title + "', '" + cost + "', '" + photo + "', '" + IsActive + "', '" + manid + "')";

            String connectionString = Properties.Settings.Default.mainConnection;
            SqlConnection sql = new SqlConnection(connectionString);
            sql.Open();
            SqlCommand command = new SqlCommand(query);
            command.Connection = sql;
            command.ExecuteNonQuery();
            this.Close();
        }
       public void updateActive(object sender, RoutedEventArgs e)
        {
            if (isActivate == true) isActivate = false;
            else isActivate = true;

            if (isActivate) checkBox.Content = "Активно";
            else checkBox.Content = "Неактивно";
        }

        private void select_man_Selected(object sender, RoutedEventArgs e)
        {
            int selected = select_man.SelectedIndex;
            if (selected == -1) return;
            manid = manids[selected];
        }
    }
}
