using Application.Common.Interfaces;
using Application.Common.Interfaces.BankAccount;
using Domain.Entities.Teledolar.Account;
using System;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ServiceAccountBank : IServiceBankAccount
    {
        private readonly IServiceProxyGet serviceProxy;
        private readonly IFenixDbContext fenixDbContext;

        public ServiceAccountBank(IServiceProxyGet serviceProxy, IFenixDbContext fenixDbContext)
        {
            this.serviceProxy = serviceProxy;
            this.fenixDbContext = fenixDbContext;
        }

        public async Task<ResponseAccountClient> GetBankAccountAsync(string iban, string identification, string typeIdentification)
        {
            if (string.IsNullOrEmpty(iban) || string.IsNullOrEmpty(identification) || string.IsNullOrEmpty(typeIdentification))
                throw new ArgumentNullException();

            var responseAccount = await serviceProxy.Get(iban, FormatIdentification(identification), typeIdentification);

            int i = await SaveChangesAccount(responseAccount, identification, iban);

            return responseAccount;
        }
        private async Task<int> SaveChangesAccount(ResponseAccountClient responseAccountClient, string Identification, string Iban)
        {
            fenixDbContext.CuentasBancariasVerificadasTeledolar.Add(new Domain.Entities.Fenix.CuentaBancariaVerificadaTeledolar()
            {
                FechaVerificacion = DateTime.Now,
                Iban = Iban,
                FechaActualizacion = DateTime.Now,
                Identificacion = Identification,
                MensajeTeledolar = responseAccountClient.execution.message[0],
                CuentaVerificadaOk = responseAccountClient.data != null ? true : false
            });
            return await fenixDbContext.SaveChangesAsync();
        }
        private string FormatIdentification(string identification)
        {
            identification = identification.Insert(1, "-");
            identification = identification.Insert(6, "-");
            return identification;
        }
    }
}
