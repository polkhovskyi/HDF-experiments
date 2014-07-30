using System;
using HDF5DotNet;

namespace HDF_experiments.HDF
{
    public class HdfHelpers
    {
        public static HdfWrapper<H5FileId> WorkWithFile(Func<H5FileId> idFunc)
        {
            return new HdfWrapper<H5FileId>(idFunc, H5F.close);
        }

        public static HdfWrapper<H5DataSetId> WorkWithDataSet(Func<H5DataSetId> idFunc)
        {
            return new HdfWrapper<H5DataSetId>(idFunc, H5D.close);
        }

        public static HdfWrapper<H5DataSpaceId> WorkWithSpace(Func<H5DataSpaceId> idFunc)
        {
            return new HdfWrapper<H5DataSpaceId>(idFunc, H5S.close);
        }

        public static HdfWrapper<H5GroupId> WorkWithGroup(Func<H5GroupId> idFunc)
        {
            return new HdfWrapper<H5GroupId>(idFunc, H5G.close);
        }

        public static HdfWrapper<H5AttributeId> WorkWithAttribute(Func<H5AttributeId> idFunc)
        {
            return new HdfWrapper<H5AttributeId>(idFunc, H5A.close);
        }

        public static HdfWrapper<H5DataTypeId> WorkWithDatatype(Func<H5DataTypeId> idFunc)
        {
            return new HdfWrapper<H5DataTypeId>(idFunc, H5T.close);
        }
    }
}