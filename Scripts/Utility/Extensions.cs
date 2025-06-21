using Godot;
using System;
using static TeroftheMagic.Scripts.Utility.Functions;

namespace TeroftheMagic.Scripts.Utility;

public static class Extensions {

	private static string values = "0";

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
}
