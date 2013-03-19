using System.IO;
using System.Net;
using System.Text;

namespace coUnity.WindowsAzure.MobileServices.Util
{
    public static class WebResponseExtensions
    {
        public static string GetResponsePayload(this WebResponse response)
        {
            var stream = response.GetResponseStream();
            var streamReader = new StreamReader(stream, Encoding.UTF8);
            var retVal = streamReader.ReadToEnd();
            streamReader.Close();
            stream.Close();
            return retVal;
        }
    }
}