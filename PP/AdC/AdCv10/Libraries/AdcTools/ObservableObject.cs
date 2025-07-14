using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace AdcTools
{
    ///////////////////////////////////////////////////////////////////////
    // Classe de base pour les ViewModel
    ///////////////////////////////////////////////////////////////////////
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

		//=================================================================
		// 
		//=================================================================
		public virtual void NotifyPropertyChanged<T>(Expression<Func<T>> expression)
		{
			MemberInfo info = GetMemberInfo(expression);
			string propertyName = info.Name;

			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		//=================================================================
		// 
		//=================================================================
		public static MemberInfo GetMemberInfo<T>(Expression<Func<T>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException("Expression is not a member", "expression");
            return member.Member;
        }

    }
}
