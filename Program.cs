// See https://github.com/Azure-Samples/ms-identity-dotnet-desktop-tutorial/tree/master/1-Calling-MSGraph/1-1-AzureAD#choose-the-azure-ad-tenant-where-you-want-to-create-your-applications
// and https://docs.microsoft.com/en-us/azure/storage/blobs/data-lake-storage-directory-file-acl-dotnet
using Azure.Storage.Files.DataLake;
using Azure.Identity;

namespace Console_Interactive_MultiTarget
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            var storageAcctName = System.Environment.GetEnvironmentVariable("STORAGE_ACCT_NAME");
            if (storageAcctName == null)
                    throw new Exception("Environment variable STORAGE_ACCT_NAME expected");
            
            var test = new InteractiveBrowserCredential();
            test.Authenticate();

            var client = new DataLakeServiceClient(new Uri($"https://{storageAcctName}.blob.core.windows.net/"), test);
            
            DataLakeFileSystemClient fileSystemClient = null;
            
            try
            {
                fileSystemClient = client.CreateFileSystem("myfilesystem")?.Value;
                if (fileSystemClient == null)
                    throw new Exception("Filesystem not created");
            }
            catch {
                fileSystemClient = client.GetFileSystemClient("myfilesystem");
            }
            
            DataLakeDirectoryClient myDirectory = null;
            try
            {
                myDirectory = fileSystemClient.CreateDirectory("MyDirectory")?.Value;
                if (myDirectory == null)
                    throw new Exception("Directory not created");
            }
            catch {
                myDirectory = fileSystemClient.GetDirectoryClient("myfilesystem");
            }
            
            
            var myFile = myDirectory.CreateFile("MyFile.txt")?.Value;
            if (myFile == null)
                throw new Exception("File not created");
            
            var tmpBytes = System.Text.Encoding.UTF8.GetBytes("Hello world!!!");

            using (var stream = myFile.OpenWrite(true))
            {
                stream.Write(tmpBytes,0,tmpBytes.Length);
                stream.Close();
            }
        }
    }
}