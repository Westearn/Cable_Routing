using System.Windows;


namespace CSharp_Cable_Routing.View
{
    /// <summary>
    /// Логика взаимодействия для SectionWindow.xaml
    /// </summary>
    public partial class SectionWindow : Window
    {
        public SectionWindow()
        {
            InitializeComponent();
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void close_button_click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
