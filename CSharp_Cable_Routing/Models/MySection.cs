using System.Collections.Generic;

namespace CSharp_Cable_Routing.Models
{
    /// <summary>
    /// Класс для описания сечения с полками и стойками
    /// </summary>
    public class MySection
    {
        /// <summary>
        /// Конструктор для создания класса
        /// </summary>
        public MySection() { }
        /// <summary>
        /// Конструктор для создания класса
        /// </summary>
        public MySection(int SectionNumber, List<List<Cabel>> LeftLowV, List<List<Cabel>> RightLowV, List<List<Cabel>> LeftHighV, List<List<Cabel>> RightHighV)
        {
            this.SectionNumber = SectionNumber;
            this.LeftLowV = LeftLowV;
            this.RightLowV = RightLowV;
            this.LeftHighV = LeftHighV;
            this.RightHighV = RightHighV;
        }
        public int SectionNumber { get; set; }
        public List<List<Cabel>> LeftLowV { get; set; }
        public List<List<Cabel>> RightLowV { get; set; }
        public List<List<Cabel>> LeftHighV { get; set; }
        public List<List<Cabel>> RightHighV { get; set; }
    }
    
}
