#if NCAD
using Teigha.Colors;
#else
using Autodesk.AutoCAD.Colors;
#endif

namespace CSharp_Cable_Routing.Models
{
    /// <summary>
    /// Класс, описывающий единицу сечения кабельной эстакады
    /// </summary>
    public class PolylineSection
    {
        /// <summary>
        /// Конструктор для создания класса
        /// </summary>
        public PolylineSection() { }
        /// <summary>
        /// Конструктор для создания класса
        /// </summary>
        public PolylineSection(int SectionNumber, double X1, double Y1, double X2, double Y2, double Length, double Angle, Color Color, string Type)
        {
            this.SectionNumber = SectionNumber;
            this.X1 = X1;
            this.Y1 = Y1;
            this.X2 = X2;
            this.Y2 = Y2;
            this.Length = Length;
            this.Angle = Angle;
            this.Color = Color;
            this.Type = Type;
        }

        /// <summary>
        /// Номер отдельного уникального участка эстакады
        /// </summary>
        public int SectionNumber { get; set; } = 0;
        /// <summary>
        /// Координата X первой точки уникального участка эстакады
        /// </summary>
        public double X1 { get; set; }
        /// <summary>
        /// Координата Y первой точки уникального участка эстакады
        /// </summary>
        public double Y1 { get; set; }
        /// <summary>
        /// Координата X второй точки уникального участка эстакады
        /// </summary>
        public double X2 { get; set; }
        /// <summary>
        /// Координата Y второй точки уникального участка эстакады
        /// </summary>
        public double Y2 { get; set; }
        /// <summary>
        /// Длина уникального участка эстакады
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// Угол уникального участка эстакады в градусах
        /// </summary>
        public double Angle { get; set; }
        /// <summary>
        /// Цвет уникального участка эстакады
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// Тип уникального участка эстакады
        /// </summary>
        public string Type { get; set; } = "ЁЛКА";
    }
}
