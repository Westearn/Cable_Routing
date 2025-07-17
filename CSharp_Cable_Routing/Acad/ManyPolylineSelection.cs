using System;
using System.Collections.Generic;
using CSharp_Cable_Routing.Models;

#if NCAD
using HostMgd.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Colors;
using Teigha.Geometry;
#else
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Geometry;
#endif


namespace CSharp_Cable_Routing.Acad
{
    /// <summary>
    /// Класс для выбора нескольких полилиний на основе интерфейса выбора
    /// </summary>
    public class ManyPolylineSelection : ISelection
    {
        /// <summary>
        /// Итог (список сечений), который является результатом работы класса
        /// </summary>
        public List<PolylineSection> polylineSections = new List<PolylineSection>();

        /// <summary>
        /// Метод для выбора полилиний
        /// </summary>
        public void DoSelection(Editor editor, Database database, string str)
        {
            //Выбор элементов с учетом фильтра по полилинии
            TypedValue[] filList = new TypedValue[]
                {new TypedValue((int)DxfCode.Start, "LWPOLYLINE")};
            SelectionFilter selectionFilter = new SelectionFilter(filList);
            PromptSelectionOptions promptSelectionOptions = new PromptSelectionOptions();
            promptSelectionOptions.MessageForAdding = str;
            PromptSelectionResult promptSelectionResult = editor.GetSelection(promptSelectionOptions, selectionFilter);

            if (promptSelectionResult.Status == PromptStatus.OK)
            {
                polylineSections = GetParam(database, promptSelectionResult);
                double tolerance = 0.1;

                for (int i = 0; i < polylineSections.Count; i++) // Проверка координат концов полилиниий на погрешность при построении
                {
                    for (int j = 0; j < polylineSections.Count; j++)
                    {
                        if (i != j)
                        {
                            if (Math.Abs(polylineSections[i].X1 - polylineSections[j].X1) < tolerance && Math.Abs(polylineSections[i].Y1 - polylineSections[j].Y1) < tolerance)
                            {
                                polylineSections[i].X1 = polylineSections[j].X1;
                                polylineSections[i].Y1 = polylineSections[j].Y1;
                            }
                            else if (Math.Abs(polylineSections[i].X1 - polylineSections[j].X2) < tolerance && Math.Abs(polylineSections[i].Y1 - polylineSections[j].Y2) < tolerance)
                            {
                                polylineSections[i].X1 = polylineSections[j].X2;
                                polylineSections[i].Y1 = polylineSections[j].Y2;
                            }
                            else if (Math.Abs(polylineSections[i].X2 - polylineSections[j].X1) < tolerance && Math.Abs(polylineSections[i].Y2 - polylineSections[j].Y1) < tolerance)
                            {
                                polylineSections[i].X2 = polylineSections[j].X1;
                                polylineSections[i].Y2 = polylineSections[j].Y1;
                            }
                            else if (Math.Abs(polylineSections[i].X2 - polylineSections[j].X2) < tolerance && Math.Abs(polylineSections[i].Y2 - polylineSections[j].Y2) < tolerance)
                            {
                                polylineSections[i].X2 = polylineSections[j].X2;
                                polylineSections[i].Y2 = polylineSections[j].Y2;
                            }
                        }
                    }
                }
            }
            else
            {
                editor.WriteMessage("\nВыбор полилиний отменен или ничего не выбрано");
                throw new InvalidOperationException("Выбор полилиний отменен или ничего не выбрано");
            }
        }

        /// <summary>
        /// Метод для получения параметров полилиний:
        /// 1) Координаты
        /// 2) Длина
        /// 3) Угол
        /// 4) Цвет
        /// </summary>
        List<PolylineSection> GetParam(Database database, PromptSelectionResult promptSelectionResult)
        {
            List<PolylineSection> polylineSections = new List<PolylineSection>();

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (SelectedObject selectedObject in promptSelectionResult.Value)
                    {
                        Polyline polyline = transaction.GetObject(selectedObject.ObjectId, OpenMode.ForRead) as Polyline;
                        Color Color = polyline.Color;
                        for (int i = 0; i < polyline.NumberOfVertices - 1; i++)
                        {
                            PolylineSection polylineSection = new PolylineSection();
                            Point2d pt1 = polyline.GetPoint2dAt(i);
                            polylineSection.X1 = pt1.X;
                            polylineSection.Y1 = pt1.Y;
                            Point2d pt2 = polyline.GetPoint2dAt(i + 1);
                            polylineSection.X2 = pt2.X;
                            polylineSection.Y2 = pt2.Y;
                            double dist = pt1.GetDistanceTo(pt2);
                            polylineSection.Length = dist;
                            polylineSection.Color = Color;
                            if (pt2.X != pt1.X)
                            {
                                polylineSection.Angle = Math.Atan((pt2.Y - pt1.Y) / (pt2.X - pt1.X)) * 180 / Math.PI;
                            }
                            else
                            {
                                polylineSection.Angle = 90;
                            }
                            polylineSections.Add(polylineSection);
                        }
                    }
                    
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при определении параметров полилиний: " + ex.Message);
                }
            }

            return polylineSections;
        }
    }
}
