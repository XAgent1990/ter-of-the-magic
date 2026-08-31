using Godot;
using System;
using System.Collections.Generic;
using static TeroftheMagic.Utility.TileUtil;
using TileSetData = TeroftheMagic.Utility.TileUtil.TileSetData;

namespace TeroftheMagic.Utility;

public static class Extensions {
	public static Vector2I Up = Vector2I.Down;
	public static Vector2I Down = Vector2I.Up;
	public static Vector2I Left = Vector2I.Left;
	public static Vector2I Right = Vector2I.Right;

	public static void UpdateCell(this TileMapLayer tml, Vector2I pos) => tml.UpdateCell(pos, new(TileSetId.block, 0));
	public static void UpdateCell(this TileMapLayer tml, Vector2I pos, TileSetData td) {
		pos.Y = -pos.Y - 1;
		if (td.ID <= 0) tml.SetCell(pos);
		else tml.SetCell(pos, (int)td.SourceId, TileMapIdToCoord(td.SourceId, td.ID), td.Alt);
	}

	public static Node GetGrandparent(this Node node) => node.GetParent().GetParent();
	public static Node GetGreatgrandparent(this Node node) => node.GetGrandparent().GetParent();

	public static Node GetNthParent(this Node node, int n) {
		Node current = node;
		for (int i = 0; i < n; i++)
			current = current.GetParent();
		return current;
	}

	public static void Toggle(this ref bool b) => b = !b;

	public static int Mod(this int i, uint mod) => i.Mod((int)mod);
	public static int Mod(this int i, int mod) {
		if (i < 0)
			i += -(i / mod - 1) * mod;
		return i % mod;
	}

	public static bool TryFind<T>(this List<T> list, Predicate<T> predicate, out T obj) {
		obj = list.Find(predicate);
		return obj is not null;
	}

	public static string AsString<T>(this T[,] arr) {
		string output = "[";
		if (arr.Rank > 2)
			output += "...";
		else {
			for (int ii = 0; ii < arr.GetLength(1); ii++) {
				if (ii != 0)
					output += '\n';
				for (int i = 0; i < arr.GetLength(0); i++) {
					if (i != 0)
						output += ", ";
					output += arr[i, ii];
				}
			}
		}
		return output += ']';
	}

	public static string AsString<T>(this List<T> list) {
		string output = "[";
		for (int i = 0; i < list.Count; i++) {
			if (i != 0)
				output += ", ";
			output += list[i];
		}
		return output += ']';
	}

	public static void SetActive(this Node node, bool active) =>
		node.ProcessMode = active ? Node.ProcessModeEnum.Inherit : Node.ProcessModeEnum.Disabled;

	public static bool IsActive(this Node node) => node.CanProcess();

	public static AudioStreamPlayer PutStream(this AudioStreamPlayer ASP, AudioStream AS) {
		ASP.Stream = AS;
		return ASP;
	}
}
