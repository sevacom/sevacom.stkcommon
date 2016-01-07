using FluentAssertions;
using NUnit.Framework;
using StkCommon.UI.Wpf.Commands;

namespace StkCommon.UI.Wpf.Test.Commands
{
	[TestFixture]
	public class DelegateCommandTest
	{
		private string _expectedCommandName;
		private DelegateCommand<object> _target;
		private int _canExecuteRiseCount;
		private int _executeRiseCount;
		private bool _canExecuteExpectedReturnValue;
		private object _canExecuteActualParam;
		private object _executeActualParam;

		#region Setup

		[SetUp]
		public void Setup()
		{
			_expectedCommandName = "TestCommand";
			_canExecuteRiseCount = 0;
			_executeRiseCount = 0;
			_canExecuteExpectedReturnValue = false;
			_canExecuteActualParam = null;
			_executeActualParam = null;
			_target = new DelegateCommand<object>(ExecuteMethod, CanExecuteMethod, _expectedCommandName);
		}

		private bool CanExecuteMethod(object o)
		{
			_canExecuteActualParam = o;
			_canExecuteRiseCount++;
			return _canExecuteExpectedReturnValue;
		}

		private void ExecuteMethod(object o)
		{
			_executeActualParam = o;
			_executeRiseCount++;
		}

		#endregion

		[Test]
		public void ShouldCommandNameBeExpected()
		{
			//Given
			//When
			//Then
			_target.CommandName.Should().Be(_expectedCommandName, "Название команды не установлено");
		}

		/// <summary>
		/// должен кидать событие CanExecuteChanged когда вызывают RaiseCanExecuteChanged
		/// </summary>
		[Test]
		public void ShouldRiseEventWhenExecuteRaiseCanExecuteChanged()
		{
			//Given
			var riseCount = 0;
			_target.CanExecuteChanged += (sender, args) => { riseCount++; };
			
			//When
			_target.RaiseCanExecuteChanged();
			
			//Then
			riseCount.Should().Be(1, "Событие CanExecuteChanged не сработало при вызове RaiseCanExecuteChanged");
		}

		/// <summary>
		/// Должен выполнять делегат CanExecute
		/// </summary>
		[Test]
		public void ShouldCanExecuteWork()
		{
			//Given
			const int canExecuteExpectedParam = 123;
			_canExecuteExpectedReturnValue = false;

			//When
			var canExecuteActualReturnValue = _target.CanExecute(canExecuteExpectedParam);
			
			//Then
			_canExecuteRiseCount.Should().Be(1, "CanExecute не вызвалось");
			_canExecuteActualParam.Should().Be(canExecuteExpectedParam, 
				"CanExecute вызвалось не с тем параметром");
			canExecuteActualReturnValue.Should().Be(_canExecuteExpectedReturnValue, 
				"Возвращаемое значение CanExecute не соответствует заданному");
		}

		/// <summary>
		/// CanExecute должен возвращать True если делегат не задан
		/// </summary>
		[Test]
		public void ShouldCanExecuteReturnTrueWhenCanExecuteFuncNull()
		{
			//Given
			//When
			var command = new DelegateCommand(p => { });

			//Then
			command.CanExecute("Test").Should().BeTrue("CanExecute должен возвращать True если не задан CanExecute Func");
		}

		/// <summary>
		/// Должен выполнять делегат Execute переданный при инициализации 
		/// </summary>
		[Test]
		public void ShouldExecuteWork()
		{
			//Given
			const int executeExpectedParam = 123;

			//When
			_target.Execute(executeExpectedParam);

			//Then
			_executeRiseCount.Should().Be(1, "Execute не вызвалось");
			_executeActualParam.Should().Be(executeExpectedParam,
				"Execute вызвалось не с тем параметром");
		}
	}
}
