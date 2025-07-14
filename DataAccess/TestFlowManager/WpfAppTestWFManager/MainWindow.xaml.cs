using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace WpfAppTestFlowManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ILogger logger;
        TextBoxOutputter outputter;

        private Dictionary<string, ServiceHost> _hosts = new Dictionary<string, ServiceHost>();


        public MainWindow()
        {
            InitializeComponent();

            MainViewModel mvm =  ClassLocator.Default.GetInstance<MainViewModel>();


            DataContext = mvm;


            outputter = new TextBoxOutputter(TestBox);
            Console.SetOut(outputter);

            logger = ClassLocator.Default.GetInstance<ILogger>();





            /*
            UnitySC.DataAccess.Service.Interface.Workflow.IWorkflowManagerSupervision sWorkflowManagerSupervision = ClassLocator.Default.GetInstance<UnitySC.DataAccess.Service.Interface.Workflow.IWorkflowManagerSupervision>();

            UnitySC.DataAccess.Service.Interface.Workflow.IWorkflowManager sWorkflowManager = ClassLocator.Default.GetInstance<UnitySC.DataAccess.Service.Interface.Workflow.IWorkflowManager>();
            

            try
            {
                StartService("IWorkflowManagerSupervision", sWorkflowManagerSupervision);

                StartService("WorkflowManager", sWorkflowManager);
            }
                catch
            { }
            */
            logger.Information("Started");


        }

        private void StartService(string name, object service)
        {
            ServiceHost host = new ServiceHost(service);
            foreach (var endpoint in host.Description.Endpoints)
            {
                logger.Information($"Creating {name} service on {endpoint.Address}");
            }
            host.Open();
            _hosts.Add(name, host);
        }
    }




    public class TextBoxOutputter : TextWriter
    {
        TextBox textBox = null;

        public TextBoxOutputter(TextBox output)
        {
            textBox = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            textBox.Dispatcher.BeginInvoke(new Action(() =>
            {
                textBox.AppendText(value.ToString());
            }));
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
