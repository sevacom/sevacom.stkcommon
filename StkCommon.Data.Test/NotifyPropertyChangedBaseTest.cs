using System.ComponentModel;
using FluentAssertions;
using NUnit.Framework;

namespace StkCommon.Data.Test
{
	[TestFixture]
	public class NotifyPropertyChangedBaseTest
	{
		private NotifyPropertyChanged _target;
		private string _risedPropertyName;
		private int _risedCount;
		// ReSharper disable once UnusedAutoPropertyAccessor.Local
		private string NameForTest { get; set; }

		[SetUp]
		public void Setup()
		{
			_target = new NotifyPropertyChanged();
			_risedCount= 0;
			_risedPropertyName = null;
			_target.PropertyChanged += TargetOnPropertyChanged;
		}

		private void TargetOnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			++_risedCount;
			_risedPropertyName = args.PropertyName;
		}

		[TearDown]
		public void TearDown()
		{
			_target.PropertyChanged -= TargetOnPropertyChanged;
		}

		[Test]
		public void ShouldGetPropertyName()
		{
			NotifyPropertyChangedBase.GetPropertyName(() => NameForTest).Should().Be("NameForTest");
		}

		[Test]
		public void ShouldRisePropertyChanged()
		{
			//Given
			//When
			_target.RisePropertyChangedTestable();

			//Then
			_risedCount.Should().Be(1);
			_risedPropertyName.Should().Be("Property1");
		}

		[Test]
		public void ShouldOnPropertyChanged()
		{
			//Given
			//When
			_target.OnPropertyChangedTestable();

			//Then
			_risedCount.Should().Be(1);
			_risedPropertyName.Should().Be("Property1");
		}

		private class NotifyPropertyChanged : NotifyPropertyChangedBase
		{
			public string Property1 { get; set; }

			public void RisePropertyChangedTestable()
			{
				RisePropertyChanged(GetPropertyName(() => Property1));
			}

			public void OnPropertyChangedTestable()
			{
				OnPropertyChanged(() => Property1);
			}
		}
	}
}
