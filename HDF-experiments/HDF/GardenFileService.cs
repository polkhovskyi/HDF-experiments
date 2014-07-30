using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HDF5DotNet;
using HDF_experiments.DTO;

namespace HDF_experiments.HDF
{
    public class GardenFileService
    {
        private List<string> groups = new List<string>();
        private H5DataTypeId my_type;

        public GardenFileService()
        {
            my_type = H5T.create(H5T.CreateClass.COMPOUND, Marshal.SizeOf(typeof(CompoundType)));
            H5T.insert(my_type, "Height", 0, H5T.H5Type.NATIVE_DOUBLE);
            H5T.insert(my_type, "Width", sizeof(double), H5T.H5Type.NATIVE_DOUBLE);
        }

        private int Target(H5GroupId @group, string objectName, object parameter)
        {
            groups.Add(objectName);
            return 0;
        }

        private byte[] GetBytes(CompoundType str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        private static void WriteAttribute(H5ObjectWithAttributes target, string name, string value)
        {
            byte[] strdata = Encoding.UTF8.GetBytes(value);
            using (var dtype = HdfHelpers.WorkWithDatatype(() => { var type = H5T.copy(H5T.H5Type.C_S1); H5T.setSize(type, strdata.Length); return type; }))
            {
                using (var space = HdfHelpers.WorkWithSpace(() => H5S.create(H5S.H5SClass.SCALAR)))
                {
                    using (var attribute = HdfHelpers.WorkWithAttribute(() => H5A.create(target, name, dtype.Id, space.Id)))
                    {
                        H5A.write(attribute.Id, dtype.Id, new H5Array<byte>(strdata));
                    }
                }
            }
        }

        public async Task<Garden> ReadData(string pathToFile)
        {
            groups = new List<string>();
            var result = new Garden();
            
            result.Trees = new List<Tree>();

            await Task.Run(() =>
            {
                try
                {
                    using (var file = HdfHelpers.WorkWithFile(() => H5F.open(pathToFile, H5F.OpenMode.ACC_RDONLY)))
                    {
                        int startIndex = 0;
                        H5G.iterate(file.Id, "/garden", (Target), null, ref startIndex);
                        foreach (var groupItem in groups)
                        {
                            var item = new Tree();
                            using (var group = HdfHelpers.WorkWithGroup(() => H5G.open(file.Id, "/garden/" + groupItem)))
                            {
                                using (var dataSet = HdfHelpers.WorkWithDataSet(() => H5D.open(group.Id, "fruits")))
                                {
                                    using (var space = HdfHelpers.WorkWithSpace(() => H5D.getSpace(dataSet.Id)))
                                    {
                                        var arrayLenght = H5S.getSimpleExtentDims(space.Id)[0];
                                        item.Fruits = new CompoundType[arrayLenght];
                                        H5D.read(dataSet.Id, my_type, new H5Array<CompoundType>(item.Fruits));
                                    }
                                }
                            }

                            result.Trees.Add(item);
                        }
                    }
                }
                catch
                {
                    //DO not show exceptions if we have no file.
                }

            });

            return result;
        }

        public void WriteData(Garden garden, string path)
        {
            using (var file = HdfHelpers.WorkWithFile(() => H5F.create(path, H5F.CreateMode.ACC_TRUNC)))
            {
                using (var rootGroup = HdfHelpers.WorkWithGroup(() => H5G.create(file.Id, "garden")))
                {
                    for (int index = 0; index < garden.Trees.Count; index++)
                    {
                        var tree = garden.Trees[index];

                        var name = string.Format("tree{0}", index);
                        using (var group = HdfHelpers.WorkWithGroup(() => H5G.create(rootGroup.Id, name)))
                        {
                            WriteAttribute(group.Id, "name", name);

                            var dims = new long[1];
                            dims[0] = tree.Fruits.Length;
                            using (var space = HdfHelpers.WorkWithSpace(() => H5S.create_simple(1, dims)))
                            {
                                using (
                                    var dataSet = HdfHelpers.WorkWithDataSet(() => H5D.create(group.Id, "fruits", my_type, space.Id)))
                                {
                                    var array = tree.Fruits.SelectMany(GetBytes).ToArray();
                                    H5D.write(dataSet.Id, my_type, new H5Array<byte>(array));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
