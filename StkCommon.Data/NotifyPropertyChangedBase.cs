using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace StkCommon.Data
{
	/// <summary>
	/// Реализация интерфейса INotifyPropertyChanged
	/// </summary>
	public abstract class NotifyPropertyChangedBase: INotifyPropertyChanged
	{
		/// <summary>
		/// Кэшировать или нет создаваемые PropertyChangedEventArgs, по умолчанию кэшировать
		/// </summary>
		protected readonly bool IsCacheEventArgs = true;

		private readonly Dictionary<string, PropertyChangedEventArgs> _eventArgsCache;
		private static readonly PropertyChangedEventArgs EmptyArgs = new PropertyChangedEventArgs(string.Empty);

		protected NotifyPropertyChangedBase()
		{
			_eventArgsCache = new Dictionary<string, PropertyChangedEventArgs>();
		}

		#region INotifyPropertyChanged Members

		/// <summary>
		/// Raised when a property on this object has a new value.
		/// </summary>
		public virtual event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises this object's PropertyChanged event for all properties
		/// </summary>
		protected virtual void OnPropertyChanged()
		{
			OnPropertyChanged(string.Empty);
		}
		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">The property that has a new value. If propertyName is Null of string.Empty, than all properties of a class will be updated.</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
#if DEBUG
			if (!string.IsNullOrEmpty(propertyName))
			{
				VerifyPropertyName(propertyName);
			}
#endif

			var handler = PropertyChanged; //-V3119
			if (handler != null)
			{
				var args = EmptyArgs;
				if (IsCacheEventArgs)
				{
					if (!string.IsNullOrEmpty(propertyName))
					{
						if (!_eventArgsCache.TryGetValue(propertyName, out args))
						{
							args = new PropertyChangedEventArgs(propertyName);
							_eventArgsCache.Add(propertyName, args);
						}
					}
				}
				else
				{
					args = new PropertyChangedEventArgs(propertyName);
				}

                OnPropertyChanged(args);
			}
		}

		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <param name="propertyExpression">The property that has a new value.</param>
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			if (PropertyChanged == null)
				return;
			OnPropertyChanged(GetPropertyName(propertyExpression));
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, args);
		}

		#endregion

		public void RisePropertyChanged(string propertyName)
		{
			if (PropertyChanged == null)
				return;
			OnPropertyChanged(propertyName);
		}

		#region Debugging Aides

		/// <summary>
		/// Warns the developer if this object does not have
		/// a public property with the specified name. This 
		/// method does not exist in a Release build.
		/// </summary>
		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public void VerifyPropertyName(string propertyName)
		{
			// This is special scenario, when we want to raise change event on all properties
			if (string.IsNullOrEmpty(propertyName))
				return;

			// Verify that the property name matches a real,  
			// public, instance property on this object.
			if (TypeDescriptor.GetProperties(this)[propertyName] == null)
			{
				string msg = "Invalid property name: " + propertyName;
				throw new ArgumentException(msg, propertyName);
			}
		}

		#endregion

		/// <summary>
		/// Получить строковое представление Property
		/// </summary>
		public static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
		{
			return PropertyName.For(propertyExpression);
		}
	}
}