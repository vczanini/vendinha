using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using vendinha.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Data.Common;

namespace vendinha.Repositories
{
    public class ClienteRepository : IRepository<Clientes>
    {
        private readonly NHibernate.ISession _session;
        private readonly ILogger<ClienteRepository> _logger;

        public ClienteRepository(NHibernate.ISession session, ILogger<ClienteRepository> logger)
        {
            _session = session;
            _logger = logger;
        }

        public async Task Add(Clientes cliente)
        {
            ITransaction transaction = null;
            try
            {
                transaction = _session.BeginTransaction();

                var clienteJson = JsonConvert.SerializeObject(cliente);

                using (var cmd = _session.Connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"
                        SELECT maximus.func_gera_cliente(:cliente_json::jsonb)";

                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = "cliente_json";
                    parameter.DbType = DbType.String;
                    parameter.Value = clienteJson;
                    cmd.Parameters.Add(parameter);

                    // Enlist the command in the current NHibernate transaction
                    _session.Transaction.Enlist(cmd);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var result = reader.GetString(0);
                            var resultJson = JsonConvert.DeserializeObject<dynamic>(result);

                            // Check for error in the function result
                            if (resultJson.erro != 0)
                            {
                                await _session.SaveAsync(cliente);

                                await transaction.CommitAsync();
                                return;
                            }

                            // If no error, save the cliente entity
                            await _session.SaveAsync(cliente);

                            await transaction.CommitAsync();
                            return;
                        }
                    }
                }

                // If no result from the function, rollback the transaction
                await transaction.RollbackAsync();
                throw new InvalidOperationException("No result from func_gera_cliente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding cliente");
                if (transaction != null && transaction.IsActive)
                {
                    await transaction.RollbackAsync();
                }
                throw;
            }
            finally
            {
                transaction?.Dispose();
            }
        }




        public IEnumerable<Clientes> FindAll() => _session.Query<Clientes>().ToList();

        public async Task<Clientes> FindByID(int id) => await _session.GetAsync<Clientes>(id);

        public async Task Remove(long id)
        {
            ITransaction transaction = null;
            try
            {
                transaction = _session.BeginTransaction();
                var item = await _session.GetAsync<Clientes>(id);
                await _session.DeleteAsync(item);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cliente with id {Id}", id);
                await transaction?.RollbackAsync();
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public async Task Update(Clientes item)
        {
            ITransaction transaction = null;
            try
            {
                transaction = _session.BeginTransaction();
                await _session.UpdateAsync(item);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cliente with id {Id}", item.clientId);
                await transaction?.RollbackAsync();
            }
            finally
            {
                transaction?.Dispose();
            }
        }
    }
}
