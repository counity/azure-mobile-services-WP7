using System;
using System.Net;
using ServiceStack.Text;
using coUnity.WindowsAzure.MobileServices.Util;
using ServiceStack.Text.Common;

namespace coUnity.WindowsAzure.MobileServices
{
    public partial class MobileServiceClient
    {
        /// <summary>
        /// SQL Server min value for date time. http://msdn.microsoft.com/en-us/library/system.data.sqltypes.sqldatetime(v=vs.110).aspx
        /// </summary>
        private static DateTime SqlMinDateTime = new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        static MobileServiceClient()
        {
            //configure JsonSerializer
            JsConfig.EmitCamelCaseNames = true;
            JsConfig.DateHandler = JsonDateHandler.ISO8601;
            JsConfig<DateTime>.SerializeFn =
                dateTime =>
                dateTime.ToUniversalTime().Equals(default(DateTime))
                    ? DateTimeSerializer.ToDateTimeString(SqlMinDateTime)
                    : DateTimeSerializer.ToDateTimeString(dateTime);
            JsConfig<DateTime>.DeSerializeFn = dateString =>
                                                   {
                                                       var dateTime = DateTime.Parse(dateString);
                                                       return dateTime.ToUniversalTime().Equals(SqlMinDateTime) ? default(DateTime) : dateTime;
                                                   };
        }

        /// <summary>
        /// Name of the config setting that stores the installation ID.
        /// </summary>
        private const string MobileServiceApplicationIdKey = "MobileServices.ApplicationId";

        /// <summary>
        /// Content type for request bodies and accepted responses.
        /// </summary>
        private const string RequestJsonContentType = "application/json";

        /// <summary>
        /// Relative URI fragment of the login endpoint.
        /// </summary>
        private const string LoginUriFragment = "login";

        /// <summary>
        /// Name of the application key header included when there's a key.
        /// </summary>
        private const string RequestApplicationKeyHeader = "X-ZUMO-APPLICATION";

        /// <summary>
        /// Name of the authentication header included when the user's logged
        /// in.
        /// </summary>
        private const string RequestAuthenticationHeader = "X-ZUMO-AUTH";

        /// <summary>
        /// Name of the Installation ID header included on each request.
        /// </summary>
        private const string RequestInstallationIdHeader = "X-ZUMO-INSTALLATION-ID";

        private string _currentUserAuthenticationToken;
        private string _applicationInstallationId;

        public MobileServiceClient(string applicationUri, Guid applicationId) 
            : this(applicationUri, null, applicationId)
        {
            
        }

        public MobileServiceClient(string applicationUri, string applicationKey, Guid applicationId)
            : this(new Uri(applicationUri), applicationKey, applicationId)
        {
            
        }

        public MobileServiceClient(Uri applicationUri, string applicationKey, Guid applicationId)
        {
            if (applicationUri == null)
                throw new ArgumentException("applicationUri");

            ApplicationUri = applicationUri;
            ApplicationKey = applicationKey;

            _applicationInstallationId = applicationId.ToString();
        }

        /// <summary>
        /// Gets the Uri to the Mobile Services application that is provided by
        /// the call to MobileServiceClient(...).
        /// </summary>
        public Uri ApplicationUri { get; private set; }

        /// <summary>
        /// Gets the Mobile Services application's name that is provided by the
        /// call to MobileServiceClient(...).
        /// </summary>
        public string ApplicationKey { get; private set; }

        /// <summary>
        /// The current authenticated user provided after a successful call to
        /// MobileServiceClient.Login().
        /// </summary>
        public MobileServiceUser CurrentUser { get; private set; }

        public MobileServiceUser Login(string liveAuthToken)
        {
            //http://msdn.microsoft.com/en-us/library/windowsazure/jj710106.aspx
            var stringResponse = PerformRequest(
                "POST", LoginUriFragment + "?mode=authenticationToken", 
                new LoginRequest(liveAuthToken).ToJson());

            var respose = stringResponse.FromJson<LoginResponse>();

            _currentUserAuthenticationToken = respose.AuthenticationToken;
            CurrentUser = respose.User;
            return CurrentUser;
        }

        public void Logout()
        {
            CurrentUser = null;
            _currentUserAuthenticationToken = null;
        }

        /// <summary>
        /// Gets a reference to a table and its data operations.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>A reference to the table.</returns>
        public IMobileServiceTable<T> GetTable<T>() where T : class
        {
            return new MobileServiceTable<T>(this);
        }

        internal string PerformRequest(string method, string uriFragment, string jsonContent)
        {
            // Create the web request
            var request =
                (HttpWebRequest)
                WebRequest.Create(new Uri(ApplicationUri, uriFragment));
            request.Method = method.ToUpper();
            request.Accept = RequestJsonContentType;

            // Set Mobile Services authentication, application, and telemetry
            // headers
            request.Headers[RequestInstallationIdHeader] =
                _applicationInstallationId;
            if (!string.IsNullOrEmpty(ApplicationKey))
            {
                request.Headers[RequestApplicationKeyHeader] = ApplicationKey;
            }
            if (!string.IsNullOrEmpty(_currentUserAuthenticationToken))
            {
                request.Headers[RequestAuthenticationHeader] =
                    _currentUserAuthenticationToken;
            }

            //Add any request as JSON
            if (!string.IsNullOrEmpty(jsonContent))
            {
                request.ContentType = RequestJsonContentType;
                request.SetRequestPayload(jsonContent);
            }

            // Send the request and get the response back as JSON
            var response = (HttpWebResponse)request.GetResponse();

            return response.GetResponsePayload();
        }
    }
}
