namespace innovate_work_admin_app.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddHttpClient(this IServiceCollection services, string baseRoot, string name)
        {
            services.AddHttpClient(name, httpClient =>
            {
                httpClient.BaseAddress = new Uri(baseRoot + "/" + name);
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                }
            });
        }
    }
}
