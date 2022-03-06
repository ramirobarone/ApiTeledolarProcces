using Application.Common.Interfaces;
using Domain.Entities.Fenix;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CreateBatch = Domain.Entities.Teledolar.CreateBatch;

namespace Application.Services
{
    public class DomiciliacionesService : IDomiciliacionesService
    {

        private readonly IFenixDbContext _fenix;
        private readonly IDateTime _datetime;
        private readonly ITeledolarProxy _teledolarProxy;
        private FiltrosProcesoDomiciliacion _filtros;
        private MantenimientoLotesTeledolarDom _currentBatch;
        private List<CustomerADA> _customersAda = new List<CustomerADA>();
        private ConstSettings _constSettings;

        public DomiciliacionesService(IFenixDbContext fenix, IDateTime datetime, ITeledolarProxy teledolarProxy)
        {
            _fenix = fenix;
            _datetime = datetime;
            _teledolarProxy = teledolarProxy;
        }

        public async Task DomiciliationPerBatches()
        {
            _filtros = await _fenix.FiltrosProcesoDomiciliacions.FirstOrDefaultAsync(x => x.Id == 1);
            _constSettings = JsonConvert.DeserializeObject<ConstSettings>(_teledolarProxy.GetConstSettings());
            //await PrepareDomiciliation ();

            foreach (var batch in await GetBatches())
            {
                _currentBatch = batch;
                var domiPerBatch = new List<DomiciliacionXLote>();
                await ProgressBatch();
                foreach (var customer in await GetCustomersInArrearsTest() /*await GetCustomersInArrears ()*/ )
                {
                    domiPerBatch.Add(await GetCustomerData(customer));
                }
                await Domiciliar(domiPerBatch);
            }
        }

        private async Task ProgressBatch()
        {
            var lastGroupBatch = await _fenix.TeledolarCabeceraGrupoLotes.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastGroupBatch.Lotes += lastGroupBatch.Lotes == null ? _currentBatch.Descripcion : $"->{_currentBatch.Descripcion}";
            _fenix.TeledolarCabeceraGrupoLotes.Update(lastGroupBatch);
            await _fenix.SaveChangesAsync();
        }
        private async Task ProgressBatch(decimal amountDebited)
        {
            var lastGroupBatch = await _fenix.TeledolarCabeceraGrupoLotes.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastGroupBatch.TotalDomiciliado += amountDebited;
            _fenix.TeledolarCabeceraGrupoLotes.Update(lastGroupBatch);
        }

        private async Task<DomiciliacionXLote> GetCustomerData(Customer customer)
        {
            var customerADA = _customersAda.Find(x => x.Customer.Identificacion == customer.Identificacion);
            var customerData = new ClientesDomiciliadosTeleDolar();
            if (customerADA == null)
            {
                customerData = await _fenix.ClientesDomiciliadosTeleDolars.Where(x => x.Identificacion == customer.Identificacion && x.Idcredito == customer.IdCredito && x.Estado.Value == 1).FirstOrDefaultAsync();
                customerADA = new CustomerADA(customer, customerData.MontoDomiciliado ?? customerData.Monto.Value);
                _customersAda.Add(customerADA);
            }

            var batchData = new DomiciliacionXLote
            {
                CCDestino = customerData.Ccdestino,
                CCOrigen = customerData.Ccorigen,
                MontoDomiciliar = await GetAmount(customerADA),
                Identificacion = customer.Identificacion,
                Paso = _currentBatch.Paso.Value,
                reference_number = $"D-{customer.Identificacion}-{DateTime.Now.ToString("yyyyMMddHHmmssffff")}",
                currency_id = _constSettings.currency_id,
                device_id = _constSettings.device_id,
                enterprise_service_id = _constSettings.enterprise_service_id,
                channel_id = _constSettings.channel_id,
                transaction_type_id = _constSettings.transaction_type_id,
                ada_service = "",
                description = "CR1.5"
            };

            return batchData;
        }

