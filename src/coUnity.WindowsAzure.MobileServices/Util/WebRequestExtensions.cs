using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace coUnity.WindowsAzure.MobileServices.Util
{
    public static class WebRequestExtensions
    {
        public static Stream GetRequestStream(this WebRequest request)
        {
            var resetEvent = new AutoResetEvent(false);
            var asyncResult = request.BeginGetRequestStream(r => resetEvent.Set(), null);
            resetEvent.WaitOne();
            return request.EndGetRequestStream(asyncResult);
        }

        public static WebResponse GetResponse(this WebRequest request)
        {
            var resetEvent = new AutoResetEvent(false);
            var asyncResult = request.BeginGetResponse(r => resetEvent.Set(), null);
            resetEvent.WaitOne();
            return request.EndGetResponse(asyncResult);
        }

        public static void SetRequestPayload(this WebRequest request, string payload)
        {
            var postData = Encoding.UTF8.GetBytes(payload);
            var requestStream = request.GetRequestStream();
            requestStream.Write(postData, 0, postData.Length);
            requestStream.Flush();
            requestStream.Close();
        }
    }
}
