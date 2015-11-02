using System;
using System.Configuration;
using StampsDemo.StampsServiceReference;

namespace StampsDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var address = new Address { Address1 = "1990 E Grand Ave", City = "El Segundo", State = "CA", ZIPCode = "90245", FullName = "Shipping Department" };

            GetCleanAddress(address);
            Console.WriteLine(
                $"Cleaned Address: {address.FullName}, {address.Company}, {address.Address1}, {address.Address2}, {address.Address3},{address.City},{address.State}, {address.ZIPCode}");
        }

        private static Address GetCleanAddress(Address address)
        {
            var intergrationId = ConfigurationManager.AppSettings["Stamps.Com.IntegratorId"];
            var userName = ConfigurationManager.AppSettings["Stamps.Com.UserName"];
            var password = ConfigurationManager.AppSettings["Stamps.Com.Password"];

            var swsim = new Swsimv42SoapClient();

            var credential = new Credentials
            {
                IntegrationID = new Guid(intergrationId),
                Username = userName,
                Password = password
            };
            DateTime lastLoginTime;
            bool clearCredential;
            string loginBannerText;
            bool passworExpired;
            bool codewordsSet;
            var authenticator = swsim.AuthenticateUser(credential, out lastLoginTime, out clearCredential, out loginBannerText,
                out passworExpired, out codewordsSet);
            System.Console.WriteLine(authenticator);


            CleanseAddressRequest request = new CleanseAddressRequest
            {
                Address = address
            };

            ResidentialDeliveryIndicatorType residential;
            bool addressMatch;
            bool cityStateZipOk;
            bool? isPoBox;
            Address[] candidateAddresses;
            StatusCodes statusCodes;
            RateV15[] rates;
            swsim.CleanseAddress(authenticator, ref request.Address, "90232", out addressMatch, out cityStateZipOk,
                out residential, out isPoBox, out candidateAddresses, out statusCodes, out rates);
            Console.WriteLine(addressMatch);
            if (addressMatch)
            {
                address = request.Address;
            }
            return address;
            
        }
    }
}
