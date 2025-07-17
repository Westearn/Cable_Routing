using System;
using System.Collections.Generic;
using CSharp_Cable_Routing.Models;

#if NCAD
using Teigha.DatabaseServices;
using Application = HostMgd.ApplicationServices.Application;
#else
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
#endif


namespace CSharp_Cable_Routing.Acad
{
    public class STMSection : ASection
    {
        /// <summary>
        /// Метод для добавления стоек СТМ по ЛЕВОЙ стороне
        /// </summary>
        override public void standgenx(string shir, int n_unisech)
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

                    objectId = blockTable["Stand" + shir];
                    using (BlockReference blockReference = new BlockReference(new Point3d(n_unisech * 500, 22.5, 0), objectId))
                    {
                        blockTableRecord.AppendEntity(blockReference);
                        transaction.AddNewlyCreatedDBObject(blockReference, true);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении стоек СТМ по левой стороне: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для добавления стоек СТМ по ПРАВОЙ стороне
        /// </summary>
        override public void standgeny(string shir, int n_unisech)
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

                    objectId = blockTable["Stand" + shir];
                    using (BlockReference blockReference = new BlockReference(new Point3d(8.6 + n_unisech * 500, 22.5, 0), objectId))
                    {
                        blockTableRecord.AppendEntity(blockReference);
                        transaction.AddNewlyCreatedDBObject(blockReference, true);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении стоек СТМ по правой стороне: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для добавления полок СТМ по ЛЕВОЙ стороне
        /// </summary>
        override public void rackgenx(int n_unisech, int y_n, double width_lot)
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

                    objectId = blockTable["Rack" + width_lot.ToString() + "L"];
                    using (BlockReference blockReference = new BlockReference(new Point3d(n_unisech * 500, 1.5 - y_n * 20, 0), objectId))
                    {
                        blockTableRecord.AppendEntity(blockReference);
                        transaction.AddNewlyCreatedDBObject(blockReference, true);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении полок СТМ по левой стороне: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для добавления полок СТМ по ПРАВОЙ стороне
        /// </summary>
        override public void rackgeny(int n_unisech, int y_n, double width_lot)
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

                    objectId = blockTable["Rack" + width_lot.ToString() + "R"];
                    using (BlockReference blockReference = new BlockReference(new Point3d(11.2 + n_unisech * 500, 1.5 - y_n * 20, 0), objectId))
                    {
                        blockTableRecord.AppendEntity(blockReference);
                        transaction.AddNewlyCreatedDBObject(blockReference, true);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении полок СТМ по правой стороне: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для добавления лотков СТМ по ЛЕВОЙ стороне
        /// </summary>
        override public void traygenx(int n_unisech, int y_n, double width_lot)
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

                    objectId = blockTable["Tray" + width_lot.ToString()];
                    using (BlockReference blockReference = new BlockReference(new Point3d(-(width_lot / 20) - 1.5 + n_unisech * 500, 1.5 - y_n * 20, 0), objectId))
                    {
                        blockTableRecord.AppendEntity(blockReference);
                        transaction.AddNewlyCreatedDBObject(blockReference, true);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении лотков СТМ по левой стороне: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для добавления лотков СТМ по ПРАВОЙ стороне
        /// </summary>
        override public void traygeny(int n_unisech, int y_n, double width_lot)
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

                    objectId = blockTable["Tray" + width_lot.ToString()];
                    using (BlockReference blockReference = new BlockReference(new Point3d(11.2 + (width_lot / 20) + 1.5 + n_unisech * 500, 1.5 - y_n * 20, 0), objectId))
                    {
                        blockTableRecord.AppendEntity(blockReference);
                        transaction.AddNewlyCreatedDBObject(blockReference, true);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении лотков СТМ по правой стороне: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для добавления текстовых выносок СТМ по ЛЕВОЙ стороне
        /// </summary>
        override public void textgenx(List<Cabel> text_lot, int n_unisech, int x_n, int y_n)
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
                        if (y_n <= 5 || x_n == 1)
                        {
                            text_lot.Reverse();
                            for (int i = 0; i < text_lot.Count; i++)
                            {
                                using (Polyline polyline = new Polyline())
                                {
                                    polyline.AddVertexAt(0, new Point2d(-45 - x_n * 20 + n_unisech * 500, 3.5 + i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(1, new Point2d(-30 - x_n * 20 + n_unisech * 500, 3.5 + i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(2, new Point2d(-30 - x_n * 20 + n_unisech * 500, 7 + i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(3, new Point2d(-45 - x_n * 20 + n_unisech * 500, 7 + i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(4, new Point2d(-45 - x_n * 20 + n_unisech * 500, 3.5 + i * 3.5 - y_n * 20), 0, 0, 0);

                                    blockTableRecord.AppendEntity(polyline);
                                    transaction.AddNewlyCreatedDBObject(polyline, true);
                                }
                                using (DBText add_text = new DBText())
                                {
                                    add_text.Position = new Point3d(-45 - x_n * 20 + n_unisech * 500, 4 + i * 3.5 - y_n * 20, 0);
                                    add_text.Height = 2.5;
                                    add_text.TextString = text_lot[i].Marking;
                                    add_text.WidthFactor = 0.7;

                                    blockTableRecord.AppendEntity(add_text);
                                    transaction.AddNewlyCreatedDBObject(add_text, true);
                                }
                            }
                            using (Polyline polyline = new Polyline())
                            {
                                polyline.AddVertexAt(0, new Point2d(-30 - x_n * 20 + n_unisech * 500, 3.5 - y_n * 20), 0, 0, 0);
                                polyline.AddVertexAt(1, new Point2d(-41.5 + n_unisech * 500, 3.5 - y_n * 20), 0, 0, 0);

                                blockTableRecord.AppendEntity(polyline);
                                transaction.AddNewlyCreatedDBObject(polyline, true);
                            }
                        }
                        else if (y_n >= 6)
                        {
                            for (int i = 0; i < text_lot.Count; i++)
                            {
                                using (Polyline polyline = new Polyline())
                                {
                                    polyline.AddVertexAt(0, new Point2d(-45 - (11 - x_n) * 20 + n_unisech * 500, -i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(1, new Point2d(-30 - (11 - x_n) * 20 + n_unisech * 500, -i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(2, new Point2d(-30 - (11 - x_n) * 20 + n_unisech * 500, 3.5 - i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(3, new Point2d(-45 - (11 - x_n) * 20 + n_unisech * 500, 3.5 - i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(4, new Point2d(-45 - (11 - x_n) * 20 + n_unisech * 500, -i * 3.5 - y_n * 20), 0, 0, 0);

                                    blockTableRecord.AppendEntity(polyline);
                                    transaction.AddNewlyCreatedDBObject(polyline, true);
                                }
                                using (DBText add_text = new DBText())
                                {
                                    add_text.Position = new Point3d(-45 - (11 - x_n) * 20 + n_unisech * 500, 0.5 - i * 3.5 - y_n * 20, 0);
                                    add_text.Height = 2.5;
                                    add_text.TextString = text_lot[i].Marking;
                                    add_text.WidthFactor = 0.7;

                                    blockTableRecord.AppendEntity(add_text);
                                    transaction.AddNewlyCreatedDBObject(add_text, true);
                                }
                            }
                            using (Polyline polyline = new Polyline())
                            {
                                polyline.AddVertexAt(0, new Point2d(-30 - (11 - x_n) * 20 + n_unisech * 500, 3.5 - y_n * 20), 0, 0, 0);
                                polyline.AddVertexAt(1, new Point2d(-41.5 + n_unisech * 500, 3.5 - y_n * 20), 0, 0, 0);

                                blockTableRecord.AppendEntity(polyline);
                                transaction.AddNewlyCreatedDBObject(polyline, true);
                            }
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении текстовых выносок СТМ по левой стороне: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для добавления текстовых выносок СТМ по ПРАВОЙ стороне
        /// </summary>
        override public void textgeny(List<Cabel> text_lot, int n_unisech, int x_n, int y_n)
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
                        if (y_n <= 5 || x_n == 1)
                        {
                            text_lot.Reverse();
                            for (int i = 0; i < text_lot.Count; i++)
                            {
                                using (Polyline polyline = new Polyline())
                                {
                                    polyline.AddVertexAt(0, new Point2d(38.2 + x_n * 20 + n_unisech * 500, 3.5 + i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(1, new Point2d(53.2 + x_n * 20 + n_unisech * 500, 3.5 + i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(2, new Point2d(53.2 + x_n * 20 + n_unisech * 500, 7 + i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(3, new Point2d(38.2 + x_n * 20 + n_unisech * 500, 7 + i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(4, new Point2d(38.2 + x_n * 20 + n_unisech * 500, 3.5 + i * 3.5 - y_n * 20), 0, 0, 0);

                                    blockTableRecord.AppendEntity(polyline);
                                    transaction.AddNewlyCreatedDBObject(polyline, true);
                                }
                                using (DBText add_text = new DBText())
                                {
                                    add_text.Position = new Point3d(38.2 + x_n * 20 + n_unisech * 500, 4 + i * 3.5 - y_n * 20, 0);
                                    add_text.Height = 2.5;
                                    add_text.TextString = text_lot[i].Marking;
                                    add_text.WidthFactor = 0.7;

                                    blockTableRecord.AppendEntity(add_text);
                                    transaction.AddNewlyCreatedDBObject(add_text, true);
                                }
                            }
                            using (Polyline polyline = new Polyline())
                            {
                                polyline.AddVertexAt(0, new Point2d(38.2 + x_n * 20 + n_unisech * 500, 3.5 - y_n * 20), 0, 0, 0);
                                polyline.AddVertexAt(1, new Point2d(52.7 + n_unisech * 500, 3.5 - y_n * 20), 0, 0, 0);

                                blockTableRecord.AppendEntity(polyline);
                                transaction.AddNewlyCreatedDBObject(polyline, true);
                            }
                        }
                        else if (y_n >= 6)
                        {
                            for (int i = 0; i < text_lot.Count; i++)
                            {
                                using (Polyline polyline = new Polyline())
                                {
                                    polyline.AddVertexAt(0, new Point2d(38.2 + (11 - x_n) * 20 + n_unisech * 500, -i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(1, new Point2d(53.2 + (11 - x_n) * 20 + n_unisech * 500, -i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(2, new Point2d(53.2 + (11 - x_n) * 20 + n_unisech * 500, 3.5 - i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(3, new Point2d(38.2 + (11 - x_n) * 20 + n_unisech * 500, 3.5 - i * 3.5 - y_n * 20), 0, 0, 0);
                                    polyline.AddVertexAt(4, new Point2d(38.2 + (11 - x_n) * 20 + n_unisech * 500, -i * 3.5 - y_n * 20), 0, 0, 0);

                                    blockTableRecord.AppendEntity(polyline);
                                    transaction.AddNewlyCreatedDBObject(polyline, true);
                                }
                                using (DBText add_text = new DBText())
                                {
                                    add_text.Position = new Point3d(38.2 + (11 - x_n) * 20 + n_unisech * 500, 0.5 - i * 3.5 - y_n * 20, 0);
                                    add_text.Height = 2.5;
                                    add_text.TextString = text_lot[i].Marking;
                                    add_text.WidthFactor = 0.7;

                                    blockTableRecord.AppendEntity(add_text);
                                    transaction.AddNewlyCreatedDBObject(add_text, true);
                                }
                            }
                            using (Polyline polyline = new Polyline())
                            {
                                polyline.AddVertexAt(0, new Point2d(38.2 + (11 - x_n) * 20 + n_unisech * 500, 3.5 - y_n * 20), 0, 0, 0);
                                polyline.AddVertexAt(1, new Point2d(52.7 + n_unisech * 500, 3.5 - y_n * 20), 0, 0, 0);

                                blockTableRecord.AppendEntity(polyline);
                                transaction.AddNewlyCreatedDBObject(polyline, true);
                            }
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении текстовых выносок СТМ по правой стороне: " + ex.Message);
                }
            }
        }
    }
}
