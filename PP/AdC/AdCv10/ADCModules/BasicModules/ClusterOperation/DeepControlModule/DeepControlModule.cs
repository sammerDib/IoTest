using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Net.Sockets;
using System.Timers;
using System.Text.Json;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules.AsoEditor;
using BasicModules.KlarfEditor;
using BasicModules.Sizing;
using System.Configuration;

namespace BasicModules.DeepControl
{ 

    public class InputJson
    {
        public string layer { get; set; }
        public string name { get; set; } // nom de l'image ex : image.tiffe
        public string path { get; set; }  // chemin vers l'image
        public string wafer_id { get; set; }
        public float center_x { get; set; } // pixel
        public float center_y { get; set; } // pixel
        public int width { get; set; } // pixel
        public int height { get; set; } // pixel
        public double radius_wafer { get; set; } // pixel
    }

    public class DeepControlModule : ClusterizerModuleBase, IClassifierModule
    {
        //string adresseClient = "172.16.5.77";
        //int    port = 5002;

        // "172.20.74.35";   // Debaleena
        // 5008
        public  String ClientAdress;
        public int Port;


        public List<DefectClass> DefectClassList { get { return ParamClassification.DefectClassList; } }
        public List<string> DefectClassLabelList
        {
            get
            {
                List<string> list = new List<string>(
                    from cl in ParamClassification.DefectClassList
                    select cl.label
                    );
                return list;
            }
        }
        public Dictionary<string, List<ImageBase>> ImagesByWafer { get; set; }
        public Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private List<InputJson> _listInputjson = new List<InputJson>();
       
        protected  List<SurCluster> clusterList = new List<SurCluster>();

      
        //=================================================================
        // Paramètres du XML
        //=================================================================
        [ExportableParameter(false)]
        public readonly ClassificationParameter ParamClassification;

        //=================================================================
        // Autres membres
        //=================================================================
        private int _nbBlobs = 0;
        private int nbParents = 0;
        
        //=================================================================
        // Constructeur
        //=================================================================
        public DeepControlModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            ClientAdress = new StringParameter(this, "Client address");
            Port = new IntParameter(this, "Port");
            ParamClassification = new ClassificationParameter(this, "DeepControl Classification");
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            var list = Recipe.ModuleList.Select(kvp => kvp.Value).OfType<IClusterizerModule>().ToList();
            ClusterizerIndex = list.IndexOf(this);
            NbClusterizers = list.Count();
            if (ClusterizerIndex < 0)
                throw new ApplicationException("Can't find Clusterizer Modules");
            ImagesByWafer = new Dictionary<string,List< ImageBase>>();

            List<ModuleBase> loaders = this.FindAncestors(m => m is IDataLoader);
            nbParents = loaders.Count;
            string param =  ConfigurationManager.AppSettings["DeepControl.AddressClient"];
            
            ClientAdress = param.Substring(0,param.IndexOf(':'));
            Port = Convert.ToInt32(param.Substring(param.IndexOf(':') + 1));
            sender.Connect(ClientAdress, Port);

            InitTimer();
        }
 
        protected override void OnStopping(eModuleState oldState)
        {

            sender.Close();
            Purge();
            base.OnStopping(oldState);
        }

        private void InitTimer()
        {
            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = false;
            aTimer.Enabled = true;

        }
        private void SetTimer(int delay)
        {
            // Hook up the Elapsed event for the timer. 
            aTimer.Interval = delay;
            stop = false;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            stop = true;
            aTimer.Close();
        }

