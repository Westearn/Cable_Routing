using System;
using System.Collections.Generic;
using CSharp_Cable_Routing.Models;

#if NCAD
using HostMgd.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;
using Application = HostMgd.ApplicationServices.Application;
#else
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
#endif


namespace CSharp_Cable_Routing.Acad
{
    /// <summary>
    /// Абстрактный класс, который создан для описания графического сечения
    /// </summary>
    public abstract class ASection
    {
        public Database database = Application.DocumentManager.MdiActiveDocument.Database;
        public Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
        public abstract void standgenx(string shir, int n_unisech);
        public abstract void standgeny(string shir, int n_unisech);
        public abstract void rackgenx(int n_unisech, int y_n, double width_lot);
        public abstract void rackgeny(int n_unisech, int y_n, double width_lot);
        public abstract void traygenx(int n_unisech, int y_n, double width_lot);
        public abstract void traygeny(int n_unisech, int y_n, double width_lot);
        public abstract void textgenx(List<Cabel> text_lot, int n_unisech, int x_n, int y_n);
        public abstract void textgeny(List<Cabel> text_lot, int n_unisech, int x_n, int y_n);

        /// <summary>
        /// Метод для добавления произвольного текста по конкретным координатам
        /// </summary>
        public void add_text(string text, double x, double y)
        {
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTable blockTable;
                    blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord blockTableRecord;
                    blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    using (DBText add_text = new DBText())
                    {
                        Point3d basepoint = new Point3d(x, y, 0);
                        add_text.Position = basepoint;
                        add_text.Height = 2.5;
                        add_text.TextString = text;
                        add_text.HorizontalMode = TextHorizontalMode.TextMid;
                        add_text.AlignmentPoint = basepoint;

                        blockTableRecord.AppendEntity(add_text);
                        transaction.AddNewlyCreatedDBObject(add_text, true);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении текста: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для добавления сечения "Траверса"
        /// </summary>
        public void travgen(int n_unisech, int kol_lot, List<Cabel> cabels)
        {
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTable blockTable;
                    blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord blockTableRecord;
                    blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    ObjectId objectId = ObjectId.Null;

                    objectId = blockTable["Trav"];
                    using (BlockReference blockReference = new BlockReference(new Point3d(-24 + n_unisech * 500, 0, 0), objectId))
                    {
                        blockTableRecord.AppendEntity(blockReference);
                        transaction.AddNewlyCreatedDBObject(blockReference, true);
                    }

                    objectId = blockTable["lot_100"];
                    int i = 0;
                    while (i <= kol_lot)
                    {
                        using (BlockReference blockReference = new BlockReference(new Point3d(0.8 + n_unisech * 500 + 12 * (i / 2) * Math.Pow(-1, i), 0.7, 0), objectId))
                        {
                            blockTableRecord.AppendEntity(blockReference);
                            transaction.AddNewlyCreatedDBObject(blockReference, true);
                        }
                        i++;
                    }

                    using (Polyline polyline = new Polyline())
                    {
                        polyline.AddVertexAt(0, new Point2d(3 - (kol_lot / 2) * 12 + n_unisech * 500, 2), 0, 0, 0);
                        polyline.AddVertexAt(1, new Point2d(42 + n_unisech * 500, 2), 0, 0, 0);

                        blockTableRecord.AppendEntity(polyline);
                        transaction.AddNewlyCreatedDBObject(polyline, true);
                    }

                    if (cabels != null)
                    {
                        cabels.Reverse();
                        for (int j = 0; j < cabels.Count; j++)
                        {
                            using (Polyline polyline = new Polyline())
                            {
                                polyline.AddVertexAt(0, new Point2d(42 + n_unisech * 500, 2 - j * 3.5), 0, 0, 0);
                                polyline.AddVertexAt(1, new Point2d(57 + n_unisech * 500, 2 - j * 3.5), 0, 0, 0);
                                polyline.AddVertexAt(2, new Point2d(57 + n_unisech * 500, 5.5 - j * 3.5), 0, 0, 0);
                                polyline.AddVertexAt(3, new Point2d(42 + n_unisech * 500, 5.5 - j * 3.5), 0, 0, 0);
                                polyline.AddVertexAt(4, new Point2d(42 + n_unisech * 500, 2 - j * 3.5), 0, 0, 0);

                                blockTableRecord.AppendEntity(polyline);
                                transaction.AddNewlyCreatedDBObject(polyline, true);
                            }
                            using (DBText add_text = new DBText())
                            {
                                add_text.Position = new Point3d(42 + n_unisech * 500, 2.5 - j * 3.5, 0);
                                add_text.Height = 2.5;
                                add_text.TextString = cabels[j].Marking;

                                blockTableRecord.AppendEntity(add_text);
                                transaction.AddNewlyCreatedDBObject(add_text, true);
                            }
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при формировании сечения в виде траверсы: " + ex.Message);
                }
            }
        }

        public void CabelCircles(List<Cabel> text_lot, double start, int n_unisech, int y_n)
        {
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTable blockTable;
                    blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord blockTableRecord;
                    blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    if (text_lot != null)
                    {
                        double x_coord = 0;
                        double c_reserve = 0;
                        if (Properties.Settings.Default.c_reserve == "0.3 диаметра")
                        {
                            c_reserve = 0.3;
                        }
                        else if (Properties.Settings.Default.c_reserve == "1 диаметр")
                        {
                            c_reserve = 1.0;
                        }
                        for (int i = 0; i < text_lot.Count; i++)
                        {
                            if (start > 0)
                            {
                                x_coord += text_lot[i].Diameter * 0.1 / 2;
                            }
                            else if (start < 0)
                            {
                                x_coord -= text_lot[i].Diameter * 0.1 / 2;
                            }

                            Circle circle = new Circle
                            {
                                Center = new Point3d(5.6 + start + x_coord + n_unisech * 500, text_lot[i].Diameter * 0.1 / 2 + 2 - y_n * 20, 0),
                                Radius = text_lot[i].Diameter * 0.1 / 2,
                            };

                            if (start > 0)
                            {
                                x_coord += text_lot[i].Diameter * 0.1 / 2;
                                x_coord += text_lot[i].Diameter * 0.1 * c_reserve;
                            }
                            else if (start < 0)
                            {
                                x_coord -= text_lot[i].Diameter * 0.1 / 2;
                                x_coord -= text_lot[i].Diameter * 0.1 * c_reserve;
                            }

                            blockTableRecord.AppendEntity(circle);
                            transaction.AddNewlyCreatedDBObject(circle, true);

                            using (Hatch hatch = new Hatch())
                            {
                                hatch.PatternScale = 0.1;
                                hatch.SetHatchPattern(HatchPatternType.PreDefined, "ANSI31");
                                
                                hatch.AppendLoop(HatchLoopTypes.Default, new ObjectIdCollection { circle.ObjectId });
                                hatch.EvaluateHatch(true);

                                blockTableRecord.AppendEntity(hatch);
                                transaction.AddNewlyCreatedDBObject(hatch, true);
                            }
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении окружностей для обозначения кабелей: " + ex.Message);
                }
            }
        }
    }
}
