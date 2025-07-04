﻿using Marketaudit.WebAPI;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace MarketAudit.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
                    WebHost.CreateDefaultBuilder(args)
                        .UseStartup<Startup>()
                        .UseSetting("detailedErrors", "true")
                        .CaptureStartupErrors(true)
                        .ConfigureKestrel(o => { o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(15); })
                        .Build();
    }
}
