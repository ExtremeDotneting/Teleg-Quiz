using Microsoft.Extensions.DependencyInjection;

namespace TelegаQuiz.DAL.OnLiteDB
{
    public static class DataAccessLayerInitExtensions
    {
        public static IServiceCollection AddLiteDataAccessLayer(this IServiceCollection @this)
        {
            //Here all database initialization.

            @this.AddSingleton<IDatabaseService>(new LiteDatabaseService());
            return @this;
        }
    }


}