        private async Task<decimal> GetAmount(CustomerADA customerADA)
        {
            decimal amount = 0;
            if (_currentBatch.MontoDomiciliacion > 0)
            {
                amount = _currentBatch.MontoDomiciliacion.Value;
            }
            //else if ()
            else
            {
                amount = (from p in _fenix.Personas
                          join c in _fenix.Creditos on p.Id equals c.IdPersona
                          join cdt in _fenix.ClientesDomiciliadosTeleDolars on p.Identificacion equals cdt.Identificacion //
                          where c.IdTipo == 14 && p.Identificacion == customerADA.Customer.Identificacion && cdt.EstatusDomiciliacion == 1 //
                          select new { IdCredito = c.Id }).ToList()
                    .Select(x => scalarDbFunc<decimal?>("TotalMora", $"{x.IdCredito},GETDATE()")).FirstOrDefault().Value;
                //select scalarDbFunc<decimal?> ("TotalMora", $"{c.Id},GETDATE()")).FirstOrDefault ().Value;
            }
            var totalDebit = await GetTotalDebitedCurrentMonth(customerADA.Customer.Identificacion);
            amount = amount > (customerADA.ADA - totalDebit) ? (customerADA.ADA - totalDebit) : amount;
            customerADA.DebitAmount(amount, (_currentBatch.PorcentajeMontoMora ?? 0) == 1.0m);

            if (_currentBatch.PorcentajeMontoMora != 1.0m && _currentBatch.MontoDomiciliacion == null)
            {
                var amountPercent = customerADA.HundredPercent * _currentBatch.PorcentajeMontoMora.Value;
                amount = amount > amountPercent ? amountPercent : amount;
            }
            return amount;
        }

        private async Task<decimal> GetTotalDebitedCurrentMonth(string customerIdentification)
        {

            var totalCredit = await _fenix.ResultadoDomiciliacionTeleDolars
                .Where(x => x.SinpeReference != null && x.SinpeReference != "0" && x.TransactionStateId.Value == 1 && x.Identificacion == customerIdentification &&
                   x.FechaEjecucion.Value.Year == _datetime.Now.Year && x.FechaEjecucion.Value.Month == _datetime.Now.Month)
                .GroupBy(x => x.Identificacion).Select(x => x.Sum(s => s.TotalCredit ?? 0)).FirstOrDefaultAsync();
            //totalCredit = totalCredit ?? 0;
            totalCredit += await _fenix.ResultadoDomiciliacionTeleDolarIndivduals
                .Where(x => x.SinpeReference != null && x.SinpeReference != "0" && x.TransactionStateId.Value == 1 && x.Identificacion == customerIdentification &&
                   x.FechaEjecucion.Value.Year == _datetime.Now.Year && x.FechaEjecucion.Value.Month == _datetime.Now.Month)
                .GroupBy(x => x.Identificacion).Select(x => x.Sum(s => s.TotalCredit ?? 0)).FirstOrDefaultAsync();
            //totalCredit = totalCredit ?? 0;
            return totalCredit;
        }

        /// <summary>
        /// Obtiene filtros de la config, agrega la cabecera del grupo de lotes que se va a ejecutar y prepara los clientes
        /// </summary>
        /// <returns></returns>
        private async Task PrepareDomiciliation()
        {
            _filtros = await _fenix.FiltrosProcesoDomiciliacions.FirstOrDefaultAsync(x => x.Id == 1);
            _constSettings = JsonConvert.DeserializeObject<ConstSettings>(_teledolarProxy.GetConstSettings());
            var a = await _fenix.TeledolarCabeceraGrupoLotes.AddAsync(new TeledolarCabeceraGrupoLote { FechaEjecucion = _datetime.Now, Lotes = string.Empty, TotalDomiciliado = 0 });
            await _fenix.SaveChangesAsync();

            await PrepareCustomers();
        }

        /// <summary>
        /// Actualiza a EstatusDomiciliacion = 1 para que siempre agarre los clientes que aun no finalizaron la domiciliacion en lotes
        /// </summary>
        /// <returns></returns>
        private async Task PrepareCustomers()
        {
            var customers = await _fenix.ClientesDomiciliadosTeleDolars.Where(x => new List<int> { 3, 4, 5 }.Contains(x.EstatusDomiciliacion.Value) && x.Estado.Value == 1).ToListAsync();
            customers.ForEach(x => x.EstatusDomiciliacion = 1);
            _fenix.ClientesDomiciliadosTeleDolars.UpdateRange(customers);
            await _fenix.SaveChangesAsync();
        }
        private async Task<List<MantenimientoLotesTeledolarDom>> GetBatches()
        {
            return await _fenix.MantenimientoLotesTeledolarDoms.Where(x => x.Estado.Value).ToListAsync();
        }

