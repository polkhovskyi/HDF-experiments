using System.Collections.Generic;
using System.Threading.Tasks;
using HDF5DotNet;
using HDF_experiments.DTO;

namespace HDF_experiments.HDF
{
    public class GeometryFileService
    {
        private readonly DataSetService _dataSetService;

        public GeometryFileService(DataSetService dataSetService)
        {
            _dataSetService = dataSetService;
        }

        public async Task<Geometry> ReadGeometry(string pathToFile)
        {
            Geometry result = new Geometry();
            await Task.Run(() =>
            {
                using (var file = HdfHelpers.WorkWithFile(() => H5F.open(pathToFile, H5F.OpenMode.ACC_RDONLY)))
                {
                    result.Vertices = _dataSetService.ReadFloatDataSet(file.Id, "/vertices");
                    result.Items = _dataSetService.ReadFloatArrays(file.Id, "/items");
                    result.Faces = _dataSetService.ReadIntDataSet(file.Id, "/faces");
                }
            });

            return result;
        }

        public void WriteGeometry(Geometry geometry, string path)
        {
            using (var file = HdfHelpers.WorkWithFile(() => H5F.create(path, H5F.CreateMode.ACC_TRUNC)))
            {
             
                _dataSetService.WriteFloatDataSet(file.Id, geometry.Vertices, "/vertices");

                using (var itemsGroup = HdfHelpers.WorkWithGroup(() => H5G.create(file.Id, "items")))
                {
                    _dataSetService.WriteFloatDataSets(itemsGroup.Id,geometry.Items, "/items", "/array{0}");
                }

                _dataSetService.WriteIntDataSet(file.Id, geometry.Faces, "/faces");
            }
        }
    }
}