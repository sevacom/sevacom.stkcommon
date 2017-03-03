using System;
using System.Windows;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using StkCommon.UI.Wpf.Exceptions;
using StkCommon.UI.Wpf.Model;
using StkCommon.UI.Wpf.Properties;
using StkCommon.UI.Wpf.ViewModels;
using StkCommon.UI.Wpf.Views;

namespace StkCommon.UI.Wpf.Test.ViewModels
{
	[TestFixture]
	public class AuthorizationViewModelBaseTest
	{
		private TestAuthorizationViewModel _target;
		private Mock<IShowDialogAgent> _agentMock;

		[SetUp]
		public void SetUpTest()
		{
			_agentMock = new Mock<IShowDialogAgent>();		
			_target = new TestAuthorizationViewModel(AuthorizationMode.LoginPassword, _agentMock.Object);
		}

		/// <summary>
		/// Должен вызывать метод Authorize при задании режима, заполнении соответствующих полей 
		/// и нажатии на кнопку OK.
		/// </summary>
		[TestCase("1", "2", null, null, AuthorizationMode.LoginPassword, 1)]
		[TestCase("1", "2", "3", null, AuthorizationMode.Server, 1)]
		[TestCase("1", "2", null, "4", AuthorizationMode.Database, 1)]
		[TestCase("1", "2", "3", "4", AuthorizationMode.ServerDatabase, 1)]
		public void ShouldRiseAuthorizeWhenExecuteOkCommand(string userName, string password, string server, string dataBase, 
			AuthorizationMode mode, int expectedRiseCount )
		{
			//Given
			_target.UserName = userName;
			_target.Password = password;
			_target.Server = server;
			_target.Database = dataBase;
			_target.SetMode(mode);

			//When
			_target.OkCommand.Execute(null);

			//Then
			_target.AuthorizeRiseCount.Should().Be(expectedRiseCount, "Метод Authorize не вызвался");
		}
		
		/// <summary>
		/// В случае пользовательского исключения должен подняться диалог с сообщением о пользовательской ошибке, при этом
		/// окно авторизации не закрывается (DialoResult = null) 
		/// </summary>
		[Test]
		public void ShouldShowDialogWhenRaiseUserExceptionInExecuteOkCommand()
		{
			//Given
			_target.UserName = "user";
			_target.Password = "123";
			var exception = new UserMessageException("Неправильно введен пароль.");
			_target.SetRaiseException(exception);
			
			//When
			_target.OkCommand.Execute(null);
			
			//Then
			_agentMock.Verify(a => a.ShowMessageDialog(exception.Message, It.IsAny<string>(), 
				MessageBoxButton.OK, MessageBoxImage.Error), Times.Once(), "Диалог вызвался c непользовательской ошибкой" );
			
			Assert.IsNull(_target.DialogResult, "DialogResult не равен null");
		}

		/// <summary>
		/// В случае непредвиденного исключения должен подняться диалог с сообщением о необработанной ошибке, при этом
		/// окно авторизации закрывается (DialoResult = false) 
		/// </summary>
		[Test]
		public void ShouldShowDialogWhenRaiseExceptionInExecuteOkCommand()
		{
			//Given	
			_target.UserName = "user";
			_target.Password = "123";
			var exception = new Exception("Не связи с системой.");
			_target.SetRaiseException(exception);
			
			//When
			_target.OkCommand.Execute(null);

			//Then
			_agentMock.Verify(a => a.ShowMessageDialog(Resources.AuthorizationWindow_FaultExceptionMessage + "\r\n" + exception.Message,
                It.IsAny<string>(), MessageBoxButton.OK, MessageBoxImage.Error), Times.Once(), "Диалог вызвался c обработанной ошибкой");

			_target.DialogResult.Should().BeFalse("DialogResult должен быть равен false");
		}

		/// <summary>
		/// Должен тихо закрыть диалог авторизации с результатом DialogResult false, если метод Authorize вернул false
		/// </summary>
		[Test]
		public void ShouldSilentCloseDialogWhenAuthorizationReturnFalse()
		{
			//Given
			_target.UserName = "user";
			_target.Password = "123";
			_target.SetMode(AuthorizationMode.LoginPassword);
			_target.ExpectedAuthorizeMethodReturnValue = false;

			//When
			_target.OkCommand.Execute(null);

			//Then
			_target.DialogResult.Should().BeFalse("DialogResult должен быть равен false");
			_agentMock.Verify(p => p.ShowMessageDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>()), 
				Times.Never(), "Диалоги не должны показываться");
		}

		/// <summary>
		/// Должен вызвать метод OnServersDropDownChanged когда поменялся IsServersDropDownOpen
		/// </summary>
		[Test]
		public void ShouldCallOnServersDropDownChangedWhenIsServersDropDownOpenChanged()
		{
			//Given //When
			_target.IsServersDropDownOpen = true;

			//Then
			_target.OnServersDropDownChangedRiseCount.Should().Be(1, "Метод OnServersDropDownChanged не сработал при изменении IsServersDropDownOpen");
		}

		/// <summary>
		/// Должен вызвать метод OnDataBasesDropDownChanged когда поменялся IsDataBaseDropDownOpen
		/// </summary>
		[Test]
		public void ShouldCallOnDataBasesDropDownChangedWhenIsDataBaseDropDownOpenChanged()
		{
			//Given //When
			_target.IsDataBaseDropDownOpen = true;

			//Then
			_target.OnDataBasesDropDownChangedRiseCount.Should().Be(1, "Метод OnDataBasesDropDownChanged не сработал при изменении IsServersDropDownOpen");
		}

		private class TestAuthorizationViewModel : AuthorizationViewModelBase
		{
			private Exception _exсeption;

			public int AuthorizeRiseCount { get; private set; }

			public int OnServersDropDownChangedRiseCount { get; private set; }

			public int OnDataBasesDropDownChangedRiseCount { get; private set; }

			public bool ExpectedAuthorizeMethodReturnValue { get; set; }

			public TestAuthorizationViewModel(AuthorizationMode mode, IShowDialogAgent agent)
				: base(mode, agent, false)
			{
				ExpectedAuthorizeMethodReturnValue = true;
			}

			public void SetMode(AuthorizationMode mode)
			{
				Mode = mode;
			}

			public void SetRaiseException(Exception exception)
			{
				_exсeption = exception;
			}

			protected override bool Authorize()
			{
				if (_exсeption != null)
				{
					throw _exсeption;
				}
					
				AuthorizeRiseCount++;
				return ExpectedAuthorizeMethodReturnValue;
			}

			protected override void OnServersDropDownChanged(bool isDropDown)
			{
				base.OnServersDropDownChanged(isDropDown);
				OnServersDropDownChangedRiseCount++;
			}

			protected override void OnDataBasesDropDownChanged(bool isDropDown)
			{
				base.OnDataBasesDropDownChanged(isDropDown);
				OnDataBasesDropDownChangedRiseCount++;
			}
		}	
	}

}
