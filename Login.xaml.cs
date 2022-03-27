using System.Windows;

namespace Автомир
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string pass = textPass.Text;
            if(pass.CompareTo("admin") == 0)
            {
                this.Close();
                return;
            }
            MessageBox.Show("Введён неверный пароль");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(4);
        }
    }
}
