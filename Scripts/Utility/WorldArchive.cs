using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using TeroftheMagic.Universe;
using static System.IO.File;

namespace TeroftheMagic.Utility;

public class WorldArchive {

	private static readonly StringBuilder sb = new();
	private static Dictionary<string, string> dic = [];

	public static void Save(string name) {
		// save += Layer(WorldData.front, "front");
		// save += ',';
		Directory.CreateDirectory($"Saves/{name}");
		CreateDictionary(name);
		WriteAllLines($"Saves/{name}/main.json", Layer(WorldData.main, "main"));
		WriteAllLines($"Saves/{name}/back.json", Layer(WorldData.back, "back"));
	}

	private static void CreateDictionary(string name) {
		string block;
		dic = [];
		Dictionary<string, uint> cDic = [];
		for (ushort x = 0; x < Game.WorldChunks.X; x++) {
			for (ushort y = 0; y < Game.WorldChunks.Y; y++) {
				for (ushort cx = 0; cx < WorldData.chunkSize; cx++) {
					for (ushort cy = 0; cy < WorldData.chunkSize; cy++) {
						block = WorldData.main.chunks[x, y][cx, cy].ToString();
						if (block != "") {
							if (!cDic.ContainsKey(block)) cDic[block] = 0;
							cDic[block]++;
						}
						block = WorldData.back.chunks[x, y][cx, cy].ToString();
						if (block != "") {
							if (!cDic.ContainsKey(block)) cDic[block] = 0;
							cDic[block]++;
						}
					}
				}
			}
		}
		ushort id = 0;
		while (cDic.Count > 0) {
			uint max = 0;
			string mKey = "";
			foreach (string key in cDic.Keys) {
				if (cDic[key] > max) {
					mKey = key;
					max = cDic[key];
				}
			}
			dic[mKey] = id++.ToString();
			cDic.Remove(mKey);
		}
		WriteDictionary(name);
	}

	private static void WriteDictionary(string name) {
		List<string> lines = [];
		foreach (string key in dic.Keys)
			lines.Add($"{key},{dic[key]}");
		WriteAllLines($"Saves/{name}/dic", lines);
	}

	private static List<string> Layer(WorldLayerData wld, string layer) {
		List<string> chunks = [];
		for (ushort x = 0; x < Game.WorldChunks.X; x++) {
			for (ushort y = 0; y < Game.WorldChunks.Y; y++) {
				chunks.Add(Chunk(wld.chunks[x, y]));
			}
		}
		return chunks;
	}

	private static string Chunk(WorldChunk c) {
		sb.Clear();
		for (ushort x = 0; x < WorldData.chunkSize; x++) {
			for (ushort y = 0; y < WorldData.chunkSize; y++) {
				if (x + y != 0) sb.Append(',');
				try { sb.Append(dic[c[x, y].ToString()]); }
				catch (NullReferenceException) { sb.Append(Block.Air); }
			}
		}
		// chunk += JsonSerializer.Serialize(c.GetBlocks());
		return sb.ToString();
	}

	public static void Load(string name) {
		ReadDictionary(name);
		string[] main = ReadAllLines($"Saves/{name}/main.json");
		string[] back = ReadAllLines($"Saves/{name}/back.json");
		WorldData.Clear();
		WorldData.New(Game.WorldChunks);
		ushort cs = WorldData.chunkSize;
		ushort chunkCX = (ushort)(WorldData.size.X / cs);
		ushort chunkCY = (ushort)(WorldData.size.Y / cs);
		string[] blockIDs;
		BlockData[,] blocks;
		for (ushort cx = 0; cx < chunkCX; cx++) {
			for (ushort cy = 0; cy < chunkCY; cy++) {
				WorldData.main.chunks[cx, cy] = new(new(cx * cs, cy * cs), WorldLayer.main);
				blocks = WorldData.main.chunks[cx, cy].GetBlocks();
				blockIDs = main[cx * chunkCY + cy].Split(',');
				for (ushort x = 0; x < WorldData.chunkSize; x++) {
					for (ushort y = 0; y < WorldData.chunkSize; y++) {
						blocks[x, y] = BlockData.FromString(dic[blockIDs[x * WorldData.chunkSize + y]]);
					}
				}
				WorldData.main.chunks[cx, cy].SetBlocks(blocks);

				WorldData.back.chunks[cx, cy] = new(new(cx * cs, cy * cs), WorldLayer.back);
				blocks = WorldData.back.chunks[cx, cy].GetBlocks();
				blockIDs = back[cx * chunkCY + cy].Split(',');
				for (ushort x = 0; x < WorldData.chunkSize; x++) {
					for (ushort y = 0; y < WorldData.chunkSize; y++) {
						blocks[x, y] = BlockData.FromString(dic[blockIDs[x * WorldData.chunkSize + y]]);
					}
				}
				WorldData.back.chunks[cx, cy].SetBlocks(blocks);
			}
		}
	}

	private static void ReadDictionary(string name) {
		dic = [];
		string[] lines = ReadAllLines($"Saves/{name}/dic");
		foreach(string line in lines) {
			string[] split = line.Split(',');
			dic.Add(split[1], split[0]);
		}
	}

}
