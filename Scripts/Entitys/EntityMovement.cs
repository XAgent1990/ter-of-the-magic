using Godot;
using System;
using System.Collections.Generic;
using TeroftheMagic.Scripts;
using static TeroftheMagic.Scripts.Utility.TileUtil;
using TileData = TeroftheMagic.Scripts.Utility.TileUtil.TileData;

public partial class EntityMovement : CharacterBody2D {
	//Move Left right up down jump
	//follow Pathfinding
	//state mashine
	//flip image

	public AStar2D aStar2D = new();

	private List<Vector2> pointsForLineDebug;
	public override void _Ready() {
		base._Ready();

		// WorldData.heightMap

		ushort[] heightMap = WorldData.heightMap;

		pointsForLineDebug = new();

		for (ushort i = 0; i < heightMap.Length; i++) {
			// Vector2 pos = new Vector2(i * 16, heightMap[i] * -16);
			Vector2I mapPos = new(i, heightMap[i]);
			TileData td = WorldData.main[mapPos];
			while (td.ID == 0 || td.SourceId != TileSetId.block) {
				mapPos.Y--;
				td = WorldData.main[mapPos];
			}

			//Calc Position
			Vector2 pos = new Vector2(mapPos.X * 16, (mapPos.Y + 1) * -16);
			// pos local to global + to center of block
			Vector2 newPos = pos - Position + new Vector2(8, 8);

			pointsForLineDebug.Add(newPos);

			aStar2D.AddPoint(i, newPos);
			if (i > 0) {
				aStar2D.ConnectPoints(i - 1, i);
			}
		}
		// List<Vector2> pointsForLineDebugTemp = new();

		// for (int i = 1; i < heightMap.Length - 1; i++) {
		// 	//height Difference muss glaube ich seperat gemacht werden
		// 	// GD.Print($"HeightDifference Pos {i}\n{pointsForLineDebug[pointsForLineDebug.Count - 1].Y - newPos.Y}");
		// 	Vector2 posLeft = pointsForLineDebug[i - 1];
		// 	Vector2 pos = pointsForLineDebug[i];
		// 	Vector2 posRight = pointsForLineDebug[i + 1];

		// 	pointsForLineDebugTemp.Add(posLeft);

		// 	GD.Print($"Pos: {i} Left: {Math.Abs(pos.Y - posLeft.Y)} Right: {Math.Abs(pos.Y - posRight.Y)}");

		// 	// if (Math.Abs(pos.Y - posLeft.Y) >= 32) {
		// 	// 	Vector2 tempSavePos = new Vector2(pos.X, posLeft.Y);
		// 	// 	pointsForLineDebugTemp.Add(tempSavePos);
		// 	// }
		// 	// if (Math.Abs(pos.Y - posRight.Y) >= 32) {
		// 	// 	Vector2 tempSavePos = new Vector2(pos.X, posRight.Y);
		// 	// 	pointsForLineDebugTemp.Add(tempSavePos);
		// 	// }

		// }
		// pointsForLineDebug = pointsForLineDebugTemp;
		//World.Main.hei

		//aStar2D.AddPoint();
		//aStar2D.ConnectPoints
	}

	public override void _Draw() {
		base._Draw();
		DrawPolyline(pointsForLineDebug.ToArray(), Color.Color8(255, 0, 0));
	}
}