        private async Task<List<Customer>> GetCustomersInArrears()
        {

            var isRenewer = false;
            var NewAndRenewer = false;
            if (new int[] { 0, 1 }.ToList().Contains(_filtros.RenovadorNuevoAmbos.Value))
                isRenewer = _filtros.RenovadorNuevoAmbos.Value == 1;
            else
                NewAndRenewer = true;

            var allCustomers = (from p in _fenix.Personas
                                join s in _fenix.Solicitudes on p.Id equals s.IdPersona
                                join c in _fenix.Creditos on s.Id equals c.IdSolicitud
                                join cdt in _fenix.ClientesDomiciliadosTeleDolars on p.Identificacion equals cdt.Identificacion //
                                where c.IdTipo == 14 &&
                                cdt.EstatusDomiciliacion == 1 &&
                                cdt.MontoDomiciliado != null &&
                                (c.EstadoCredito == null ? 0 : c.EstadoCredito.Value) != 3 &&
                                cdt.Idcredito == c.Id &&
                                (NewAndRenewer || s.Renovacion.Value == isRenewer) // 
                                select new { IdCredito = c.Id, Identificacion = cdt.Identificacion, Ccorigen = cdt.Ccorigen, RefInterna = cdt.RefInterna, Ccdestino = cdt.Ccdestino })
                .ToList();

            var customersWithPRP = await (from g in _fenix.GestionCobros
                                          join c in _fenix.Creditos on g.IdCredito equals c.Id
                                          join p in _fenix.Personas on c.IdPersona equals p.Id
                                          join cdt in _fenix.ClientesDomiciliadosTeleDolars on p.Identificacion equals cdt.Identificacion // 
                                          where c.IdTipo == 14 &&
                                          g.IdRespuestaGestion == "PRP" &&
                                          (_datetime.Now >= g.FechaGestion.Value && _datetime.Now < g.FechaPromPago.Value) //
                                          select new { cdt.Identificacion }).ToListAsync();
            customersWithPRP = customersWithPRP.Distinct().ToList();

            allCustomers.RemoveAll(x => customersWithPRP.Select(z => z.Identificacion).ToList().Contains(x.Identificacion));

            var preFilteredCustomers = allCustomers.Select(x => new { TotalMora = scalarDbFunc<decimal?>("TotalMora", $"{x.IdCredito},GETDATE()"), DiasEnMora = scalarDbFunc<int?>("DiasEnMora", $"{x.IdCredito},GETDATE()"), Identificacion = x.Identificacion, Ccorigen = x.Ccorigen, RefInterna = x.RefInterna, Ccdestino = x.Ccdestino }).ToList();
            var filteredCustomers = preFilteredCustomers.Where(x =>
                   ((_filtros.MaxDiasMora == null || _filtros.MinDiasMora == null) || (x.DiasEnMora.Value >= _filtros.MinDiasMora && x.DiasEnMora.Value < _filtros.MaxDiasMora)) &&
                   (_filtros.MontoMora == null || (x.TotalMora.Value > 0 ? x.TotalMora.Value : 0) >= _filtros.MontoMora.Value))
                .Select(x => new Customer { Identificacion = x.Identificacion, Ccorigen = x.Ccorigen, RefInterna = x.RefInterna, Ccdestino = x.Ccdestino }).ToList();

            return filteredCustomers;
        }

