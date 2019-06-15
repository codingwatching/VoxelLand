using System.Collections.Generic;
using UnityEngine;

namespace Voxels {
	public static class BlockModelGenerator {
		static Dictionary<Byte2, Vector2[]> _fullTileUVCache = new Dictionary<Byte2, Vector2[]>();

		public static void PrepareGenerator() {
			
		}

		public static void AddBlock(GeneratableMesh meshInfo, TilesetHelper helper, BlockDescription desc, BlockData data, Vector3 rootPos, VisibilityFlags visibility, LightInfo light) {
			if ( desc == null ) {
				return;
			}
			switch ( desc.ModelType ) {
				case BlockModelType.None:
					return;
				case BlockModelType.FullBlockSimple:
					GenerateFullBlockSimple(meshInfo, helper, desc, data, rootPos, visibility, light);
					return;
				case BlockModelType.FullBlockComplex:
					GenerateFullBlockComplex(meshInfo, helper, desc, data, rootPos, visibility, light);
					return;
				case BlockModelType.HalfBlockDown:
					GenerateHalfBlockDown(meshInfo, helper, desc, data, rootPos, visibility, light);
					return;
				case BlockModelType.HalfBlockUp:
					return;
				case BlockModelType.Stairs:
					return;
				case BlockModelType.Plate:
					return;
				case BlockModelType.Grass:
					return;
				case BlockModelType.HorizontalPlane:
					GenerateHorizontalPlane(meshInfo, helper, desc, data, rootPos, visibility);
					return;
				case BlockModelType.SmallerBlock:
					return;
				case BlockModelType.CrossedVPlanes:
					GenerateCrossedVPlanes(meshInfo, helper, desc, data, rootPos, visibility);
					return;
				default:
					return;
			}
		}

		static Byte2     _lastTile = new Byte2(255,255);
		static Vector2[] _lastPlaneUVs;

		public static void GenerateCrossedVPlanes(GeneratableMesh meshInfo, TilesetHelper helper, BlockDescription desc, BlockData data, Vector3 rootPos, VisibilityFlags visibility) {
			if ( visibility == VisibilityFlags.None ) {
				return;
			}
			var pointer = meshInfo.Vertices.Count;
			var tile    = desc.Subtypes[Mathf.Clamp(data.Subtype, 0, desc.Subtypes.Count - 1)].FaceTiles[0];
			var calcColor = new Color32(data.SunLevel, data.LightLevel, 0, 0);
			AddPlane(meshInfo, _rotVertPlaneVerts_0, helper, tile, rootPos, pointer, calcColor);
			pointer += 4;
			AddPlane(meshInfo, _rotVertPlaneVerts_1, helper, tile, rootPos, pointer, calcColor);
			pointer += 4;
			AddPlaneInv(meshInfo, _rotVertPlaneVerts_0, helper, tile, rootPos, pointer, calcColor);
			pointer += 4;
			AddPlaneInv(meshInfo, _rotVertPlaneVerts_1, helper, tile, rootPos, pointer, calcColor);
			pointer += 4;
		}

