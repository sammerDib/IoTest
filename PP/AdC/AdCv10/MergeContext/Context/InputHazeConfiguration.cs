using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Shared.Tools;

namespace MergeContext.Context
{
    [Obsolete("Only For Darkview Module", false)]
    public class InputHazeConfiguration : AdcTools.Serializable
    {
        #region internal class
        public class ColorMapElt
        {
            [XmlAttribute("R")]
            public byte R { get; set; }
            [XmlAttribute("G")]
            public byte G { get; set; }
            [XmlAttribute("B")]
            public byte B { get; set; }

            public ColorMapElt()
            {
                R = 0; G = 0; B = 0;
            }

            public ColorMapElt(byte r, byte g, byte b)
            {
                R = r; G = g; B = b;
            }

            public override string ToString()
            {
                return String.Format("{0} {1} {2}", R, G, B);
            }

        }
        public class CoreValues
        {
            [XmlArray("Values"), XmlArrayItem(typeof(byte), ElementName = "c")]
            public List<byte> values { get; set; }
            public CoreValues()
            {
                values = new List<byte>();
            }
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (var val in values)
                {
                    sb.Append(val); sb.Append(";");
                }
                return sb.ToString();
            }
        }
        public class HazeCore8b
        {
            [XmlAttribute("Id")]
            public int Index { get; set; }
            [XmlElement("Cores")]
            public CoreValues Cores { get; set; }

            public HazeCore8b()
            {
                Index = -1;
                Cores = new CoreValues();
            }

            public override string ToString()
            {
                return String.Format("{0};{1}", Index, Cores.ToString()); ;
            }
        }
        public class HazeCore256b : HazeCore8b
        {
            [XmlAttribute("Chan")]
            public byte Chan { get; set; } // channel R2 G1 B0

            public HazeCore256b() : base()
            {
                Chan = 255;
            }
            public override string ToString()
            {
                return String.Format("{0};{1}|{2}", Index, Chan, Cores.ToString()); ;
            }

        }
        #endregion

        [XmlElement("RangeName", IsNullable = false)]
        public String RangeName { get; set; }

        [XmlArray("RangeScaleMax", IsNullable = false), XmlArrayItem(typeof(float), ElementName = "rcm")]
        public float[] RangeScaleMax { get; set; }  // vecteur de 7 float //avec le range Scale Max je peux générer les haze factors

        [XmlArray("ColorMap8", IsNullable = false), XmlArrayItem(typeof(ColorMapElt), ElementName = "color")]
        public ColorMapElt[] ColorMap8 { get; set; } // 256 elements  // ancien C:/Altasight/Inirep/Color8Map.txt

        [XmlArray("CoreVal8", IsNullable = false), XmlArrayItem(typeof(HazeCore8b), ElementName = "cv8")]
        public HazeCore8b[] CoreVal8 { get; set; } // ancien C:/Altasight/Inirep/HazeCoreVal8.dat

        [XmlArray("ColorMap256", IsNullable = false), XmlArrayItem(typeof(ColorMapElt), ElementName = "color")]
        public ColorMapElt[] ColorMap256 { get; set; }  // 256 elements // ancien C:/Altasight/Inirep/Color256Map.txt

        [XmlArray("CoreVal256", IsNullable = false), XmlArrayItem(typeof(HazeCore256b), ElementName = "cv256")]
        public HazeCore256b[] CoreVal256 { get; set; } // ancien C:/Altasight/Inirep/HazeCoreVal256.dat

        public InputHazeConfiguration() : base()
        {
            RangeName = null;
            RangeScaleMax = new float[7];
            ColorMap8 = null;
            CoreVal8 = null;
            ColorMap256 = null;
            CoreVal256 = null;
        }

        public void InitColorMapFromFile(PathString filepath, bool bIs256bit)
        {
            ColorMapElt[] colormap = new ColorMapElt[256];
            try
            {
                int nCount = 0;
                using (StreamReader sr = new StreamReader(filepath.ToString()))
                {
                    char[] seps = new char[] { ' ', '\t' };
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (String.IsNullOrWhiteSpace(line))
                            continue;
                        String[] clrstrings = line.Split(seps, StringSplitOptions.RemoveEmptyEntries);
                        if (clrstrings.Length != 3)
                            throw new ApplicationException(String.Format("Colormap Line bad format in {0} : <{1}>", filepath, line));

                        byte r, g, b;
                        if (!Byte.TryParse(clrstrings[0], out r))
                            throw new ApplicationException(String.Format("Colormap Line bad format RED Plane in {0} : <{1}>", filepath, line));
                        if (!Byte.TryParse(clrstrings[1], out g))
                            throw new ApplicationException(String.Format("Colormap Line bad format GREEN Plane in {0} : <{1}>", filepath, line));
                        if (!Byte.TryParse(clrstrings[2], out b))
                            throw new ApplicationException(String.Format("Colormap Line bad format BLUE Plane in {0} : <{1}>", filepath, line));

                        if (nCount >= 256)
                            throw new ApplicationException(String.Format("Colormap Too much Lines in {0}", filepath));
                        colormap[nCount] = new ColorMapElt(r, g, b);
                        nCount++;
                    }
                }

                if (nCount < 256)
                    throw new ApplicationException(String.Format("Colormap Missing some levels (expected 256 elements in {0})", filepath));
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(String.Format("The file <{0}> could not be read : {1} )", filepath, ex.Message));
            }

