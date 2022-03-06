dotnet ef migrations add InitialCreate -o "../Infrastructure/Persistence/Migrations"
dotnet ef database update

dotnet ef dbcontext scaffold "Data Source=172.16.0.5,61499;Initial Catalog=FINTECH_PRD;Persist Security Info=True;User ID=usrfintech;Password=BtRpLiOLKEc;Application Name=FinTech_PRD" Microsoft.EntityFrameworkCore.SqlServer -o Borrar -t MantenimientoLotesTeledolarDom -t TeledolarCabeceraGrupoLotes -t ClientesDomiciliadosTeleDolar -t Tab_ConfigSys -t FiltrosProcesoDomiciliacion -t GestionCobros -t Credito -t Personas -t Solicitudes -t PlanPagos -t EncabezadoPlanPagosCredito -t Resultado_domiciliacionTeleDolar -t Resultado_domiciliacionTeleDolar_indivdual -f
