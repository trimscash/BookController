using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Collections.ObjectModel;
using Microsoft.Maui.ApplicationModel;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;

namespace BookControllerApp
{
    public partial class SettingPage : ContentPage
    {

        public  SettingPage()
        {

            InitializeComponent();
            CopyScreen();


        }

        private async void TakeScreenShot()
        {
            var screen = await Screenshot.CaptureAsync();
            Stream s = await screen.OpenReadAsync();
            ScreenImg.Source = ImageSource.FromStream(() => s);
        }

        private void OnbackButtonClicked(object sender, EventArgs e)
        {

        }

        private void OnturnButtonClicked(object sender, EventArgs e)
        {

        }

        private void CopyScreen()
        {
            int width = (int)DeviceDisplay.Current.MainDisplayInfo.Width;
            int height = (int)DeviceDisplay.Current.MainDisplayInfo.Height;
            using (var screenBmp = new Bitmap(
                100,
                100))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(0, 0, 0, 0, screenBmp.Size);
                }
                using (MemoryStream s = new MemoryStream())
                {
                    screenBmp.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                    s.Position = 0;

                    ScreenImg.Source = ImageSource.FromStream(() => s);
                    return;
                }

            }
        }
    }
}
