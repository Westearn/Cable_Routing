using System.Collections.Generic;

namespace CSharp_Cable_Routing.Models
{
    /// <summary>
    /// Класс, описывающий единицу кабеля
    /// </summary>
    public class Cabel
    {
        /// <summary>
        /// Конструктор для создания класса
        /// </summary>
        public Cabel() { }
        /// <summary>
        /// Конструктор для создания класса
        /// </summary>
        public Cabel(string Object, string Marking, string Brand, string Size, int Start, int End, double Diameter, List<PolylineSection> mySections)
        {
            this.Object = Object;
            this.Marking = Marking;
            this.Brand = Brand;
            this.Size = Size;
            this.Start = Start;
            this.End = End;
            this.Diameter = Diameter;
            this.mySections = mySections;
        }

        /// <summary>
        /// Объект, к которому относится кабель
        /// </summary>
        public string Object { get; set; }
        /// <summary>
        /// Маркировка кабеля
        /// </summary>
        public string Marking { get; set; }
        /// <summary>
        /// Марка кабеля
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// Количество жил и сечение кабеля
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// Начальное сечение
        /// </summary>
        public int Start { get; set; }
        /// <summary>
        /// Конечное сечение
        /// </summary>
        public int End { get; set; }
        /// <summary>
        /// Диаметр кабеля
        /// </summary>
        public double Diameter { get; set; }
        /// <summary>
        /// Путь кабеля по эстакаде
        /// </summary>
        public List<PolylineSection> mySections { get; set; }
    }

    /// <summary>
    /// Класс для сравнения кабелей
    /// </summary>
    public class CabelComparer : IComparer<Cabel>
    {
        public int Compare(Cabel a, Cabel b)
        {
            int comparison = a.Diameter.CompareTo(b.Diameter);
            if (comparison == 0)
            {
                return a.Marking.CompareTo(b.Marking);
            }
            return comparison;
        }
    }
}
