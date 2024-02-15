using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.BLE.Abstractions;
using System.Collections.ObjectModel;

namespace BookControllerApp
{
	public partial class MainPage : ContentPage
	{

		IBluetoothLE Ble;
		IAdapter Adapter;
		IDevice SelectedDevice;
		ObservableCollection<IDevice> DeviceList = new ObservableCollection<IDevice>();

		private const string DEVICE_NAME = "BookController";
		private string SERVICE_UUID = "da3bb75d-0ea5-43f0-80d0-52fcda5567b5";
		private string CHARACTERISTIC_UUID = "118bbbd5-2554-4272-93a3-689dec6d7182";

		[Obsolete]
		public MainPage()
		{
			Ble = CrossBluetoothLE.Current;
			Adapter = CrossBluetoothLE.Current.Adapter;

			Adapter.ScanTimeout = 5000;
			Adapter.DeviceDiscovered += async (s, a) =>
			{
				await Device.InvokeOnMainThreadAsync(() =>
				{
					if (a.Device.Name.Contains(DEVICE_NAME)) { DeviceList.Add(a.Device); }
				});

			};

			InitializeComponent();
		}

		private void ToggleFindingIndicator(bool a)
		{
			findingIndicator.IsRunning = a;

		}
		private void ToggleConnectingIndicator(bool a)
		{
			connectingIndicator.IsRunning = a;

		}

		private async void OnFindButtonClicked(object sender, EventArgs e)
		{
			ToggleFindingIndicator(true);
			DeviceList.Clear();
			await Adapter.StartScanningForDevicesAsync();
			BLEListView.ItemsSource = DeviceList;
			ToggleFindingIndicator(false);

		}


		private async void OnSettingButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new SettingPage());
		}

		[Obsolete]
		private async void BLEListViewSelected(object sender, SelectedItemChangedEventArgs e)
		{

			SelectedDevice = e.SelectedItem as IDevice;
			if (SelectedDevice == null)
			{
				return;
			}

			try
			{
				ToggleConnectingIndicator(true);

				await Adapter.ConnectToDeviceAsync(SelectedDevice);
				if (SelectedDevice.State == DeviceState.Connected || SelectedDevice.State == DeviceState.Limited)
				{
					var service = await SelectedDevice.GetServiceAsync(Guid.Parse(SERVICE_UUID));
					if (service == null)
					{
						await DisplayAlert("BookControllerApp", "this is not BookController", "OK");
						ToggleConnectingIndicator(false);
						return;
					}
					var characteristics = await service.GetCharacteristicsAsync();
					var characteristic = characteristics.FirstOrDefault(x => x.Uuid == CHARACTERISTIC_UUID);
					if (characteristic == null)
					{
						await DisplayAlert("BookControllerApp", "this is not BookController", "OK");
						ToggleConnectingIndicator(false);
						return;
					}

					await Navigation.PushAsync(new BookControllerPage(characteristic));
					DeviceList.Clear();
					ToggleConnectingIndicator(false);
				}
			}
			catch (DeviceConnectionException)
			{
				await DisplayAlert("BookControllerApp", "Error has occured", "OK");
				ToggleConnectingIndicator(false);
			}
		}
		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (SelectedDevice != null)
			{
				await Adapter.DisconnectDeviceAsync(SelectedDevice);
			}

			var devices = Adapter.ConnectedDevices;
			foreach (var device in devices)
			{
				await Adapter.DisconnectDeviceAsync(device);
			}
		}
	}
}
