using Domain.Entities;
using Domain.Entities.Fenix;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IFenixDbContext
    {

        public DbSet<CobrosDePrima> CobrosDePrima { get; set; }
        DbSet<ClientesDomiciliadosTeleDolar> ClientesDomiciliadosTeleDolars { get; set; }
        DbSet<Credito> Creditos { get; set; }
        DbSet<EncabezadoPlanPagosCredito> EncabezadoPlanPagosCreditos { get; set; }
        DbSet<FiltrosProcesoDomiciliacion> FiltrosProcesoDomiciliacions { get; set; }
        DbSet<GestionCobro> GestionCobros { get; set; }
        DbSet<MantenimientoLotesTeledolarDom> MantenimientoLotesTeledolarDoms { get; set; }
        DbSet<Persona> Personas { get; set; }
        DbSet<PlanPago> PlanPagos { get; set; }
        DbSet<CuentaBancariaVerificadaTeledolar> CuentasBancariasVerificadasTeledolar { get; set; }
        DbSet<ResultadoDomiciliacionTeleDolar> ResultadoDomiciliacionTeleDolars { get; set; }
        DbSet<ResultadoDomiciliacionTeleDolarIndivdual> ResultadoDomiciliacionTeleDolarIndivduals { get; set; }
        DbSet<Solicitud> Solicitudes { get; set; }
        DbSet<TabConfigSy> TabConfigSys { get; set; }
        DbSet<TeledolarCabeceraGrupoLote> TeledolarCabeceraGrupoLotes { get; set; }

        public DbSet<LogPrima> LogPrimas { get; set; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityEntry Entry([NotNullAttribute] object entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
        int DiasEnMora(int IdCredito, DateTime FechaPago);
        List<T> RawSqlQuery<T>(string query, Func<DbDataReader, T> map);
    }
}
