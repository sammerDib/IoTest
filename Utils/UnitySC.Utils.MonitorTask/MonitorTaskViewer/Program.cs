using System;
using System.Windows.Forms;

namespace MonitorTaskViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Set Deployment Key for Arction components -- à offusquer  //for Ligthning chart v8.x
            //string deploymentKeyv8 = "lgCAAJcGHGyahdUBJABVcGRhdGVhYmxlVGlsbD0yMDIwLTEwLTE4I1JldmlzaW9uPTAC/z9ufJSC0I6ZJJRW8jZA0ttYsQ4/0HWN+zipmIcErZhF2Zo2Fdcma9OYf/LVSRzYeZbr6tBg4tmlIOyFC0Zf3SNXIiKQ1Go1uRkGQvGxBkjLPXPY9B24C6fZx3lrMwjvI9jLHDxsXaJWXCAB9Z+QQMur5IM9xf6SG8j4ykAhXgy1AlT1Fl1HTvBd+BY5Vzzk7z0ACuqdmqb+4Z4MXXzh4cvA8atInfieTGKOChtxLmDOp9cJWzgnvE4kuqMED6al7kMrBs0PptoZOrFt+gOG/RRoGaezUnbcbSilUE09KWZB/oxLORdP7IKobmtX7ZlJ7k/2IYCsyJriRYyXim1G4M1nKoW8nmUuGL2VbR/JDI5JCQXPuZMJpTVh1BGahi5DQ3WcpuGblvZXJh2NrY6Wu8hm79Z7dNhU4XsdpFitRZ2s97cu4QFefOECalH3nGu6B88xH2vto+UicJyP6HAaI8/8xZmflJyn4ArNvsb7nbDZg4rpvn0Pe1+v4qMjgNzNB6U=";
            //Arction.WinForms.Charting.LightningChartUltimate.SetDeploymentKey(deploymentKey);

            string deploymentKey = "lgCAAMDQWCaC7NkBJABVcGRhdGVhYmxlVGlsbD0yMDI1LTEwLTE1I1JldmlzaW9uPTAI/z8AAAAA/AEy9k+WSPYWl8F5j8DR3/xHV8rmwsD6bgDd0+Kv9Aki7UvzB0KJxrUPqzZmcZ1hEhm6bFr+fezEYuukPWEkI7pybF86LTroOuA934Gci/KuDUrhHiaqtxFeaR30Gcgr25NjTyEpauRATjQ4BFk32TnkLwotmJoCv+HYAJkkvd85VCzS0o5fd4w99JHK3/XtyJSYL8/OCCrqumTQZm5A8s7q95M8AfxmeLTEUjPFJp/k+m0oTPHF4er+PTE/m1R/r1+yL6ZeiCzkuFB5m4vLE1vxa7ZEp0aRQ01Xw+0LPPBusgBj4089eXfVWH3DsnFfDmPrFn63MByaFqpzT/hK4J0EiXGqHRaGz8CCiRVxAO3mAT7DirAypxLrrF+142Z3f3iQnd88mRsFiTN2rqfbZDFmPPaK2j4LwDwqKiaVCOz6ISQpG8W7UOMSZjX1KnMiS+FdQRYJJPuuE0WGRMutSyrNHGawAsMY6J4hOh4hDsJsRgN3onrFG+pCHwFG/fUD154=";
            LightningChartLib.WinForms.Charting.LightningChart.SetDeploymentKey(deploymentKey);
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MonitorTaskViewForm(args));
        }
    }
}
