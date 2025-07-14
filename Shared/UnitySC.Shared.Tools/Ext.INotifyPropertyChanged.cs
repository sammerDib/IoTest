using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace UnitySC.Shared.Tools
{
    public static class INotifyPropertyChangedExt
    {
        /// <summary>
        /// Usage:
        /// this.CallPropertyChanged(), from the current property accessor.
        /// or
        /// this.CallPropertyChanged(nameof(AProperty)), when outside of the property accessor.
        /// </summary>
        public static void CallPropertyChanged(this INotifyPropertyChanged this_t, [CallerMemberName] string ùpropertyName_s = null)
        {
            var ùpropChanged_fi = this_t.GetType().getFieldRecursive("PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            (ùpropChanged_fi.GetValue(this_t) as PropertyChangedEventHandler)?.Invoke(this_t, new PropertyChangedEventArgs(ùpropertyName_s));
        }
    }
}