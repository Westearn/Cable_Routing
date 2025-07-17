using System;
using System.Collections.Generic;
using CSharp_Cable_Routing.Models;
using CSharp_Cable_Routing.Helpers;
using ClosedXML.Excel;

namespace CSharp_Cable_Routing.Excel
{
    public class Import
    {
        /// <summary>
        /// Метод для выписывания кабелей
        /// </summary>
        public List<Cabel> Load_Info_Excel(string filePath)
        {
            using (XLWorkbook workbook = new XLWorkbook(filePath))
            {
                List<Cabel> cabelList = new List<Cabel>();
                var worksheet = workbook.Worksheet("Общая база");
                var row = worksheet.LastRowUsed().RowNumber();

                string startC;
                string endC;

                if (worksheet.Cell(4, "AC").Value.ToString() == "Начало")
                {
                    startC = "AC";
                    endC = "AD";
                }
                else if (worksheet.Cell(4, "AA").Value.ToString() == "Начало")
                {
                    startC = "AA";
                    endC = "AB";
                }
                else
                {
                    throw new Exception("Загружен некорректный КЖ, не указаны Начало и Конец кабельных линий");
                }

                for (int i = 1; i <= row; i++)
                {
                    if (worksheet.Cell(i + 4, startC).Value.ToString() != "" && worksheet.Cell(i + 4, endC).Value.ToString() != "")
                    {
                        var size = worksheet.Cell(i + 4, "J").Value.ToString();
                        var cabel = new Cabel()
                        {
                            Object = worksheet.Cell(i + 4, "B").Value.ToString(),
                            Marking = worksheet.Cell(i + 4, "H").Value.ToString(),
                            Brand = worksheet.Cell(i + 4, "I").Value.ToString(),
                            Size = size,
                            Start = Convert.ToInt32(worksheet.Cell(i + 4, startC).Value),
                            End = Convert.ToInt32(worksheet.Cell(i + 4, endC).Value),
                            Diameter = Constants.section_dict[size],
                        };
                        cabelList.Add(cabel);
                    }
                }

                return cabelList;
            }
        }
    }
}
