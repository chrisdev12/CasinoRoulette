using Amazon;
using Amazon.DynamoDBv2;
using Amazon.SQS;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Roulette.Core.Context
using Roulette.Core.Entities.DTO.User;
using Roulette.Core.Entities.User;
using Roulette.Core.Services.Filters;
using Roulette.Core.Services.Logs;
using Roulette.Core.Services.Queue;
using Roulette.Core.Services.Security;
using Roulette.Core.Services.Security.System;

namespace Roulette
{
    public class Startup : EnviromentVariables
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            new CloudWatch().SetupSerilog();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
            services.AddControllers();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<SecretKeyService>();
            services.AddSingleton<EnviromentVariables>();
            services.AddTransient<CognitoService>();
            services.AddScoped(options => new DynamoContext(
                new AmazonDynamoDBClient(
                    awsAccessKeyId: awsAccessKey,
                    awsSecretAccessKey: awsSecretKey,
                    region: RegionEndpoint.GetBySystemName(awsRegion))
                ));
            services.AddScoped<IAmazonSQS>(
                x => new AmazonSQSClient(
                    awsAccessKeyId: awsAccessKey,
                    awsSecretAccessKey: awsSecretKey,
                    region: RegionEndpoint.GetBySystemName(awsRegion)
                ));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => new CognitoConfig().SetBearerJwt(options));
            services.AddScoped<BetHandler>();
            services.AddMvc(options => options.Filters.Add(new ErrorFilter()));
            services.AddAutoMapper(configuration =>
            {
                configuration.CreateMap<User, UserDynamo>();
            }, typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
