using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Newtonsoft.Json;

namespace AzureStorageAccountService
{
    public sealed class BlobService : IBlobService
    {
        private readonly BlobServiceClient blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
        }
        async Task<BlobContentInfo> IBlobService.Upload(string containerName, string blobName, BinaryData data)
        {

            
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            blobContainerClient.CreateIfNotExists();

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            var commitResponse = await blobClient.UploadAsync(data);

            return commitResponse;
        }

        async Task<bool> IBlobService.DeleteContainer(string containerName)
        {


            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var response = await blobContainerClient.DeleteIfExistsAsync();


            return response.Value;
        }


        async Task<string> IBlobService.UploadInBlocksFS(string containerName, string blobName, FileStream memoryStream)
        {

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            blobContainerClient.CreateIfNotExists();
            BlockBlobClient blobClient = blobContainerClient.GetBlockBlobClient(blobName);

            int maxSize = 1 * 1024 * 1024; // 4 MB

            if (memoryStream.Length > maxSize)
            {

                Stream s = memoryStream;
                int streamEnd = Convert.ToInt32(s.Length);
                byte[] data = new byte[streamEnd];
                s.Read(data, 0, streamEnd);


                int id = 0;
                int byteslength = data.Length;
                int bytesread = 0;
                int index = 0;
                List<string> blocklist = new List<string>();
                int numBytesPerChunk = 250 * 1024; //250KB per block

                do
                {
                    byte[] buffer = new byte[numBytesPerChunk];
                    int limit = index + numBytesPerChunk;
                    for (int loops = 0; index < limit; index++)
                    {
                        buffer[loops] = data[index];
                        loops++;
                    }
                    bytesread = index;


                    using (var stream = new MemoryStream(buffer, true))
                    {
                        string blockIdBase64 = Convert.ToBase64String(BitConverter.GetBytes(id));
                        await blobClient.StageBlockAsync(blockIdBase64, stream, null);
                        blocklist.Add(blockIdBase64);
                        id++;
                    }


                } while (byteslength - bytesread > numBytesPerChunk);

                int final = byteslength - bytesread;
                byte[] finalbuffer = new byte[final];
                for (int loops = 0; index < byteslength; index++)
                {
                    finalbuffer[loops] = data[index];
                    loops++;
                }
                string blockId = Convert.ToBase64String(BitConverter.GetBytes(id));
                await blobClient.StageBlockAsync(blockId, new MemoryStream(finalbuffer, true), null);
                blocklist.Add(blockId);

                var commitResponse = await blobClient.CommitBlockListAsync(blocklist);

                return blobClient.Name;

            }
            else
            {
                await blobClient.UploadAsync(memoryStream);
                return blobClient.Name;
            }
        }


        async Task<string> IBlobService.UploadInBlocks(string containerName, string blobName, MemoryStream memoryStream)
        {

            //   var blobServiceClient = new BlobServiceClient(storageConnectionString, options);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync();
            BlockBlobClient blobClient = blobContainerClient.GetBlockBlobClient(blobName);

            int maxSize = 1 * 1024 * 1024; // 4 MB

            if (memoryStream.Length > maxSize)
            {

                byte[] data = memoryStream.ToArray();
                int id = 0;
                int byteslength = data.Length;
                int bytesread = 0;
                int index = 0;
                List<string> blocklist = new List<string>();
                int numBytesPerChunk = 250 * 1024; //250KB per block

                do
                {
                    byte[] buffer = new byte[numBytesPerChunk];
                    int limit = index + numBytesPerChunk;
                    for (int loops = 0; index < limit; index++)
                    {
                        buffer[loops] = data[index];
                        loops++;
                    }
                    bytesread = index;


                    using (var stream = new MemoryStream(buffer, true))
                    {
                        string blockIdBase64 = Convert.ToBase64String(BitConverter.GetBytes(id));
                        //string blockID = Convert.ToBase64String
                        //    (Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));

                        await blobClient.StageBlockAsync(blockIdBase64, stream, null);
                        blocklist.Add(blockIdBase64);
                        id++;
                    }


                } while (byteslength - bytesread > numBytesPerChunk);

                int final = byteslength - bytesread;
                byte[] finalbuffer = new byte[final];
                for (int loops = 0; index < byteslength; index++)
                {
                    finalbuffer[loops] = data[index];
                    loops++;
                }
                string blockId = Convert.ToBase64String(BitConverter.GetBytes(id));
                await blobClient.StageBlockAsync(blockId, new MemoryStream(finalbuffer, true), null);
                blocklist.Add(blockId);

                var commitResponse = await blobClient.CommitBlockListAsync(blocklist);

                return blobClient.Name;

            }
            else
            {
                await blobClient.UploadAsync(memoryStream);
                return blobClient.Name;
            }
        }

