using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Shutdown
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Starting up");
			var isShutingDown = false;
			var stop = false;
			var timeDelay = TimeSpan.FromSeconds(10);
			// GPIO 24 which is physical pin 18
			int offPin1 = 24;
			using var controller = new GpioController();

			controller.OpenPin(offPin1, PinMode.InputPullDown);

			using var source = new CancellationTokenSource();
			var cancelationToken = source.Token;

			void callback(object sender, PinValueChangedEventArgs args)
			{
				Console.WriteLine("PinChanged");
				if (args.ChangeType == PinEventTypes.Falling)
				{
					if (!isShutingDown)
					{
						Console.WriteLine("Shutdown Requested");
						isShutingDown = true;
						stop = true;
						source.Cancel();
					}
				}
			}

			Console.WriteLine("Register call back");
			controller.RegisterCallbackForPinValueChangedEvent(offPin1, PinEventTypes.Falling, callback);

			while(!stop)
			{
				Console.WriteLine($"Tick {DateTimeOffset.Now}");
				try
				{
					await Task.Delay(timeDelay, cancelationToken);
				}
				catch { Console.WriteLine("Cancelation Exception"); }
			}

			shutdown();

		}

		private static void shutdown()
		{
			Console.WriteLine("Shutting Down");
			var processStartInfo = new ProcessStartInfo("init", "0");
			processStartInfo.UseShellExecute = true;
			Process.Start(processStartInfo);
		}
	}
}
