using System;
using System.Device.I2c;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Sannel.Keyboard;

namespace KeyboardListener
{
	class Program
	{
		public const int AVARAGE_SIZE = 20;
		private static List<ushort> pin1 = new List<ushort>(AVARAGE_SIZE);
		private static List<ushort> pin2 = new List<ushort>(AVARAGE_SIZE);

		static void addValue(byte pin, ushort value)
		{
			switch(pin)
			{
				case 1:
					pin1.Add(value);
					return;

				case 2:
					pin2.Add(value);
					return;
			}
		}

		static async Task Main(string[] args)
		{
			using var keyboard = Sannel.Keyboard.Drivers.Uhid.UhidDriver.Create("steering-wheel");

			byte[] buffer = new byte[4];
			using var bus = System.Device.I2c.I2cBus.Create(1);
			using var device = bus.CreateDevice(8);
			Console.Clear();

			while(true)
			{
				device.Read(buffer);
				var (pin, value) = getInfo(buffer[0], buffer[1]);
				addValue(pin, value);
				(pin, value) = getInfo(buffer[2], buffer[3]);
				addValue(pin, value);

				if(pin1.Count >=AVARAGE_SIZE || pin2.Count >= AVARAGE_SIZE)
				{
					var pin1Avarage = getAvarage(pin1);
					var pin2Avarage = getAvarage(pin2);
					Console.WriteLine($"1:{pin1Avarage:0000} 2:{pin2Avarage:0000}");
					Console.SetCursorPosition(0, 0);
					pin1.Clear();
					pin2.Clear();
					bool keyPressed = false;
					if(pin2Avarage >= 609 && pin2Avarage <= 616)
					{
						// Volume Up
						keyboard.PressKey(SpecialKeys.F8);
						keyPressed = true;
					}
					else if(pin2Avarage >= 65 && pin2Avarage <= 75)
					{
						// Previous
						keyboard.PressKey('p');
						keyPressed = true;
					}
					else if(pin2Avarage >= 1011)
					{
						// mode
						keyboard.PressKey('v');
						keyPressed = true;
					}
					else if(pin1Avarage >= 609 && pin1Avarage <= 616)
					{
						// Volume Down
						keyboard.PressKey(SpecialKeys.F7);
						keyPressed = true;
					}
					else if(pin1Avarage >= 65 && pin1Avarage <= 75)
					{
						// Next
						keyboard.PressKey('n');
						keyPressed = true;
					}

					if (keyPressed)
					{
						await Task.Delay(250);
					}
				}
			}
		}

		private static ushort getAvarage(List<ushort> list)
		{
			var avarage = list.Average(i => i);
			var min = list.Min();
			var max = list.Max();

			if(min < avarage - 50)
			{
				ushort min2 = (ushort)(min + 50);
				if(list.Where(i => i >= min && i <= min + 10).Count() <= 5)
				{
					list.RemoveAll(i => i >= min && i <= min2);
				}
			}
			if(max > avarage + 50)
			{
				ushort max2 = (ushort)(max + 50);
				if(list.Count(i => i >= max && i <= max2) <= 5)
				{
					list.RemoveAll(i => i >= max && i <= max2);
				}
			}

			return (ushort)list.Average(i => i);
		}

		static (byte pin, ushort value) getInfo(byte first, byte second)
		{
			ushort value = (ushort)(first << 8 | second);

			byte pin = (byte)(value >> 10 & 0xFF);
			value = (ushort)(value & 0x3FF);

			return (pin, value);
		}
	}
}
