using Azure.Storage.Blobs;
using AzureStorageAccountService;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageAccountServiceTest
{
    [TestClass]
    public class StoageAccountTests
    {
        IBlobService blobService;

        [TestInitialize]
        public void Init()
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var config = builder.Build();

            var storage_connection_string = config["storage_connection_string"];

            var blobServiceClient = new BlobServiceClient(storage_connection_string);
            blobService = new BlobService(blobServiceClient: blobServiceClient);
        }

        [TestMethod]
        public async Task upload_file_to_storage_account_test()
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "MOCK_DATA.json");
            var json = File.ReadAllText(jsonPath);
            var stream = GenerateStreamFromString(json);
            var container = Guid.NewGuid().ToString();
            var response = await blobService.UploadInBlocks(container, "sample.json", stream);

            response.Should().NotBeEmpty();


            var downloadResponse = await blobService.DownloadBlob(container, "sample.json");


            var containerDeleteResponse = await blobService.DeleteContainer(container);
            containerDeleteResponse.Should().BeTrue();
        }

        [TestMethod]
        public async Task upload_binarydata_to_storage_account_test()
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "MOCK_DATA.json");
            var json = File.ReadAllText(jsonPath);
            var stream = GenerateStreamFromString(json);
            var binaryData = BinaryData.FromString(json);

            var container = Guid.NewGuid().ToString();
            var response = await blobService.Upload(container, "sample.json", binaryData);

            response.Should().NotBeNull();

            var containerDeleteResponse = await blobService.DeleteContainer(container);
            containerDeleteResponse.Should().BeTrue();
        }



        public MemoryStream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}