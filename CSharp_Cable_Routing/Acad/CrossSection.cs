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
    /// Класс для построения сечений полилиний
    /// </summary>
    public class CrossSection
    {
        /// <summary>
        /// Метод для построения сечений полилиний
        /// </summary>
        public void DoCrossSection(List<PolylineSection> polylineSections, Editor editor, Database database)
        {
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTable blockTable;
                    blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;

                    string blockName = "CrossSection";
                    if (!blockTable.Has(blockName)) // Проверяем наличие блока, если нет, то создаем по примитивам
                    {
                        using (BlockTableRecord btr = new BlockTableRecord())
                        {
                            btr.Name = blockName;

                            blockTable.UpgradeOpen();
                            blockTable.Add(btr);
                            transaction.AddNewlyCreatedDBObject(btr, true);

                            // Определяем три точки треугольника
                            Point3d point1 = new Point3d(-2.7, 1.2, 0);
                            Point3d point2 = new Point3d(-3.2, 1.48, 0);
                            Point3d point3 = new Point3d(-1, 1.2, 0);

                            // Определяем цвет
                            Color color = Color.FromColorIndex(ColorMethod.ByAci, 4); // Голубой цвет

                            // Создаем оригинальный треугольник
                            Solid triangle = new Solid(point1, point2, point3, point3);
                            triangle.Color = color;
                            btr.AppendEntity(triangle);
                            transaction.AddNewlyCreatedDBObject(triangle, true);

                            // Трансформация для копирования и отражения треугольника
                            for (int i = 1; i <= 3; i++)
                            {
                                Matrix3d transformMatrix = Matrix3d.Identity;

                                if (i == 1)
                                {
                                    transformMatrix = Matrix3d.Mirroring(new Line3d(point1, point3));
                                }
                                else if (i == 2)
                                {
                                    transformMatrix = transformMatrix.PreMultiplyBy(Matrix3d.Displacement(new Vector3d(0, -2.4, 0)));
                                }
                                else if (i == 3)
                                {
                                    transformMatrix = Matrix3d.Mirroring(new Line3d(point1, point3));
                                    transformMatrix = transformMatrix.PreMultiplyBy(Matrix3d.Displacement(new Vector3d(0, -2.4, 0)));
                                }

                                using (Solid clonedTriangle = (Solid)triangle.Clone())
                                {
                                    clonedTriangle.TransformBy(transformMatrix);
                                    btr.AppendEntity(clonedTriangle);
                                    transaction.AddNewlyCreatedDBObject(clonedTriangle, true);
                                }
                            }

                            // Создание отрезков
                            Line line1 = new Line(new Point3d(-4, 1.2, 0), new Point3d(-1, 1.2, 0));
                            line1.Color = color;
                            btr.AppendEntity(line1);
                            transaction.AddNewlyCreatedDBObject(line1, true);

                            Line line2 = new Line(new Point3d(-1, 1.9, 0), new Point3d(-1, 0.7, 0));
                            line2.Color = color;
                            btr.AppendEntity(line2);
                            transaction.AddNewlyCreatedDBObject(line2, true);

                            // Трансформация для отражения отрезком относительно оси Х
                            Matrix3d reflectionMatrix = Matrix3d.Mirroring(new Line3d(Point3d.Origin, new Point3d(1, 0, 0)));

                            // Отражение отрезков относительно оси Х
                            Line reflectedline1 = (Line)line1.Clone();
                            reflectedline1.TransformBy(reflectionMatrix);
                            btr.AppendEntity(reflectedline1);
                            transaction.AddNewlyCreatedDBObject(reflectedline1, true);

                            Line reflectedline2 = (Line)line2.Clone();
                            reflectedline2.TransformBy(reflectionMatrix);
                            btr.AppendEntity(reflectedline2);
                            transaction.AddNewlyCreatedDBObject(reflectedline2, true);
                        }
                    }

                    BlockTableRecord blockTableRecord;
                    blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    ObjectId objectId = ObjectId.Null;
                    objectId = blockTable[blockName];

                    foreach (PolylineSection polylinesection in polylineSections)
                    {
                        double X = (polylinesection.X1 + polylinesection.X2) / 2;
                        double Y = (polylinesection.Y1 + polylinesection.Y2) / 2;

                        using (BlockReference blockReference = new BlockReference(new Point3d(X, Y, 0), objectId))
                        {
                            blockReference.Rotation = polylinesection.Angle * Math.PI / 180;
                            blockTableRecord.AppendEntity(blockReference);
                            transaction.AddNewlyCreatedDBObject(blockReference, true);
                        }

                        // Вычисляем координаты для расположения текста
                        double X90 = X - Math.Abs(polylinesection.X2 - polylinesection.X1) * 4 / polylinesection.Length;
                        double Y90;

                        if (polylinesection.Angle >= 0)
                        {
                            Y90 = Y - Math.Abs(polylinesection.Y2 - polylinesection.Y1) * 4 / polylinesection.Length;
                        }
                        else
                        {
                            Y90 = Y + Math.Abs(polylinesection.Y2 - polylinesection.Y1) * 4 / polylinesection.Length;
                        }

                        double A, MX1, MY1, MX2, MY2;
                        Maths.Maths maths = new Maths.Maths();
                        if (Y != Y90)
                        {
                            A = -(X - X90) / (Y - Y90);
                            maths.CircleLineIntersection(2.5, A, out MX1, out MY1, out MX2, out MY2);
                        }
                        else
                        {
                            MX1 = 0;
                            MY1 = 2.5;
                            MX2 = 0;
                            MY2 = -2.5;
                        }
                        

                        // Добавляем текст по полученным координатам
                        MText mText1 = new MText()
                        {
                            Attachment = AttachmentPoint.MiddleCenter,
                            Location = new Point3d(MX1 + X90, MY1 + Y90, 0),
                            TextHeight = 1.25, // Высота текста
                            Color = Color.FromColorIndex(ColorMethod.ByAci, 4), // Голубой цвет
                            Contents = $"\\W0.8;{polylinesection.SectionNumber}",
                        };

                        blockTableRecord.AppendEntity(mText1);
                        transaction.AddNewlyCreatedDBObject(mText1, true);

                        MText mText2 = new MText()
                        {
                            Attachment = AttachmentPoint.MiddleCenter,
                            Location = new Point3d(MX2 + X90, MY2 + Y90, 0),
                            TextHeight = 1.25, // Высота текста
                            Color = Color.FromColorIndex(ColorMethod.ByAci, 4), // Голубой цвет
                            Contents = $"\\W0.8;{polylinesection.SectionNumber}",
                        };

                        blockTableRecord.AppendEntity(mText2);
                        transaction.AddNewlyCreatedDBObject(mText2, true);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении разрезов: " + ex.Message);
                }
            }
        }
    }
}