		public static void GenerateFullBlockSimple(GeneratableMesh meshInfo, TilesetHelper helper, BlockDescription desc, BlockData data, Vector3 rootPos, VisibilityFlags visibility, LightInfo light) {
			if ( visibility == VisibilityFlags.None ) {
				return;
			}
			var pointer    = meshInfo.Vertices.Count;
			var tile = desc.Subtypes[Mathf.Clamp(data.Subtype, 0, desc.Subtypes.Count - 1)].FaceTiles[0];

			var uvs = _lastPlaneUVs;
			if ( !tile.Equals(_lastTile) ) {
				uvs = GetCachedUVsForTile(helper, tile);
				_lastPlaneUVs = uvs;
				_lastTile = tile;
			}
			
			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Backward) ) {
				var calcColor = new Color32(light.SunBackward, light.OBackward, 0, 0);
				AddPlaneWithUVs(meshInfo, _fullBlockBackSide, uvs, rootPos, pointer, calcColor);
				pointer += 4;
			}
			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Forward) ) {
				var calcColor = new Color32(light.SunForward, light.OForward, 0, 0);
				AddPlaneWithUVs(meshInfo, _fullBlockFrontSide, uvs, rootPos, pointer, calcColor);
				pointer += 4;
			}
			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Left) ) {
				var calcColor = new Color32(light.SunLeft, light.OLeft, 0, 0);
				AddPlaneWithUVs(meshInfo, _fullBlockLeftSide, uvs, rootPos, pointer, calcColor);
				pointer += 4;
			}
			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Right) ) {
				var calcColor = new Color32(light.SunRight, light.ORight, 0, 0);
				AddPlaneWithUVs(meshInfo, _fullBlockRightSide, uvs, rootPos, pointer, calcColor);
				pointer += 4;
			}
			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Up) ) {
				var calcColor = new Color32(light.SunUp, light.OUp, 0, 0);
				AddPlaneWithUVs(meshInfo, _fullBlockUpSide, uvs, rootPos, pointer, calcColor);
				pointer += 4;
			}
			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Down) ) {
				var calcColor = new Color32(light.SunDown, light.ODown, 0, 0);
				AddPlaneWithUVs(meshInfo, _fullBlockDownSide, uvs, rootPos, pointer, calcColor);
				pointer += 4;
			}
		}

		public static void GenerateFullBlockComplex(GeneratableMesh meshInfo, TilesetHelper helper, BlockDescription desc, BlockData data, Vector3 rootPos, VisibilityFlags visibility, LightInfo light) {
			if ( visibility == VisibilityFlags.None ) {
				return;
			}
			var pointer = meshInfo.Vertices.Count;
			var sub = desc.Subtypes[Mathf.Clamp(data.Subtype, 0, desc.Subtypes.Count - 1)];

			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Backward) ) {
				var calcColor = new Color32(light.SunBackward, light.OBackward, 0, 0);
				AddPlane(meshInfo, _fullBlockBackSide, helper, sub.FaceTiles[0], rootPos,  pointer, calcColor);
				pointer += 4;
			}
			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Forward) ) {
				var calcColor = new Color32(light.SunForward, light.OForward, 0, 0);
				AddPlane(meshInfo, _fullBlockFrontSide, helper, sub.FaceTiles[1], rootPos,  pointer,calcColor);
				pointer += 4;
			}
			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Left) ) {
				var calcColor = new Color32(light.SunLeft, light.OLeft, 0, 0);
				AddPlane(meshInfo, _fullBlockLeftSide, helper, sub.FaceTiles[2], rootPos,  pointer, calcColor);
				pointer += 4;
			}
			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Right) ) {
				var calcColor = new Color32(light.SunRight, light.ORight, 0, 0);
				AddPlane(meshInfo, _fullBlockRightSide, helper, sub.FaceTiles[3], rootPos,  pointer, calcColor);
				pointer += 4;
			}
			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Up) ) {
				var calcColor = new Color32(light.SunUp, light.OUp, 0, 0);
				AddPlane(meshInfo, _fullBlockUpSide, helper, sub.FaceTiles[4], rootPos, pointer, calcColor);
				pointer += 4;
			}
			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Down) ) {
				var calcColor = new Color32(light.SunDown, light.ODown, 0, 0);
				AddPlane(meshInfo, _fullBlockDownSide, helper, sub.FaceTiles[5], rootPos, pointer, calcColor);
				pointer += 4;
			}
		}

		public static void GenerateHalfBlockDown(GeneratableMesh meshInfo, TilesetHelper helper, BlockDescription desc, BlockData data, Vector3 rootPos, VisibilityFlags visibility, LightInfo light) {
			if ( visibility == VisibilityFlags.None ) {
				return;
			}
			var pointer = meshInfo.Vertices.Count;
			var sub = desc.Subtypes[Mathf.Clamp(data.Subtype, 0, desc.Subtypes.Count - 1)];

			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Backward) ) {
				var calcColor = new Color32(light.SunBackward, light.OBackward, 0, 0);
				AddPlane(meshInfo, _halfBlockDBackSide, helper, sub.FaceTiles[0], rootPos, pointer, calcColor);
				pointer += 4;
			}
			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Forward) ) {
				var calcColor = new Color32(light.SunForward, light.OForward, 0, 0);
				AddPlane(meshInfo, _halfBlockDFrontSide, helper, sub.FaceTiles[1], rootPos, pointer, calcColor);
				pointer += 4;
			}
			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Left) ) {
				var calcColor = new Color32(light.SunLeft, light.OLeft, 0, 0);
				AddPlane(meshInfo, _halfBlockDLeftSide, helper, sub.FaceTiles[2], rootPos, pointer, calcColor);
				pointer += 4;
			}
			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Right) ) {
				var calcColor = new Color32(light.SunRight, light.ORight, 0, 0);
				AddPlane(meshInfo, _halfBlockDRightSide, helper, sub.FaceTiles[3], rootPos, pointer, calcColor);
				pointer += 4;
			}

			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Up) ) {
				var calcColor = new Color32(light.SunUp, light.OUp, 0, 0);
				AddPlane(meshInfo, _halfBlockDUpside, helper, sub.FaceTiles[4], rootPos, pointer, calcColor);
				pointer += 4;
			}
			if ( VisibilityFlagsHelper.IsSet(visibility, VisibilityFlags.Down) ) {
				var calcColor = new Color32(light.SunDown, light.ODown, 0, 0);
				AddPlane(meshInfo, _fullBlockDownSide, helper, sub.FaceTiles[5], rootPos, pointer, calcColor);
				pointer += 4;
			}
		}

		static Vector3[] _halfBlockDUpside = new Vector3[4] {
			new Vector3(0f,0.5f,0f),
			new Vector3(0f,0.5f,1f),
			new Vector3(1f,0.5f,0f),
			new Vector3(1f,0.5f,1f)
		};

		static Vector3[] _halfBlockDLeftSide = new Vector3[4] {
			new Vector3(0f,0f,1f),
			new Vector3(0f,0.5f,1f),
			new Vector3(0f,0f,0f),
			new Vector3(0f,0.5f,0f)
		};

		static Vector3[] _halfBlockDRightSide = new Vector3[4] {
			new Vector3(1f,0f,0f),
			new Vector3(1f,0.5f,0f),
			new Vector3(1f,0f,1f),
			new Vector3(1f,0.5f,1f)
		};

		static Vector3[] _halfBlockDBackSide = new Vector3[4] {
			new Vector3(0f,0f,0f),
			new Vector3(0f,0.5f,0f),
			new Vector3(1f,0f,0f),
			new Vector3(1f,0.5f,0f)
		};

		static Vector3[] _halfBlockDFrontSide = new Vector3[4] {
			new Vector3(1f,0f,1f),
			new Vector3(1f,0.5f,1f),
			new Vector3(0f,0f,1f),
			new Vector3(0f,0.5f,1f)
		};

		static Vector2[] _halfTileUV = new Vector2[4] {
			new Vector2(0f,0.5f),
			new Vector2(0f,0f),
			new Vector2(1f,0.5f),
			new Vector2(1f,0f)
		};

		static Vector3[] _fullBlockUpSide = new Vector3[4] {
			new Vector3(0f,1f,0f),
			new Vector3(0f,1f,1f),
			new Vector3(1f,1f,0f),
			new Vector3(1f,1f,1f)
		};
		static Vector3[] _fullBlockDownSide = new Vector3[4] {
			new Vector3(1f,0f,0f),
			new Vector3(1f,0f,1f),
			new Vector3(0f,0f,0f),
			new Vector3(0f,0f,1f)
		};

		static Vector3[] _fullBlockLeftSide = new Vector3[4] {
			new Vector3(0f,0f,1f),
			new Vector3(0f,1f,1f),
			new Vector3(0f,0f,0f),
			new Vector3(0f,1f,0f)
		};

		static Vector3[] _fullBlockRightSide = new Vector3[4] {
			new Vector3(1f,0f,0f),
			new Vector3(1f,1f,0f),
			new Vector3(1f,0f,1f),
			new Vector3(1f,1f,1f)
		};

		static Vector3[] _fullBlockBackSide = new Vector3[4] {
			new Vector3(0f,0f,0f),
			new Vector3(0f,1f,0f),
			new Vector3(1f,0f,0f),
			new Vector3(1f,1f,0f)
		};

		static Vector3[] _fullBlockFrontSide = new Vector3[4] {
			new Vector3(1f,0f,1f),
			new Vector3(1f,1f,1f),
			new Vector3(0f,0f,1f),
			new Vector3(0f,1f,1f)
		};

		static Vector2[] _fullTileUV = new Vector2[4] {
			new Vector2(0f,1f),
			new Vector2(0f,0f),
			new Vector2(1f,1f),
			new Vector2(1f,0f)
		};

		static Vector3[] _horizontalPlaneVerts = new Vector3[4] {
			new Vector3(0f,0.02f,0f),
			new Vector3(0f,0.02f,1f),
			new Vector3(1f,0.02f,0f),
			new Vector3(1f,0.02f,1f)

		};

		static Vector3[] _rotVertPlaneVerts_0 = new Vector3[4] {
			new Vector3(0f,0f,0f),
			new Vector3(0f,1f,0f),
			new Vector3(1f,0f,1f),
			new Vector3(1f,1f,1f)

		};

		static Vector3[] _rotVertPlaneVerts_1 = new Vector3[4] {
			new Vector3(0f,0f,1f),
			new Vector3(0f,1f,1f),
			new Vector3(1f,0f,0f),
			new Vector3(1f,1f,0f)

		};

		static int[] _planeTris         = new int[6] {0,1,2,2,1,3};
		static int[] _planeTrisInverted = new int[6] {2,1,0,1,2,3};
		//Неполные блоки не рисуем только если они полностью ограждены полными блоками.
		public static void GenerateHorizontalPlane(GeneratableMesh meshInfo, TilesetHelper helper, BlockDescription desc, BlockData data, Vector3 rootPos, VisibilityFlags visibility) {
			if ( visibility == VisibilityFlags.None ) {
				return;
			}
			var pointer = meshInfo.Vertices.Count;
			var calcColor = new Color32(data.SunLevel, data.LightLevel, 0, 0);
			AddPlane(meshInfo, _horizontalPlaneVerts, helper, desc.Subtypes[Mathf.Clamp(data.Subtype, 0, desc.Subtypes.Count - 1)].FaceTiles[0], rootPos, pointer, calcColor);
		}

		public static void AddPlaneInv(GeneratableMesh meshInfo, Vector3[] verts, TilesetHelper helper, Byte2 tile, Vector3 rootPos, int pointer, Color32 color) {
			meshInfo.AddVerticesWithPos(verts, rootPos);
			meshInfo.AddTriangles(_planeTrisInverted, pointer);
			meshInfo.AddPlaneUVs(helper, tile);
			//Ага
			meshInfo.Colors.Add(color);
			meshInfo.Colors.Add(color);
			meshInfo.Colors.Add(color);
			meshInfo.Colors.Add(color);
		}

		public static void AddPlane(GeneratableMesh meshInfo, Vector3[] verts, TilesetHelper helper, Byte2 tile, Vector3 rootPos, int pointer, Color32 color) {
			meshInfo.AddVerticesWithPos(verts, rootPos);
			meshInfo.AddTriangles(_planeTris, pointer);
			meshInfo.AddPlaneUVs(helper, tile);
			//Ага
			meshInfo.Colors.Add(color);
			meshInfo.Colors.Add(color);
			meshInfo.Colors.Add(color);
			meshInfo.Colors.Add(color);
		}

		public static void AddPlaneWithUVs(GeneratableMesh meshInfo, Vector3[] verts, Vector2[] uvs, Vector3 rootPos, int pointer, Color32 color) {
			meshInfo.AddVerticesWithPos(verts, rootPos);
			meshInfo.AddTriangles(_planeTris, pointer);
			meshInfo.AddUVs(uvs);
			//Ага
			meshInfo.Colors.Add(color);
			meshInfo.Colors.Add(color);
			meshInfo.Colors.Add(color);
			meshInfo.Colors.Add(color);
		}

		static void AddVerticesWithPos(this GeneratableMesh meshInfo, Vector3[] verts, Vector3 rootPos) {
			var vertsList = meshInfo.Vertices;
			for ( int i = 0; i < verts.Length; i++ ) {
				vertsList.Add(verts[i] + rootPos);
			}
		}

		static void AddTriangles(this GeneratableMesh meshInfo, int[] indexes, int rootIndex) {
			var trisList = meshInfo.Triangles;
			for ( int i = 0; i < indexes.Length; i++ ) {
				trisList.Add(indexes[i] + rootIndex);
			}
		}

		static void AddPlaneUVs(this GeneratableMesh meshInfo, TilesetHelper helper, Byte2 tilePos) {
			meshInfo.AddUVs(GetCachedUVsForTile(helper, tilePos));
		}

		static void AddUVs(this GeneratableMesh meshInfo, Vector2[] uvs) {
			var uvsList = meshInfo.UVs;
			for ( int i = 0; i < uvs.Length; i++ ) {
				uvsList.Add(uvs[i]);
			}
		}

		static Vector2[] CacheUVsForTile(TilesetHelper helper, Byte2 tilePos) {
			var uvs = new Vector2[4];
			for ( int i = 0; i < uvs.Length; i++ ) {
				uvs[i] = helper.RelativeToAbsolute(_fullTileUV[i].x, _fullTileUV[i].y, tilePos);
			}
			_fullTileUVCache.Add(tilePos, uvs);
			return uvs;
		}

		static Vector2[] GetCachedUVsForTile(TilesetHelper helper, Byte2 tilePos) {
			return _fullTileUVCache.ContainsKey(tilePos) ? _fullTileUVCache[tilePos] : CacheUVsForTile(helper, tilePos);
		}
	}
}
