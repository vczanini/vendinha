using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using vendinha.Entities;
using vendinha.Maps;

namespace vendinha.Extensions
{
    public static class NHibernateExtensions
    {
        public static IServiceCollection AddNHibernate(this IServiceCollection services, string connectionString)
        {
            var configuration = new Configuration();
            configuration.DataBaseIntegration(db =>
            {
                db.ConnectionString = connectionString;
                db.Driver<NpgsqlDriver>();
                db.Dialect<NHibernate.Dialect.PostgreSQLDialect>();
                db.LogFormattedSql = true;
                db.LogSqlInConsole = true;
            });

            // Add your mappings here
            var mapper = new ModelMapper();
            mapper.AddMapping<ClienteMap>();

            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            configuration.AddMapping(mapping);

            var sessionFactory = configuration.BuildSessionFactory();

            services.AddSingleton(sessionFactory);
            services.AddScoped(factory => sessionFactory.OpenSession());

            return services;
        }
    }
}
