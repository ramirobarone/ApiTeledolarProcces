using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities.Teledolar.Ada;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AdaService : IAdaService
    {
        private readonly IFenixDbContext context;
        private readonly IAdaService adaService;

        public AdaService(IFenixDbContext context, IAdaProxy adaService)
        {
            this.context = context;
            this.adaService = adaService;
        }
        public async Task<ResponseAda> RegisterAda(RequestAda ada)
        {
            if (await AdaExist(ada.identification))
            {
                throw new AdaException("Ada Vigente");
            }

            ada.timestamp = GetTimeStamp();
            var response = await adaService.RegisterAda(ada);

            if (response?.data == null)
                throw new AdaException(response?.execution.message[0]);

            await SaveAdaInDataBase(response);

            return response;
        }

        private string GetTimeStamp()
        {
            DateTimeOffset ExpirationTime = DateTime.UtcNow;
            long unixTimeStampInSeconds = ExpirationTime.ToUnixTimeSeconds();
            return unixTimeStampInSeconds.ToString();
        }

        public Task<ResponseAda> StatusAda(string Identification)
        {
            throw new NotImplementedException();
        }
        //DescripcionTeledolar FechaRegistro FecFinalVigencia, Moneda, CCDestino, Servicio, TitularServicio, IdClienteOrigen, ClienteOrigen, CCOrigen
        private async Task<int> SaveAdaInDataBase(ResponseAda responseAda)
        {
            if (responseAda.data == null)
                throw new ArgumentNullException(nameof(responseAda));

            context.ClientesDomiciliadosTeleDolars.Add(new Domain.Entities.Fenix.ClientesDomiciliadosTeleDolar()
            {
                Monto = responseAda.data.current_sinpe_ada != null ? Convert.ToDecimal(responseAda.data.current_sinpe_ada.amount) : 0,
                Ccdestino = responseAda.data.number,
                CodMoneda = responseAda.data.currency,
                DescripcionTeledolar = responseAda.execution.message[0],
                Servicio = responseAda.data.identification.Replace("-", ""),
                Identificacion = responseAda.data.identification,
                IdPersona = BuscarIdPersona(responseAda.data.identification),
                FecFinalVigencia = responseAda.data.current_sinpe_ada != null ?  responseAda.data.current_sinpe_ada.end_date : "2026-12-26",
                IdClienteDestino = responseAda.data.identification,
                FechaRegistro = DateTime.Now.Date
            });

            return await context.SaveChangesAsync();
        }

        private int? BuscarIdPersona(string identification)
        {
            return context.Personas.Where(x => x.Identificacion == identification).Select(x => x.Id).FirstOrDefault();
        }

        private async Task<bool> AdaExist(string identification)
        {
            var adaResult = await context.ClientesDomiciliadosTeleDolars.Where(x => x.Servicio == identification).FirstOrDefaultAsync();

            if (adaResult != null)
            {
                return Convert.ToDateTime(adaResult.FecFinalVigencia) > DateTime.Now;
            }
            return false;
        }
    }
}
