using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.Fenix;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Fenix
{
    public partial class FenixDbContext : DbContext, IFenixDbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public FenixDbContext(DbContextOptions<FenixDbContext> options) : base(options) { }

        public FenixDbContext(
            DbContextOptions<FenixDbContext> options,
            ICurrentUserService currentUserService,
            IDateTime dateTime) : base(options)
        {
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public virtual DbSet<CobrosDePrima> CobrosDePrima { get; set; }
        public virtual DbSet<ClientesDomiciliadosTeleDolar> ClientesDomiciliadosTeleDolars { get; set; }
        public virtual DbSet<Credito> Creditos { get; set; }
        public virtual DbSet<EncabezadoPlanPagosCredito> EncabezadoPlanPagosCreditos { get; set; }
        public virtual DbSet<FiltrosProcesoDomiciliacion> FiltrosProcesoDomiciliacions { get; set; }
        public virtual DbSet<GestionCobro> GestionCobros { get; set; }
        public virtual DbSet<MantenimientoLotesTeledolarDom> MantenimientoLotesTeledolarDoms { get; set; }
        public virtual DbSet<Persona> Personas { get; set; }
        public virtual DbSet<PlanPago> PlanPagos { get; set; }
        public virtual DbSet<ResultadoDomiciliacionTeleDolar> ResultadoDomiciliacionTeleDolars { get; set; }
        public virtual DbSet<ResultadoDomiciliacionTeleDolarIndivdual> ResultadoDomiciliacionTeleDolarIndivduals { get; set; }
        public virtual DbSet<Solicitud> Solicitudes { get; set; }
        public virtual DbSet<TabConfigSy> TabConfigSys { get; set; }
        public virtual DbSet<TdetEcbanco> TdetEcbancos { get; set; }
        public virtual DbSet<TeledolarCabeceraGrupoLote> TeledolarCabeceraGrupoLotes { get; set; }
        public virtual DbSet<TencEcbanco> TencEcbancos { get; set; }
        public DbSet<CuentaBancariaVerificadaTeledolar> CuentasBancariasVerificadasTeledolar { get; set; }
        public DbSet<LogPrima> LogPrimas { get; set; }

        public override DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
        public override EntityEntry Entry([NotNull] object entity)
        {
            return base.Entry(entity);
        }

        public List<T> RawSqlQuery<T>(string query, Func<DbDataReader, T> map)
        {
            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                this.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    var entities = new List<T>();

                    while (result.Read())
                    {
                        entities.Add(map(result));
                    }

                    return entities;
                }
            }
        }

        public int DiasEnMora(int IdCredito, DateTime FechaPago)
        {
            var FechaRetraso = (from pp in this.PlanPagos
                                join eppc in this.EncabezadoPlanPagosCreditos on pp.FkIdEncabezadoPlanPagosCredito equals eppc.IdEncabezadoPlanPagosCredito // 
                                where pp.IdCredito == IdCredito &&
                                (pp.MontoCapital ?? 0 + pp.MontoInteres ?? 0 + pp.MontoOriginacion ?? 0 + pp.MontoIntMora ?? 0 + pp.MontoCargoMora ?? 0) > 0 &&
                                eppc.Estado == 1 //
                                select pp.FechaVencimiento).OrderBy(x => x).FirstOrDefault();
            return (int)(FechaRetraso - FechaPago).TotalDays;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _currentUserService.AuthorizedUserId;
                        entry.Entity.GeneratedDate = _dateTime.Now;
                        entry.Entity.ModifiedDate = _dateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _currentUserService.AuthorizedUserId;
                        entry.Entity.ModifiedDate = _dateTime.Now;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public void DetachAllEntities()
        {
            var changedEntriesCopy = this.ChangeTracker.Entries().ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<ClientesDomiciliadosTeleDolar>(entity =>
            {
                entity.ToTable("ClientesDomiciliadosTeleDolar");

                entity.HasIndex(e => new { e.EstatusDomiciliacion, e.Estado }, "IX_ClientesDomiciliadosTeleDolar");

                entity.HasIndex(e => new { e.Estado, e.RegistroTeledolar }, "IX_ClientesDomiciliadosTeleDolar_Estado_RegistroTeledolar");

                entity.HasIndex(e => e.Identificacion, "IX_ClientesDomiciliadosTeleDolar_IdIdentificacion");

                entity.HasIndex(e => e.IdPersona, "IX_ClientesDomiciliadosTeleDolar_IdPersona");

                entity.HasIndex(e => e.IdSolicitud, "IX_ClientesDomiciliadosTeleDolar_IdSolicitud");

                entity.HasIndex(e => new { e.RegistroTeledolar, e.Estado }, "IX_ClientesDomiciliadosTeleDolar_RT_Estado");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AreaSolicitante)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Canal)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ccdestino)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CCDestino");

                entity.Property(e => e.Ccorigen)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CCOrigen");

                entity.Property(e => e.ClienteOrigen)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CobrarComision)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CodMoneda)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Codigo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Comentario)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.DescripcionTeledolar)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.EmailClienteDestino)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EmailClienteOrigen)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FecFinalVigencia)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FecValor)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaDebitoAutomatico).HasColumnType("datetime");

                entity.Property(e => e.FechaIngresa).HasColumnType("datetime");

                entity.Property(e => e.FechaModifica).HasColumnType("datetime");

                entity.Property(e => e.FechaModificaExclu).HasColumnType("date");

                entity.Property(e => e.FechaRegistro).HasColumnType("datetime");

                entity.Property(e => e.FechaVigenciaExclu).HasColumnType("date");

                entity.Property(e => e.FkClientesDomiciliados).HasColumnName("Fk_Clientes_Domiciliados");

                entity.Property(e => e.IdClienteDestino)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IdClienteOrigen)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Idcredito).HasColumnName("idcredito");

                entity.Property(e => e.Identificacion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MontoDomiciliado).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProcesadoXml).HasColumnName("ProcesadoXML");

                entity.Property(e => e.RefInterna)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RespuestaXml).HasColumnName("RespuestaXML");

                entity.Property(e => e.Servicio)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.TitularServicio)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UltimaRespuestaDelBacRegistrada)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UsrDebitoAutomatico)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UsrModifica)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UsuarioIngresa)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UsuarioModifica)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Credito>(entity =>
            {
                entity.ToTable("Credito");

                entity.HasIndex(e => e.CapitalPendiente, "<Name of Missing Index, sysname,>");

                entity.HasIndex(e => e.IdTipo, "IDX_CreditoIDTIPO_29012019");

                entity.HasIndex(e => e.EstadoCredito, "IX_Credito_EstadoCredito");

                entity.HasIndex(e => e.IdProducto, "IX_Credito_IdProducto");

                entity.HasIndex(e => new { e.IdTipo, e.CapitalPendiente }, "IX_Credito_IdTipo_CapitalPendiente");

                entity.HasIndex(e => e.IdTipo, "idx_Credito_20200311");

                entity.HasIndex(e => e.IdSolicitud, "missing_index_110");

                entity.HasIndex(e => e.IdSolicitud, "missing_index_118244");

                entity.HasIndex(e => new { e.IdPersona, e.IdTipo }, "missing_index_12576");

                entity.HasIndex(e => new { e.IdSolicitud, e.FechaTransferencia }, "missing_index_127");

                entity.HasIndex(e => e.IdPersona, "missing_index_131");

                entity.HasIndex(e => e.IdPersona, "missing_index_214");

                entity.HasIndex(e => new { e.IdSolicitud, e.IdTipo }, "missing_index_29");

                entity.HasIndex(e => new { e.IdPersona, e.IdSolicitud }, "missing_index_4971");

                entity.HasIndex(e => e.IdTipo, "missing_index_57");

                entity.HasIndex(e => new { e.IdPersona, e.IdTipo }, "missing_index_63");

                entity.HasIndex(e => e.IdTipo, "missing_index_65");

                entity.Property(e => e.CapitalPendiente).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Comentario)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CondicionFinanciera).IsUnicode(false);

                entity.Property(e => e.FactorPaga).IsUnicode(false);

                entity.Property(e => e.FechaCierre).HasColumnType("datetime");

                entity.Property(e => e.FechaCredito).HasColumnType("datetime");

                entity.Property(e => e.FechaFirma).HasColumnType("datetime");

                entity.Property(e => e.FechaIngreso).HasColumnType("datetime");

                entity.Property(e => e.FechaModificacion).HasColumnType("datetime");

                entity.Property(e => e.FechaPrimerPago).HasColumnType("datetime");

                entity.Property(e => e.FechaTransferencia).HasColumnType("datetime");

                entity.Property(e => e.FechaUltimoCargoMora).HasColumnType("datetime");

                entity.Property(e => e.FechaUltimoPago).HasColumnType("datetime");

                entity.Property(e => e.FkProcedencia).HasColumnName("FK_Procedencia");

                entity.Property(e => e.Identificacion)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.InteresCorriente).HasColumnName("Interes_Corriente");

                entity.Property(e => e.Matriz).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MontoCuota).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MontoFinanciado).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MontoOriginacion).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NotaPermanente)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.SaldoDia).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SaldoTotal).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TasaInteresNormal).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.UsrModifica)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EncabezadoPlanPagosCredito>(entity =>
            {
                entity.HasKey(e => e.IdEncabezadoPlanPagosCredito);

                entity.ToTable("EncabezadoPlanPagosCredito");

                entity.HasIndex(e => e.Estado, "IX_EncabezadoPlanPagosCredito_Estado");

                entity.Property(e => e.Cuota).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaCambio).HasColumnType("datetime");

                entity.Property(e => e.FechaCreacion).HasColumnType("datetime");

                entity.Property(e => e.FechaModificacion).HasColumnType("datetime");

                entity.Property(e => e.UsuInclusion)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UsuModificacion)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FiltrosProcesoDomiciliacion>(entity =>
            {
                entity.ToTable("FiltrosProcesoDomiciliacion");

                entity.HasIndex(e => e.Estado, "IX_FiltrosProcesoDomiciliacion_Estado");

                entity.Property(e => e.FechaModificacion).HasColumnType("datetime");

                entity.Property(e => e.FechaUltimoCobro).HasColumnType("datetime");

                entity.Property(e => e.MontoMora).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.RenovadorNuevoAmbos).HasColumnName("Renovador_Nuevo_Ambos");

                entity.Property(e => e.UsuarioModificacion)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<GestionCobro>(entity =>
            {
                entity.HasKey(e => e.IdGestionCobro)
                    .HasName("GestionCobro");

                entity.HasIndex(e => e.FechaGestion, "GestionCobros_20190429");

                entity.HasIndex(e => new { e.MontoPromesaPago, e.FechaPromPago }, "IDX_GestionCobros_29012019");

                entity.HasIndex(e => e.FechaPromPago, "IND_GestionCobros");

                entity.HasIndex(e => new { e.IdCredito, e.UsrModifica }, "missing_index_1112");

                entity.HasIndex(e => e.IdCredito, "missing_index_114");

                entity.HasIndex(e => e.IdCredito, "missing_index_176");

                entity.HasIndex(e => e.UsrModifica, "missing_index_380");

                entity.HasIndex(e => new { e.IdCredito, e.FechaGestion, e.MontoPromesaPago, e.FechaPromPago }, "missing_index_548");

                entity.HasIndex(e => new { e.FechaPromPago, e.UsrModifica, e.Accion }, "missing_index_84");

                entity.Property(e => e.Accion)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.BndAvisoPromesa)
                    .HasColumnName("bndAvisoPromesa")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.BndAvisoPromesaVencio)
                    .HasColumnName("bndAvisoPromesaVencio")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Detalle)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.FechaGestion).HasColumnType("datetime");

                entity.Property(e => e.FechaPromPago).HasColumnType("date");

                entity.Property(e => e.FechaPromesaPago)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.IdMotivoMora)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.IdRespuestaGestion)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.MontoPromesaPago).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Realizada)
                    .HasColumnName("realizada")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.UsrModifica)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCreditoNavigation)
                    .WithMany(p => p.GestionCobros)
                    .HasForeignKey(d => d.IdCredito)
                    .HasConstraintName("FK_GestionCobros_Credito");
            });

            modelBuilder.Entity<MantenimientoLotesTeledolarDom>(entity =>
            {
                entity.ToTable("MantenimientoLotesTeledolarDom");

                entity.HasIndex(e => new { e.Estado, e.Paso }, "IX_MantenimientoLotesTeledolarDom_Estado_Paso");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaModificacion).HasColumnType("datetime");

                entity.Property(e => e.MontoDomiciliacion).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PorcentajeMontoMora).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UsuarioModifica)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Identificacion })
                    .IsClustered(false);

                entity.HasIndex(e => e.Identificacion, "IDX_PERSONAS_29012019");

                entity.HasIndex(e => new { e.TelefonoCel, e.Identificacion }, "IDX_Personas_TelefonoCel_Identificacion_E6104");

                entity.HasIndex(e => new { e.IdTipoIdentificacion, e.Identificacion }, "IndPersonas-20180717")
                    .IsUnique()
                    .IsClustered()
                    .HasFillFactor((byte)100);

                entity.HasIndex(e => e.IdTAgenciasExternas, "missing_index_155983");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Identificacion)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Correo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CorreoDomiciliacion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CorreoDomiciliacionAlternativo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CorreoOpcional)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DetalleDireccion)
                    .HasMaxLength(999)
                    .IsUnicode(false);

                entity.Property(e => e.FechaIngreso).HasColumnType("datetime");

                entity.Property(e => e.FechaIngresoAgenciaExterna).HasColumnType("datetime");

                entity.Property(e => e.FechaModificacion).HasColumnType("datetime");

                entity.Property(e => e.FechaNacimiento).HasColumnType("datetime");

                entity.Property(e => e.IdTAgenciasExternas)
                    .HasColumnName("IdT_AgenciasExternas")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.PrimerApellido)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PrimerNombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SegundoApellido)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SegundoNombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UsrModifica)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VencimientoIdentificacion).HasColumnType("datetime");
            });

            modelBuilder.Entity<PlanPago>(entity =>
            {
                entity.HasIndex(e => new { e.IdCredito, e.Cuota }, "INX_PlanPagos_20190129");

                entity.HasIndex(e => e.FkIdEncabezadoPlanPagosCredito, "IX_PlanPagos_FK_IdEncabezadoPlanPagosCredito");

                entity.HasIndex(e => e.IdCredito, "IX_PlanPagos_IdCredito_Inc_MontoCapital");

                entity.Property(e => e.AdelantoCapital).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CapitalCobrado).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CapitalOriginal).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CargoMoraCobrado).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CargoMoraOriginal).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.FechaCorte).HasColumnType("datetime");

                entity.Property(e => e.FechaPago).HasColumnType("datetime");

                entity.Property(e => e.FechaVencimiento).HasColumnType("datetime");

                entity.Property(e => e.FkIdEncabezadoPlanPagosCredito).HasColumnName("FK_IdEncabezadoPlanPagosCredito");

                entity.Property(e => e.InteresCobrado).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.InteresMoraCobrado).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.InteresMoraOriginal).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.InteresOriginal).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.InteresPendiente).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.InteresesDelMes).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.MontoCapital).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MontoCargoMora).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MontoCobrado)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.MontoIntMora)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.MontoInteres).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MontoOriginacion).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.OriginacionCobrado).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.OriginacionOriginal).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Usuario)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ResultadoDomiciliacionTeleDolar>(entity =>
            {
                entity.ToTable("Resultado_domiciliacionTeleDolar");

                entity.HasIndex(e => new { e.Identificacion, e.BatchId }, "IX_Resultado_domiciliacionTeleDolar_");

                entity.HasIndex(e => new { e.EstadoDom, e.FechaEjecucion }, "IX_Resultado_domiciliacionTeleDolar_Estado_Fecha");

                entity.HasIndex(e => new { e.FechaEjecucion, e.Paso }, "IX_Resultado_domiciliacionTeleDolar_Fecha_Ejecuciion_Paso");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.BankAccountTypeId).HasColumnName("bank_account_type_id");

                entity.Property(e => e.BankAccountTypeIdDesnity).HasColumnName("bank_account_type_id_desnity");

                entity.Property(e => e.BatchId).HasColumnName("batch_id");

                entity.Property(e => e.ChannelId).HasColumnName("channel_id");

                entity.Property(e => e.Code).HasColumnName("code");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.DescriptionDomi)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("description_domi");

                entity.Property(e => e.EstadoDom)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("estadoDom");

                entity.Property(e => e.ExternalMessage)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("external_message");

                entity.Property(e => e.FechaEjecucion)
                    .HasColumnType("datetime")
                    .HasColumnName("fecha_ejecucion");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("full_name");

                entity.Property(e => e.FullNameDesnity)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("full_name_desnity");

                entity.Property(e => e.Identificacion)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.InsertedAt)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("inserted_at");

                entity.Property(e => e.MainCredit)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("main_credit");

                entity.Property(e => e.Message)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("message");

                entity.Property(e => e.MontoEnviado).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Number)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("number");

                entity.Property(e => e.NumberDesnity)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("number_desnity");

                entity.Property(e => e.PasoLote)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PorcentajeMontoMora).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ReferenceNumber)
                    .HasMaxLength(200)
                    .HasColumnName("Reference_number");

                entity.Property(e => e.SinpeReference)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("sinpe_reference");

                entity.Property(e => e.Timestamp)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("timestamp");

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("total");

                entity.Property(e => e.TotalCredit)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("total_credit");

                entity.Property(e => e.TotalDebit)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("total_debit");

                entity.Property(e => e.TransactionStateId).HasColumnName("transaction_state_id");

                entity.Property(e => e.TransactionTypeId).HasColumnName("transaction_type_id");

                entity.HasOne(d => d.IdGrupoLotesNavigation)
                    .WithMany(p => p.ResultadoDomiciliacionTeleDolars)
                    .HasForeignKey(d => d.IdGrupoLotes)
                    .HasConstraintName("FK_Resultado_domiciliacionTeleDolar_TeledolarCabeceraGrupoLotes");
            });

            modelBuilder.Entity<ResultadoDomiciliacionTeleDolarIndivdual>(entity =>
            {
                entity.ToTable("Resultado_domiciliacionTeleDolar_Indivdual");

                entity.HasIndex(e => e.IdCredito, "I_idCredito_ResultadoDomIndividual");

                entity.HasIndex(e => e.Identificacion, "I_identificacion_ResultadoDomIndividual");

                entity.HasIndex(e => e.Referencia, "I_referencia_ResultadoDomIndividual");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.BankAccountTypeId).HasColumnName("bank_account_type_id");

                entity.Property(e => e.BankAccountTypeIdDesnity).HasColumnName("bank_account_type_id_desnity");

                entity.Property(e => e.ChannelId).HasColumnName("channel_id");

                entity.Property(e => e.Code).HasColumnName("code");

                entity.Property(e => e.Cuotas)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.DescriptionDomi)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("description_domi");

                entity.Property(e => e.DeviceId)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("device_id");

                entity.Property(e => e.ExternalMessage)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("external_message");

                entity.Property(e => e.FechaEjecucion)
                    .HasColumnType("datetime")
                    .HasColumnName("fecha_ejecucion");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("full_name");

                entity.Property(e => e.FullNameDesnity)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("full_name_desnity");

                entity.Property(e => e.Identificacion)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.InsertedAt)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("inserted_at");

                entity.Property(e => e.MainCredit)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("main_credit");

                entity.Property(e => e.Message)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("message");

                entity.Property(e => e.MontoDomiciliacion).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Number)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("number");

                entity.Property(e => e.NumberDesnity)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("number_desnity");

                entity.Property(e => e.Referencia)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SinpeReference)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("sinpe_reference");

                entity.Property(e => e.Timestamp)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("timestamp");

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("total");

                entity.Property(e => e.TotalCredit)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("total_credit");

                entity.Property(e => e.TotalDebit)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("total_debit");

                entity.Property(e => e.TransactionStateId).HasColumnName("transaction_state_id");

                entity.Property(e => e.TransactionTypeId).HasColumnName("transaction_type_id");
            });

            modelBuilder.Entity<Solicitud>(entity =>
            {
                entity.HasIndex(e => e.FechaTransferencia, "<Name of Missing Index, sysname,>");

                entity.HasIndex(e => e.IdSolicitud, "IDX_IDSOLICITUD");

                entity.HasIndex(e => e.Status, "IDX_Solicitudes02212020");

                entity.HasIndex(e => new { e.Status, e.IdUsrAsig }, "IDX_SolicitudesStatus_29012019");

                entity.HasIndex(e => new { e.ApruebaGini, e.ApruebaRules, e.Renovacion, e.ApruebaFlex }, "IDX_Solicitudes_11032020");

                entity.HasIndex(e => new { e.Status, e.IdUsrAsig }, "IDX_Solicitudes_29012019");

                entity.HasIndex(e => new { e.IdNuevoOrigen, e.Status, e.Id }, "IX_Solicitudes_IdNuevoOrigen_Status_Id");

                entity.HasIndex(e => e.IdPersona, "IX_Solicitudes_IdPersona");

                entity.HasIndex(e => e.IdProducto, "IX_Solicitudes_IdProducto");

                entity.HasIndex(e => e.Origen, "IX_Solicitudes_Origen");

                entity.HasIndex(e => e.Status, "IX_Solicitudes_Status");

                entity.HasIndex(e => new { e.Status, e.Renovacion }, "IX_Solicitudes_Status_Renovacion");

                entity.HasIndex(e => new { e.UsuarioAsignado, e.FechaTransferencia }, "IX_Solicitudes_UsuarioAsignado_FechaTransferencia");

                entity.HasIndex(e => e.BndRevisionDomiciliacion, "idx_Solicitudes_25022020");

                entity.HasIndex(e => e.Status, "missing_index_122298");

                entity.HasIndex(e => new { e.Status, e.ApruebaRules }, "missing_index_122303");

                entity.HasIndex(e => e.FechaIngreso, "missing_index_122305");

                entity.HasIndex(e => new { e.Status, e.ApruebaGini }, "missing_index_122311");

                entity.HasIndex(e => e.Status, "missing_index_122316");

                entity.HasIndex(e => new { e.Matriz, e.Status }, "missing_index_122318");

                entity.HasIndex(e => new { e.ApruebaRules, e.FechaIngreso, e.Status, e.Score }, "missing_index_122322");

                entity.HasIndex(e => new { e.Renovacion, e.FechaIngreso, e.Status }, "missing_index_122334");

                entity.HasIndex(e => new { e.SubOrigen, e.FechaIngreso }, "missing_index_122342");

                entity.HasIndex(e => e.Status, "missing_index_122438");

                entity.HasIndex(e => e.Status, "missing_index_158166");

                entity.Property(e => e.AgenteVentaZenziya)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Asignado).HasDefaultValueSql("((0))");

                entity.Property(e => e.AsignarCola).HasDefaultValueSql("((0))");

                entity.Property(e => e.Bases).HasDefaultValueSql("((0))");

                entity.Property(e => e.BitAprobadaExcepcion).HasColumnName("BIT_APROBADA_EXCEPCION");

                entity.Property(e => e.BndIntentosRechazo).HasColumnName("bndIntentosRechazo");

                entity.Property(e => e.BndListaNegra).HasColumnName("bndListaNegra");

                entity.Property(e => e.BndTransferencia).HasColumnName("bndTransferencia");

                entity.Property(e => e.ComentarioComercio)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ComentarioRevisionDomiciliacion)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ComentarioZenziya)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Comentarios)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Cuota).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.DisponibleCuota).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.EstadoSugef).HasColumnName("EstadoSUGEF");

                entity.Property(e => e.FechaAsignacion).HasColumnType("datetime");

                entity.Property(e => e.FechaAsignacionVentas).HasColumnType("datetime");

                entity.Property(e => e.FechaDebitoAutomatico).HasColumnType("datetime");

                entity.Property(e => e.FechaEnviaVenta).HasColumnType("datetime");

                entity.Property(e => e.FechaGeneraPagare).HasColumnType("datetime");

                entity.Property(e => e.FechaIngreso).HasColumnType("datetime");

                entity.Property(e => e.FechaModificacion).HasColumnType("datetime");

                entity.Property(e => e.FechaPrimerPago).HasColumnType("datetime");

                entity.Property(e => e.FechaProcesamiento).HasColumnType("datetime");

                entity.Property(e => e.FechaReintento).HasColumnType("datetime");

                entity.Property(e => e.FechaSolicitud).HasColumnType("datetime");

                entity.Property(e => e.FechaSugef)
                    .HasColumnType("datetime")
                    .HasColumnName("FechaSUGEF");

                entity.Property(e => e.FechaTransferencia).HasColumnType("datetime");

                entity.Property(e => e.FechaUltimoPago).HasColumnType("datetime");

                entity.Property(e => e.FkIdArbolDecision).HasColumnName("FK_Id_ArbolDecision");

                entity.Property(e => e.ForzamientoSolicitud)
                    .HasColumnName("forzamiento_solicitud")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.FotoFirma)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.IdAgencia)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("idAgencia");

                entity.Property(e => e.IdFacebook)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IdNuevoOrigen)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.IdUsrAsig)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.IdVendedor)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Marca)
                    .HasMaxLength(90)
                    .IsUnicode(false);

                entity.Property(e => e.Matriz)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MontoFinanciado).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MontoMaximo).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MontoOriginacion).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MotivoRechazo)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Origen)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Pin).HasColumnName("PIN");

                entity.Property(e => e.ProcesoPayBac)
                    .HasColumnName("ProcesoPayBAC")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Renovacion).HasDefaultValueSql("((0))");

                entity.Property(e => e.ResultTextCed).IsUnicode(false);

                entity.Property(e => e.Score).HasDefaultValueSql("((0))");

                entity.Property(e => e.ScoreDiferenciadoFlexRecalibrado).HasColumnName("ScoreDiferenciadoFlex_Recalibrado");

                entity.Property(e => e.ScorePrimarios).HasColumnType("decimal(20, 17)");

                entity.Property(e => e.ScorePrimariosVersion)
                    .HasColumnType("decimal(20, 17)")
                    .HasColumnName("ScorePrimarios_Version");

                entity.Property(e => e.ScorePrincipalFlexRecalibrado).HasColumnName("ScorePrincipalFlex_Recalibrado");

                entity.Property(e => e.StatusOriginal).HasColumnName("Status_Original");

                entity.Property(e => e.SubOrigen)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TipoCredito)
                    .HasColumnName("Tipo_Credito")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UrlDirectorioPagare)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.UrlDocMovist)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.UrlDomiciliacion)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UrlFotoCedula)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UrlFotoCedulaTrasera)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UrlFotoSelfie)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UsoCredito)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UsrDebitoAutomatico)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UsrGeneraConPaga)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UsrModifica)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UsuarioAsignado)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.UsuarioEnviaVenta)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("Usuario_Envia_Venta");

                entity.Property(e => e.UsuarioModificacion)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("USUARIO_MODIFICACION");
            });

            modelBuilder.Entity<TabConfigSy>(entity =>
            {
                entity.ToTable("Tab_ConfigSys");

                entity.HasIndex(e => new { e.LlaveConfig1, e.LlaveConfig2, e.LlaveConfig3, e.LlaveConfig4, e.DatoChar1, e.LlaveConfig5 }, "IX_Tab_ConfigSys_Varios");

                entity.HasIndex(e => new { e.LlaveConfig1, e.LlaveConfig2, e.LlaveConfig3, e.LlaveConfig4, e.LlaveConfig5, e.Activo }, "IX_Tab_ConfigSys_llave_config");

                entity.Property(e => e.CodigoPais)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.ConfiguracionChar)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.DatoChar1)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("Dato_Char1");

                entity.Property(e => e.DatoChar2)
                    .IsUnicode(false)
                    .HasColumnName("Dato_Char2");

                entity.Property(e => e.DatoChar3)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("Dato_Char3");

                entity.Property(e => e.DatoInt1).HasColumnName("Dato_Int1");

                entity.Property(e => e.DatoInt2).HasColumnName("Dato_Int2");

                entity.Property(e => e.DatoInt3).HasColumnName("Dato_Int3");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LlaveConfig1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("llave_Config1");

                entity.Property(e => e.LlaveConfig2)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("llave_Config2");

                entity.Property(e => e.LlaveConfig3)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("llave_Config3");

                entity.Property(e => e.LlaveConfig4)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("llave_Config4");

                entity.Property(e => e.LlaveConfig5)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("llave_Config5");
            });

            modelBuilder.Entity<TdetEcbanco>(entity =>
            {
                entity.HasKey(e => new { e.IdEc, e.FecTran, e.DocRef });

                entity.ToTable("TDetECBancos");

                entity.HasIndex(e => new { e.FecTran, e.DocRef }, "missing_index_1198");

                entity.HasIndex(e => e.DocRef, "missing_index_1912");

                entity.HasIndex(e => e.IdTipDepBan, "missing_index_2655");

                entity.HasIndex(e => new { e.DocRef, e.CredTran }, "missing_index_2657");

                entity.HasIndex(e => e.IdTipDepBan, "missing_index_529");

                entity.HasIndex(e => e.DocRef, "missing_index_545");

                entity.Property(e => e.IdEc).HasColumnName("IdEC");

                entity.Property(e => e.FecTran).HasColumnType("date");

                entity.Property(e => e.DocRef)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CredTran).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.DebTran).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.DescripTran)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Estado).HasDefaultValueSql("((1))");

                entity.Property(e => e.FecNac).HasColumnType("date");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.TipTran)
                    .HasMaxLength(3)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TeledolarCabeceraGrupoLote>(entity =>
            {
                entity.Property(e => e.FechaEjecucion)
                    .HasColumnType("datetime")
                    .HasColumnName("Fecha_Ejecucion");

                entity.Property(e => e.Lotes)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TotalDomiciliado).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<TencEcbanco>(entity =>
            {
                entity.HasKey(e => e.IdEc);

                entity.ToTable("TEncECBancos");

                entity.Property(e => e.IdEc)
                    .ValueGeneratedNever()
                    .HasColumnName("IdEC");

                entity.Property(e => e.CtaBanco)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.FecCar).HasColumnType("datetime");

                entity.Property(e => e.SalDis).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SalIni).HasColumnType("decimal(18, 2)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
