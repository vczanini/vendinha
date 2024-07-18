using static System.Runtime.InteropServices.JavaScript.JSType;

namespace vendinha.Entities
{
    public class Clientes
    {
        public virtual int clientId { get; set; }
        public virtual string nomeCompleto { get; set; } 
        public virtual string cpf { get; set; }
        public virtual DateTime dataNascimento { get; set; }
        public virtual string email { get; set; }
        public virtual decimal totalDividas { get; set; }
    }
}
