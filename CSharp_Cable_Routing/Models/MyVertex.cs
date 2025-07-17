using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Cable_Routing.Models
{
    /// <summary>
    /// Класс, описывающий единичный узел
    /// </summary>
    public class MyVertex
    {
        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public MyVertex(int ID, double X, double Y)
        {
            this.ID = ID;
            this.X = X;
            this.Y = Y;
        }
    }
}
