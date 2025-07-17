using System;
using Microsoft.Win32;

namespace CSharp_Cable_Routing.Helpers
{
    public class Loader
    {
        /// <summary>
        /// Метод необходим для выбора файла кабельнотрубного журнала
        /// </summary>
        public string Load_Excel()
        {
            OpenFileDialog fileschoosed = new OpenFileDialog
            {
                Filter = "Excel files (*.xls, *.xlsx, *.xlsm)|*.xls*",
                Multiselect = false
            };
            if (fileschoosed.ShowDialog() == true)
            {
                return fileschoosed.FileName;
            }
            else
            {
                throw new InvalidOperationException("Загрузка КЖ отменена");
            }
        }
    }
}
