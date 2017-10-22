using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Framework.MVVM
{
    public class RelayCommand : ICommand
	{
		private readonly Action<object> execute;
		private readonly Predicate<object> canExecute;

		public RelayCommand(Action<object> execute)
			: this(execute, null)
		{
		}

		public RelayCommand(Action<object> execute, Predicate<object> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			this.execute = execute;
			this.canExecute = canExecute;
		}

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return canExecute == null || canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			execute(parameter);
		}

		public void RaiseCanExecuteChanged()
		{
			var copy = CanExecuteChanged;
			if (copy != null)
				copy(this, new EventArgs());
		}

		public event EventHandler CanExecuteChanged;
	}
}
