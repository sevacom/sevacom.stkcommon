using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// Конвертер, преобразование массива байт в BitmapImage
	/// </summary>
	[ValueConversion(typeof(byte[]), typeof(ImageSource))]
	public class ByteArrayToBitmapImageConverter : ValueConverterBase
	{
		#region Implementation of IValueConverter

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var byteArrayImage = value as byte[];

			if (byteArrayImage != null && byteArrayImage.Length > 0)
			{
				var image = new BitmapImage();
				using (var mem = new MemoryStream(byteArrayImage))
				{
					mem.Position = 0;
					image.BeginInit();
					image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
					image.CacheOption = BitmapCacheOption.OnLoad;
					image.UriSource = null;
					image.StreamSource = mem;
					image.EndInit();
				}
				image.Freeze();
				return image;
			}

			return null;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}

		#endregion
	}
}
