using System;
using System.ComponentModel;

namespace StkCommon.Data
{
	/// <summary>
	/// Базовый интерфейс модельных объектов, поддерживающих биндинг
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IModelObject<T> : INotifyPropertyChanged, IEquatable<T>
	{
	}
}