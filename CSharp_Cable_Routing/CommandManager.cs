using System;
using System.Collections.Generic;
using System.Linq;
using CSharp_Cable_Routing.Acad;
using CSharp_Cable_Routing.Helpers;
using CSharp_Cable_Routing.View;
using CSharp_Cable_Routing.Models;
using CSharp_Cable_Routing.Maths;
using CSharp_Cable_Routing.Excel;
using QuikGraph;
using Exception = System.Exception;

#if NCAD
using Teigha.Runtime;
using HostMgd.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Colors;
using Application = HostMgd.ApplicationServices.Application;
#else
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Colors;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
#endif


namespace CSharp_Cable_Routing
{
    /// <summary>
    /// Команды для работы в AutoCAD
    /// </summary>
    public class CommandManager
    {
        List<PolylineSection> polylineSections;
        List<Cabel> cabelList;
        private readonly ForGraph forGraph = new ForGraph();
        UndirectedGraph<MyVertex, MyEdge> graph = null;

        /// <summary>
        /// Создание команды "ПАРАМЕТРЫ_СЕЧЕНИЙ"
        /// </summary>
        [CommandMethod("CR_ПАРАМЕТРЫ_СЕЧЕНИЙ")]
        public void Settings_sech()
        {
            try
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                SettingsWindow settingsWindow = new SettingsWindow();
                if (settingsWindow.ShowDialog() == true)
                {
                    editor.WriteMessage("Параметры сохранены");
                }
                else
                {
                    throw new InvalidOperationException("Команда параметров отменена");
                }
            }
            catch (InvalidOperationException e)
            {
                Application.ShowAlertDialog($"{e.Message}");
            }
        }

