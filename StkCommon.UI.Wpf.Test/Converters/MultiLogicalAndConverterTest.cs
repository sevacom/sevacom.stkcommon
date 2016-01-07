using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.UI.Wpf.Converters;

namespace StkCommon.UI.Wpf.Test.Converters
{
	[TestFixture]
	public class MultiLogicalAndConverterTest
	{
		/// <summary>
		/// Должен правильно обработать различные варианты преобразуемого значения value
		/// </summary>
		[TestCaseSource("ToBeConverted")]
		public void ShouldCorrectlyConvertValueToVisibility(object [] values, MultiLogicalAndConverter.LogicalOperationType operationType,  bool expected)
		{
			// Given
			var target = new MultiLogicalAndConverter
						{
							LogicalOperation = operationType
						};

			// When
			var result = target.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

			// Then
			result.Should().Be(expected, "Не было возвращено значение " + expected);
		}

		/// <summary>
		/// Если тип, к которому надо преобразовать, не bool, то должен кинуть NotSupportedException
		/// </summary>
		[Test]
		public void ShouldThrowExceptionIfInvalidTargetType()
		{
			// Given
			var testedType = typeof (int);
			var target = new MultiLogicalAndConverter();

			// When
			// Then
			Assert.Throws<NotSupportedException>(() => target.Convert(null, testedType, null, CultureInfo.InvariantCulture));
		}

		public static readonly object ToBeConverted = new object[]
											{
												new object[]
												{
													new object[] {true, true, true}, MultiLogicalAndConverter.LogicalOperationType.And,  true
												},
												new object[]
												{
													new object[] {true, false}, MultiLogicalAndConverter.LogicalOperationType.And, false
												},

												new object[]
												{
													new object[] {1, (byte) 5, 1.1}, MultiLogicalAndConverter.LogicalOperationType.And, true
												},
												new object[]
												{
													new[] {"abc", new object(), DateTime.Now}, MultiLogicalAndConverter.LogicalOperationType.And, true
												},
												new object[]
												{
													new[] {new object(), null}, MultiLogicalAndConverter.LogicalOperationType.And, false
												},
												// Or
												new object[]
												{
													new object[] {true, true, true}, MultiLogicalAndConverter.LogicalOperationType.Or,  true
												},
												new object[]
												{
													new object[] {true, false}, MultiLogicalAndConverter.LogicalOperationType.Or, true
												},
												new object[]
												{
													new object[] {1, (byte) 5, 1.1}, MultiLogicalAndConverter.LogicalOperationType.Or, true
												},
												new object[]
												{
													new[] {"abc", new object()}, MultiLogicalAndConverter.LogicalOperationType.Or, true
												},
												new object[]
												{
													new[] {new object(), null}, MultiLogicalAndConverter.LogicalOperationType.Or, true
												},
												new object[]
												{
													new[] {0, (object)null}, MultiLogicalAndConverter.LogicalOperationType.Or, false
												}

											};
	}
}
