using System.Collections.Generic;
using System.Linq;
using HDF5DotNet;

namespace HDF_experiments.HDF
{
    public class DataSetService
    {
        public void WriteFloatDataSets(H5FileOrGroupId id, IList<float[]> outlines, string groupName, string setName)
        {
            for (int index = 0; index < outlines.Count; index++)
            {
                float[] item = outlines[index];

                var dims = new long[] {item.Length};
                using (HdfWrapper<H5DataSpaceId> space = HdfHelpers.WorkWithSpace(() => H5S.create_simple(1, dims)))
                {
                    string dataSetName = string.Format(groupName + setName, index);
                    using (
                        HdfWrapper<H5DataSetId> dataSet =
                            HdfHelpers.WorkWithDataSet(
                                () => H5D.create(id, dataSetName, H5T.H5Type.NATIVE_FLOAT, space.Id)))
                    {
                        H5D.write(dataSet.Id, new H5DataTypeId(H5T.H5Type.NATIVE_FLOAT), new H5Array<float>(item));
                    }
                }
            }
        }

        public float[] ReadFloatDataSet(H5FileOrGroupId groupId, string groupItem)
        {
            var result = new float[0];

            try
            {
                using (HdfWrapper<H5DataSetId> dataSet = HdfHelpers.WorkWithDataSet(() => H5D.open(groupId, groupItem)))
                {
                    using (HdfWrapper<H5DataSpaceId> space = HdfHelpers.WorkWithSpace(() => H5D.getSpace(dataSet.Id)))
                    {
                        long arrayLenght = H5S.getSimpleExtentDims(space.Id)[0];
                        result = new float[arrayLenght];
                        H5D.read(dataSet.Id, new H5DataTypeId(H5T.H5Type.NATIVE_FLOAT), new H5Array<float>(result));
                    }
                }
            }
            catch (H5DopenException)
            {
                result = null;
            }

            return result;
        }

        public List<float[]> ReadFloatArrays(H5FileId id, string rootGroup)
        {
            var result = new List<float[]>();
            int startIndex = 0;
            var groups = new List<string>();
            H5G.iterate(id, rootGroup, (@group, name, parameter) =>
            {
                groups.Add(name);
                return 0;
            }, null, ref startIndex);

            using (HdfWrapper<H5GroupId> group = HdfHelpers.WorkWithGroup(() => H5G.open(id, rootGroup)))
            {
                result.AddRange(groups.Select(groupItem => ReadFloatDataSet(@group.Id, groupItem)));
            }

            return result;
        }

        public void WriteFloatDataSet(H5FileOrGroupId id, float[] data, string dataSetName)
        {
            var dims = new long[1];
            dims[0] = data.Length;
            using (HdfWrapper<H5DataSpaceId> space = HdfHelpers.WorkWithSpace(() => H5S.create_simple(1, dims)))
            {
                using (
                    HdfWrapper<H5DataSetId> dataSet =
                        HdfHelpers.WorkWithDataSet(() => H5D.create(id, dataSetName, H5T.H5Type.NATIVE_FLOAT, space.Id))
                    )
                {
                    H5D.write(dataSet.Id, new H5DataTypeId(H5T.H5Type.NATIVE_FLOAT), new H5Array<float>(data));
                }
            }
        }

        public void WriteIntDataSet(H5FileOrGroupId id, int[] data, string dataSetName)
        {
            var dims = new long[1];
            dims[0] = data.Length;
            using (HdfWrapper<H5DataSpaceId> space = HdfHelpers.WorkWithSpace(() => H5S.create_simple(1, dims)))
            {
                using (
                    HdfWrapper<H5DataSetId> dataSet =
                        HdfHelpers.WorkWithDataSet(() => H5D.create(id, dataSetName, H5T.H5Type.NATIVE_INT, space.Id)))
                {
                    H5D.write(dataSet.Id, new H5DataTypeId(H5T.H5Type.NATIVE_INT), new H5Array<int>(data));
                }
            }
        }

        public int[] ReadIntDataSet(H5FileOrGroupId groupId, string groupItem)
        {
            var result = new int[0];

            try
            {
                using (HdfWrapper<H5DataSetId> dataSet = HdfHelpers.WorkWithDataSet(() => H5D.open(groupId, groupItem)))
                {
                    using (HdfWrapper<H5DataSpaceId> space = HdfHelpers.WorkWithSpace(() => H5D.getSpace(dataSet.Id)))
                    {
                        long arrayLenght = H5S.getSimpleExtentDims(space.Id)[0];
                        result = new int[arrayLenght];
                        H5D.read(dataSet.Id, new H5DataTypeId(H5T.H5Type.NATIVE_INT), new H5Array<int>(result));
                    }
                }
            }
            catch (H5DopenException)
            {
                result = null;
            }

            return result;
        }
    }
}