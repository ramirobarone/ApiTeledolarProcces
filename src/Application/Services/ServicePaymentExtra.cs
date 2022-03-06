using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Entities.CommodEntities;
using Domain.Entities.Fenix;
using Domain.Entities.Teledolar.SingleDomiciliation;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ServicePaymentExtra : IServicePaymentExtra
    {
        private readonly IServiceSingleDomiciliation serviceSingleDomiciliation;
        private readonly IFenixDbContext _fenixDbContext;

        public ServicePaymentExtra(IServiceSingleDomiciliation serviceSingleDomiciliation, IFenixDbContext fenixDbContext)
        {
            this.serviceSingleDomiciliation = serviceSingleDomiciliation;
            _fenixDbContext = fenixDbContext;
        }
        public async Task<ResultDomiciliation> ExecuteDomiciliation(CreateTransaction execute)
        {
            var transaction = new ExecuteTransaction
            {
                source = execute.source,
                target = execute.target,
                reference_number = execute.reference_number,
                description = execute.description,
                amount = execute.amount,
                channel_id = execute.channel_id,
                currency_id = execute.currency_id,
                device_id = execute.device_id,
                timestamp = Timestamp(),
                transaction_type_id = execute.transaction_type_id
            };

            ResultDomiciliation ExtraPay = await serviceSingleDomiciliation.ExecuteSingleDomiciliation(transaction);

            //SaveLogTeledolar(transaction, ExtraPay);

            await SaveChanges(ExtraPay, execute.IdSolicitud, execute.identification);

            return ExtraPay;
        }
        private async Task<int> SaveChanges(ResultDomiciliation resultDomiciliation, int idSolicitud, string identification)
        {
            if (resultDomiciliation.data == null)
                return 0;

            _fenixDbContext.CobrosDePrima.Add(new CobrosDePrima()
            {
                bank_account = resultDomiciliation.data.transaction.summary.target.bank_account.number,
                DescriptionTeledolar = resultDomiciliation.data.transaction.description,
                fechaCobro = DateTime.Now.Date,
                inserted_at = resultDomiciliation.data.transaction.inserted_at,
                idTeledolar = resultDomiciliation.data.transaction.id,
                mainCredit = Convert.ToDecimal(resultDomiciliation.data.transaction.summary.target.total_debit),
                montoPrima = Convert.ToDecimal(resultDomiciliation.data.transaction.summary.target.total_debit),
                Sinpe_reference = resultDomiciliation.data.transaction.sinpe_reference,
                timestampTeledolar = resultDomiciliation.execution.timestamp.ToString(),
                message = resultDomiciliation.execution.message[0],
                transactionId = resultDomiciliation.data.transaction.id,
                entity = resultDomiciliation.data.entities.target.name,
                idSolicitud = idSolicitud,
                identificacionCliente = identification
            });
            return await _fenixDbContext.SaveChangesAsync();
        }
        private string Timestamp()
        {
            DateTimeOffset ExpirationTime = DateTime.UtcNow;
            long unixTimeStampInSeconds = ExpirationTime.ToUnixTimeSeconds();
            return unixTimeStampInSeconds.ToString();
        }
        private async void SaveLogTeledolar(ExecuteTransaction ObjectSend, ResultDomiciliation ObjectResponse)
        {
            _fenixDbContext.LogPrimas.Add(new LogPrima()
            {
                Enviado = "Api Teledolar:" + JsonConvert.SerializeObject(ObjectSend),
                Recibido = "Api Teledolar:" + JsonConvert.SerializeObject(ObjectResponse),
                FechaPrima = DateTime.Now.Date
            });
            await _fenixDbContext.SaveChangesAsync();
        }
    }
}
