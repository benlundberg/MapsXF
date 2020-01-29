using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.Tasks.Offline;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Esri.Core.Services
{
    public class OfflineMapService
    {
        public async Task DownloadMapAsync(Geometry area, string downloadFolderPath)
        {
			try
            {
                AuthenticationManager.Current.ChallengeHandler = new ChallengeHandler(CredentialService.CreateEsriCredential);

                Uri imageryUri = new Uri(@"https://tiledbasemaps.arcgis.com/arcgis/rest/services/World_Imagery/MapServer");

                Uri hillshadeUri = new Uri(@"https://tiledbasemaps.arcgis.com/arcgis/rest/services/Elevation/World_Hillshade/MapServer");
                
                await ExportTilesAsync(area, downloadFolderPath + "_imagery", imageryUri);

                await ExportTilesAsync(area, downloadFolderPath + "_hillshade", hillshadeUri);
            }
            catch (Exception ex)
			{
                Debug.WriteLine(ex.Message);
                throw;
			}
        }

        private async Task ExportTilesAsync(Geometry area, string downloadFolderPath, Uri uri)
        {
            var offlineTask = await ExportTileCacheTask.CreateAsync(uri);

            var parameters = new ExportTileCacheParameters
            {
                AreaOfInterest = area,
                CompressionQuality = 100
            };

            parameters.LevelIds.Add(4);
            parameters.LevelIds.Add(6);
            parameters.LevelIds.Add(8);
            parameters.LevelIds.Add(12);

            var job = offlineTask.ExportTileCache(parameters, downloadFolderPath + ".tpk");

            job.Start();

            await job.GetResultAsync();
        }

        public async Task<Layer> LoadLayerAsync(string path)
        {
            try
            {
                ArcGISTiledLayer layer = new ArcGISTiledLayer(new Uri(path))
                {
                    Id = path,
                    Name = path,
                    MinScale = 8000000,
                    MaxScale = 50
                };

                await layer.LoadAsync();

                return layer;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }
    }
}
