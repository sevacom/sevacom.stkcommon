using System;
using StkCommon.Data.Repositories;
using StkCommon.UI.Wpf.ViewModels;

namespace StkCommon.UI.Wpf.Repositories
{
	/// <summary>
	/// Стандатный механизм оповещений для IChangableListViewModel
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	public class ChangableListViewModelNotifier<TModel> : AsyncRepositorySubscriberBase<TModel>
	{
		public ChangableListViewModelNotifier(IChangableListViewModel<TModel> changableListViewModel, IDispatcher dispatcher)
			: base(p => SyncAction(dispatcher, p), changableListViewModel.Update, changableListViewModel.Update, changableListViewModel.Delete)
		{
		}

		private static void SyncAction(IDispatcher dispatcher, Action action)
		{
			if (dispatcher.CheckAccess())
				action();
			else
				dispatcher.BeginInvoke(action);
		}
	}
}