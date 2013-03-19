namespace coUnity.WindowsAzure.MobileServices
{
    //http://msdn.microsoft.com/en-us/library/windowsazure/jj710106.aspx
    public class LoginRequest
    {
        public LoginRequest(string authenticationToken)
        {
            this.authenticationToken = authenticationToken;
        }

        public string authenticationToken { get; private set; }
    }

    //http://msdn.microsoft.com/en-us/library/windowsazure/jj710106.aspx
    public class LoginResponse
    {
        public MobileServiceUser User { get; set; }
        public string AuthenticationToken { get; set; }
    }
   
}