        private async Task Domiciliar(IEnumerable<DomiciliacionXLote> dxl)
        {
            if (dxl.Count() == 0) return;

            CultureInfo info = CultureInfo.GetCultureInfo("es-ES");

            DateTimeOffset ExpirationTime = DateTime.UtcNow;
            long unixTimeStampInSeconds = ExpirationTime.ToUnixTimeSeconds();
            string time = unixTimeStampInSeconds.ToString();
            var resultBatch = new List<ResultadoDomiciliacionTeleDolar>();

            var obj = new CreateBatch.BatchRequest();

            //obj.channel_id = Convert.ToInt32(dt.Rows[0].ItemArray[6]); //15;
            //obj.device_id = dt.Rows[0].ItemArray[7].ToString(); //"54321";
            obj.timestamp = time;
            obj.transactions = new List<CreateBatch.Transaction>();
            int contador = 0;
            foreach (var domiciliacion in dxl)
            {

                obj.transactions.Add(new CreateBatch.Transaction
                {
                    source = new CreateBatch.Source
                    {
                        client = domiciliacion.CCDestino,
                        enterprise_service_id = domiciliacion.enterprise_service_id, //3,
                        ada_service = domiciliacion.ada_service //"Servicio de prueba"
                    },
                    target = new CreateBatch.Target
                    {
                        client = domiciliacion.CCOrigen
                    },
                    reference_number = domiciliacion.reference_number, //"prueba lotes 000100",
                    description = domiciliacion.description, //"prueba recarga de saldo",
                    transaction_type_id = domiciliacion.transaction_type_id, //121,
                    currency_id = domiciliacion.currency_id, //1,
                    amount = domiciliacion.MontoDomiciliar.ToString()
                });

                obj.transactions[contador].amount = obj.transactions[contador].amount.Replace(",", "."); //String.Format("{0:#,###,###.##}", obj.transactions[contador].amount);

                contador++;

                resultBatch.Add(new ResultadoDomiciliacionTeleDolar
                {
                    Identificacion = domiciliacion.Identificacion,
                    PasoLote = _currentBatch.Descripcion,
                    PorcentajeMontoMora = _currentBatch.PorcentajeMontoMora,
                    FechaEjecucion = _datetime.Now,
                    Paso = _currentBatch.Paso.Value,
                    MontoDomiciliacion = (int?)_currentBatch.MontoDomiciliacion ?? 0,
                    MontoEnviado = domiciliacion.MontoDomiciliar,
                    IdGrupoLotes = (await _fenix.TeledolarCabeceraGrupoLotes.OrderByDescending(x => x.Id).FirstOrDefaultAsync()).Id,
                    ReferenceNumber = domiciliacion.reference_number,
                });
            }

            obj.channel_id = dxl.First().channel_id; //15;
            obj.device_id = dxl.First().device_id; //"54321";

            var response = await _teledolarProxy.CreateBatch(obj);

            resultBatch.ForEach(x => { x.BatchId = response.Result.data.batch_id; x.EstadoDom = response.Result.data.state; });

            await _fenix.ResultadoDomiciliacionTeleDolars.AddRangeAsync(resultBatch);
            await _fenix.SaveChangesAsync();

            var statusResponse = await _teledolarProxy.GetBatchStatus(response.Result.data.batch_id.ToString(), 100, 1);

            while (statusResponse.Result.data.batch.state == "pending")
            {
                System.Threading.Thread.Sleep(240000); //300000
                statusResponse = await _teledolarProxy.GetBatchStatus(response.Result.data.batch_id.ToString(), 100, 1);
            }

            if (statusResponse.Result.data.batch.state == "done") //empezar actulizar todos los registros 
            {
                var total_pages = statusResponse.Result.data.total_pages;
                var batches = await _fenix.ResultadoDomiciliacionTeleDolars.Where(x => x.BatchId == response.Result.data.batch_id).AsTracking().ToListAsync();
                decimal totalCredit = 0;
                for (int page = 1; page <= total_pages; page++) // Recorrer todas las pages
                {
                    statusResponse = await _teledolarProxy.GetBatchStatus(response.Result.data.batch_id.ToString(), 100, page);

                    foreach (var x in statusResponse.Result.data.entries)
                    {
                        var batch = batches.Where(b => b.ReferenceNumber == x.@params.reference_number).FirstOrDefault();
                        var identification = x.@params.reference_number.Split('-')[1];
                        batch.NumberDesnity = x.@params.source.client;
                        batch.Description = x.transaction.summary.target.bank_account.description;
                        batch.FullName = x.transaction.summary.target.full_name;
                        batch.Number = x.@params.target.client;
                        batch.MainCredit = decimal.Parse(x.transaction.summary.target.main_credit);
                        batch.Total = decimal.Parse(x.transaction.summary.target.total);
                        batch.EstadoDom = x.state;
                        batch.TotalCredit = decimal.Parse(x.transaction.summary.target.total_credit);
                        batch.TotalDebit = decimal.Parse(x.transaction.summary.target.total_debit);
                        batch.TransactionStateId = x.transaction.transaction_state_id;
                        batch.TransactionTypeId = x.transaction.transaction_type_id;
                        batch.ChannelId = x.@params.channel_id;
                        batch.Code = x.response.code;
                        batch.DescriptionDomi = x.response.message[0];
                        batch.InsertedAt = DateTime.Now.ToString("yyyy-MM-dd");
                        batch.SinpeReference = x.transaction.sinpe_reference;
                        batch.Timestamp = statusResponse.Result.execution.timestamp.ToString();
                        batch.Message = x.response.message[0];
                        batch.ExternalMessage = x.response.message[0];
                        batch.Amount = decimal.Parse(x.transaction.summary.target.total_debit);
                        batch.BankAccountTypeId = 1;
                        batch.FullNameDesnity = await _fenix.Personas.Where(p => p.Identificacion == identification).Select(s => $"{s.PrimerNombre} {s.SegundoNombre} {s.PrimerApellido} {s.SegundoApellido}").FirstOrDefaultAsync();
                        batch.BankAccountTypeIdDesnity = 2;

                        _fenix.ResultadoDomiciliacionTeleDolars.Update(batch);

                        var clientDomi = await _fenix.ClientesDomiciliadosTeleDolars.Where(x => x.Identificacion == identification).FirstOrDefaultAsync();
                        if (batch.Code != 0 && batch.MontoDomiciliacion > 0 && !_filtros.RechazoCurso.Value)
                            clientDomi.EstatusDomiciliacion = 3;
                        else if ((batch.Code == 0 && batch.PorcentajeMontoMora.Value == 1.0m) || (batch.Code == 0 && batch.PorcentajeMontoMora.Value > 0 && _filtros.ProcesadoCurso.Value))
                            clientDomi.EstatusDomiciliacion = 4;
                        _fenix.ClientesDomiciliadosTeleDolars.Update(clientDomi);
                        if (batch.MontoDomiciliacion > 0 && batch.TransactionStateId == 1)
                        {
                            await ProgressBatch(batch.TotalCredit.Value);
                            _fenix.RawSqlQuery("", x => x[0]);
                        }

                        await _fenix.SaveChangesAsync();
                    }

                    System.Threading.Thread.Sleep(18000); //300000

                }
            }
        }

