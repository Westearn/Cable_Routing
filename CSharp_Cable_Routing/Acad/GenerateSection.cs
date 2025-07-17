using System;
using System.Collections.Generic;
using System.Linq;
using CSharp_Cable_Routing.Models;
using QuikGraph;

#if NCAD
using HostMgd.EditorInput;
using Application = HostMgd.ApplicationServices.Application;
#else
using Autodesk.AutoCAD.EditorInput;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
#endif


namespace CSharp_Cable_Routing.Acad
{
    /// <summary>
    /// Класс для построения сечений
    /// </summary>
    public class GenerateSection
    {
        /// <summary>
        /// Метод для распределения кабелей группам
        /// </summary>
        /// <param name="polylineSection"></param>
        /// <param name="cabelList"></param>
        /// <returns></returns>
        public MySection mySection(PolylineSection polylineSection, List<Cabel> cabelList)
        {
            try
            {
                MySection section = new MySection();
                section.SectionNumber = polylineSection.SectionNumber;
                section.LeftHighV = new List<List<Cabel>>();
                section.RightHighV = new List<List<Cabel>>();
                section.LeftLowV = new List<List<Cabel>>();
                section.RightLowV = new List<List<Cabel>>();


                List<Cabel> LLV = new List<Cabel>();
                List<Cabel> RLV = new List<Cabel>();
                List<Cabel> LHV = new List<Cabel>();
                List<Cabel> RHV = new List<Cabel>();

                foreach (Cabel cabel in cabelList)
                {
                    bool iff = false;
                    foreach (PolylineSection polyline in cabel.mySections)
                        if (polylineSection == polyline)
                        {
                            iff = true;
                        }
                    if (iff)
                    {
                        if (cabel.Object.Contains("II секция"))
                        {
                            if (cabel.Brand.Contains("КПБК-90") || cabel.Brand.Contains("КПБП-90"))
                            {
                                RHV.Add(cabel);
                            }
                            else
                            {
                                RLV.Add(cabel);
                            }
                        }
                        else if (cabel.Object.Contains("I секция"))
                        {
                            if (cabel.Brand.Contains("КПБК-90") || cabel.Brand.Contains("КПБП-90"))
                            {
                                LHV.Add(cabel);
                            }
                            else
                            {
                                LLV.Add(cabel);
                            }
                        }
                    }
                }

                var comparer = new CabelComparer();
                List<List<Cabel>> lists = new List<List<Cabel>>() { LLV, RLV, LHV, RHV };
                foreach (List<Cabel> list in lists)
                {
                    list.Sort(comparer);
                }

                CabelsInTrays(polylineSection, LLV, section.LeftLowV);
                CabelsInTrays(polylineSection, LHV, section.LeftHighV);
                CabelsInTrays(polylineSection, RLV, section.RightLowV);
                CabelsInTrays(polylineSection, RHV, section.RightHighV);

                return section;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при распределении кабелей по группам: " + ex.Message);
            }
        }

