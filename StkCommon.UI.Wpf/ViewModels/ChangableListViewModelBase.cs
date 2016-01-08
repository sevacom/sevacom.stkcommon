using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using StkCommon.Data;

namespace StkCommon.UI.Wpf.ViewModels
{
	//TODO: тесты 
	/// <summary>
	/// Список изменяемых объектов с поддержкой фильтрации
	/// </summary>
	public abstract class ChangableListViewModelBase<TModel, TVModel> : ViewModelBase
		where TModel : IModelObject<TModel>
		where TVModel : ChangableViewModelBase<TModel>
	{
		private ObservableCollection<TVModel> _items;
		private ListCollectionView _itemsCollectionView;
		private TVModel _selectedItem;

		protected ChangableListViewModelBase()
		{
			_items = new ObservableCollection<TVModel>();
		}

		/// <summary>
		/// Представление списка заданий для фильтрации, сортировки и т.д.
		/// </summary>
		public virtual ListCollectionView ItemsCollectionView
		{
			get { return _itemsCollectionView; }
			private set
			{
				_itemsCollectionView = value;

				OnCollectionViewChanged();
				OnPropertyChanged(() => ItemsCollectionView);
			}
		}

		/// <summary>
		/// Список TVModel на основе которого строится ItemsCollectionView
		/// </summary>
		protected virtual ObservableCollection<TVModel> Items
		{
			get { return _items; }
			set
			{
				if (_items == value)
					return;
				_items = value;
				OnPropertyChanged(() => Items);
				ItemsCollectionView = GetCollectionView(_items);
			}
		}

		protected virtual SortDescription[] ItemsSortDescriptions
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Выбранный элемент
		/// </summary>
		public virtual TVModel SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				if (Equals(_selectedItem, value))
				{
					return;
				}
				_selectedItem = value;
				OnPropertyChanged(() => SelectedItem);
			}
		}

		/// <summary>
		/// Заполнить
		/// </summary>
		public virtual void Fill(IEnumerable<TModel> models)
		{
			models = models ?? new TModel[0];
			Items = new ObservableCollection<TVModel>(CreateViewModels(models, false));
		}

		/// <summary>
		/// Удалить 
		/// </summary>
		public virtual void Delete(TModel model)
		{
			if (model == null) throw new ArgumentNullException("model");

			var delViewModel = Items.FirstOrDefault(t => t.Model.Equals(model));
			if (delViewModel == null)
				return;
			OnDeleteViewModel(delViewModel);
		}

		/// <summary>
		/// Изменить или добавить 
		/// </summary>
		public virtual void Update(TModel model)
		{
			if (model == null) throw new ArgumentNullException("model");

			var existViewModel = Items.FirstOrDefault(t => t.Model.Equals(model));
			if (existViewModel == null)
			{
				OnAddViewModel(model);
			}
			else
			{
				OnUpdateViewModel(existViewModel, model);
			}
		}

		/// <summary>
		/// Вызвать для каждой TVModel из Items метод ResetChanges
		/// </summary>
		public void ResetChanges()
		{
			Items.ResetChanaged();
		}

		/// <summary>
		/// Создание TVModel из TModel
		/// </summary>
		/// <param name="model"></param>
		/// <param name="isAdd">true - добавление к существующим, false - первоначальное наполнение</param>
		/// <returns></returns>
		protected abstract TVModel CreateViewModel(TModel model, bool isAdd);
		
		protected virtual IEnumerable<TVModel> CreateViewModels(IEnumerable<TModel> models, bool isAdd)
		{
			return models.Select(p => CreateViewModel(p, isAdd));
		}

		/// <summary>
		/// Добавление нового элемента
		/// </summary>
		protected virtual void OnAddViewModel(TModel model)
		{
			var viewModel = CreateViewModel(model, true);
			Items.Insert(0, viewModel);
		}

		/// <summary>
		/// Обновление существующей ViewModel
		/// </summary>
		/// <param name="existViewModel"></param>
		/// <param name="model"></param>
		protected virtual void OnUpdateViewModel(TVModel existViewModel, TModel model)
		{
			existViewModel.Update(model);
			RefreshCollectionViewElement(existViewModel);
		}

		/// <summary>
		/// Удаление существующей ViewModel
		/// </summary>
		/// <param name="delViewModel"></param>
		protected virtual void OnDeleteViewModel(TVModel delViewModel)
		{
			Items.Remove(delViewModel);

			if (Equals(delViewModel, SelectedItem))
			{
				SelectedItem = null;
			}
		}

		/// <summary>
		/// Устанавливает фильтрующую функцию, если SelectedItem не проходит фильтр то он сбрасывается в null
		/// </summary>
		protected virtual void ApplyFilterFuncToCollectionView()
		{
			if (SelectedItem != null && !FilterFunction(SelectedItem))
				SelectedItem = null;

			if (ItemsCollectionView != null)
			{
				ItemsCollectionView.Filter = FilterFunction;
			}
		}

		/// <summary>
		/// Устанавливает сортировку из ItemsSortDescriptions
		/// </summary>
		protected virtual void ApplySortToCollectionView()
		{
			if (ItemsCollectionView == null || ItemsSortDescriptions == null)
				return;

			foreach (var sortDescription in ItemsSortDescriptions)
			{
				ItemsCollectionView.SortDescriptions.Add(sortDescription);
			}
		}

		/// <summary>
		/// Изменилась ItemsCollectionView, выполняются настройки фильтрации и сортировки
		/// </summary>
		protected virtual void OnCollectionViewChanged()
		{
			using (ItemsCollectionView.DeferRefresh())
			{
				ApplyFilterFuncToCollectionView();
				ApplySortToCollectionView();
			}
		}

		/// <summary>
		/// Фильтрация TVModel
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		protected virtual bool FilterFunction(TVModel viewModel)
		{
			return true;
		}

		protected virtual ListCollectionView GetCollectionView(IEnumerable collection)
		{
			var listCollectionView = CollectionViewSource.GetDefaultView(collection) as ListCollectionView;

			if (listCollectionView == null)
				throw new NotSupportedException("Список не поддерживает ListCollectionView");

			return listCollectionView;
		}

		protected void RefreshCollectionViewElement<T>(T item)
		{
			if (ItemsCollectionView != null)
			{
				ItemsCollectionView.EditItem(item);
				ItemsCollectionView.CommitEdit();
			}
		}

		private bool FilterFunction(object o)
		{
			var viewModel = o as TVModel;
			if (viewModel == null)
				return false;

			return FilterFunction(viewModel);
		}

	}
}