        private async Task<List<Customer>> GetCustomersInArrearsTest()
        {

            // var isRenewer = false;
            // var NewAndRenewer = false;
            // if (new int[] { 0, 1 }.ToList ().Contains (_filtros.RenovadorNuevoAmbos.Value))
            //     isRenewer = _filtros.RenovadorNuevoAmbos.Value == 1;
            // else
            //     NewAndRenewer = true;

            // var filteredCustomers = await (from p in _fenix.Personas join s in _fenix.Solicitudes on p.Id equals s.IdPersona join c in _fenix.Creditos on s.Id equals c.IdSolicitud join cdt in _fenix.ClientesDomiciliadosTeleDolars on p.Identificacion equals cdt.Identificacion //
            //     where c.IdTipo == 14 &&
            //     cdt.EstatusDomiciliacion == 1 &&
            //     cdt.MontoDomiciliado != null &&
            //     (c.EstadoCredito == null? 0 : c.EstadoCredito.Value) != 3 &&
            //     cdt.Idcredito == c.Id &&
            //     (NewAndRenewer || s.Renovacion.Value == isRenewer) // 
            //     select new Customer { IdCredito = c.Id, Identificacion = cdt.Identificacion, Ccorigen = cdt.Ccorigen, RefInterna = cdt.RefInterna, Ccdestino = cdt.Ccdestino }).Take (10).ToListAsync ();
            // //filteredCustomers = filteredCustomers.ToList().Select (x => new Customer { Identificacion = x.Identificacion, Ccorigen = x.Ccorigen, RefInterna = x.RefInterna, Ccdestino = x.Ccdestino }).ToList();

            // return filteredCustomers;

            var isRenewer = false;
            var NewAndRenewer = false;
            if (new int[] { 0, 1 }.ToList().Contains(_filtros.RenovadorNuevoAmbos.Value))
                isRenewer = _filtros.RenovadorNuevoAmbos.Value == 1;
            else
                NewAndRenewer = true;

            var allCustomers = (from p in _fenix.Personas
                                join s in _fenix.Solicitudes on p.Id equals s.IdPersona
                                join c in _fenix.Creditos on s.Id equals c.IdSolicitud
                                join cdt in _fenix.ClientesDomiciliadosTeleDolars on p.Identificacion equals cdt.Identificacion //
                                where c.IdTipo == 14 &&
                                cdt.EstatusDomiciliacion == 1 &&
                                cdt.MontoDomiciliado != null &&
                                (c.EstadoCredito == null ? 0 : c.EstadoCredito.Value) != 3 &&
                                cdt.Idcredito == c.Id && cdt.MontoDomiciliado != null && cdt.Estado.Value == 1 &&
                                (NewAndRenewer || s.Renovacion.Value == isRenewer) // 
                                select new { IdCredito = c.Id, Identificacion = cdt.Identificacion, Ccorigen = cdt.Ccorigen, RefInterna = cdt.RefInterna, Ccdestino = cdt.Ccdestino }).Take(10).ToList().
            Select(x => new { IdCredito = x.IdCredito, TotalMora = scalarDbFunc<decimal?>("TotalMora", $"{x.IdCredito},GETDATE()"), DiasEnMora = scalarDbFunc<int?>("DiasEnMora", $"{x.IdCredito},GETDATE()"), Identificacion = x.Identificacion, Ccorigen = x.Ccorigen, RefInterna = x.RefInterna, Ccdestino = x.Ccdestino }).ToList();

            var filteredCustomers = allCustomers.Where(x =>
                   //((_filtros.MaxDiasMora == null || _filtros.MinDiasMora == null) || (x.DiasEnMora.Value >= _filtros.MinDiasMora && x.DiasEnMora.Value < _filtros.MaxDiasMora)) &&
                   (_filtros.MontoMora == null || (x.TotalMora.Value > 0 ? x.TotalMora.Value : 0) >= _filtros.MontoMora.Value))
                .Select(x => new Customer { IdCredito = x.IdCredito, Identificacion = x.Identificacion, Ccorigen = x.Ccorigen, RefInterna = x.RefInterna, Ccdestino = x.Ccdestino }).ToList();

            return filteredCustomers;
        }
        public T scalarDbFunc<T>(string funcName, string parameters)
        {
            return _fenix.RawSqlQuery($"SELECT dbo.{funcName}({parameters})", x => (T)(x.IsDBNull(0) ? null : x[0])).FirstOrDefault();
        }
    }

