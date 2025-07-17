using System;
using System.Collections.Generic;
using System.Linq;
using CSharp_Cable_Routing.Models;

namespace CSharp_Cable_Routing.Maths
{
    /// <summary>
    /// Класс для матетматических вычислений
    /// </summary>
    public class Maths
    {
        /// <summary>
        /// Метод для расчета расстояния от точки до отрезка
        /// </summary>
        public double DistanceToLineSegment(PolylineSection polylineSection, double X, double Y)
        {
            try
            {
                // Получаем координаты отрезка
                double X1 = polylineSection.X1;
                double Y1 = polylineSection.Y1;
                double X2 = polylineSection.X2;
                double Y2 = polylineSection.Y2;

                // Рассчитываем векторы от начала отрезка к точке и от начала до конца отрезка
                double A = X - X1;
                double B = Y - Y1;
                double C = X2 - X1;
                double D = Y2 - Y1;

                // Рассчитываем проекцию точки "param" на линию, содержащую отрезок
                // Для этого определяем скалярное произведение и квадрат длины отрезка
                double dot = A * C + B * D;
                double len_sq = C * C + D * D;

                // Обработка случая, когда отрезок представляет собой точку
                if (len_sq == 0)
                {
                    // Отрезок вырождает в точку (X1, Y1)
                    return Math.Sqrt(A * A + B * B);
                }

                // Параметр для определения ближайшей точки на отрезке
                double param = dot / len_sq;

                // Координаты ближайшей точки на отрезке
                double xx, yy;

                // Определение ближайшей точки
                if (param < 0)
                {
                    xx = X1;
                    yy = Y1;
                }
                else if (param > 1)
                {
                    xx = X2;
                    yy = Y2;
                }
                else
                {
                    xx = X1 + param * C;
                    yy = Y1 + param * D;
                }

                // Вычисление расстояния до ближайшей точки
                double dx = X - xx;
                double dy = Y - yy;
                return Math.Sqrt(dx * dx + dy * dy);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при определении расстояния от точки до отрезка: " + ex.Message);
            }
        }

        /// <summary>
        /// Метод для проверки принадлежности точки уравнению полилинии
        /// </summary>
        public bool IsPointOnLineSegment(PolylineSection polylineSection, double X, double Y)
        {
            try
            {
                double tolerance = 0.1;
                double distance = DistanceToLineSegment(polylineSection, X, Y);
                return distance < tolerance;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при проверке принадлежности точки уравнению полилинии: " + ex.Message);
            }
        }

        /// <summary>
        /// Метод для проверки всех точек на принадлежность прямым
        /// </summary>
        public List<PolylineSection> DoCheck(List<PolylineSection> polylineSections)
        {
            try 
            { 
                List<PolylineSection> result = new List<PolylineSection>();
                for (int i = 0; i < polylineSections.Count; i++)
                {
                    // Создание списка для координат с использованием List ValueTuple
                    List<(double X, double Y)> points = new List<(double X, double Y)>();

                    for (int j = 0; j < polylineSections.Count; j++)
                    {
                        if (i != j)
                        {
                            if (IsPointOnLineSegment(polylineSections[i], polylineSections[j].X1, polylineSections[j].Y1))
                            {
                                points.Add((polylineSections[j].X1, polylineSections[j].Y1));
                            }

                            if (IsPointOnLineSegment(polylineSections[i], polylineSections[j].X2, polylineSections[j].Y2))
                            {
                                points.Add((polylineSections[j].X2, polylineSections[j].Y2));
                            }
                        }
                    }

                    if (points.Count != 0)
                    {
                        points.Add((polylineSections[i].X1, polylineSections[i].Y1));
                        points.Add((polylineSections[i].X2, polylineSections[i].Y2));
                    }
                    else
                    {
                        result.Add(polylineSections[i]);
                        continue;
                    }

                    List<(double X, double Y)> pointsUnique = points.Distinct().ToList();

                    pointsUnique.Sort((a, b) =>
                    {
                        int comparison = a.X.CompareTo(b.X);
                        if (comparison == 0)
                        {
                            return a.Y.CompareTo(b.Y);
                        }
                        return comparison;
                    });

                    for (int k = 0; k < pointsUnique.Count - 1; k++)
                    {
                        PolylineSection polylineSection = new PolylineSection();
                        polylineSection.X1 = pointsUnique[k].X;
                        polylineSection.X2 = pointsUnique[k + 1].X;
                        polylineSection.Y1 = pointsUnique[k].Y;
                        polylineSection.Y2 = pointsUnique[k + 1].Y;
                        polylineSection.Length = Math.Sqrt(Math.Pow(pointsUnique[k + 1].Y - pointsUnique[k].Y, 2) + Math.Pow(pointsUnique[k + 1].X - pointsUnique[k].X, 2));
                        polylineSection.Angle = polylineSections[i].Angle;
                        polylineSection.Color = polylineSections[i].Color;
                        polylineSection.Type = polylineSections[i].Type;
                        result.Add(polylineSection);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при проверке всех точек на принадлежность прямым: " + ex.Message);
            }
        }

        /// <summary>
        /// Метод для нахождения двух точек пересечения окружности и линии
        /// Условия: окружность перенесена в начало координат, линия проходит через центр
        /// radius - радиус окружности
        /// A - коэффициент прямой
        /// </summary>
        public void CircleLineIntersection(double radius, double A, out double MX1, out double MY1, out double MX2, out double MY2)
        {
            MX1 = radius / Math.Sqrt(1 + Math.Pow(A, 2));
            MY1 = radius * A / Math.Sqrt(1 + Math.Pow(A, 2));
            MX2 = - radius / Math.Sqrt(1 + Math.Pow(A, 2));
            MY2 = - radius * A / Math.Sqrt(1 + Math.Pow(A, 2));
        }
    }
}
