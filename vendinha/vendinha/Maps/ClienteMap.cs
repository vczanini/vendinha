using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using vendinha.Entities;

namespace vendinha.Maps
{
    public class ClienteMap : ClassMapping<Clientes>
    {
        public ClienteMap()
        {
            Id(x => x.clientId, x =>
            {
                x.Generator(Generators.Increment);
                x.Type(NHibernateUtil.Int32);
                x.Column("client_id");
            });

            Property(b => b.nomeCompleto, x =>
            {
                x.Length(200);
                x.Type(NHibernateUtil.String);
                x.NotNullable(true);
                x.Column("nome_completo");
            });

            Property(b => b.cpf, x =>
            {
                x.Type(NHibernateUtil.String);
                x.NotNullable(true);
                x.Unique(true);
                x.Column("cpf");
            });

            Property(b => b.dataNascimento, x =>
            {
                x.Type(NHibernateUtil.Date);
                x.NotNullable(true);
                x.Column("data_nascimento");
            });

            Property(b => b.email, x =>
            {
                x.Type(NHibernateUtil.String);
                x.Length(100);
                x.Column("email");
            });

            Property(b => b.totalDividas, x =>
            {
                x.Type(NHibernateUtil.Decimal);
                x.Scale(2);
                x.Column("total_dividas");
            });

            Table("maximus.clientes");
        }
    }
}
