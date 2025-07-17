using System;
using System.Collections.Generic;
using System.Linq;
using QuikGraph;
using QuikGraph.Algorithms;
using CSharp_Cable_Routing.Models;

namespace CSharp_Cable_Routing.Maths
{
    /// <summary>
    /// Создан класс для описания работы с графами
    /// </summary>
    public class ForGraph
    {
        /// <summary>
        /// Словарь для связи ребер и соответствующих полилиний
        /// </summary>
        public List<(MyEdge, PolylineSection)> Coherency = new List<(MyEdge, PolylineSection)>();
        /// <summary>
        /// Метод для создания графа
        /// </summary>
        public UndirectedGraph<MyVertex, MyEdge> CreateGraph(List<PolylineSection> polylineSections)
        {
            Coherency.Clear();
            //Создаем ненаправленный граф
            var graph = new UndirectedGraph<MyVertex, MyEdge>();
            
            //Добавляем узлы
            List<(double X, double Y)> vertices = new List<(double X, double Y)>();
            foreach (PolylineSection polylineSection in polylineSections)
            {
                vertices.Add((polylineSection.X1, polylineSection.Y1));
                vertices.Add((polylineSection.X2, polylineSection.Y2));
            }
            
            List<(double X, double Y)> verticesUnique;
            verticesUnique = vertices.Distinct().ToList();
            
            for (int i = 0; i < verticesUnique.Count; i++)
            {
                graph.AddVertex(new MyVertex(i, verticesUnique[i].X, verticesUnique[i].Y));
            }
            
            // Добавляем ребра
            foreach (PolylineSection polylineSection in polylineSections)
            {
                MyVertex source = null;
                MyVertex target = null;

                foreach (MyVertex myVertex in graph.Vertices)
                {
                    if (myVertex.X == polylineSection.X1 && myVertex.Y == polylineSection.Y1)
                    {
                        source = myVertex;
                    }
                    else if (myVertex.X == polylineSection.X2 && myVertex.Y == polylineSection.Y2)
                    {
                        target = myVertex;
                    }
                }

                if (source != null && target != null)
                {
                    MyEdge myEdge = new MyEdge(source, target, polylineSection.Length);
                    Coherency.Add((myEdge, polylineSection));
                    graph.AddEdge(myEdge);
                }
                else
                {
                    throw new Exception("Не удалось сформировать граф, для рёбер не найдены соответствующие узлы");
                }    
            }

            return graph;
        }

        /// <summary>
        /// Метод для нумерации сечений в ширину BFS
        /// </summary>
        public void NumberEdgeBFS(UndirectedGraph<MyVertex, MyEdge> graph, MyVertex startNode)
        {
            //Множества для хранения посещённых узлов и рёбер
            var visitedNodes = new HashSet<MyVertex>();
            var visitedEdges = new HashSet<MyEdge>();

            //Очередь для обхода в ширину
            var queue = new Queue<MyVertex>();
            
            //Переменная для присвоения номера ребру
            int number = 1;

            // Добавляем начальный узел в очередь и помечаем его как посещенный
            queue.Enqueue(startNode);
            visitedNodes.Add(startNode);

            // Пока очередь не пуста
            while (queue.Count > 0)
            {
                // Извлекаем узел из очереди
                var node = queue.Dequeue();

                // Обрабатываем все рёбра, смежные с текущим узлом
                foreach (var edge in graph.AdjacentEdges(node))
                {
                    // Определяем целевой узел  (исходя из того, является ли текущий узел началом или концом ребра)
                    var targetNode = edge.Source.Equals(node) ? edge.Target : edge.Source;

                    // Если ребро ещё не было посещено
                    if (!visitedEdges.Contains(edge))
                    {
                        // Присваиваем ребру номер
                        for (int i = 0; i < Coherency.Count; i++)
                        {
                            if (Coherency[i].Item1 == edge)
                            {
                                edge.SectionNumber = number;
                                Coherency[i].Item1.SectionNumber = number;
                                Coherency[i].Item2.SectionNumber = number;
                                number++;
                                break;
                            }
                        }

                        // Помечаем ребро как посещённое
                        visitedEdges.Add(edge);
                    }

                    // Если целевой узел ещё не был посещён
                    if (!visitedNodes.Contains(targetNode))
                    {
                        // Добавляем целевой узел в очередь
                        queue.Enqueue(targetNode);

                        // Помечаем целевой узел как посещённый
                        visitedNodes.Add((targetNode));
                    }
                }
            }
        }

        /// <summary>
        /// Метод для поиска картчайшего пути
        /// </summary>
        public List<PolylineSection> GetPath(UndirectedGraph<MyVertex, MyEdge> graph, MyEdge startEdge, MyEdge endEdge)
        {
            // Определяем метод для поиска пути с учетом параметра длины ребер
            var tryGetPath = graph.ShortestPathsDijkstra(e => e.Length, startEdge.Source);
            List<MyEdge> pathEdge;
            List<PolylineSection> pathSection;

            MyVertex vertex = endEdge.Target;
            // Проверка на совпадение узлов
            if (startEdge.Source == endEdge.Target)
            {
                vertex = endEdge.Source;
            }

            // Производим поиск пути
            if (tryGetPath(vertex, out IEnumerable<MyEdge> path))
            {
                pathEdge = new List<MyEdge>(path);


                // Проверяем попали ли крайнии ребра в список
                if (!pathEdge.Contains(startEdge))
                {
                    pathEdge.Insert(0, startEdge);
                }
                if (!pathEdge.Contains(endEdge))
                {
                    pathEdge.Add(endEdge);
                }

                pathSection = new List<PolylineSection>();
                foreach (MyEdge myEdge in pathEdge)
                {
                    for (int i = 0; i < Coherency.Count; i++)
                    {
                        if (Coherency[i].Item1 == myEdge)
                        {
                            pathSection.Add(Coherency[i].Item2);
                            break;
                        }
                    }
                }

                return pathSection;
            }
            else
            {
                throw new Exception($"Не удалось найти кратчайший путь с использованием алгоритма Дейкстры из { startEdge.SectionNumber } ({ startEdge.Source.X }; { startEdge.Source.Y }) в { endEdge.SectionNumber } ({ endEdge.Target.X }; { endEdge.Target.Y })");
            }
        }
    }
}
