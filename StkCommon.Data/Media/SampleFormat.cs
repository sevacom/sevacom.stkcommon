using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StkCommon.Data.Media
{
	/// <summary>
	/// Информация о формате протока сэмплов.
	/// </summary>
	public class SampleFormat
	{
		private const int MilliSecondInSecond = 1000;

		/// <summary>
		/// Пустой
		/// </summary>
		public static readonly SampleFormat Empty = new SampleFormat();

		private SampleFormat()
		{
			Channels = 0;
			SampleRate = 0;
		}

		public SampleFormat(int channels, int sampleRate = 0)
		{
			Channels = channels;
			SampleRate = sampleRate;
		}

		/// <summary>
		///  Количество каналов.
		/// </summary>
		public int Channels { get; private set; }

		/// <summary>
		/// Sample rate http://en.wikipedia.org/wiki/Sampling_rate
		/// </summary>
		public int SampleRate { get; private set; }

		/// <summary>
		/// Получить длительность в милисекундах по количеству семплов
		/// </summary>
		public long GetDuration(long samplesCount)
		{
			if (SampleRate == 0 || Channels == 0)
				return -1;
			return (long)(MilliSecondInSecond * samplesCount / (double)(SampleRate * Channels));
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Channels * 397) ^ SampleRate;
			}
		}
	}
}
