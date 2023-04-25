using DbgCensus.Rest;
using DbgCensus.Rest.Extensions;
using Masthead.Api.Abstractions.Services;
using Masthead.Api.Services;
using Masthead.Api.Services.GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Masthead.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSystemd();

        builder.Services.Configure<CensusQueryOptions>(builder.Configuration.GetSection("CensusQueryOptions"));

        builder.Services.AddMemoryCache();
        builder.Services.AddControllers();

        builder.Services.AddCensusRestServices();
        builder.Services.AddTransient<CensusRestService>();
        builder.Services.AddTransient<ICensusRestService, CachingCensusRestService>
        (
            s => new CachingCensusRestService
            (
                s.GetRequiredService<CensusRestService>(),
                s.GetRequiredService<IMemoryCache>()
            )
        );

        builder.Services.AddGraphQLServer()
            .AddQueryType<GraphQLQueryService>()
            .AddTypeExtension<WeaponExtensions>();

        WebApplication app = builder.Build();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseHttpsRedirection();
        app.UseCors
        (
            x => x.WithMethods(HttpMethods.Get)
                .AllowAnyHeader()
                .AllowAnyOrigin()
        );
        app.UseAuthorization();
        app.MapGraphQL();
        app.MapControllers();

        app.Run();
    }
}
