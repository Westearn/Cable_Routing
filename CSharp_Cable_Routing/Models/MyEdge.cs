using QuikGraph;

namespace CSharp_Cable_Routing.Models
{
    /// <summary>
    /// Класс, описывающий единичное ребро
    /// </summary>
    public class MyEdge : Edge<MyVertex>
    {
        public double Length { get; set; }
        public int SectionNumber { get; set; }
        
        public MyEdge(MyVertex source, MyVertex target, double Length, int SectionNumber = 0) : base(source, target)
        {
            this.Length = Length;
            this.SectionNumber = SectionNumber;
        }
    }
}