            if (bIs256bit)
            {
                ColorMap256 = colormap;
            }
            else
            {
                ColorMap8 = colormap;

            }
        }

        public void InitHazeCoreFromFile_8(PathString filepath)
        {
            HazeCore8b[] hazecores = new HazeCore8b[8];
            try
            {
                int nCount = 0;
                using (StreamReader sr = new StreamReader(filepath.ToString()))
                {
                    char[] seps = new char[] { ';', ' ', '\t' };
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (String.IsNullOrWhiteSpace(line))
                            continue;

                        String[] corstrings = line.Split(seps, StringSplitOptions.RemoveEmptyEntries);
                        if (corstrings.Length < 2)
                            throw new ApplicationException(String.Format("Hazecore 8 Line bad format in {0} : <{1}>", filepath, line));

                        HazeCore8b corval = new HazeCore8b();

                        int nIdx;
                        if (!int.TryParse(corstrings[0], out nIdx))
                            throw new ApplicationException(String.Format("Hazecore 8 cannot parse index value in {0} : <{1}>", filepath, line));
                        if (nIdx < 0 || nIdx > 7)
                            throw new ApplicationException(String.Format("Hazecore 8 Wrong index value in {0} : <{1}> [0-7]", filepath, line));
                        corval.Index = nIdx;

                        byte val;
                        for (int k = 1; k < corstrings.Length; k++)
                        {
                            if (!byte.TryParse(corstrings[k], out val))
                                throw new ApplicationException(String.Format("Hazecore 8 bad format in {0} : <{1}>", filepath, line));
                            corval.Cores.values.Add(val);
                        }

                        if (nCount >= 8)
                            throw new ApplicationException(String.Format("Hazecore 8 Too much Lines in {0}", filepath));
                        hazecores[nCount] = corval;
                        nCount++;
                    }
                }

                if (nCount < 8)
                    throw new ApplicationException(String.Format("Hazecore 8 Missing some levels (expected 8 elements in {0})", filepath));
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(String.Format("The file <{0}> could not be read : {1} )", filepath, ex.Message));
            }
            CoreVal8 = hazecores;
        }

        public void InitHazeCoreFromFile_256(PathString filepath)
        {
            List<HazeCore256b> hazecores = new List<HazeCore256b>(256);
            try
            {
                int nCount = 0;
                using (StreamReader sr = new StreamReader(filepath.ToString()))
                {
                    char[] seps = new char[] { ';', ' ', '\t' };
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (String.IsNullOrWhiteSpace(line))
                            continue;

                        String[] corstrings = line.Split(seps, StringSplitOptions.RemoveEmptyEntries);
                        if (corstrings.Length < 3)
                            throw new ApplicationException(String.Format("Hazecore 256 Line bad format in {0} : <{1}>", filepath, line));

                        HazeCore256b corval = new HazeCore256b();

                        int nIdx;
                        if (!int.TryParse(corstrings[0], out nIdx))
                            throw new ApplicationException(String.Format("Hazecore 256 cannot parse index value in {0} : <{1}>", filepath, line));
                        if (nIdx < 0 || nIdx > 7)
                            throw new ApplicationException(String.Format("Hazecore 256 Wrong index value in {0} : <{1}> [0-7]", filepath, line));
                        corval.Index = nIdx;
                        byte val;
                        if (!byte.TryParse(corstrings[1], out val))
                            throw new ApplicationException(String.Format("Hazecore 256 cannot parse channel value in {0} : <{1}>", filepath, line));
                        if (val < 0 || val > 2)
                            throw new ApplicationException(String.Format("Hazecore 256 Wrong channel value in {0} : <{1}> [0-2]", filepath, line));
                        corval.Chan = val;
                        for (int k = 2; k < corstrings.Length; k++)
                        {
                            if (!byte.TryParse(corstrings[k], out val))
                                throw new ApplicationException(String.Format("Hazecore 256 bad format in {0} : <{1}>", filepath, line));
                            corval.Cores.values.Add(val);
                        }

                        if (nCount >= 256)
                            throw new ApplicationException(String.Format("Hazecore 256 Too much Lines in {0}", filepath));
                        //hazecores[nCount] = corval;
                        hazecores.Add(corval);
                        nCount++;
                    }
                }

                // if (nCount < 256)
                // {
                //     // we complete the missing values by the last ones
                //     int nLastElementIndex = nCount - 1;
                //     if (nCount == 0)
                //         throw new ApplicationException(String.Format("Hazecore 256 have no cores in {0}", filepath));
                //
                //     for (int k = nCount; k < 256; k++)
                //     {
                //         hazecores[k] = hazecores[nLastElementIndex];
                //     }
                // }
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(String.Format("The file <{0}> could not be read : {1} )", filepath, ex.Message));
            }
            CoreVal256 = hazecores.ToArray();
        }

        public void InitScaleMaxFromString(String sStr)
        {
            if (String.IsNullOrEmpty(sStr) || String.IsNullOrWhiteSpace(sStr))
                throw new ApplicationException("Empty or null string");

            char[] seps = new char[] { ';', ' ', '\t' };
            String[] strvals = sStr.Split(seps, StringSplitOptions.RemoveEmptyEntries);
            if (strvals.Length != 7)
                throw new ApplicationException("Wrong format : expecting exactly 7 scale max values");

            float val;
            for (int k = 0; k < 7; k++)
            {
                if (!float.TryParse(strvals[k], out val))
                    throw new ApplicationException(String.Format("Error while parsing float Range scale max <{0}>", strvals[k]));
                RangeScaleMax[k] = val;
            }
        }

        public bool CheckLevels_8(out String sErrorMsg)
        {
            sErrorMsg = String.Empty;
            if (CoreVal8 == null || ColorMap8 == null)
            {
                sErrorMsg = "Empty Colormap or Core values";
                return false;
            }

            int nSum = 0;
            List<int> ll = new List<int>(256);
            foreach (var Hazecore in CoreVal8)
            {
                ll.Clear();
                foreach (var Coreval in Hazecore.Cores.values)
                {
                    for (int i = 0; i < 256; i++)
                    {
                        if ((ColorMap8[i].R == Coreval) || (ColorMap8[i].G == Coreval) || (ColorMap8[i].B == Coreval))
                        {
                            ll.Add(i);
                        }
                    }
                }
                nSum += ll.Distinct().Count();
            }
            if (nSum != 256)
            {
                sErrorMsg = "Haze levels 8 total Error(!= 256) Check your color map  and core values configuration";
                return false;
            }
            return true;
        }

        public bool CheckLevels_256(out String sErrorMsg)
        {
            sErrorMsg = String.Empty;

            Dictionary<int, int>[] dic = new Dictionary<int, int>[3]; //  <coreval, count> [Chan]
            dic[0] = new Dictionary<int, int>();
            dic[1] = new Dictionary<int, int>();
            dic[2] = new Dictionary<int, int>();
            int[] Levels = new int[8];

            for (int i = 0; i < 256; i++)
            {
                // red
                if (dic[2].ContainsKey(ColorMap256[i].R))
                    dic[2][ColorMap256[i].R] += 1;
                else
                    dic[2][ColorMap256[i].R] = 1;

                // green
                if (dic[1].ContainsKey(ColorMap256[i].G))
                    dic[1][ColorMap256[i].G] += 1;
                else
                    dic[1][ColorMap256[i].G] = 1;

                // blue
                if (dic[0].ContainsKey(ColorMap256[i].B))
                    dic[0][ColorMap256[i].B] += 1;
                else
                    dic[0][ColorMap256[i].B] = 1;
            }

            foreach (var Hazecore in CoreVal256)
            {
                foreach (var Coreval in Hazecore.Cores.values)
                {
                    if (dic[Hazecore.Chan].ContainsKey(Coreval))
                    {
                        Levels[Hazecore.Index] += (dic[Hazecore.Chan])[Coreval];
                    }
                }
            }
            // if (CoreVal256.Length < 256)
            // {
            //     Levels[7] += 256 - CoreVal256.Length;
            // }

            int nSum = Levels.Sum();
            if (nSum < 254 || nSum > 256)
            {
                sErrorMsg = "Haze levels 256 total Error (!= 256) Check your color map  and core values configuration";
                return false;
            }
            return true;
        }
    }
}
