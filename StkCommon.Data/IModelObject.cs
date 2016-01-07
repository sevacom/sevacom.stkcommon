using System;
using System.ComponentModel;

namespace StkCommon.Data
{
	public interface IModelObject<T> : INotifyPropertyChanged, IEquatable<T>
	{
	}
}