using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnitySC.PM.ANA.Service.Core.Calibration
{
    public class MaterialData
    {
        public MaterialData()
        {
            Name = string.Empty;
            Datas = new List<MaterialDataItem>();
        }

        public static MaterialData LoadMaterialDataFromFile(string path, string materialName)
        {
            var item = new MaterialData()
            {
                Name = materialName,
                Datas = new List<MaterialDataItem>()
            };

            string line = null;
            double lambda, indice, extinction;

            StreamReader reader = File.OpenText(path);
            while ((line = reader.ReadLine()) != null)
            {
                string[] words = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length != 3)
                    continue;
                if (!Double.TryParse(words[0], out lambda)) return null;
                if (!Double.TryParse(words[1], out indice)) return null;
                if (!Double.TryParse(words[2], out extinction)) return null;
                item.Datas.Add(new MaterialDataItem()
                {
                    Lambda = lambda,
                    Indice = indice,
                    Extinction = extinction
                });
            }
            if (item.Datas.Count < 2)
            {
                throw new Exception("Fail to load material data from file '" + path + "' : Table must have at least 2 points !");
            }
            if (item.Datas[0].Lambda > item.Datas[1].Lambda)
            {
                item.Datas.Reverse();
            }
            MaterialDataItem ge = item.Datas.FirstOrDefault(d => d.Lambda > 600);
            MaterialDataItem le = item.Datas.LastOrDefault(d => d.Lambda <= 600);
            if (ge == null || le == null)
            {
                ge = item.Datas.FirstOrDefault(d => d.Lambda > 0.6);
                le = item.Datas.LastOrDefault(d => d.Lambda <= 0.6);
                if (ge == null || le == null)
                {
                    throw new Exception("Fail to load material data from file '" + path + "' : Table is not including a point at 600nm");
                }
            }
            else
            {
                item.Datas.ForEach(d => d.Lambda /= 1000.0);
            }
            var BaseWaveLength = 1.33;
            item.DefaultGroupRefractiveIndex = item.CalculateGroupRefractiveIndexAccordingToWavelength(BaseWaveLength);

            return item;
        }

        public double CalculateGroupRefractiveIndexAccordingToWavelength(double waveLength)
        {
            double BaseWaveLength = waveLength;
            double HalfRange = 0.050;
            List<MaterialDataItem> filteredList = Datas.Where(item => item.Indice != 0).Where(item => item.Lambda >= (BaseWaveLength - HalfRange) && item.Lambda <= (BaseWaveLength + HalfRange)).ToList();

            double[] references = new double[filteredList.Count - 1];
            double[] dispersions = new double[filteredList.Count - 1];

            MathNet.Numerics.Polynomial fitPolynomialRefIndex = MathNet.Numerics.Polynomial.Fit(filteredList.Select(item => item.Lambda).ToArray(), filteredList.Select(item => item.Indice).ToArray(), 2);
            var groupRefractiveIndex = fitPolynomialRefIndex.Coefficients[0] - (fitPolynomialRefIndex.Coefficients[2] * Math.Pow((waveLength), 2));

            return groupRefractiveIndex;
        }

        public string Name { get; set; }

        public double DefaultGroupRefractiveIndex { get; set; }

        public List<MaterialDataItem> Datas { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
