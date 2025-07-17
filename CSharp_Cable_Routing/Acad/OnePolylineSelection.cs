using System;

#if NCAD
using HostMgd.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Colors;
#else
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Colors;
#endif


namespace CSharp_Cable_Routing.Acad
{
    /// <summary>
    /// Класс для выбора одной полилини и определения ее цвета на основе интерфейса выбора
    /// </summary>
    public class OnePolylineSelection : ISelection
    {
        /// <summary>
        /// Итог (цвет), который является результатом работы класса
        /// </summary>
        public Color color = new Color();

        /// <summary>
        /// Метод для выбора полилинии
        /// </summary>
        public void DoSelection(Editor editor, Database database, string str)
        {
            //Выбор элементов с учетом фильтра полилинии
            PromptEntityOptions promptEntityOptions = new PromptEntityOptions(str);
            promptEntityOptions.SetRejectMessage("\nОшибка: объект не является полилинией");
            promptEntityOptions.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult promptEntityResult = editor.GetEntity(promptEntityOptions);

            if (promptEntityResult.Status == PromptStatus.OK)
            {
                color = GetParam(editor, database, promptEntityResult);
            }
            else
            {
                editor.WriteMessage("\nВыбор полилинии отменен или ничего не выбрано");
                color = null;
                throw new InvalidOperationException("Выбор полилинии отменен или ничего не выбрано");
            }
        }

        /// <summary>
        /// Метод для получения цвета полилинии
        /// </summary>
        Color GetParam(Editor editor, Database database, PromptEntityResult promptEntityResult)
        {
            Color color = new Color();
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                try
                {
                    Polyline polyline = transaction.GetObject(promptEntityResult.ObjectId, OpenMode.ForRead) as Polyline;
                    color = polyline.Color;

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Abort();
                    throw new Exception("Ошибка при определении цвета полилинии: " + ex.Message);
                }
            }

            return color;
        }
    }
}
