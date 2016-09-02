using System;
using System.Collections.Generic;
using System.Linq;
using StkCommon.Data;

namespace StkCommon.UI.Wpf.ViewModels
{
	/// <summary>
	/// Базовая вьюмодель для элементов, поддерживающих изменения
	/// </summary>
	public abstract class ChangableViewModelBase<T> : ViewModelBase where T : IModelObject<T>
	{
		private bool _isUpdated;
		private bool _isAdd;

		protected ChangableViewModelBase(T model)
		{
			if (model == null) throw new ArgumentNullException("model");
			Model = model;
		}

		protected ChangableViewModelBase(T model, bool isAdd):this(model)
		{
			_isAdd = isAdd;
		}

		public bool IsUpdated
		{
			get { return _isUpdated; }
			protected set
			{
				if (value == _isUpdated) 
					return;
				_isUpdated = value;
				OnPropertyChanged(() => IsUpdated);
			}
		}

		public bool IsAdd
		{
			get { return _isAdd; }
			protected set
			{
				if (value == _isAdd) 
					return;
				_isAdd = value;
				OnPropertyChanged(() => IsAdd);
			}
		}

		public T Model { get; protected set; }

		/// <summary>
		/// Сбросить изменения IsUpdate и IsAdd
		/// </summary>
		public void ResetChanges()
		{
			IsUpdated = false;
			IsAdd = false;
		}

		public void Update(T model)
		{
			if (model == null) throw new ArgumentNullException("model");

			Model = model;
			_isUpdated = true;
			UpdateInternal();
			OnPropertyChanged();
		}

		/// <summary>
		/// Обновление внутреннего состояния свойст, когда произошло обновление модели 
		/// После будет вызвано OnPropertyChanged()
		/// </summary>
		protected virtual void UpdateInternal()
		{
		}
	}

	public static class ChangableViewModelBaseExtensions
	{
		public static void ResetChanaged<TObj>(this IEnumerable<ChangableViewModelBase<TObj>> viewModels) 
			where TObj : IModelObject<TObj>
		{
			if (viewModels == null)
				return;
			var changedViewModels = viewModels.ToArray();
			foreach (var changedViewModel in changedViewModels)
			{
				changedViewModel.ResetChanges();
			}
		}
	}
}