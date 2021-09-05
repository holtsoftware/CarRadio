using System;
using System.Device.Gpio;
using System.Diagnostics;
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

			controller.RegisterCallbackForPinValueChangedEvent(offPin1, PinEventTypes.Falling, (sender, args) =>
			{
				Console.WriteLine("PinChanged");
				if(args.ChangeType == PinEventTypes.Falling)
				{
					if (!isShutingDown)
					{
						isShutingDown = true;
						Console.WriteLine("Shutting Down");
						var processStartInfo = new ProcessStartInfo("shutdown", "-h now");
						var process = Process.Start(processStartInfo);

						process.WaitForExit();
						stop = true;
					}
				}
			});

			while(!stop)
			{
				Console.WriteLine($"Tick {DateTimeOffset.Now}");
				await Task.Delay(timeDelay);
			}
		}
	}
}
