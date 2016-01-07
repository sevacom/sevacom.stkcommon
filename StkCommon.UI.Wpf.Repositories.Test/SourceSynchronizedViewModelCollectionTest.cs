using System;
using System.Linq;
using System.Windows.Threading;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.Data.Repositories;
using StkCommon.UI.Wpf.Repositories.Test.TestHelpers;

namespace StkCommon.UI.Wpf.Repositories.Test
{
	[TestFixture]
	public class SourceSynchronizedViewModelCollectionTest
    {
		/// <summary>
		/// Должен вернуть такое же количество элементов, что и в модельном репозиториии
		/// </summary>
		[Test]
		public void ShouldReturnTheSameCountOfElementsAsSource()
		{
			// Given:
			var target = CreateSourceSynchronizedViewModelCollection();
			var modelRepository = new ModelRepository();
			
			// When:
			target.Source = modelRepository;
			target.ModelFilter = ConstFilters.AllElementsFilter;

			// Then:
			target.Count.Should().Be(modelRepository.Count(), "Неверное количество элементов в коллекции");
		}

		/// <summary>
		/// Должен в свойстве CollectionView вернуть такое же количество элементов, что и в модельном репозиториии
		/// </summary>
		[Test]
		public void ShouldCollectionViewReturnTheSameCountOfElementsAsSource()
		{
			// Given:
			var target = CreateSourceSynchronizedViewModelCollection();
			var modelRepository = new ModelRepository();

			// When:
			target.Source = modelRepository;
			target.ModelFilter = ConstFilters.AllElementsFilter;

			// Then:
			target.CollectionView.Cast<TestViewModel>().Count().Should().Be(modelRepository.Count(), "Неверное количество элементов в коллекции");
		}
		
		private static SourceSynchronizedViewModelCollection<TestViewModel, TestModel, int> CreateSourceSynchronizedViewModelCollection()
		{
			Func<TestModel, TestViewModel> factory = model =>
			{
				var vm = new TestViewModel(); vm.Parse(model); 
				return vm;
			};
			var target = new SourceSynchronizedViewModelCollection<TestViewModel, TestModel, int>(
				factory, vm => vm.ModelId, (model, vm) => vm.Parse(model), new DispatcherAdapter(Dispatcher.CurrentDispatcher));
			return target;
		}

		#region Nested Types

		private class TestViewModel : IModelReadOnlyWrapper<TestModel, int>
		{
			public string Caption { get; set; }
			public void Parse(TestModel model)
			{
				Caption = model.Caption;
				ModelId = model.Id;
			}

			public int ModelId { get; private set; }
		}

		#endregion
	}
}
