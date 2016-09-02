using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Markup;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// A converter that accepts <see cref="SwitchConverterCase"/>s and converts them to the 
	/// Then property of the case.
	//<converters:SwitchConverter x:Key="SeanceTypeToImageConverter" Default="{StaticResource UnknownImage}">
	//	<converters:SwitchConverterCase When="1" Then="{StaticResource MailIcon}" />
	//	<converters:SwitchConverterCase When="4" Then="{StaticResource HttpIcon}" />
	//	<converters:SwitchConverterCase When="100" Then="{StaticResource OkIcon}" />
	//	<converters:SwitchConverterCase When="101" Then="{StaticResource VkIcon}" />
	//</converters:SwitchConverter>
	/// </summary>
	[ContentProperty("Cases")]
	public class SwitchConverter : IValueConverter
	{
		public SwitchConverter()
		{
			// Create the cases array.
			Cases = new List<SwitchConverterCase>();
		}

		/// <summary>
		/// Gets or sets an array of <see cref="SwitchConverterCase"/>s that this converter can use to produde values from.
		/// </summary>
		public List<SwitchConverterCase> Cases { get; set; }

		public object Default { get; set; }

		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// This will be the results of the operation.
			var results = Default;

			// I'm only willing to convert SwitchConverterCases in this converter and no nulls!
			if (value == null) return results;

			// I need to find out if the case that matches this value actually exists in this converters cases collection.
			if (Cases != null && Cases.Count > 0)
				foreach (var targetCase in Cases)
				{
					// Check to see if the value is the cases When parameter.
					if (value == targetCase || value.ToString().ToUpper() == targetCase.When.ToUpper())
					{
						// We've got what we want, the results can now be set to the Then property
						// of the case we're on.
						results = targetCase.Then;

						// All done, get out of the loop.
						break;
					}
				}

			// return the results.
			return results;
		}

		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

	/// <summary>
	/// Represents a case for a switch converter.
	/// </summary>
	[ContentProperty("Then")]
	public class SwitchConverterCase
	{
		public SwitchConverterCase()
		{
		}

		/// <summary>
		/// Gets or sets the condition of the case.
		/// </summary>
		public string When { get; set; }

		/// <summary>
		/// Gets or sets the results of this case when run through a <see cref="SwitchConverter"/>
		/// </summary>
		public object Then { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SwitchConverterCase"/> class.
		/// </summary>
		/// <param name="when">The condition of the case.</param>
		/// <param name="then">The results of this case when run through a <see cref="SwitchConverter"/>.</param>
		public SwitchConverterCase(string when, object then)
		{
			// Hook up the instances.
			Then = then;
			When = when;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("When={0}; Then={1}", When, Then);
		}
	}
}
