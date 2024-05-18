using System.Collections.Generic;
using UnityEngine;

namespace Milhouzer.ProceduralGeneration
{

    [CreateAssetMenu(fileName = "TilesMeshLookupTable", menuName = "MarchingSquares/TilesMeshLookupTable", order = 1)]
    public class TilesMeshLookupTable : LookupTable<MeshFilter>
    {
        [SerializeField]
        private List<MeshFilter> tilesTable;
        public override List<MeshFilter> Table => tilesTable;
    }
}