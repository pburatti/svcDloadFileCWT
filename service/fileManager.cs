using Microsoft.AspNetCore.StaticFiles;
using webapi_DLoadFile.Helper;

namespace webapi_DLoadFile.service
{
    public class fileManager : IfileManager
    {
        public async Task<(Byte[], string, string)> DownloadFile(string uncFileName)
        {
            try
            {
                var _GetFilePath = Common.GetFilePath(uncFileName);
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(_GetFilePath, out var _ContentType))
                {
                    _ContentType = "application/octet-stream";
                }
                var _ReadAllBytesAsync = await File.ReadAllBytesAsync(_GetFilePath);
                return (_ReadAllBytesAsync, _ContentType, Path.GetFileName(_GetFilePath));
            }
            catch (Exception ex) {
                throw ex;            
            }
        }
    }
}
