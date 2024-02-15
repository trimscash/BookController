using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;

namespace BookControllerApp
{
	public partial class ServicesPage : ContentPage
	{
		private IDevice Device = null;
        private List<IService> ServiceList = new List< IService>();

        public ServicesPage(IDevice device)
		{
			InitializeComponent();
			Device = device;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

            ServiceList.Clear();
			var services = await Device.GetServicesAsync();
			foreach (var service in services)
			{
                ServiceList.Add(service);
			}
			ServiceListView.ItemsSource = ServiceList;
		}
    }
}

