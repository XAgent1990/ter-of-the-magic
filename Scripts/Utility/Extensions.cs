using Godot;
using System;
using static TeroftheMagic.Scripts.Utility.Functions;

namespace TeroftheMagic.Scripts.Utility;

public static class Extensions {

    public static void UpdateCell(this TileMapLayer tml, Vector2I pos, TileData td) {
        pos.Y *= -1;
        if (td.id == 0) tml.SetCell(pos);
        else tml.SetCell(pos, (int)td.sourceId, TileMapIdToCoord(td.id, TileMapWidth(td.sourceId)), td.alt);
    }
}