        /// <summary>
        /// Создание команды "ЗАГРУЗКА_ТРАССЫ"
        /// Команда необходима для загрузки трассы из полилиний и определения уникальных участков
        /// </summary>
        [CommandMethod("CR_ЗАГРУЗКА_ТРАССЫ")]
        public void Load_sections()
        {
            try
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                Database database = Application.DocumentManager.MdiActiveDocument.Database;

                ManyPolylineSelection manyPolylineSelection = new ManyPolylineSelection();
                manyPolylineSelection.DoSelection(editor, database, "Выберите полилинии, определяющие эстакаду");
                polylineSections = manyPolylineSelection.polylineSections;

                OnePolylineSelection onePolylineSelection = new OnePolylineSelection();
                onePolylineSelection.DoSelection(editor, database, "Выберите полилинию, которая определяет эстакаду в виде траверсы");
                if (onePolylineSelection.color != null)
                {
                    Color Color = onePolylineSelection.color; // Цвет определенный по выбранной траверсе
                    foreach (PolylineSection polylineSection in polylineSections) // Присвоение типа трассы по её цвету в модели
                    {
                        if (polylineSection.Color.Equals(Color))
                        {
                            polylineSection.Type = "ТРАВЕРСА";
                        }
                    }
                }

                Maths.Maths maths = new Maths.Maths();
                polylineSections = maths.DoCheck(polylineSections); // Проверка точек на пренадлежность существующим участкам полилиний

                graph = forGraph.CreateGraph(polylineSections);
                MyVertex startNode = graph.Vertices.First();
                
                foreach (MyVertex vertex in graph.Vertices)
                {
                    if (vertex.X <= startNode.X)
                    {
                        startNode = vertex;
                    }
                }

                PromptKeywordOptions promptKeywordOptions = new PromptKeywordOptions("\nКаким образом пронумировать сечения?")
                {
                    AllowNone = false,
                };
                promptKeywordOptions.Keywords.Add("Ш", "В ширину");
                promptKeywordOptions.Keywords.Add("Г", "В глубину");
                promptKeywordOptions.Keywords.Default = "Ш";

                PromptResult promptResult = editor.GetKeywords(promptKeywordOptions);
                if (promptResult.Status == PromptStatus.OK)
                {
                    string selectedOption = promptResult.StringResult;
                    switch (selectedOption)
                    {
                        case "Ш":
                            forGraph.NumberEdgeBFS(graph, startNode);
                            polylineSections = forGraph.Coherency.Select(vt => vt.Item2).ToList();
                            break;
                        case "Г":
                            forGraph.NumberEdgeBFS(graph, startNode);
                            polylineSections = forGraph.Coherency.Select(vt => vt.Item2).ToList();
                            break;
                    }
                }
                else
                {
                    throw new Exception("Дальнейшая обработка отменена");
                }

                SectionWindow sectionWindow = new SectionWindow(); // Создание окна для добавления разрезов эстакады
                if (sectionWindow.ShowDialog() == true)
                {
                    CrossSection crossSection = new CrossSection();
                    crossSection.DoCrossSection(polylineSections, editor, database); // Метод для добавления разрезов эстакады
                }
                else
                {
                    editor.WriteMessage("Команда добавления сечений отменена");
                }
            }
            catch (InvalidOperationException e)
            {
                Application.ShowAlertDialog($"{e.Message}");
            }
        }

        /// <summary>
        /// Создание команды "Загрузка КЖ"
        /// Команда необходима для загрузки кабелей из файла КЖ и определения картчайшего пути кабелей
        /// </summary>
        [CommandMethod("CR_ЗАГРУЗКА_КЖ")]
        public void Prepare()
        {
            try
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                Loader loader = new Loader();
                string ListExcel = loader.Load_Excel();
                Properties.Settings.Default.kzh_path = ListExcel;
                Properties.Settings.Default.Save();
                editor.WriteMessage("Кабельнотрубный журнал загружен");

                Import import = new Import();
                cabelList = import.Load_Info_Excel(Properties.Settings.Default.kzh_path);
                
                if (graph == null)
                {
                    throw new Exception("Не был сформирован граф, используйте команду CR_ЗАГРУЗКА_ТРАССЫ");
                }

                if (cabelList != null)
                {
                    foreach (Cabel cabel in cabelList)
                    {
                        MyEdge edgeStart = null;
                        MyEdge edgeEnd = null;

                        foreach (var edge in graph.Edges)
                        {
                            if (cabel.Start == edge.SectionNumber)
                            {
                                edgeStart = edge;
                            }
                            if (cabel.End == edge.SectionNumber)
                            {
                                edgeEnd = edge;
                            }
                        }
                        if (edgeStart != null && edgeEnd != null)
                        {
                            cabel.mySections = forGraph.GetPath(graph, edgeStart, edgeEnd);
                        }
                        else
                        {
                            throw new Exception("Сечения из КЖ не соответствуют сечениям на плане");
                        }
                    }
                }
                else
                {
                    throw new Exception("Не найдено кабелей");
                }
            }
            catch (InvalidOperationException e)
            {
                Application.ShowAlertDialog($"{e.Message}");
            }
        }

        /// <summary>
        /// Создание команды "ГЕНЕРАЦИЯ_СЕЧЕНИЙ"
        /// Путь до файла записывается в переменную
        /// </summary>
        [CommandMethod("CR_ГЕНЕРАЦИЯ_СЕЧЕНИЙ")]
        public void Sech_gen()
        {
            try
            {
                GenerateSection generateSection = new GenerateSection();
                List<MySection> sections = new List<MySection>();
                foreach (PolylineSection polylineSection in polylineSections)
                {
                    sections.Add(generateSection.mySection(polylineSection, cabelList));
                }

                generateSection.TrayCheck(sections, polylineSections, graph);

                generateSection.aSections(sections, polylineSections);
            }
            catch (InvalidOperationException e)
            {
                Application.ShowAlertDialog($"{e.Message}");
            }
            catch (Exception ex)
            {
                Application.ShowAlertDialog($"{ex.Message}");
            }
        }
    }
}
