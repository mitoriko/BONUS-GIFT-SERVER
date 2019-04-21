using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACBC.Common;
using ACBC.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senparc.CO2NET;
using Senparc.CO2NET.Cache;
using Senparc.CO2NET.Cache.Redis;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.RegisterServices;

namespace ACBC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddMvcOptions(options =>
            {
                // This adds both Input and Output formatters based on DataContractSerializer
                //options.AddXmlDataContractSerializerFormatter();

                // To add XmlSerializer based Input and Output formatters.
                //options.InputFormatters.Add(new XmlSerializerInputFormatter());
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            });

            services.AddCors(options =>
                             options.AddPolicy("AllowSameDomain", builder =>
                                                builder.AllowAnyOrigin()
                                                .AllowAnyHeader()
                                                .AllowAnyMethod()
                                                .WithExposedHeaders(new string[] { "code", "msg" })
                                                .AllowCredentials()));
            services.AddSenparcGlobalServices(Configuration).AddSenparcWeixinServices(Configuration);

            Global.StartUp();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<SenparcSetting> senparcSetting, IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("AllowSameDomain");
            app.UseMvc();
            app.Map(Global.ROUTE_PX + "/ws", SocketController.Map);

            //IRegisterService register = RegisterService.Start(env, senparcSetting.Value)
            //                                           .UseSenparcGlobal(false, () => GetExCacheStrategies(senparcSetting.Value));
            

            var redisConfigurationStr = senparcSetting.Value.Cache_Redis_Configuration;
            var useRedis = !string.IsNullOrEmpty(redisConfigurationStr) && redisConfigurationStr != Global.REDIS;
            if(!useRedis)
            {
                senparcSetting.Value.Cache_Redis_Configuration = Global.REDIS;
            }
            IRegisterService register = RegisterService.Start(env, senparcSetting.Value).UseSenparcGlobal();
            register.UseSenparcWeixin(senparcWeixinSetting.Value, senparcSetting.Value);

            CacheStrategyFactory.RegisterObjectCacheStrategy(() => RedisObjectCacheStrategy.Instance);

        }

        //private IList<IDomainExtensionCacheStrategy> GetExCacheStrategies(SenparcSetting senparcSetting)
        //{
        //    var exContainerCacheStrategies = new List<IDomainExtensionCacheStrategy>();
        //    senparcSetting = senparcSetting ?? new SenparcSetting();
        //    return exContainerCacheStrategies;
        //}
    }
}
