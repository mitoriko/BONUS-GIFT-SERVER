using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO;
using Senparc.CO2NET;
using Senparc.CO2NET.Cache;
using Senparc.Weixin.RegisterServices;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin.Entities;
using Senparc.CO2NET.Cache.Redis;//DPBMARK Redis DPBMARK_END
using Senparc.Weixin.WxOpen;//DPBMARK MiniProgram DPBMARK_END
using Senparc.CO2NET.Utilities;
using Microsoft.AspNetCore.Mvc.Formatters;
using ACBC.Common;
using ACBC.Controllers;
using Senparc.Weixin;
using System.Collections.Generic;
using System;

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
            services.AddSenparcGlobalServices(Configuration)
                    .AddSenparcWeixinServices(Configuration);
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

            Senparc.CO2NET.Cache.Redis.Register.SetConfigurationOption(Global.REDIS);
            IRegisterService register = RegisterService.Start(env, senparcSetting.Value)
                .UseSenparcGlobal().UseSenparcWeixin(senparcWeixinSetting.Value, senparcSetting.Value);
            Senparc.CO2NET.Cache.Redis.Register.UseHashRedisNow();
        }

    }
}
