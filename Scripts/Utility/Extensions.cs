using Godot;
using System;
using System.Collections.Generic;
using static TeroftheMagic.Scripts.Utility.TileUtil;
using TileData = TeroftheMagic.Scripts.Utility.TileUtil.TileData;

namespace TeroftheMagic.Scripts.Utility;

public static class Extensions {

	public static void UpdateCell(this TileMapLayer tml, Vector2I pos, TileData td = new()) {
		pos.Y *= -1;
		if (td.id == 0) tml.SetCell(pos);
		else tml.SetCell(pos, (int)td.sourceId, TileMapIdToCoord(td.id, TileMapWidth(td.sourceId)), td.alt);
	}

	public static int Mod(this int i, int mod) {
		if (i >= 0)
			return i % mod;
		else {
			i -= (i / mod - 1) * mod;
			return i % mod;
		}
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
}
