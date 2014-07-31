using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HDF_experiments.DTO;
using HDF_experiments.HDF;

namespace ConsoleApplication
{
    internal class Reader
    {
        public static async void TestReaders()
        {
            float[] vertices =
            {
                -1f, -1f, 0,
                1, -1, 0,
                1, 1, 0,
                -1, 1, 0
            };

            int[] faces =
            {
                0, 1, 2,
                0, 2, 3
            };

            var geometry = new Geometry { Faces = faces, Vertices = vertices, Items = new List<float[]> { vertices, vertices, vertices } };
            var dataSetService = new DataSetService();

            var fs = new GeometryFileService(dataSetService);
            string path = Path.GetTempFileName();
            fs.WriteGeometry(geometry, path);

            Geometry data = await fs.ReadGeometry(path);
            Debug.Assert(data.Vertices.SequenceEqual(vertices), "Vertices are not the same!");
            Debug.Assert(data.Faces.SequenceEqual(faces), "Faces are not the same!");
            AssertListOfFloatsAreEqual(new List<float[]> { vertices, vertices, vertices }, data.Items);


            Garden garden = new Garden();
            garden.Trees = new List<Tree>();

            var tree = new Tree();
            tree.Fruits = new CompoundType[]
            {
                new CompoundType{Height = 1.0, Width = 1.0}, 
                new CompoundType{Height = 1.1, Width = 1.1}, 
                new CompoundType{Height = 1.2, Width = 1.2}, 
            };

            garden.Trees.Add(tree);

            tree = new Tree();
            tree.Fruits = new CompoundType[]
            {
                new CompoundType{Height = 2.0, Width = 2.0}, 
                new CompoundType{Height = 2.1, Width = 2.1}, 
                new CompoundType{Height = 2.2, Width = 2.2}, 
            };

            garden.Trees.Add(tree);

            path = Path.GetTempFileName();
            GardenFileService gs = new GardenFileService();
            gs.WriteData(garden, path);

            var result = await gs.ReadData(path);

            Debug.Assert(garden.Trees.Count == result.Trees.Count);
            for (var i = 0; i < result.Trees.Count; i++)
            {
                var expectedItem = garden.Trees[i];
                var resultItem = result.Trees[i];
                Debug.Assert(expectedItem.Fruits.Length == resultItem.Fruits.Length);
                for (var j = 0; j < expectedItem.Fruits.Length; j++)
                {
                    var actualfruit = resultItem.Fruits[j];
                    var expectedfruit = expectedItem.Fruits[j];

                    Debug.Assert(Math.Abs(actualfruit.Height - expectedfruit.Height) < double.Epsilon);
                    Debug.Assert(Math.Abs(actualfruit.Width - expectedfruit.Width) < double.Epsilon);
                }
            }
        }

        private static void AssertListOfFloatsAreEqual(List<float[]> expected, List<float[]> actual)
        {
            Debug.Assert(expected.Count == actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                var expectedItem = actual[i];
                var resultItem = expected[i];
                Debug.Assert(expectedItem.Length == resultItem.Length);
                for (var j = 0; j < expectedItem.Length; j++)
                {
                    Debug.Assert(Math.Abs(expectedItem[j] - resultItem[j]) < double.Epsilon);
                }
            }
        }
    }
}