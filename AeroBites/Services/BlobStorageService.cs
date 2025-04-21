using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Threading.Tasks;

namespace AeroBites.Services
{
    public class BlobStorageService(IConfiguration config)
    {
        private readonly BlobServiceClient _client = new BlobServiceClient(config["ConnectionStrings:AzureBlob"]);

        private BlobContainerClient GetContainer()
        {
            return this._client.GetBlobContainerClient("web");
        }

        public async Task<string> Upload(IFormFile Image) {
            var blob_container = this.GetContainer();
            await blob_container.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            string extension = Image.ContentType == "image/png" ? ".png" : ".jpg";
            string blob_name = $"{Guid.NewGuid().ToString()}{extension}";

            var blob = blob_container.GetBlobClient(blob_name);
            var blob_http = new BlobHttpHeaders { ContentType = Image.ContentType };

            await blob.UploadAsync(Image.OpenReadStream(), blob_http);

            return "https://staerobites.blob.core.windows.net/web/" + blob_name;
        }

        public async Task<bool> Delete(string url)
        {
            var blob_container = this.GetContainer();
            await blob_container.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            await blob_container.DeleteBlobIfExistsAsync(url.Split("/").Last());
            return true;
        }
    }
}
