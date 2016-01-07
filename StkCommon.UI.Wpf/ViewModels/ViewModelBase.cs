using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Windows;
using StkCommon.Data;

namespace StkCommon.UI.Wpf.ViewModels
{
	/// <summary>
	/// Base class for all ViewModel classes.
	/// It provides support for property change notifications 
	/// </summary>
	public abstract class ViewModelBase : NotifyPropertyChangedBase
	{
		#region Design time

		private static bool? _isInDesignMode;

		/// <summary>
		/// Gets a value indicating whether the control is in design mode (running in Blend
		/// or Visual Studio).
		/// </summary>
		public static bool IsInDesignModeStatic
		{
			get
			{
				if (!_isInDesignMode.HasValue)
				{
#if SILVERLIGHT
					_isInDesignMode = DesignerProperties.IsInDesignTool;
#else
					var prop = DesignerProperties.IsInDesignModeProperty;
					_isInDesignMode = (bool)DependencyPropertyDescriptor
						.FromProperty(prop, typeof(FrameworkElement))
						.Metadata.DefaultValue;
#endif
				}

				return _isInDesignMode.Value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the control is in design mode (running under Blend
		/// or Visual Studio).
		/// </summary>
		[SuppressMessage(
			"Microsoft.Performance",
			"CA1822:MarkMembersAsStatic",
			Justification = "Non static member needed for data binding")]
		public bool IsInDesignMode
		{
			get
			{
				return IsInDesignModeStatic;
			}
		}

		#endregion
	}

	public static class ViewModelBaseExtensions
	{
		/// <summary>
		/// Получить строковое представление Property
		/// </summary>
		public static string GetPropertyName<T, TProperty>(this T owner, Expression<Func<T, TProperty>> expression)
		{
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null)
			{
				var unaryExpression = expression.Body as UnaryExpression;
				if (unaryExpression != null)
				{
					memberExpression = unaryExpression.Operand as MemberExpression;

					if (memberExpression == null)
						throw new ArgumentNullException("expression");
				}
				else
					throw new ArgumentNullException("expression");
			}

			return memberExpression.Member.Name;
		}
	}
}