        async Task<string> IBlobService.UploadBytesInBlocks(string containerName, string blobName, byte[] data)
        {

            //   var blobServiceClient = new BlobServiceClient(storageConnectionString, options);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            blobContainerClient.CreateIfNotExists();
            BlockBlobClient blobClient = blobContainerClient.GetBlockBlobClient(blobName);

            int maxSize = 1 * 1024 * 1024; // 4 MB

            if (data.Length > maxSize)
            {
                int id = 0;
                int byteslength = data.Length;
                int bytesread = 0;
                int index = 0;
                List<string> blocklist = new List<string>();
                int numBytesPerChunk = 250 * 1024; //250KB per block

                do
                {
                    byte[] buffer = new byte[numBytesPerChunk];
                    int limit = index + numBytesPerChunk;
                    for (int loops = 0; index < limit; index++)
                    {
                        buffer[loops] = data[index];
                        loops++;
                    }
                    bytesread = index;


                    using (var stream = new MemoryStream(buffer, true))
                    {
                        string blockIdBase64 = Convert.ToBase64String(BitConverter.GetBytes(id));
                        //string blockID = Convert.ToBase64String
                        //    (Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));

                        await blobClient.StageBlockAsync(blockIdBase64, stream, null);
                        blocklist.Add(blockIdBase64);
                        id++;
                    }


                } while (byteslength - bytesread > numBytesPerChunk);

                int final = byteslength - bytesread;
                byte[] finalbuffer = new byte[final];
                for (int loops = 0; index < byteslength; index++)
                {
                    finalbuffer[loops] = data[index];
                    loops++;
                }

                using (var stream = new MemoryStream(finalbuffer, true))
                {
                    string blockId = Convert.ToBase64String(BitConverter.GetBytes(id));
                    await blobClient.StageBlockAsync(blockId, stream, null);
                    blocklist.Add(blockId);
                }

                await blobClient.CommitBlockListAsync(blocklist);

                return blobClient.Name;

            }
            else
            {
                using (var stream = new MemoryStream(data, true))
                {
                    await blobClient.UploadAsync(stream);
                }
                return blobClient.Name;
            }
        }


        async Task<T> IBlobService.DownloadBlob<T>(string containerName, string blobName)
        {

            BlobDownloadOptions blobDownloadOptions = new BlobDownloadOptions()
            {

            };
            //   var blobServiceClient = new BlobServiceClient(storageConnectionString, options);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            blobContainerClient.CreateIfNotExists();
            BlockBlobClient blobClient = blobContainerClient.GetBlockBlobClient(blobName);

            using (var stream = new MemoryStream())
            {
                var downloadResponse = await blobClient.DownloadToAsync(stream);

                stream.Position = 0;
                StreamReader sr = new StreamReader(stream);
                string myStr = sr.ReadToEnd();

                return JsonConvert.DeserializeObject<T>(myStr);
            }
        }

        async Task<BlobDownloadStreamingResult> IBlobService.DownloadBlobStream(string containerName, string blobName)
        {

            BlobDownloadOptions blobDownloadOptions = new BlobDownloadOptions()
            {

            };
            //   var blobServiceClient = new BlobServiceClient(storageConnectionString, options);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlockBlobClient blobClient = blobContainerClient.GetBlockBlobClient(blobName);

            var reponse = await blobClient.DownloadStreamingAsync();

            return reponse.Value;
        }

        async Task<T> IBlobService.DownloadBlobAsBinaryData<T>(string containerName, string blobName)
        {

            BlobDownloadOptions blobDownloadOptions = new BlobDownloadOptions()
            {

            };
            //   var blobServiceClient = new BlobServiceClient(storageConnectionString, options);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlockBlobClient blobClient = blobContainerClient.GetBlockBlobClient(blobName);

            var reponse = await blobClient.DownloadContentAsync();

            return reponse.Value.Content.ToObjectFromJson<T>();
        }

        public void Dispose()
        {
            Console.WriteLine("- {0} was disposed!", GetType().Name);
        }
    }
}