        public void CabelsInTrays(PolylineSection polylineSection, List<Cabel> listCabels, List<List<Cabel>> listListCabels)
        {
            try
            {
                int width_tray = Properties.Settings.Default.width_tray;
                double width_tray_fact = width_tray * (1 - Properties.Settings.Default.k_reserve / 100.0);
                double width_tray100_fact = 100 * (1 - Properties.Settings.Default.k_reserve / 100.0);
                double width_tray200_fact = 200 * (1 - Properties.Settings.Default.k_reserve / 100.0);
                double zap = 0;
                double c_reserve = 0;
                if (Properties.Settings.Default.c_reserve == "0.3 диаметра")
                {
                    c_reserve = 0.3;
                }
                else if (Properties.Settings.Default.c_reserve == "1 диаметр")
                {
                    c_reserve = 1;
                }
                List<Cabel> text_tray = new List<Cabel>();

                if (polylineSection.Type == "ЁЛКА")
                {
                    foreach (Cabel cabel in listCabels)
                    {
                        zap += cabel.Diameter * (1 + c_reserve);
                        if (zap > width_tray_fact)
                        {
                            listListCabels.Add(text_tray.ToList());
                            zap = 0;
                            text_tray.Clear();
                            zap += cabel.Diameter * (1 + c_reserve);
                        }
                        text_tray.Add(cabel);
                    }

                    listListCabels.Add(text_tray.ToList());
                    zap = 0;
                    text_tray.Clear();
                }
                else if (polylineSection.Type == "ТРАВЕРСА")
                {
                    foreach (Cabel cabel in listCabels)
                    {
                        zap += cabel.Diameter * (1 + c_reserve);
                        if (zap > width_tray100_fact)
                        {
                            listListCabels.Add(text_tray.ToList());
                            zap = 0;
                            text_tray.Clear();
                            zap += cabel.Diameter * (1 + c_reserve);
                        }
                        text_tray.Add(cabel);
                    }
                    listListCabels.Add(text_tray.ToList());
                    zap = 0;
                    text_tray.Clear();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при распределении кабелей по лоткам: " + ex.Message);
            }
        }

        /// <summary>
        /// Методя для устранения перепрыгивания кабелей с полки на полку
        /// </summary>
        /// <param name="sections"></param>
        /// <param name="polylineSections"></param>
        /// <param name="graph"></param>
        public void TrayCheck(List<MySection> sections, List<PolylineSection> polylineSections, UndirectedGraph<MyVertex, MyEdge> graph)
        {
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    List<(int sectionNumber, List<List<Cabel>> cabels)> list = new List<(int SectionNumber, List<List<Cabel>> cabels)>();
                    List<(int sectionNumber, int kol_lot, int kol_kab)> kol_list = new List<(int sectionNumber, int kol_lot, int kol_kab)>();

                    switch (i)
                    {
                        case 0:
                            foreach (MySection section in sections)
                            {
                                list.Add((section.SectionNumber, section.LeftLowV));
                                int kol = 0;
                                foreach (List<Cabel> l_cabels in section.LeftLowV)
                                {
                                    kol += l_cabels.Count;
                                }
                                kol_list.Add((section.SectionNumber, section.LeftLowV.Count, kol));
                            }
                            break;
                        case 1:
                            foreach (MySection section in sections)
                            {
                                list.Add((section.SectionNumber, section.RightLowV));
                                int kol = 0;
                                foreach (List<Cabel> l_cabels in section.RightLowV)
                                {
                                    kol += l_cabels.Count;
                                }
                                kol_list.Add((section.SectionNumber, section.RightLowV.Count, kol));
                            }
                            break;
                        case 2:
                            foreach (MySection section in sections)
                            {
                                list.Add((section.SectionNumber, section.LeftHighV));
                                int kol = 0;
                                foreach (List<Cabel> l_cabels in section.LeftHighV)
                                {
                                    kol += l_cabels.Count;
                                }
                                kol_list.Add((section.SectionNumber, section.LeftHighV.Count, kol));
                            }
                            break;
                        case 3:
                            foreach (MySection section in sections)
                            {
                                list.Add((section.SectionNumber, section.RightHighV));
                                int kol = 0;
                                foreach (List<Cabel> l_cabels in section.RightHighV)
                                {
                                    kol += l_cabels.Count;
                                }
                                kol_list.Add((section.SectionNumber, section.RightHighV.Count, kol));
                            }
                            break;
                    }

                    kol_list.Sort((a, b) =>
                    {
                        int comparison = b.kol_lot.CompareTo(a.kol_lot);
                        if (comparison == 0)
                        {
                            return b.kol_kab.CompareTo(a.kol_kab);
                        }
                        return comparison;
                    });


                    List<(int sectionNumber, int kol_lot, int kol_kab)> kol_list_dub = kol_list.ToList();

                    var visitedNodes = new HashSet<MyVertex>();
                    while (kol_list.Count > 1)
                    {

                        PolylineSection polyline = polylineSections.FirstOrDefault(a => a.SectionNumber == kol_list[0].sectionNumber);
                        MyEdge myEdge = graph.Edges.FirstOrDefault(a => a.SectionNumber == kol_list[0].sectionNumber);
                        List<MyVertex> vertices = new List<MyVertex>() { myEdge.Source, myEdge.Target };

                        if (polyline.Type != "ЁЛКА")
                        {
                            kol_list.RemoveAt(0);
                            continue;
                        }

                        foreach (MyVertex myVertex in vertices)
                        {
                            if (visitedNodes.Contains(myVertex))
                            {
                                continue;
                            }

                            visitedNodes.Add(myVertex);
                            var edges = graph.AdjacentEdges(myVertex).Where(e => !e.Equals(myEdge)).ToList();

                            if (edges.Count() == 3 || edges.Count() == 2)
                            {
                                var kol_kab = kol_list_dub.FirstOrDefault(a => a.sectionNumber == myEdge.SectionNumber).kol_kab;
                                var kol = 0;
                                foreach (var edge in edges)
                                {
                                    kol += kol_list_dub.FirstOrDefault(a => a.sectionNumber == edge.SectionNumber).kol_kab;
                                }

                                foreach (var edge in edges)
                                {
                                    if (polylineSections.FirstOrDefault(a => a.SectionNumber == edge.SectionNumber).Type == "ЁЛКА" &&
                                        kol_list_dub.FirstOrDefault(a => a.sectionNumber == edge.SectionNumber).kol_lot ==
                                        kol_list_dub.FirstOrDefault(a => a.sectionNumber == myEdge.SectionNumber).kol_lot)
                                    {
                                        var cabels = list.FirstOrDefault(a => a.sectionNumber == edge.SectionNumber).cabels.ToList();
                                        for (int j = 0; j < kol_list_dub.FirstOrDefault(a => a.sectionNumber == edge.SectionNumber).kol_lot; j++)
                                        {
                                            cabels[j] = list.FirstOrDefault(a => a.sectionNumber == myEdge.SectionNumber).cabels[j].ToList();
                                        }
                                        foreach (var edge_ in edges.Where(e => !e.Equals(edge)))
                                        {
                                            foreach (var cabel_list in cabels)
                                            {
                                                for (int n = cabel_list.Count - 1; n >= 0; n--)
                                                {
                                                    foreach (var c_list in list.FirstOrDefault(a => a.sectionNumber == edge_.SectionNumber).cabels)
                                                    {
                                                        if (c_list.Contains(cabel_list[n]))
                                                        {
                                                            cabel_list.RemoveAt(n);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        int indexToUpdate = list.FindIndex(a => a.sectionNumber == edge.SectionNumber);
                                        list[indexToUpdate] = (list[indexToUpdate].sectionNumber, cabels);
                                    }
                                }
                            }
                            else if (edges.Count() == 1)
                            {
                                var ex = false;
                                if (polylineSections.FirstOrDefault(a => a.SectionNumber == edges[0].SectionNumber).Type == "ЁЛКА" &&
                                    kol_list_dub.FirstOrDefault(a => a.sectionNumber == edges[0].SectionNumber).kol_lot ==
                                    kol_list_dub.FirstOrDefault(a => a.sectionNumber == myEdge.SectionNumber).kol_lot &&
                                    kol_list_dub.FirstOrDefault(a => a.sectionNumber == edges[0].SectionNumber).kol_kab !=
                                    kol_list_dub.FirstOrDefault(a => a.sectionNumber == myEdge.SectionNumber).kol_kab)
                                {
                                    foreach (var cabel_list in list.FirstOrDefault(a => a.sectionNumber == edges[0].SectionNumber).cabels)
                                    {
                                        for (int n = cabel_list.Count - 1; n >= 0; n--)
                                        {
                                            ex = false;
                                            foreach (var c_list in list.FirstOrDefault(a => a.sectionNumber == myEdge.SectionNumber).cabels)
                                            {
                                                if (c_list.Contains(cabel_list[n]))
                                                {
                                                    ex = true;
                                                }
                                            }
                                            if (!ex)
                                            {
                                                break;
                                            }
                                        }
                                        if (!ex)
                                        {
                                            break;
                                        }
                                    }

                                    if (ex)
                                    {
                                        var cabels = list.FirstOrDefault(a => a.sectionNumber == edges[0].SectionNumber).cabels.ToList();
                                        for (int j = 0; j < kol_list_dub.FirstOrDefault(a => a.sectionNumber == edges[0].SectionNumber).kol_lot; j++)
                                        {
                                            cabels[j] = list.FirstOrDefault(a => a.sectionNumber == myEdge.SectionNumber).cabels[j].ToList();
                                        }
                                        foreach (var cabel_list in cabels)
                                        {
                                            for (int n = cabel_list.Count - 1; n >= 0; n--)
                                            {
                                                var exx = false;
                                                foreach (var c_list in list.FirstOrDefault(a => a.sectionNumber == edges[0].SectionNumber).cabels)
                                                {
                                                    if (c_list.Contains(cabel_list[n]))
                                                    {
                                                        exx = true;
                                                    }
                                                }

                                                if (!exx)
                                                {
                                                    cabel_list.RemoveAt(n);
                                                }
                                            }
                                        }

                                        int indexToUpdate = list.FindIndex(a => a.sectionNumber == edges[0].SectionNumber);
                                        list[indexToUpdate] = (list[indexToUpdate].sectionNumber, cabels);
                                    }
                                }
                            }
                        }
                        kol_list.RemoveAt(0);
                    }

                    switch (i)
                    {
                        case 0:
                            foreach (MySection section in sections)
                            {
                                section.LeftLowV = list.FirstOrDefault(a => a.sectionNumber == section.SectionNumber).cabels.ToList();
                            }
                            break;
                        case 1:
                            foreach (MySection section in sections)
                            {
                                section.RightLowV = list.FirstOrDefault(a => a.sectionNumber == section.SectionNumber).cabels.ToList();

                                var razn = section.RightLowV.Count() - section.LeftLowV.Count();
                                if (razn > 0)
                                {
                                    for (int j = 0; j < razn; j++)
                                    {
                                        section.LeftLowV.Add(new List<Cabel>());
                                    }
                                }
                                else if (razn < 0)
                                {
                                    for (int j = 0; j < Math.Abs(razn); j++)
                                    {
                                        section.RightLowV.Add(new List<Cabel>());
                                    }
                                }
                            }
                            break;
                        case 2:
                            foreach (MySection section in sections)
                            {
                                section.LeftHighV = list.FirstOrDefault(a => a.sectionNumber == section.SectionNumber).cabels.ToList();
                            }
                            break;
                        case 3:
                            foreach (MySection section in sections)
                            {
                                section.RightHighV = list.FirstOrDefault(a => a.sectionNumber == section.SectionNumber).cabels.ToList();

                                var razn = section.RightHighV.Count() - section.LeftHighV.Count();
                                if (razn > 0)
                                {
                                    for (int j = 0; j < razn; j++)
                                    {
                                        section.LeftHighV.Add(new List<Cabel>());
                                    }
                                }
                                else if (razn < 0)
                                {
                                    for (int j = 0; j < Math.Abs(razn); j++)
                                    {
                                        section.RightHighV.Add(new List<Cabel>());
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при работе алгоритма, устраняющего перепрыгивание кабелей: " + ex.Message);
            }
        }

        public void aSections(List<MySection> sections, List<PolylineSection> polylineSections)
        {
            try 
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

                ASection aSection = null;
                double set = 0;
                Dictionary<int, string> dict = null;
                switch (Properties.Settings.Default.type_sech)
                {
                    case "СТМ":
                        aSection = new STMSection();
                        set = 27;
                        dict = new Dictionary<int, string>()
                            {
                                {1, "400"}, {2, "600"}, {3, "800"}, {4, "1200"}, {5, "1200"}, {6, "1800"}, {7, "1800"}, {8, "1800"}, {9, "2200"}, {10, "2200"},
                            };
                        break;
                    case "ДКС":
                        aSection = new DKSSection();
                        set = 42;
                        dict = new Dictionary<int, string>()
                            {
                                {1, "500"}, {2, "800"}, {3, "1100"}, {4, "1400"}, {5, "1600"}, {6, "1900"}, {7, "2100"}, {8, "2400"}, {9, "2600"}, {10, "3000"},
                            };
                        break;
                }

                double width_lot = Properties.Settings.Default.width_tray;

                foreach (MySection section in sections)
                {
                    int n_oatp = Convert.ToInt32(Math.Round((section.LeftLowV.Count + section.LeftHighV.Count) * Properties.Settings.Default.oatp_layer / 100.0));
                    int n_seo = Convert.ToInt16(Math.Round((section.LeftLowV.Count + section.LeftHighV.Count) * Properties.Settings.Default.seo_layer / 100.0));
                    int nx = 1;
                    int ny = n_oatp;

                    aSection.add_text(section.SectionNumber.ToString() + "-" + section.SectionNumber.ToString(), 5.6 + section.SectionNumber * 500, 37.5);

                    if (polylineSections.FirstOrDefault(a => a.SectionNumber == section.SectionNumber).Type == "ЁЛКА")
                    {
                        for (int i = 0; i < ny; i++)
                        {
                            aSection.rackgenx(section.SectionNumber, ny, width_lot);
                            aSection.add_text("ОАТП", -set + 5.6 + section.SectionNumber * 500, 4.5 - ny * 20);
                        }
                        ny++;

                        for (int i = 0; i < section.LeftLowV.Count; i++)
                        {
                            List<Cabel> text_lot = section.LeftLowV[i].ToList();
                            if (ny == 6)
                            {
                                nx = 6 + 5 - section.LeftHighV.Count + 5 - n_oatp - section.LeftLowV.Count;
                            }
                            if (text_lot.Count == 0 && section.RightLowV[0].Count != 0)
                            {
                                aSection.rackgenx(section.SectionNumber, ny, width_lot);
                                aSection.traygenx(section.SectionNumber, ny, width_lot);
                                aSection.add_text("Резерв", -set + 5.6 + section.SectionNumber * 500, 4.5 - ny * 20);
                                ny++;
                            }
                            else if (0 < text_lot.Count && text_lot.Count <= 5)
                            {
                                aSection.rackgenx(section.SectionNumber, ny, width_lot);
                                aSection.traygenx(section.SectionNumber, ny, width_lot);
                                aSection.textgenx(text_lot, section.SectionNumber, 1, ny);
                                aSection.CabelCircles(text_lot, -8.0, section.SectionNumber, ny);
                                nx++;
                                ny++;
                            }
                            else if (text_lot.Count > 5)
                            {
                                aSection.rackgenx(section.SectionNumber, ny, width_lot);
                                aSection.traygenx(section.SectionNumber, ny, width_lot);
                                aSection.textgenx(text_lot, section.SectionNumber, nx, ny);
                                aSection.CabelCircles(text_lot, -8.0, section.SectionNumber, ny);
                                nx++;
                                ny++;
                            }
                        }

                        
                        for (int i = 0; i < n_seo; i++)
                        {
                            if (ny == 6)
                            {
                                nx = 6 + 5 - section.LeftHighV.Count;
                            }
                            aSection.rackgenx(section.SectionNumber, ny, width_lot);
                            aSection.traygenx(section.SectionNumber, ny, width_lot);
                            aSection.add_text("Кабели СЭО", -set + 5.6 + section.SectionNumber * 500, 4.5 - ny * 20);
                            ny++;
                        }
                    
                        for (int i = 0; i < section.LeftHighV.Count; i++)
                        {
                            if (ny == 6)
                            {
                                nx = 6 + 5 - section.LeftHighV.Count;
                            }
                            List<Cabel> text_lot = section.LeftHighV[i].ToList();
                            if (text_lot.Count == 0 && section.RightHighV[0].Count != 0)
                            {
                                aSection.rackgenx(section.SectionNumber, ny, width_lot);
                                aSection.traygenx(section.SectionNumber, ny, width_lot);
                                aSection.add_text("Резерв", -set + 5.6 + section.SectionNumber * 500, 4.5 - ny * 20);
                                ny++;
                            }
                            else if (0 < text_lot.Count && text_lot.Count <= 5)
                            {
                                aSection.rackgenx(section.SectionNumber, ny, width_lot);
                                aSection.traygenx(section.SectionNumber, ny, width_lot);
                                aSection.textgenx(text_lot, section.SectionNumber, 1, ny);
                                aSection.add_text("Кабели 6 кВ", -set + 5.6 + section.SectionNumber * 500, 4 + 4.5 - ny * 20);
                                aSection.CabelCircles(text_lot, -8.0, section.SectionNumber, ny);
                                nx++;
                                ny++;
                            }
                            else if (text_lot.Count > 5)
                            {
                                aSection.rackgenx(section.SectionNumber, ny, width_lot);
                                aSection.traygenx(section.SectionNumber, ny, width_lot);
                                aSection.textgenx(text_lot, section.SectionNumber, nx, ny);
                                aSection.add_text("Кабели 6 кВ", -set + 5.6 + section.SectionNumber * 500, 4 + 4.5 - ny * 20);
                                aSection.CabelCircles(text_lot, -8.0, section.SectionNumber, ny);
                                nx++;
                                ny++;
                            }
                        }

                        nx = 1;
                        ny = n_oatp;

                        for (int i = 0; i < ny; i++)
                        {
                            aSection.rackgeny(section.SectionNumber, ny, width_lot);
                            aSection.add_text("ОАТП", set + 5.6 + section.SectionNumber * 500, 4.5 - ny * 20);
                        }
                        ny++;

                        for (int i = 0; i < section.RightLowV.Count; i++)
                        {
                            List<Cabel> text_lot = section.RightLowV[i].ToList();
                            if (ny == 6)
                            {
                                nx = 6 + 5 - section.RightHighV.Count + 5 - n_oatp - section.RightLowV.Count;
                            }
                            if (text_lot.Count == 0 && section.LeftLowV[0].Count != 0)
                            {
                                aSection.rackgeny(section.SectionNumber, ny, width_lot);
                                aSection.traygeny(section.SectionNumber, ny, width_lot);
                                aSection.add_text("Резерв", set + 5.6 + section.SectionNumber * 500, 4.5 - ny * 20);
                                ny++;
                            }
                            else if (0 < text_lot.Count && text_lot.Count <= 5)
                            {
                                aSection.rackgeny(section.SectionNumber, ny, width_lot);
                                aSection.traygeny(section.SectionNumber, ny, width_lot);
                                aSection.textgeny(text_lot, section.SectionNumber, 1, ny);
                                aSection.CabelCircles(text_lot, 8.0, section.SectionNumber, ny);
                                nx++;
                                ny++;
                            }
                            else if (text_lot.Count > 5)
                            {
                                aSection.rackgeny(section.SectionNumber, ny, width_lot);
                                aSection.traygeny(section.SectionNumber, ny, width_lot);
                                aSection.textgeny(text_lot, section.SectionNumber, nx, ny);
                                aSection.CabelCircles(text_lot, 8.0, section.SectionNumber, ny);
                                nx++;
                                ny++;
                            }
                        }

                        for (int i = 0; i < n_seo; i++)
                        {
                            if (ny == 6)
                            {
                                nx = 6 + 5 - section.RightHighV.Count;
                            }
                            aSection.rackgeny(section.SectionNumber, ny, width_lot);
                            aSection.traygeny(section.SectionNumber, ny, width_lot);
                            aSection.add_text("Кабели СЭО", set + 5.6 + section.SectionNumber * 500, 4.5 - ny * 20);
                            ny++;
                        }

                        for (int i = 0; i < section.RightHighV.Count; i++)
                        {
                            if (ny == 6)
                            {
                                nx = 6 + 5 - section.RightHighV.Count;
                            }
                            List<Cabel> text_lot = section.RightHighV[i].ToList();
                            if (text_lot.Count == 0 && section.LeftHighV[0].Count != 0)
                            {
                                aSection.rackgeny(section.SectionNumber, ny, width_lot);
                                aSection.traygeny(section.SectionNumber, ny, width_lot);
                                aSection.add_text("Резерв", set + 5.6 + section.SectionNumber * 500, 4.5 - ny * 20);
                                ny++;
                            }
                            else if (0 < text_lot.Count && text_lot.Count <= 5)
                            {
                                aSection.rackgeny(section.SectionNumber, ny, width_lot);
                                aSection.traygeny(section.SectionNumber, ny, width_lot);
                                aSection.textgeny(text_lot, section.SectionNumber, 1, ny);
                                aSection.add_text("Кабели 6 кВ", set + 5.6 + section.SectionNumber * 500, 4 + 4.5 - ny * 20);
                                aSection.CabelCircles(text_lot, 8.0, section.SectionNumber, ny);
                                nx++;
                                ny++;
                            }
                            else if (text_lot.Count > 5)
                            {
                                aSection.rackgeny(section.SectionNumber, ny, width_lot);
                                aSection.traygeny(section.SectionNumber, ny, width_lot);
                                aSection.textgeny(text_lot, section.SectionNumber, nx, ny);
                                aSection.add_text("Кабели 6 кВ", set + 5.6 + section.SectionNumber * 500, 4 + 4.5 - ny * 20);
                                aSection.CabelCircles(text_lot, 8.0, section.SectionNumber, ny);
                                nx++;
                                ny++;
                            }
                        }
                        aSection.standgenx(dict[ny], section.SectionNumber);
                        aSection.standgeny(dict[ny], section.SectionNumber);
                    }
                    else if (polylineSections.FirstOrDefault(a => a.SectionNumber == section.SectionNumber).Type == "ТРАВЕРСА")
                    {
                        int kol_lot = 0;
                        if (section.LeftLowV.Count > 0)
                        {
                            if (section.LeftLowV[0].Count > 0)
                            {
                                kol_lot += section.LeftLowV.Count;
                            }
                        }
                        if (section.RightLowV.Count > 0)
                        {
                            if (section.RightLowV[0].Count > 0)
                            {
                                kol_lot += section.RightLowV.Count;
                            }
                        }
                        if (section.LeftHighV.Count > 0)
                        {
                            if (section.LeftHighV[0].Count > 0)
                            {
                                kol_lot += section.LeftHighV.Count;
                            }
                        }
                        if (section.RightHighV.Count > 0)
                        {
                            if (section.RightHighV[0].Count > 0)
                            {
                                kol_lot += section.RightHighV.Count;
                            }
                        }
                        List<Cabel> cabels = new List<Cabel>();
                        foreach (var cabs in section.LeftHighV)
                        {
                            cabels.AddRange(cabs);
                        }
                        foreach (var cabs in section.RightLowV)
                        {
                            cabels.AddRange(cabs);
                        }
                        foreach (var cabs in section.LeftHighV)
                        {
                            cabels.AddRange(cabs);
                        }
                        foreach (var cabs in section.RightHighV)
                        {
                            cabels.AddRange(cabs);
                        }
                        aSection.travgen(section.SectionNumber, kol_lot, cabels);
                    }    
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при работе алгоритма по отрисовке сечений: " + ex.Message);
            }
        }
    }
}
