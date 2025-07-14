using System.Windows;


namespace AdcTools
{
    /// <summary>
    /// La MessageBox de base avec une icône Attention
    /// </summary>
    public class AttentionMessageBox
    {
        public static MessageBoxResult Show(string msg)
        {
            MessageBoxResult result = MessageBox.Show(
                msg,
                Application.ResourceAssembly.GetName().Name,
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Exclamation);

            return result;
        }
    }
}
