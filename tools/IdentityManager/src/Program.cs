using System;

namespace AD.IdentityManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var identityManager = new IdentityManager();
            identityManager.CreateUsers().Wait();
        }
    }
}
