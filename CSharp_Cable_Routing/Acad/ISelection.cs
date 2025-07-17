#if NCAD
using HostMgd.EditorInput;
using Teigha.DatabaseServices;
#else
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
#endif


namespace CSharp_Cable_Routing.Acad
{
    /// <summary>
    /// Интерфейс для определения классов, которые выбирают элементы и получают параметры данных элементов
    /// </summary>
    public interface ISelection
    {
        /// <summary>
        /// Метод для выбора элементов
        /// </summary>
        void DoSelection(Editor editor, Database database, string str);
    }
}

