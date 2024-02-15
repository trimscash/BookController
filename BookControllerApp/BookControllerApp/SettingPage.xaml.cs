using System.Drawing;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System.IO;
using System;
using System.Diagnostics;
using Microsoft.Maui.Layouts;
using Microsoft.Maui;
using BookControllerApp.Platforms.Windows;


namespace BookControllerApp;

public partial class SettingPage : ContentPage
{

	private bool isDragging = false;
	private string settingFilePath = "";
	private string currentPath = "";
	public SettingPage()
	{
		currentPath = System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/");
		settingFilePath = Path.GetTempPath().Replace("\\", "/") + "/get_click_pos.json";
		InitializeComponent();
	}

	private async void OnSettingButtonClicked(object sender, EventArgs e)
	{
		settingIndicator.IsRunning = true;
		settingIndicator.IsVisible = true;
		SettingButton.IsVisible = false;


		Debug.WriteLine(currentPath);

		try
		{
			// 外部プロセスの実行
			await ClickMousePos.SettingMousePos();
		}
		finally
		{
			// 処理が完了したら、ボタンを表示し、インジケータを非表示に
			SettingButton.IsVisible = true;
			settingIndicator.IsRunning = false;
			settingIndicator.IsVisible = false;
		}

		ClickMousePos.GetSettingFromJsonFile();
	}



	protected override async void OnAppearing()
	{
		base.OnAppearing();
		ClickMousePos.GetSettingFromJsonFile();

		leftPosText.Text = "左のクリック位置: " + ClickMousePos.left;
		rightPosText.Text = "右のクリック位置: " + ClickMousePos.right;
	}

	//	var manager = new ScreenCaptureManager();
	//	var screen = manager.GetScreens().FirstOrDefault();

	//	using var capture = await manager.Capture(screen);
	//	using var bitmap = await capture.ToBitmap();

	//	MemoryStream memoryStream = new MemoryStream();
	//	bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
	//	bitmap.Save("C:/Users/trimscash/Downloads/b.png", System.Drawing.Imaging.ImageFormat.Png);
	//	memoryStream.Seek(0, System.IO.SeekOrigin.Begin);

	//	var currentPath = System.AppDomain.CurrentDomain.BaseDirectory;


	//	Microsoft.Maui.Controls.Image image = new Microsoft.Maui.Controls.Image
	//	{
	//		Source = ImageSource.FromStream(() => memoryStream)
	//	};
	//	Button button = new Button
	//	{
	//		Text = "a",
	//	};

	//	var panGesture = new PanGestureRecognizer();
	//	panGesture.PanUpdated += OnPanUpdated;
	//	button.GestureRecognizers.Add(panGesture);

	//	AbsoluteLayout absoluteLayout = new AbsoluteLayout { };

	//	absoluteLayout.Add(image);
	//	absoluteLayout.Add(button);
	//	AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.All);
	//	AbsoluteLayout.SetLayoutBounds(image, new Rect(0, 0, 1, 1));
	//	AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.None);
	//	AbsoluteLayout.SetLayoutBounds(button, new Rect(0.5, 0.5, 100, 50));

	//	// AbsoluteLayoutに子要素を追加します。

	//	SettingContent.Content = absoluteLayout;



	//}


	//private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
	//{
	//	if (e.StatusType == GestureStatus.Running)
	//	{
	//		var button = (Button)sender;
	//		_x += e.TotalX;
	//		_y += e.TotalY;
	//		button.TranslationX = _x;
	//		button.TranslationY = _y;
	//	}
	//}


}