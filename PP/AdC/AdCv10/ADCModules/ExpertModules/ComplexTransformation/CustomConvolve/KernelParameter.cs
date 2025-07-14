using System;
using System.Text;
using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ExpertModules.ComplexTransformation.CustomConvolve
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class KernelParameter : ParameterBase
    {
        public int Width { get { return Kernel.GetLength(1); } }
        public int Height { get { return Kernel.GetLength(0); } }
        public double[,] Kernel = new double[0, 0];

        // Propriété bindable
        private string _kernelString = "";
        public string KernelString
        {
            get { return _kernelString; }
            set
            {
                try
                {
                    _kernelString = value;
                    StringToKernel(value);
                    _kernelError = null;
                }
                catch (Exception ex)
                {
                    _kernelError = ex.Message;
                }
                OnPropertyChanged();
                ReportChange();
            }
        }

        private string _kernelError = null;

        //=================================================================
        // Constructeur
        //=================================================================
        public KernelParameter(CustomConvolveModule module, string name)
            : base(module, name)
        {
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList parameterNodes)
        {
            XmlNode node = ReadParameter(Name, parameterNodes);
            KernelString = node.InnerText.Trim();
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            XmlElement elem = SaveParameter(xmlNode, Name, this);

            elem.InnerText = "\r\n" + KernelToString();
            return null;
        }

        //=================================================================
        // IHM
        //=================================================================
        private KernelParameterView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterUI = new KernelParameterView();
                    _parameterUI.DataContext = this;
                }

                return _parameterUI;
            }
        }


        //=================================================================
        // 
        //=================================================================
        private string KernelToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("");
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                    sb.Append(Kernel[y, x].ToString() + "\t");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        //=================================================================
        // 
        //=================================================================
        private void StringToKernel(string str)
        {
            int x = 0, y = 0;
            int w, h;
            try
            {
                // Taille du Kernel
                //.................
                string[] lines = str.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                h = lines.Length;
                if (h == 0)
                {
                    w = 0;
                }
                else
                {
                    string[] fields = lines[0].Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    w = fields.Length;
                }

                // Lecture du Kernel
                //..................
                Kernel = new double[h, w];

                for (y = 0; y < Height; y++)
                {
                    string[] fields = lines[y].Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (x = 0; x < Width; x++)
                        Kernel[y, x] = double.Parse(fields[x]);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to parse Kernel line:" + (y + 1) + " column:" + (x + 1), ex);
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            if (_kernelError != null)
                return _kernelError;
            if (Width <= 0 || Height <= 0)
                return "No Kernel";
            return base.Validate();
        }

        //=================================================================
        // 
        //=================================================================
        public override bool HasSameValue(object obj)
        {
            var parameter = obj as KernelParameter;

            return parameter != null &&
                   Extension.ArraysEqual(Kernel, parameter.Kernel);
        }

        //=================================================================
        // Formattage du Kernel
        //=================================================================
        private AutoRelayCommand _formatCommand = null;
        public AutoRelayCommand FormatCommand
        {
            get
            {
                return _formatCommand ?? (_formatCommand = new AutoRelayCommand(
                    () =>
                    {
                        try
                        {
                            // StringToKernel(KernelString); déjà fait par la perte de focus 
                            _kernelString = KernelToString();
                            _kernelError = null;
                            OnPropertyChanged(nameof(KernelString));
                            ReportChange();
                        }
                        catch (Exception ex)
                        {
                            _kernelError = ex.Message;
                        }
                    }
                    ));
            }
        }

        //=================================================================
        // Transposition
        //=================================================================
        private AutoRelayCommand _transposeCommand = null;
        public AutoRelayCommand TransposeCommand
        {
            get
            {
                return _transposeCommand ?? (_transposeCommand = new AutoRelayCommand(
                    () =>
                    {
                        // StringToKernel(KernelString); déjà fait par la perte de focus 
                        if (_kernelError != null)
                        {
                            AttentionMessageBox.Show("Please fix the error before transposing the matrix");
                            return;
                        }

                        double[,] kernelT = new double[Width, Height];
                        for (int y = 0; y < Height; y++)
                        {
                            for (int x = 0; x < Width; x++)
                                kernelT[x, y] = Kernel[y, x];
                        }
                        Kernel = kernelT;
                        _kernelString = KernelToString();
                        OnPropertyChanged(nameof(KernelString));
                        ReportChange();
                    }));
            }
        }

    }
}
