using System;
using System.Windows.Threading;

namespace StkCommon.UI.Wpf.Common
{
	/// <summary>
	/// Таймер выполняющий действия в том же потоке из которого создали
	/// </summary>
	public interface IUiTimer : IDisposable
	{
		/// <summary>
		/// Gets or sets a value that indicates whether the timer is running.
		/// </summary>
		/// <returns>
		/// true if the timer is enabled; otherwise, false.  The default is false.
		/// </returns>
		bool IsEnabled { get; set; }

		/// <summary>
		/// Gets or sets the period of time between timer ticks.
		/// </summary>
		/// <returns>
		/// The period of time between ticks. The default is 00:00:00.
		/// </returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// Interval is less than 0 or greater than <see cref="F:System.Int32.MaxValue" /> milliseconds.
		/// </exception>
		TimeSpan Interval { get; set; }

		/// <summary>
		/// Occurs when the timer interval has elapsed.
		/// </summary>
		event EventHandler Tick;

		/// <summary>
		/// Starts the timer
		/// </summary>
		/// <param name="startOnce">Выполнить Tick один раз</param>
		void Start(bool startOnce = false);

		/// <summary>
		/// Stops the timer
		/// </summary>
		void Stop();

		/// <summary>
		/// Start immediately
		/// </summary>
		void StartOnceImmediately();
	}

	/// <summary>
	/// реализация таймера через DispatcherTimer
	/// </summary>
	public class UiTimer : IUiTimer
	{
		private DispatcherTimer _dispatcherTimer;
		private bool _isDisposed;
		private bool _isStartOnce;

		#region Public Properties

		public bool IsEnabled
		{
			get
			{
				if (_isDisposed)
					throw new ObjectDisposedException("_dispatcherTimer");

				return _dispatcherTimer.IsEnabled;
			}
			set
			{
				if (_isDisposed)
					throw new ObjectDisposedException("_dispatcherTimer");

				_dispatcherTimer.IsEnabled = value;
			}
		}

		public TimeSpan Interval
		{
			get
			{
				if (_isDisposed)
					throw new ObjectDisposedException("_dispatcherTimer");

				return _dispatcherTimer.Interval;
			}
			set
			{
				if (_isDisposed)
					throw new ObjectDisposedException("_dispatcherTimer");

				_dispatcherTimer.Interval = value;
			}
		}

		#endregion

		public UiTimer()
		{
			_dispatcherTimer = new DispatcherTimer();
			_dispatcherTimer.Tick += DispatcherTimerOnTick;
		}

		public event EventHandler Tick;

		public void Dispose()
		{
			if (_isDisposed)
				return;

			_dispatcherTimer.Tick -= DispatcherTimerOnTick;
			_dispatcherTimer.Stop();
			_dispatcherTimer = null;

			_isDisposed = true;
		}

		public void Start(bool startOnce = false)
		{
			if (_isDisposed)
				throw new ObjectDisposedException("_dispatcherTimer");
			_dispatcherTimer.Stop();
			_isStartOnce = startOnce;
			_dispatcherTimer.Start();
		}

		public void Stop()
		{
			if (_isDisposed)
				throw new ObjectDisposedException("_dispatcherTimer");
			_dispatcherTimer.Stop();
			_isStartOnce = false;
		}

		public void StartOnceImmediately()
		{
			if (_isDisposed)
				throw new ObjectDisposedException("_dispatcherTimer");
			_dispatcherTimer.Stop();
			OnTick();
		}

		protected virtual void OnTick()
		{
			var handler = Tick;
			if (handler != null) handler(this, EventArgs.Empty);
		}

		private void DispatcherTimerOnTick(object sender, EventArgs eventArgs)
		{
			if (_isDisposed)
				return;

			if (_isStartOnce)
			{
				Stop();
			}

			OnTick();
		}
	}
}
