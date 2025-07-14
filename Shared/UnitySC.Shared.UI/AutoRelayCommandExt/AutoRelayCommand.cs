using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

namespace UnitySC.Shared.UI.AutoRelayCommandExt
{
    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other
    /// objects by invoking delegates. The default return value for the <see cref="CanExecute"/>
    /// method is <see langword="true"/>. This type does not allow you to accept command parameters
    /// in the <see cref="Execute"/> and <see cref="CanExecute"/> callback methods.
    /// </summary>
    public sealed class AutoRelayCommand : IRelayCommand
    {
        /// <summary>
        /// The <see cref="Action"/> to invoke when <see cref="Execute"/> is used.
        /// </summary>
        private readonly Action execute;

        /// <summary>
        /// The optional action to invoke when <see cref="CanExecute"/> is used.
        /// </summary>
        private readonly Func<bool> canExecute;


        /// <inheritdoc/>
        //public event EventHandler CanExecuteChanged;

        private EventHandler _requerySuggestedLocal;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> is <see langword="null"/>.</exception>
        public AutoRelayCommand(Action execute)
        {
            if (execute is null) throw (new ArgumentNullException(nameof(execute)));

            this.execute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> or <paramref name="canExecute"/> are <see langword="null"/>.</exception>
        public AutoRelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute is null) throw (new ArgumentNullException(nameof(execute)));
            if (canExecute is null) throw (new ArgumentNullException(nameof(canExecute)));


            this.execute = execute;
            this.canExecute = canExecute;
        }
        public void NotifyCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(object parameter)
        {
            return this.canExecute?.Invoke() != false;
        }

        /// <inheritdoc/>
        public void Execute(object parameter)
        {
            this.execute();
        }


        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecute != null)
                {
                    // add event handler to local handler backing field in a thread safe manner
                    EventHandler handler2;
                    EventHandler canExecuteChanged = _requerySuggestedLocal;

                    do
                    {
                        handler2 = canExecuteChanged;
                        EventHandler handler3 = (EventHandler)Delegate.Combine(handler2, value);
                        canExecuteChanged = System.Threading.Interlocked.CompareExchange<EventHandler>(
                            ref _requerySuggestedLocal,
                            handler3,
                            handler2);
                    }
                    while (canExecuteChanged != handler2);

                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (canExecute != null)
                {
                    // removes an event handler from local backing field in a thread safe manner
                    EventHandler handler2;
                    EventHandler canExecuteChanged = this._requerySuggestedLocal;

                    do
                    {
                        handler2 = canExecuteChanged;
                        EventHandler handler3 = (EventHandler)Delegate.Remove(handler2, value);
                        canExecuteChanged = System.Threading.Interlocked.CompareExchange<EventHandler>(
                            ref this._requerySuggestedLocal,
                            handler3,
                            handler2);
                    }
                    while (canExecuteChanged != handler2);

                    CommandManager.RequerySuggested -= value;
                }
            }
        }
    }
}
