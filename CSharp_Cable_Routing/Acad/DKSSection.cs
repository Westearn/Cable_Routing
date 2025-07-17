using System;
using System.Collections.Generic;
using CSharp_Cable_Routing.Models;

#if NCAD
using Teigha.DatabaseServices;
using Teigha.Geometry;
using Application = HostMgd.ApplicationServices.Application;
#else
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
#endif


namespace CSharp_Cable_Routing.Acad
{
    public class DKSSection : ASection
    {
        /// <summary>
        /// Метод для добавления стоек ДКС по ЛЕВОЙ стороне
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

                    objectId = blockTable["DKSStand" + shir + "L"];
                    using (BlockReference blockReference = new BlockReference(new Point3d(n_unisech * 500, 0, 0), objectId))
                    {
                        blockTableRecord.AppendEntity(blockReference);
                        transaction.AddNewlyCreatedDBObject(blockReference, true);
                    }

                    objectId = blockTable["DKSBase"];
                    using (BlockReference blockReference = new BlockReference(new Point3d(n_unisech * 500, 0, 0), objectId))
                    {
                        blockTableRecord.AppendEntity(blockReference);
                        transaction.AddNewlyCreatedDBObject(blockReference, true);
                    }

                    objectId = blockTable["DKSBaseEnd"];
                    using (BlockReference blockReference = new BlockReference(new Point3d(n_unisech * 500, -Convert.ToDouble(shir) / 10, 0), objectId))
                    {
                        blockTableRecord.AppendEntity(blockReference);
                        transaction.AddNewlyCreatedDBObject(blockReference, true);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении стоек ДКС по левой стороне: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для добавления стоек ДКС по ПРАВОЙ стороне
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

                    objectId = blockTable["DKSStand" + shir + "R"];
                    using (BlockReference blockReference = new BlockReference(new Point3d(18.29 + n_unisech * 500, 0, 0), objectId))
                    {
                        blockTableRecord.AppendEntity(blockReference);
                        transaction.AddNewlyCreatedDBObject(blockReference, true);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении стоек ДКС по правой стороне: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для добавления полок ДКС по ЛЕВОЙ стороне
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

                    objectId = blockTable["DKSRack" + width_lot.ToString() + "L"];
                    using (BlockReference blockReference = new BlockReference(new Point3d(n_unisech * 500, -43.9 - y_n * 26, 0), objectId))
                    {
                        blockTableRecord.AppendEntity(blockReference);
                        transaction.AddNewlyCreatedDBObject(blockReference, true);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении полок ДКС по левой стороне: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для добавления полок ДКС по ПРАВОЙ стороне
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

                    objectId = blockTable["DKSRack" + width_lot.ToString() + "R"];
                    using (BlockReference blockReference = new BlockReference(new Point3d(15.39 + n_unisech * 500, -43.9 - y_n * 26, 0), objectId))
                    {
                        blockTableRecord.AppendEntity(blockReference);
                        transaction.AddNewlyCreatedDBObject(blockReference, true);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении полок ДКС по правой стороне: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для добавления лотков ДКС по ЛЕВОЙ стороне
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

                    objectId = blockTable["DKSTray" + width_lot.ToString()];
                    using (BlockReference blockReference = new BlockReference(new Point3d(-3.45 - (width_lot / 10) + n_unisech * 500, -43.9 - y_n * 26, 0), objectId))
                    {
                        blockTableRecord.AppendEntity(blockReference);
                        transaction.AddNewlyCreatedDBObject(blockReference, true);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении лотков ДКС по левой стороне: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для добавления лотков ДКС по ПРАВОЙ стороне
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

                    objectId = blockTable["DKSTray" + width_lot.ToString()];
                    using (BlockReference blockReference = new BlockReference(new Point3d(-11.16 + (width_lot / 20) + n_unisech * 500, -43.9 - y_n * 26, 0), objectId))
                    {
                        blockTableRecord.AppendEntity(blockReference);
                        transaction.AddNewlyCreatedDBObject(blockReference, true);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при добавлении лотков ДКС по правой стороне: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для добавления текстовых выносок ДКС по ЛЕВОЙ стороне
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
                                    polyline.AddVertexAt(0, new Point2d(-63 - x_n * 20 + n_unisech * 500, -39 + i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(1, new Point2d(-48 - x_n * 20 + n_unisech * 500, -39 + i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(2, new Point2d(-48 - x_n * 20 + n_unisech * 500, -35.5 + i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(3, new Point2d(-63 - x_n * 20 + n_unisech * 500, -35.5 + i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(4, new Point2d(-63 - x_n * 20 + n_unisech * 500, -39 + i * 3.5 - y_n * 26), 0, 0, 0);

                                    blockTableRecord.AppendEntity(polyline);
                                    transaction.AddNewlyCreatedDBObject(polyline, true);
                                }
                                using (DBText add_text = new DBText())
                                {
                                    add_text.Position = new Point3d(-63 - x_n * 20 + n_unisech * 500, -38.5 + i * 3.5 - y_n * 26, 0);
                                    add_text.Height = 2.5;
                                    add_text.TextString = text_lot[i].Marking;
                                    add_text.WidthFactor = 0.7;

                                    blockTableRecord.AppendEntity(add_text);
                                    transaction.AddNewlyCreatedDBObject(add_text, true);
                                }
                            }
                            using (Polyline polyline = new Polyline())
                            {
                                polyline.AddVertexAt(0, new Point2d(-48 - x_n * 20 + n_unisech * 500, -39 - y_n * 26), 0, 0, 0);
                                polyline.AddVertexAt(1, new Point2d(-43 + n_unisech * 500, -39 - y_n * 26), 0, 0, 0);

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
                                    polyline.AddVertexAt(0, new Point2d(-63 - (11 - x_n) * 20 + n_unisech * 500, -42.5 - i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(1, new Point2d(-48 - (11 - x_n) * 20 + n_unisech * 500, -42.5 - i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(2, new Point2d(-48 - (11 - x_n) * 20 + n_unisech * 500, -39 - i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(3, new Point2d(-63 - (11 - x_n) * 20 + n_unisech * 500, -39 - i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(4, new Point2d(-63 - (11 - x_n) * 20 + n_unisech * 500, -42.5 - i * 3.5 - y_n * 26), 0, 0, 0);

                                    blockTableRecord.AppendEntity(polyline);
                                    transaction.AddNewlyCreatedDBObject(polyline, true);
                                }
                                using (DBText add_text = new DBText())
                                {
                                    add_text.Position = new Point3d(-63 - (11 - x_n) * 20 + n_unisech * 500, -42 - i * 3.5 - y_n * 26, 0);
                                    add_text.Height = 2.5;
                                    add_text.TextString = text_lot[i].Marking;
                                    add_text.WidthFactor = 0.7;

                                    blockTableRecord.AppendEntity(add_text);
                                    transaction.AddNewlyCreatedDBObject(add_text, true);
                                }
                            }
                            using (Polyline polyline = new Polyline())
                            {
                                polyline.AddVertexAt(0, new Point2d(-48 - (11 - x_n) * 20 + n_unisech * 500, -39 - y_n * 26), 0, 0, 0);
                                polyline.AddVertexAt(1, new Point2d(-43 + n_unisech * 500, -39 - y_n * 26), 0, 0, 0);

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
                    throw new Exception("Ошибка при добавлении текстовых выносок ДКС по левой стороне: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для добавления текстовых выносок ДКС по ПРАВОЙ стороне
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
                                    polyline.AddVertexAt(0, new Point2d(63.2 + x_n * 20 + n_unisech * 500, -39 + i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(1, new Point2d(78.2 + x_n * 20 + n_unisech * 500, -39 + i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(2, new Point2d(78.2 + x_n * 20 + n_unisech * 500, -35.5 + i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(3, new Point2d(63.2 + x_n * 20 + n_unisech * 500, -35.5 + i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(4, new Point2d(63.2 + x_n * 20 + n_unisech * 500, -39 + i * 3.5 - y_n * 26), 0, 0, 0);

                                    blockTableRecord.AppendEntity(polyline);
                                    transaction.AddNewlyCreatedDBObject(polyline, true);
                                }
                                using (DBText add_text = new DBText())
                                {
                                    add_text.Position = new Point3d(63.2 + x_n * 20 + n_unisech * 500, -38.5 + i * 3.5 - y_n * 26, 0);
                                    add_text.Height = 2.5;
                                    add_text.TextString = text_lot[i].Marking;
                                    add_text.WidthFactor = 0.7;

                                    blockTableRecord.AppendEntity(add_text);
                                    transaction.AddNewlyCreatedDBObject(add_text, true);
                                }
                            }
                            using (Polyline polyline = new Polyline())
                            {
                                polyline.AddVertexAt(0, new Point2d(63.2 + x_n * 20 + n_unisech * 500, -39 - y_n * 26), 0, 0, 0);
                                polyline.AddVertexAt(1, new Point2d(58.2 + n_unisech * 500, -39 - y_n * 26), 0, 0, 0);

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
                                    polyline.AddVertexAt(0, new Point2d(63.2 + (11 - x_n) * 20 + n_unisech * 500, -42.5 - i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(1, new Point2d(78.2 + (11 - x_n) * 20 + n_unisech * 500, -42.5 - i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(2, new Point2d(78.2 + (11 - x_n) * 20 + n_unisech * 500, -39 - i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(3, new Point2d(63.2 + (11 - x_n) * 20 + n_unisech * 500, -39 - i * 3.5 - y_n * 26), 0, 0, 0);
                                    polyline.AddVertexAt(4, new Point2d(63.2 + (11 - x_n) * 20 + n_unisech * 500, -42.5 - i * 3.5 - y_n * 26), 0, 0, 0);

                                    blockTableRecord.AppendEntity(polyline);
                                    transaction.AddNewlyCreatedDBObject(polyline, true);
                                }
                                using (DBText add_text = new DBText())
                                {
                                    add_text.Position = new Point3d(63.2 + (11 - x_n) * 20 + n_unisech * 500, -42 - i * 3.5 - y_n * 26, 0);
                                    add_text.Height = 2.5;
                                    add_text.TextString = text_lot[i].Marking;
                                    add_text.WidthFactor = 0.7;

                                    blockTableRecord.AppendEntity(add_text);
                                    transaction.AddNewlyCreatedDBObject(add_text, true);
                                }
                            }
                            using (Polyline polyline = new Polyline())
                            {
                                polyline.AddVertexAt(0, new Point2d(63.2 + (11 - x_n) * 20 + n_unisech * 500, -39 - y_n * 26), 0, 0, 0);
                                polyline.AddVertexAt(1, new Point2d(58.2 + n_unisech * 500, -39 - y_n * 26), 0, 0, 0);

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
                    throw new Exception("Ошибка при добавлении текстовых выносок ДКС по правой стороне: " + ex.Message);
                }
            }
        }
    }
}
