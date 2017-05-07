using System;
using System.Linq.Expressions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace StkCommon.TestUtils
{
	public class AutoMockerTestsBase
	{
		/// <summary>
		/// Помечен атрибутом SetUp вызывается перед каждым тестом.
		/// </summary>
		[SetUp]
		public virtual void SetUp()
		{
            Mocker = new AutoMocker();
        }

		/// <summary>
		/// Помечен атрибутом TearDown вызывается после каждого теста.
		/// </summary>
	    [TearDown]
	    public virtual void TearDown()
	    {
	        
	    }

        protected AutoMocker Mocker { get; private set; }

		protected T CreateInstance<T>() where T : class
		{
			return Mocker.CreateInstance<T>();
		}

		protected T Get<T>() where T : class
		{
			try
			{
				return Mocker.Get<T>();
			}
			catch
			{
				//Если это не получилось, то пытаетмся создать для этого мок и получить от него объект
				return GetMock<T>().Object;
			}
		}

		protected Mock<T> GetMock<T>() where T : class
		{
			return Mocker.GetMock<T>();
		}

		public void Use<TService>(TService service)
		{
			Mocker.Use(service);
		}

		public void Use<TService>(Mock<TService> mockedService) where TService : class
		{
			Mocker.Use(mockedService);
		}

		public void Use<TService>(Expression<Func<TService, bool>> setup) where TService : class
		{
			Mocker.Use(setup);
		}
	}

	public class AutoMockerTestsBase<TTarget> : AutoMockerTestsBase where TTarget : class
	{
		private TTarget _target;

		/// <summary>
		/// Помечен атрибутом SetUp вызывается перед каждым тестом.
		/// </summary>
		public override void SetUp()
		{
            base.SetUp();
			_target = null;
		}

		/// <summary>
		/// Помечен атрибутом TearDown вызывается после каждого теста.
		/// </summary>
		public override void TearDown()
		{
            var disposable = _target as IDisposable;
			if (disposable != null)
				disposable.Dispose();
            base.TearDown();
		}

		protected TTarget Target
		{
			get
			{
				return _target ?? (_target = DirectCreateTarget());
			}
		}

		protected TTarget TargetAsNew
		{
			get
			{
				return DirectCreateTarget();
			}
		}

		/// <summary>
		/// Создание Target, можно переопределить если необходимо или использовать когда сама ссылка на Target не нужна
		/// </summary>
		/// <returns></returns>
		protected virtual TTarget DirectCreateTarget()
		{
			return CreateInstance<TTarget>();
		}
	}
}