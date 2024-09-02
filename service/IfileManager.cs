namespace webapi_DLoadFile.service
{
    public interface IfileManager
    {
        Task<(Byte[], string, string)> DownloadFile(string uncFileName);
    }
}
