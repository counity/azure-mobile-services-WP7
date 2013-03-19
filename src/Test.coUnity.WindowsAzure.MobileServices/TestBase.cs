using System;
using coUnity.WindowsAzure.MobileServices;

namespace Test.coUnity.WindowsAzure.MobileServices
{
    public class TestBase
    {
        private Guid ApplicationId { get { return Guid.Parse("A26A0578-70F9-4ACC-88CC-0F9F7F9C8ADC"); } }
        private string ApplicationUri { get { return "YOUR_APPLICATION_URL"; } }
        private string ApplicationKey { get { return "YOUR_APPLICATION_KEY"; } }

        protected MobileServiceClient Client
        {
            get { return new MobileServiceClient(ApplicationUri, ApplicationKey, ApplicationId);}
        } 
    }
}
