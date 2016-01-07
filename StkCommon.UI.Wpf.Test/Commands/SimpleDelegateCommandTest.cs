using FluentAssertions;
using NUnit.Framework;
using StkCommon.UI.Wpf.Commands;

namespace StkCommon.UI.Wpf.Test.Commands
{
	[TestFixture]
	public class SimpleDelegateCommandTest
	{
		private SimpleDelegateCommand _target;
		private int _canExecuteRiseCount;
		private int _executeRiseCount;
		private bool _canExecuteExpectedReturnValue;

		#region Setup

		[SetUp]
		public void Setup()
		{
			_canExecuteRiseCount = 0;
			_executeRiseCount = 0;
			_canExecuteExpectedReturnValue = false;
			_target = new SimpleDelegateCommand(ExecuteMethod, CanExecuteMethod);
		}

		private bool CanExecuteMethod()
		{
			_canExecuteRiseCount++;
			return _canExecuteExpectedReturnValue;
		}

		private void ExecuteMethod()
		{
			_executeRiseCount++;
		}

		#endregion

		[Test]
		public void ShouldCanExecuteWork()
		{
			//Given
			_canExecuteExpectedReturnValue = false;

			//When
			var canExecuteActualReturnValue = _target.CanExecute();

			//Then
			_canExecuteRiseCount.Should().Be(1, "CanExecute не вызвалось");
			canExecuteActualReturnValue.Should().Be(_canExecuteExpectedReturnValue,
				"¬озвращаемое значение CanExecute не соответствует заданному");
		}

		[Test]
		public void ShouldCanExecuteReturnTrueWhenCanExecuteFuncNull()
		{
			//Given
			//When
			var command = new SimpleDelegateCommand(() => { });

			//Then
			command.CanExecute("Test").Should().BeTrue("CanExecute должен возвращать True если не задан CanExecute Func");
		}

		[Test]
		public void ShouldExecuteWork()
		{
			//Given
			//When
			_target.Execute();

			//Then
			_executeRiseCount.Should().Be(1, "Execute не вызвалось");
		}
	}
}