using System.Collections.Generic;
using UnityEngine;

namespace Milhouzer.MarchingSquares
{
    public abstract class LookupTable<T> : ScriptableObject 
    {
        public abstract List<T> Table { get; }
        public T this[int index] { get 
        { 
            if(index > Table.Count || index < 0)
                throw new System.ArgumentException("Index out of table range.");
                
            return Table[index]; 
        }}
    }
}