using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions;
using Telegram.Bot.AspNetPipeline.WebhookSupport;
using TelegаQuiz.BL;
using TelegаQuiz.DAL.OnLiteDB;

namespace TelegаQuiz
{
    public class Startup
    {
        const string _domain = "https://101733ba.ngrok.io";

        BotManager _botManager;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            var token = BotTokenResolver.GetToken();
            var bot = new Telegram.Bot.TelegramBotClient(
                token, 
                new QueuedHttpClient(TimeSpan.FromSeconds(1))
                );
            _botManager = new BotManager(bot);

            //Invoked synchronous.
            _botManager.ConfigureServices((servicesWrap) =>
            {
                //Init our LiteDB.
                servicesWrap.Services.AddLiteDataAccessLayer();
                servicesWrap.Services.AddSingleton<IGameLogicService, GameLogicService>();

                servicesWrap.AddMvc(new Telegram.Bot.AspNetPipeline.Mvc.Builder.MvcOptions()
                {
                    //Useful for debugging.
                    CheckEqualsRouteInfo = true
                });
            });

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.Use(async (ctx, next) =>
            {
                var path = ctx.Request.Path.ToString();
                if (path == "" || path == "/")
                    await ctx.Response.WriteAsync("Webhooks server. Please don't forget update domain constant.");
                await next();
            });

            _botManager.ConfigureBuilder(builder =>
            {
                //builder.UseExceptionHandler(async (ctx, ex) =>
                //{
                //    await ctx.SendTextMessageAsync("Unhandled bot exception.");
                //    return false;
                //});
                builder.UseDevEceptionMessage();
                builder.UseOldUpdatesIgnoring();
                builder.UseMvc(mvcBuilder =>
                {
                    mvcBuilder.UseDebugInfo();
                });
            });

            //Note: update your pathTemplate and add there some string, that will identify telegram webhooks.
            //Something like: "wad5kK2PVL0SAEPq43q5cR2qwFWF4434/{0}". It must be same for all server processes.
            //=========
            //Use setWebhookAutomatically:false to configure how telegram webhook will work.
            var webhookReceiver = WebhookUpdatesReceiver.Create(
                app,
                _domain,
                pathTemplate: "telegram/update/{0}",
                setWebhookAutomatically: false
                );

            _botManager.Setup(
                updatesReceiver: webhookReceiver
                );

            //SetWebhookAsync not needed if setWebhookAutomatically is true.
            _botManager.BotContext.Bot.SetWebhookAsync(
                webhookReceiver.WebhookFullUrl,
                allowedUpdates: UpdateTypeExtensions.All
                ).Wait();

            _botManager.Start();
        }
    }
}
