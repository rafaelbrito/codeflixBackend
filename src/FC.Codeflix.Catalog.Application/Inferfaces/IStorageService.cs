using System.Net.Mime;

namespace FC.Codeflix.Catalog.Application.Inferfaces
{
    public interface IStorageService
    {
        public Task Delete(string filePath, CancellationToken cancellationToken);

        public Task<string> Upload(
            string fileName, 
            Stream fileStream, 
            string contentType, 
            CancellationToken cancellationToken);
    }
}
