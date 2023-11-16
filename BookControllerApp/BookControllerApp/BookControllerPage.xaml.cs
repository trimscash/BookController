using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Primitives;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;

namespace BookControllerApp
{
	public partial class BookControllerPage : ContentPage
	{
		private ICharacteristic Characteristic = null;
		private byte[] RecievedData;
		private bool IsListening = false;


		private const int LEFT = 0;
		private const int RIGHT = 1;

		private const int MOUSEEVENTF_LEFTDOWN = 0x2;
		private const int MOUSEEVENTF_LEFTUP = 0x4;


		[Obsolete]
		public BookControllerPage(ICharacteristic characteristic)
		{
			Characteristic = characteristic;
			InitializeComponent();
			List<ICharacteristic> a = new List<ICharacteristic>();
			a.Add(characteristic);
			CharacteristicListView.ItemsSource = a;

			Characteristic.ValueUpdated += async (o, args) => // ボタンが押されたら
			{
				await Device.InvokeOnMainThreadAsync(() =>
				{
					RecievedData = args.Characteristic.Value;
					int data = Convert.ToInt32(RecievedData[0]);
					string dataText = data.ToString() + " ";
					RecievedDataLabel.Text = dataText;
					if (data == LEFT)
					{
						ClickDisplay(0, 0);
						LeftIndicator.BackgroundColor = Colors.Blue;
						RightIndicator.BackgroundColor = Colors.LightPink;
					}
					else if (data == RIGHT)
					{
						int width = (int)DeviceDisplay.Current.MainDisplayInfo.Width;
						int height = (int)DeviceDisplay.Current.MainDisplayInfo.Height;

						ClickDisplay(width - 100, 0);
						LeftIndicator.BackgroundColor = Colors.LightBlue;
						RightIndicator.BackgroundColor = Colors.Red;
					}
				});
			};

		}


		[DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]
		static extern void SetCursorPos(int X, int Y);

		[DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]
		static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

		private void ClickDisplay(int x, int y)
		{
			SetCursorPos(x, y);
			mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
			mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
		}

		[Obsolete]
		private async void OnButtonClicked(object sender, EventArgs e)
		{
			try
			{
				await Characteristic.StartUpdatesAsync();
				await Device.InvokeOnMainThreadAsync(() =>
				{
					IsListening = true;
					IsListeningLabel.Text = "受信，操作中";
				});
			}
			catch (DeviceConnectionException)
			{
				await DisplayAlert("BookControllerApp", "Error has occured. Maybe Controller is OFF", "OK");
			}
		}
	}
}

