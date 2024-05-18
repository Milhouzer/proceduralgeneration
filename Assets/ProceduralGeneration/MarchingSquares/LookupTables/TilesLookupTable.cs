using System.Collections.Generic;
using UnityEngine;

namespace Milhouzer.ProceduralGeneration
{
    [CreateAssetMenu(fileName = "TilesLookupTable", menuName = "MarchingSquares/TilesLookupTable", order = 0)]
    public class TilesLookupTable : LookupTable<GameObject>
    {
        [SerializeField]
        private List<GameObject> tilesTable;
        public override List<GameObject> Table => tilesTable;
    }
}