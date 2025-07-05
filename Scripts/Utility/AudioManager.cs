using Godot;
using System;
using System.Collections.Generic;
using System.IO;

namespace TeroftheMagic.Scripts.Utility;

public partial class AudioManager : AudioStreamPlayer {
	public static Dictionary<string, AudioStreamMP3> Audios = [];

	public static AudioManager Instance;

	static string AudioPath = "res://Assets/Audio/";

	private AudioManager() => Instance = this;
	static AudioManager() {
		foreach (string file in DirAccess.GetFilesAt(AudioPath)) {
			if (file.Contains(".import")) continue;
			Audios.Add(file.Split('.')[0], GD.Load<AudioStreamMP3>(AudioPath + file));
		}
	}

	public static void PlayAudio(string audio) =>
		Instance.PutStream(Audios[audio]).Play();
}
