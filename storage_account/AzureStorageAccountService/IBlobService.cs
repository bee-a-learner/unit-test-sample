using Azure.Storage.Blobs.Models;

namespace AzureStorageAccountService
{
    public interface IBlobService : IDisposable
    {
        Task<BlobContentInfo> Upload(string containerName, string blobName, BinaryData data);
        Task<bool> DeleteContainer(string containerName);

        Task<string> UploadInBlocks(string containerName, string blobName, MemoryStream memoryStream);

        Task<string> UploadInBlocksFS(string containerName, string blobName, FileStream fileStream);

        Task<string> UploadBytesInBlocks(string containerName, string blobName, byte[] data);

        Task<T> DownloadBlob<T>(string containerName, string blobName);

        Task<BlobDownloadStreamingResult> DownloadBlobStream(string containerName, string blobName);

        Task<T> DownloadBlobAsBinaryData<T>(string containerName, string blobName);

    }
}