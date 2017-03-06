using System.Net.Http;

namespace DotNet.VersionManager.Utils
{
    public static class Http
    {
        public static readonly HttpClient DefaultHttpClient = new HttpClient();
    }
}
