using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TeroftheMagic.Scripts.Utility;

public partial class Logger {

	static readonly string path = @".\log.txt";

	static Dictionary<string, FunctionTime> logFunctions = [];
	static System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

	public static void StartTimer(string path) {
		if (!logFunctions.TryGetValue(path, out FunctionTime ft)) {
			ft = new();
		}
		ft.msCurrent = (int)sw.ElapsedMilliseconds;
		if (ft.callsTimer == 0)
			ft.callsTimer = ft.msCurrent;
		ft.callsCurrent++;
		logFunctions[path] = ft;
	}

	public static void StopTimer(string path) {
		if (logFunctions.TryGetValue(path, out FunctionTime ft)) {
			long delta = sw.ElapsedMilliseconds - ft.msCurrent;
			if (ft.msMax < delta) {
				ft.msMax = (int)delta;
			}
			if (sw.ElapsedMilliseconds - ft.callsTimer >= 1000) {
				ft.callsTimer = 0;
				if (ft.callsCurrent > ft.callsMax)
					ft.callsMax = ft.callsCurrent;
				ft.callsCurrent = 0;
			}
			logFunctions[path] = ft;
		}
	}

	public static void LogCurrentMax() {
		// Delete the file if it exists.
		if (File.Exists(path)) {
			File.Delete(path);
		}

		//Create the file.
		using FileStream fs = File.Create(path);
		// AddText(fs, "This is some text");
		// AddText(fs, "This is some more text,");
		// AddText(fs, "\r\nand this is on a new line");
		// AddText(fs, "\r\n\r\nThe following is a subset of characters:\r\n");

		// for (int i = 1; i < 120; i++) {
		// 	AddText(fs, Convert.ToChar(i).ToString());
		// }
		foreach (string path in logFunctions.Keys) {
			if (logFunctions.TryGetValue(path, out FunctionTime ft)) {
				AddText(fs, $"[{DateTime.Now}][{path}]: {ft.msMax}ms | {ft.callsMax}/s\r\n");
			}
		}

		//Open the stream and read it back.
		// using (FileStream fs = File.OpenRead(path)) {
		// 	byte[] b = new byte[1024];
		// 	UTF8Encoding temp = new(true);
		// 	int readLen;
		// 	while ((readLen = fs.Read(b, 0, b.Length)) > 0) {
		// 		GD.Print(temp.GetString(b, 0, readLen));
		// 	}
		// }
	}

	private static void AddText(FileStream fs, string value) {
		byte[] info = new UTF8Encoding(true).GetBytes(value);
		fs.Write(info, 0, info.Length);
	}

	private struct FunctionTime() {
		public int msMax = 0;
		public int msCurrent = 0;
		public int callsMax = 0;
		public int callsCurrent = 0;
		public double callsTimer = 0;
	}
}