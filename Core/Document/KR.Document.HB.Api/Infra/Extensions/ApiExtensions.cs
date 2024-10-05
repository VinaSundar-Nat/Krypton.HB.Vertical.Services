namespace KR.Document.HB.Api;

public static class ApiExtensions
{
    public static bool IsLocal(this IHostEnvironment hostEnvironment) =>
        hostEnvironment.EnvironmentName == "Local";

    public static bool IsDev(this IHostEnvironment hostEnvironment) =>
         hostEnvironment.EnvironmentName == "Local" || 
         hostEnvironment.EnvironmentName == "Development";

}