    public class CustomerADA
    {
        public CustomerADA(Customer customer, decimal ADA)
        {
            this.ADA = ADA;
            this.Customer = customer;
        }
        public Customer Customer { get; private set; }
        public decimal ADA { get; private set; } = 0;
        public decimal AmountDebited { get; private set; } = 0;
        public decimal HundredPercent { get; private set; } = 0;
        public void DebitAmount(decimal amount, bool isHundredPercent = false)
        {
            AmountDebited += amount;
            HundredPercent = (isHundredPercent && HundredPercent == 0) ? AmountDebited : HundredPercent;
        }

    }
    public class Customer
    {
        public int IdCredito { get; set; }
        public string Identificacion { get; set; }
        public string Ccorigen { get; set; }
        public string RefInterna { get; set; }
        public string Ccdestino { get; set; }
    }
    public class DomiciliacionXLote
    {
        public decimal MontoDomiciliar { get; set; }
        public string Identificacion { get; set; }
        public int Paso { get; set; }
        public string CCOrigen { get; set; }
        public string RefInterna { get; set; }
        public string CCDestino { get; set; }
        public int channel_id { get; set; }
        public string device_id { get; set; }
        public int enterprise_service_id { get; set; }
        public string ada_service { get; set; }
        public string reference_number { get; set; }
        public string description { get; set; }
        public int transaction_type_id { get; set; }
        public int currency_id { get; set; }
    }

    public class ConstSettings
    {
        public int channel_id { get; set; }
        public string device_id { get; set; }
        public int enterprise_service_id { get; set; }
        public int transaction_type_id { get; set; }
        public int currency_id { get; set; }
    }
}
