﻿using Fur;
using Microsoft.Extensions.DependencyInjection;

namespace FurMvc.EntityFramework.Core
{
    public class DbConfigureStartup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseAccessor(options =>
            {
            }, "FurMvc.Database.Migrations");
        }
    }
}