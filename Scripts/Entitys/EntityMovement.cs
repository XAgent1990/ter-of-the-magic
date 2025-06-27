using Godot;
using System;
using System.Collections.Generic;
using TeroftheMagic.Scripts;
using TeroftheMagic.Scripts.Universe;
using static TeroftheMagic.Scripts.Utility.TileUtil;

public partial class EntityMovement : CharacterBody2D {
	//Move Left right up down jump
	//follow Pathfinding
	//state mashine
	//flip image


	//instructions for moveing etc.
	//set of actions

	//path generrated for whole group of types?

	public AStar2D aStar2D = new();

	private bool WalkLeft = true;
	long value = 0;

	//private List<Vector2> pointsForLineDebug;
	public override void _Ready() {
		base._Ready();

		// WorldData.heightMap

		ushort[] heightMap = WorldData.heightMap;

		//pointsForLineDebug = new();

		for (ushort i = 0; i < heightMap.Length; i++) {
			// Vector2 pos = new Vector2(i * 16, heightMap[i] * -16);
			Vector2I mapPos = new(i, heightMap[i]);
			// TileSetData td = Item.Get(WorldData.main[mapPos].ID).TileSetData;
			BlockData bd = WorldData.main[mapPos];
			while (bd.ID == Block.Air || Item.Get(bd.ID).GetTileSetData(bd.Variant).SourceId != TileSetId.block) {
				mapPos.Y--;
				bd = WorldData.main[mapPos];
				// td = WorldData.main[mapPos];
			}

			//Calc Position
			Vector2 pos = new Vector2(mapPos.X * 16, (mapPos.Y + 1) * -16);
			// pos local to global + to center of block
			Vector2 newPos = pos - Position + new Vector2(8, 8);

			//pointsForLineDebug.Add(pos);

			aStar2D.AddPoint(i, pos);
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

		if (WalkLeft) {
			value = aStar2D.GetPointCount() - 1;
		}
		else {
			value = 0;
		}
	}


	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);

		if (WalkLeft) {
			Position = aStar2D.GetPointPosition(value--);
			if (value < 0) {
				value = aStar2D.GetPointCount() - 1;
			}
		}
		else {
			Position = aStar2D.GetPointPosition(value++);
			if (value > aStar2D.GetPointCount() - 1) {
				value = 0;
			}
		}


	}


	public override void _Draw() {
		base._Draw();
		//DrawPolyline(pointsForLineDebug.ToArray(), Color.Color8(255, 0, 0));
	}
}
