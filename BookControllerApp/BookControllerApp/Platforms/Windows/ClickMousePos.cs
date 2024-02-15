using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BookControllerApp.Platforms.Windows
{
	public static class ClickMousePos
	{
		public static (int x, int y) left;
		public static (int x, int y) right;
		public static string settingJsonFilePath = Path.GetTempPath().Replace("\\", "/") + "/get_click_pos.json";
		public static string currentPath = System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/");
		public static string scriptPath = currentPath + "/../../../../../../../ClickPos/get_click_pos.py";


		public static void GetSettingFromJsonFile()
		{
			var jsontext = File.ReadAllText(settingJsonFilePath);
			var json = JObject.Parse(jsontext);
			Debug.WriteLine(json);
			left.x = Int32.Parse((string)json?["left"]?["x"]);
			left.y = Int32.Parse((string)json?["left"]?["y"]);

			right.x = Int32.Parse((string)json?["right"]?["x"]);
			right.y = Int32.Parse((string)json?["right"]?["y"]);
		}

		public static async Task SettingMousePos()
		{
			Debug.WriteLine(settingJsonFilePath);
			// 実行したいコマンドと引数
			string command = "python3";

			string arguments = scriptPath;
			arguments += " " + settingJsonFilePath;

			//Debug.WriteLine(currentPath);

			await ExecuteExternalProcess(command, arguments);
			GetSettingFromJsonFile();
		}

		private static async Task ExecuteExternalProcess(string fileName, string arguments)
		{
			using (Process process = new Process())
			{
				ProcessStartInfo psi = new ProcessStartInfo
				{
					FileName = fileName,
					Arguments = arguments,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};

				process.StartInfo = psi;

				// 外部プロセスの開始
				process.Start();

				// 非同期で外部プロセスの終了待機
				await Task.Run(() =>
				{
					process.WaitForExit();
				});

				string output = process.StandardOutput.ReadToEnd();
				string error = process.StandardError.ReadToEnd();
				Debug.WriteLine("標準出力:\n" + output);
				Debug.WriteLine("標準エラー:\n" + error);

				// 外部プロセスの終了コード
				int exitCode = process.ExitCode;
			}
		}

	}
}
