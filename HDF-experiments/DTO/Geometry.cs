using System.Collections.Generic;

namespace HDF_experiments.DTO
{
    public class Geometry
    {
        public float[] Vertices { get; set; }
        public int[] Faces { get; set; }

        public List<float[]> Items { get; set; } 
    }
}