        private object mutex = new object();

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            lock (mutex)
            {
                logDebug("process " + obj);
                Interlocked.Increment(ref nbObjectsIn);
                ImageBase image = (ImageBase)obj;
                string WaferID = image.Layer.Wafer.waferInfo[AcquisitionAdcExchange.eWaferInfo.WaferID];

                obj.AddRef();
                if (ImagesByWafer.ContainsKey(WaferID))
                {
                    ImagesByWafer[WaferID].Add(image);
                }
                else
                {
                    ImagesByWafer.Add(WaferID, new List<ImageBase> { image });
                }
                if (ImagesByWafer[WaferID].Count == nbParents)
                {
                    SendWafer(ImagesByWafer[WaferID], WaferID);
                    ImagesByWafer.Remove(WaferID);  
                }
                //ProcessChildren(image);    // ?  on attends d'avoir traité toutes les images du WaferID avant d'envoyer ??
            }
        }

        protected override void ProcessQueueElement(Cluster cluster)
        {
            if (State != eModuleState.Aborting)
            {
                ProcessChildren(cluster);
            }
        }


        private void SendWafer(List<ImageBase> images, string WaferID)
        {

            List<lineJson> result = new List<lineJson>();

            InputJson Input = null;

            foreach (ImageBase image in images)
            {
                Input = new InputJson();
                Input.layer = image.Layer.name;
                Input.center_y = ((RectangularMatrix)image.Layer.Matrix).WaferCenterX;
                Input.center_x = ((RectangularMatrix)image.Layer.Matrix).WaferCenterX;
                Input.height = image.Height;
                Input.width = image.Width;
                Input.path = Path.GetDirectoryName(image.Filename);
               

                Input.name = Path.GetFileName(image.Filename);
                Input.wafer_id = image.Layer.Wafer.GetWaferInfo(eWaferInfo.WaferID);
                //Calcul du diamçtre en pixel. Le diamettre est en micron d'origine
                Input.radius_wafer = ((NotchWafer)image.Layer.Wafer).Diameter / (2 * ((RectangularMatrix)image.Layer.Matrix).PixelHeight);


                _listInputjson.Add(Input);
            }

            bool ret = true;
            
            ret = SendImageList(_listInputjson);
            if (!ret)
            {
                Console.WriteLine("send files fail");
                return;
            }
            ret = GetAnalyseResult(out result);
            if (!ret)
            {
                Console.WriteLine("receive  result  fail");
                return;
            }
            
            CreateClusterList(result,images);

            foreach (SurCluster Sc in clusterList)
            {
                //cluster.blobList.Clear();
                if (State != eModuleState.Aborting)
                {
                    if (Sc.cluster.OriginalProcessingImage.GetMilImage().MilId == 0)
                    {
                        AllocateVignette(Sc.image, Sc.cluster);
                        CopyVignette(Sc.image, Sc.cluster);
                        Sc.cluster.micronQuad = Sc.image.Layer.Matrix.pixelToMicron(Sc.cluster.pixelRect);
                    }
                    outputQueue.Enqueue(Sc.cluster);
                }
                Sc.cluster.DelRef();
            }

            if (State == eModuleState.Aborting)
                outputQueue.AbortQueue();
            else
                outputQueue.CloseQueue();
        
        }

        /// <summary>
        /// récupère les données d'analyse en json
        /// </summary>
        /// <returns></returns>
        private bool GetAnalyseResult(out List<lineJson> result)
        {
            bool ret = true;

            try
            {
                // on demande si l'analyse est prête

                //goto testc;
                byte[] b = new byte[100];
                //sender.ReceiveTimeout = 2000;
                int k;
                
                SetTimer(600000);  // 10 mn l'analyse python peut prendre du temps
                do
                {
                    k = sender.Receive(b);  // demande si pret 
                } while ((k == 0) && (!stop));    // prevoir une limite temps ?

                if (stop)  // pas de réponse au bout du timeOut ?
                    throw new ApplicationException("No response from socket client ");

                //responseBytes = sender.Receive();
                string response = Encoding.ASCII.GetString(b);
                if (!response.Contains("ok"))
                {
                    ManageError(response);
                    throw new ApplicationException("Ko response from socket client ");
                }

                // l'analyse est prête

                // on signal au client qu'on est prêt à la recevoir

                byte[] byData = System.Text.Encoding.ASCII.GetBytes("ok");
                sender.Send(byData);

                sender.ReceiveTimeout = 30000;   // 30 secondes

                // réception de l'analyse

                var buffer = new List<byte>();
                while (sender.Available == 0)
                {
                    Thread.Sleep(100);
                }

                while (sender.Available > 0)
                {

                    var currByte = new Byte[sender.Available];
                    var byteCounter = sender.Receive(currByte, 0, sender.Available, SocketFlags.None);
                    if (byteCounter > 0)
                    {
                        buffer.AddRange(new List<byte>(currByte));
                    }
                    Thread.Sleep(100);
                }
                byte[] datas = new byte[buffer.Count];
                datas = buffer.ToArray();

                char[] chars = new char[buffer.Count];

                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(datas, 0, buffer.Count, chars, 0);
                System.String jsonString = new System.String(chars);
   //testc:
             //string jsonString = File.ReadAllText(@"E:\DeepControl\reponseJsonMatthieu.txt");

                //List<lineJson> toto = JsonSerializer.Deserialize<List<lineJson>>(jsonString); 
                result = JsonSerializer.Deserialize<List<lineJson>>(jsonString);
                
            }
            catch(Exception e)
            {
                logDebug(e.ToString());
                result = null;
                ret = false;

            }
            return ret;
        }

        private void ManageError(string response)
        {
            StringBuilder message = new StringBuilder();
            
            throw new ApplicationException("Ko response from socket client ");
        }

        private bool SendImageList(List<InputJson> listInputjson)
        {
            if (listInputjson is null)
            {
                throw new ArgumentNullException(nameof(listInputjson));
            }

            bool ret = true;
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    WriteIndented = true
                };

                JsonSerializerOptions optionsCopy = new JsonSerializerOptions(options);
                string result = JsonSerializer.Serialize<List<InputJson>>(listInputjson, optionsCopy);
                //result = File.ReadAllText(@"E:\DeepControl\socket_client_images.json");
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(result);
                sender.Send(byData);
            }
            catch (Exception e) 
            {
                logDebug(e.ToString());
            }
            return ret;
        }

       

        private bool SendString(string message, bool getReponse)
        {
            bool ret = true;
            {

                byte[] byData = System.Text.Encoding.ASCII.GetBytes(message);

                sender.Send(byData);

                if (getReponse)
                {
                    //stream.
                    byte[] buffer = new byte[1024];
                    int iRx = sender.Receive(buffer);
                    char[] chars = new char[iRx];
                    System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                    int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
                    System.String recv = new System.String(chars);
                    if (!recv.Contains("ok"))
                        ret = false;
                }

            }
            return ret;
        }

       
        private void Purge()
        {
            foreach (string layer in ImagesByWafer.Keys)
            {
                foreach (ImageBase image in ImagesByWafer[layer])
                {
                    image.Dispose();
                }
            }
            ImagesByWafer.Clear();  
        }
        public override UserControl RenderingUI
        {
            get
            {
                return GetUI();
            }
        }

        private static string s_baseName;
        private static byte[] responseBytes = new byte[] { };
        //static string response;
        private System.Timers.Timer aTimer;

        private bool stop;

        private void CreateClusterList(List<lineJson> listDefects, List<ImageBase> images)
        {
            if (images is null)
            {
                throw new ArgumentNullException(nameof(images));
            }
            ImageBase image;

            Interlocked.Increment(ref _nbBlobs);
            if (paramDefectCountLimit.IsUsed && ((_nbBlobs > paramDefectCountLimit.Value)))
            {
                Recipe.PartialAnalysis = true;
                return ;
            }
            
            var unknownDefectClasses = listDefects.Select(line => line.name).Where(name => !DefectClassLabelList.Contains(name)).ToList();
            if (unknownDefectClasses.Count > 0)
            {
                throw new ApplicationException($"The following DefectClasses does not exist in the DeepControl DefectClass list : {string.Join(", ", unknownDefectClasses)}");
            }
            
            
            foreach (lineJson line in listDefects)
            {
               // images[0].Layer.name = "PL";
              //  images[1].Layer.name = "DDF90";
                image = GetImage(images , line);
                Characteristic carcteBbox = new Characteristic(typeof(string), "DefecClasseName");

                for (int i = 0; i <  line.bbox.Count; i++)
                {
                    var Scluster = CreateSurCluster(image, i, line.bbox[i], carcteBbox, line);
                    clusterList.Add(Scluster);
                }
            }

            HashSet<ModuleBase> subtreeNodes  = GetAllDescendants();

            foreach (ModuleBase child in subtreeNodes)
            {
                switch (child)
                {
                    case KlarfEditorModule klarfModule:
                        klarfModule.ParamRoughBins.Synchronize();
                        break;
                    case AsoEditorModule asoModule:
                        asoModule.paramDefectClasses.Synchronize();
                        break;
                    case SizingWithBlobCalculationModule sizingWithBlobCalculationModule:
                        sizingWithBlobCalculationModule.paramSizing.SynchronizeWithClassification();
                        break;
                    case SizingModule sizingModule:
                        sizingModule.paramSizing.SynchronizeWithClassification();
                        break;
                }
            }

        }

        private SurCluster CreateSurCluster(ImageBase image, int i, BoundingBox boundingBox, Characteristic carcteBbox,
            lineJson line)
        {
            int num = CreateClusterNumber(image, i);
            Cluster cluster = new Cluster(num, image.Layer);
            SurCluster Scluster = new SurCluster();
            Scluster.cluster = cluster;
            Scluster.image = image;

            if (image is FullImage)
            {
                int lfDataBlobLeft = boundingBox.topleft.X;
                int lfDataBlobTop = boundingBox.topleft.Y;
                int lfDataBlobRight = boundingBox.bottomright.X;
                int lfDataBlobBottom = boundingBox.bottomright.Y;
                        
                int left = image.imageRect.Left + (int)lfDataBlobLeft;
                int top = image.imageRect.Top + (int)lfDataBlobTop;
                int right = image.imageRect.Left + (int)lfDataBlobRight;
                int bottom = image.imageRect.Top + (int)lfDataBlobBottom;
                cluster.pixelRect = Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
                        
                Blob blob = new Blob(0, cluster);
                blob.pixelRect.X = cluster.imageRect.X + (int)lfDataBlobLeft;
                blob.pixelRect.Y = cluster.imageRect.Y + (int)lfDataBlobTop;
                blob.pixelRect.Width = (int)(lfDataBlobRight - lfDataBlobLeft) + 1;
                blob.pixelRect.Height = (int)(lfDataBlobBottom - lfDataBlobTop) + 1;

                blob.pixelArea = (int)cluster.pixelRect.Area();
                
                cluster.blobList.Add(blob);
                cluster.defectClassList.Add(line.name);
            }

            return Scluster;
        }

        private ImageBase GetImage(List<ImageBase> images, lineJson line)
        {
            ImageBase image = images.Where<ImageBase>(x => x.Layer.name == line.layer).First();
            return image;
        }




        /// <summary>
        /// Permits to set the basename of the selected wafer to name the generated result JSON file.
        /// </summary>
        /// <param name="waferBaseName"></param>
        public static void SetBaseName(string waferBaseName)
        {
            s_baseName = waferBaseName;
        }
    }

    public class BoundingBox
    {
        public Point topleft { get; set; }
        public Point bottomright { get; set; }

    }

    public class SurCluster
    {
        public ImageBase image { get; set; }
        public Cluster cluster { get; set; }
    }

    public class lineJson
    {
        //[JsonProperty("Layer")]
        public string layer { get; set; }
        //[JsonProperty("Type")]
        public string name { get; set; }
        //[JsonProperty("boundingBoxes")]
        public IList<BoundingBox> bbox { get; set; }
        //[JsonProperty("Confiance")]
    }

}


