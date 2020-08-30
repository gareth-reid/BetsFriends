using System;
namespace EssentialUIKit.DataService
{
    public static class ApiDataService
    {
#if DEBUG
        public const string HorseVenuesApi = "http://localhost:7071/api/BFHorseVenues?";//?mock=true";
        public const string CreateQuaddieGroupApi = "http://localhost:7071/api/CreateQuaddieGroup?";//?mock=true";
        public const string AddUserApi = "http://localhost:7071/api/AddUser?";//?mock=true";
        public const string CheckUserApi = "http://localhost:7071/api/CheckUser?";//?mock=true";
        public const string GetQuaddieGroupsApi = "http://localhost:7071/api/GetQuaddieGroup?";//?mock=true";
        public const string HorseRunnerApi = "http://localhost:7071/api/BFHorseRunners?";//?mock=true";
        public const string QuaddieBuilderApi = "http://localhost:7071/api/QuaddieBuilder?";//?mock=true";


#else
        public const string HorseVenuesApi = "http://betsfriendsapi.azurewebsites.net/api/BFHorseVenues?";//?mock=true";
        public const string CreateQuaddieGroupApi = "http://betsfriendsapi.azurewebsites.net/api/CreateQuaddieGroup?";//?mock=true";
        public const string AddUserApi = "http://betsfriendsapi.azurewebsites.net/api/AddUser?";//?mock=true";
        public const string CheckUserApi = "http://betsfriendsapi.azurewebsites.net/api/CheckUser?";//?mock=true";
        public const string GetQuaddieGroupsApi = "http://betsfriendsapi.azurewebsites.net/api/GetQuaddieGroup?";//?mock=true";
        public const string HorseRunnerApi = "http://betsfriendsapi.azurewebsites.net/api/BFHorseRunners?";//?mock=true";
        public const string QuaddieBuilderApi = "http://betsfriendsapi.azurewebsites.net/api/QuaddieBuilder?";//?mock=true";


#endif
    }
}
