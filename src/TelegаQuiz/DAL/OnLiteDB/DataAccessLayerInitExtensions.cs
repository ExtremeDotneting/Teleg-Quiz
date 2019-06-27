using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace TelegаQuiz.DAL.OnLiteDB
{
    public static class DataAccessLayerInitExtensions
    {
        public static IServiceCollection AddLiteDataAccessLayer(this IServiceCollection @this)
        {
            //Here all database initialization.
            var connectionString = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.litedb");
            var service=new LiteDatabaseService(connectionString);
            @this.AddSingleton<IDatabaseService>(service);
            return @this;
        }
    }


}
