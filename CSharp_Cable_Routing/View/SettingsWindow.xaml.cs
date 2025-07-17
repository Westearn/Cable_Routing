using System;
using System.Windows;
using Microsoft.Win32;

namespace CSharp_Cable_Routing.View
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        /// <summary>
        /// Инициализация компонентов и запись в комбобоксы значений из сохраненных настроек
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        /// <summary>
        /// Динамиечское изменение shir_combobox при изменении sech_combobox
        /// </summary>
        private void sech_combobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if ((string)sech_combobox.SelectedItem == "ДКС")
            {
                shir_combobox.Items.Clear();
                shir_combobox.Items.Add(600);
                shir_combobox.SelectedValue = 600;
            }
            else if ((string)sech_combobox.SelectedItem == "СТМ")
            {
                shir_combobox.Items.Clear();
                shir_combobox.Items.Add(400);
                shir_combobox.SelectedValue = 400;
            }    
        }

        /// <summary>
        /// Сохранение данных и закрытие диалогового окна
        /// </summary>
        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.type_sech = sech_combobox.SelectedItem.ToString();
            Properties.Settings.Default.width_tray = (int)shir_combobox.SelectedItem;
            Properties.Settings.Default.k_reserve = (int)k_combobox.SelectedItem;
            Properties.Settings.Default.c_reserve = c_combobox.SelectedItem.ToString();
            Properties.Settings.Default.oatp_layer = (int)oatp_combobox.SelectedItem;
            Properties.Settings.Default.seo_layer = (int)seo_combobox.SelectedItem;
            Properties.Settings.Default.Save();
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Закрытие диалогового окна без сохранения данных
        /// </summary>
        private void close_button_click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Метод для загрузки настроек
        /// </summary>
        private void LoadSettings()
        {
            sech_combobox.SelectedItem = Properties.Settings.Default.type_sech;
            shir_combobox.SelectedItem = Properties.Settings.Default.width_tray;
            k_combobox.SelectedItem = Properties.Settings.Default.k_reserve;
            c_combobox.SelectedItem = Properties.Settings.Default.c_reserve;
            oatp_combobox.SelectedItem = Properties.Settings.Default.oatp_layer;
            seo_combobox.SelectedItem = Properties.Settings.Default.seo_layer;
        }
    }
}
