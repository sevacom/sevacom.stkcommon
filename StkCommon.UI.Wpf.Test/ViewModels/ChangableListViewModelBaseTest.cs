using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.Data;
using StkCommon.UI.Wpf.ViewModels;

namespace StkCommon.UI.Wpf.Test.ViewModels
{
	[TestFixture]
	public class ChangableListViewModelBaseTest
	{
		private TestChangableListViewModel _target;
		private TestModel _expectedModel;

		[SetUp]
		public void SetUp()
		{
			_expectedModel = new TestModel();
			_target = new TestChangableListViewModel();
		}

		[Test]
		public void ShouldInitialConstruct()
		{
			//Given //When //Then
			_target.ItemsCollectionView.Should().NotBeNull();
			_target.ItemsCollectionView.Count.Should().Be(0);
		}

		[Test]
		public void ShouldFill()
		{
			//Given
			var models = new[] {new TestModel(), new TestModel()};

			//When
			_target.Fill(models);

			//Then
			_target.ItemsInternal.ShouldAllBeEquivalentTo(GetViewModels(_target.ItemsCollectionView));
			_target.ItemsInternal.Select(p => p.Model).ShouldAllBeEquivalentTo(models);
			_target.ItemsInternal.Should().OnlyContain(p => !p.IsAdd && !p.IsUpdated);
		}

		/// <summary>
		/// Должен вызвать функции ApplyFilterToCollectionView и ApplySortToCollectionView с isDeferRefresh true
		/// когда инициализировали коллекцию
		/// </summary>
		[Test]
		public void ShouldApplyFilterAndSortWithIsDeferRefreshTrueWhenInitializeCollectionView()
		{
			//Given
			_target.ApplyFilterToCollectionViewIsDeferRefresh = false;
			_target.ApplySortToCollectionViewIsDeferRefresh = false;
			
			//When
			_target.Fill(new TestModel[0]);

			//Then
			_target.ApplyFilterToCollectionViewIsDeferRefresh.Should().BeTrue();
			_target.ApplySortToCollectionViewIsDeferRefresh.Should().BeTrue();
		}

		[Test]
		public void ShouldFillWithNull()
		{
			//Given //When
			_target.Fill(null);

			//Then
			_target.ItemsInternal.ShouldAllBeEquivalentTo(GetViewModels(_target.ItemsCollectionView));
			_target.ItemsInternal.Select(p => p.Model).ShouldAllBeEquivalentTo(new TestModel[0]);
		}

		[Test]
		public void ShouldDeleteAndResetSelection()
		{
			//Given 
			var deletedModel = new TestModel();
			_target.Fill(new[] { _expectedModel, deletedModel});
			_target.SelectedItem = _target.ItemsInternal.Single(p => p.Model == deletedModel);

			//When
			_target.Delete(deletedModel);

			//Then
			_target.ItemsInternal.Should().OnlyContain(p => p.Model == _expectedModel);
			_target.SelectedItem.Should().BeNull();
		}

		[Test]
		public void ShouldAddIfNotExistWhenUpdate()
		{
			//Given 
			var addModel = new TestModel();
			_target.Fill(new[] { _expectedModel });

			//When
			_target.Update(addModel);

			//Then
			_target.ItemsInternal.Select(p => p.Model)
				.Should().ContainInOrder(addModel, _expectedModel);
			_target.ItemsInternal.Should().HaveCount(2);
			_target.ItemsInternal.Should().Contain(p => p.IsAdd && p.Model == addModel);
		}

		[Test]
		public void ShouldUpdateIfExistWhenUpdate()
		{
			//Given 
			var updateModel = new TestModel(_expectedModel.Id);
			_target.Fill(new[] { _expectedModel });

			//When
			_target.Update(updateModel);

			//Then
			_target.ItemsInternal.Should().OnlyContain(p => p.Model == updateModel && p.IsUpdated);
		}

		[Test]
		public void ShouldFilterFunctionApply()
		{
			//Given
			
			//When
			_target.FilterFunctionResult = false;
			_target.Fill(new[] { _expectedModel });

			//Then
			_target.ItemsCollectionView.Should().BeEmpty();
		}

		private static IEnumerable<TestChangableViewModel> GetViewModels(IEnumerable collectionView)
		{
			return collectionView.Cast<TestChangableViewModel>();
		}

		private class TestModel : IModelObject<TestModel>
		{
			private static int _id = 1;

			public TestModel()
			{
				Id = _id++;
			}

			public TestModel(int id)
			{
				Id = id;
			}

			public int Id { get; set; }

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
				return Id == other.Id;
			}
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
		}

		private class TestChangableListViewModel : ChangableListViewModelBase<TestModel, TestChangableViewModel>
		{
			public TestChangableListViewModel()
			{
				FilterFunctionResult = true;
			}
			
			public ObservableCollection<TestChangableViewModel> ItemsInternal
			{
				get { return Items; }
			}

			public bool FilterFunctionResult { get; set; }

			public bool ApplyFilterToCollectionViewIsDeferRefresh { get; set; }

			public bool ApplySortToCollectionViewIsDeferRefresh { get; set; }

			/// <summary>
			/// Создание TVModel из TModel
			/// </summary>
			/// <param name="model"></param>
			/// <param name="isAdd">true - добавление к существующим, false - первоначальное наполнение</param>
			/// <returns></returns>
			protected override TestChangableViewModel CreateViewModel(TestModel model, bool isAdd)
			{
				return new TestChangableViewModel(model, isAdd);
			}

			/// <summary>
			/// Фильтрация TVModel
			/// </summary>
			/// <param name="viewModel"></param>
			/// <returns></returns>
			protected override bool FilterFunction(TestChangableViewModel viewModel)
			{
				return FilterFunctionResult;
			}

			/// <summary>
			/// Устанавливает фильтрующую функцию, если SelectedItem не проходит фильтр то он сбрасывается в null
			/// </summary>
			/// <param name="isDeferRefresh">true - выполняется внутри DeferRefresh отложенные изменения, false - нет</param>
			protected override void ApplyFilterToCollectionView(bool isDeferRefresh = false)
			{
				base.ApplyFilterToCollectionView(isDeferRefresh);

				ApplyFilterToCollectionViewIsDeferRefresh = isDeferRefresh;
			}

			/// <summary>
			/// Устанавливает сортировку из ItemsSortDescriptions
			/// </summary>
			protected override void ApplySortToCollectionView(bool isDeferRefresh = false)
			{
				base.ApplySortToCollectionView(isDeferRefresh);
				ApplySortToCollectionViewIsDeferRefresh = isDeferRefresh;
			}
		}
	}
}
