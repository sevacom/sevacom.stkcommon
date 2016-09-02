using System.ComponentModel;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.Data;
using StkCommon.UI.Wpf.ViewModels;

namespace StkCommon.UI.Wpf.Test.ViewModels
{
	[TestFixture]
	public class ChangableViewModelBaseTest
	{
		private TestChangableViewModel _target;
		private TestModel _expectedModel;

		[SetUp]
		public void Setup()
		{
			_expectedModel = new TestModel();
			_target = new TestChangableViewModel(_expectedModel);
		}

		[Test]
		public void ShouldConstruct()
		{
			//Given //When
			var target = new TestChangableViewModel(_expectedModel, true);

			//Then
			target.Model.Should().Be(_expectedModel);
			target.IsAdd.Should().BeTrue();
		}

		[Test]
		public void ShouldResetChanges()
		{
			//Given
			_target.SetAddUpdateTrue();

			//When
			_target.ResetChanges();

			//Then
			_target.IsAdd.Should().BeFalse();
			_target.IsUpdated.Should().BeFalse();
		}

		[Test]
		public void ShouldUpdate()
		{
			//Given 
			var updatedModel = new TestModel();
			var actualEmptyPropertyChangedRiseCount = 0;
			_target.PropertyChanged += (sender, args) =>
			{
				if (string.IsNullOrEmpty(args.PropertyName))
					actualEmptyPropertyChangedRiseCount++;
			};

			//When
			_target.Update(updatedModel);

			//Then
			_target.Model.Should().Be(updatedModel);
			_target.IsUpdated.Should().BeTrue();
			_target.UpdateInternalCallCount.Should().Be(1);
			actualEmptyPropertyChangedRiseCount.Should().Be(1);
		}

		private class TestChangableViewModel : ChangableViewModelBase<TestModel>
		{
			public TestChangableViewModel(TestModel model) : base(model)
			{
;
			}

			public TestChangableViewModel(TestModel model, bool isAdd) : base(model, isAdd)
			{
			}

			public void SetAddUpdateTrue()
			{
				IsAdd = IsUpdated = true;
			}

			/// <summary>
			/// Обновление внутреннего состояния свойст, когда произошло обновление модели 
			/// После будет вызвано OnPropertyChanged()
			/// </summary>
			protected override void UpdateInternal()
			{
				UpdateInternalCallCount++;
			}

			public int UpdateInternalCallCount { get; private set; }
		}

		private class TestModel : IModelObject<TestModel>
		{
			public event PropertyChangedEventHandler PropertyChanged;

			/// <summary>
			/// Indicates whether the current object is equal to another object of the same type.
			/// </summary>
			/// <returns>
			/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
			/// </returns>
			/// <param name="other">An object to compare with this object.</param>
			public bool Equals(TestModel other)
			{
				return ReferenceEquals(this, other);
			}
		}
	}
}
