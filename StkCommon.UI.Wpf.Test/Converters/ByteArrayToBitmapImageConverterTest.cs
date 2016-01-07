using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.UI.Wpf.Converters;
using StkCommon.UI.Wpf.Test.Properties;

namespace StkCommon.UI.Wpf.Test.Converters
{
	[TestFixture]
	public class ByteArrayToBitmapImageConverterTest
	{
		private ByteArrayToBitmapImageConverter _target;

		[SetUp]
		public void SetUpTest()
		{
			_target = new ByteArrayToBitmapImageConverter();
		}

		/// <summary>
		/// должен конвертировать массив байт в BitmapImage
		/// </summary>
		[TestCaseSource("Bitmaps")]
		public void ShouldConvertToBitmapImage(Bitmap bitmap, ImageFormat format)
		{
			//Given
			var bytArray = ReadFully(bitmap, format);

			//When
			var actualValue = _target.Convert(bytArray, typeof(BitmapImage), null, CultureInfo.InvariantCulture);

			//Then
			actualValue.Should().BeOfType<BitmapImage>("Конвертация в BitmapImage не произошла");

		}

		#region Private methods

		private static byte[] ReadFully(Image bitmap, ImageFormat format)
		{
			using (var ms = new MemoryStream())
			{
				bitmap.Save(ms, format);
				return ms.ToArray();
			}
		}

		private static readonly object[] Bitmaps =
			{
				new object[] {Resources._1, ImageFormat.Bmp},
				new object[] {Resources._2, ImageFormat.Png}
			};

		#endregion

	}
}