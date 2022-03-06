using System;
using System.Linq;
using Application.Common.Interfaces;

namespace Application.Services
{

    public class ServicesHealthDataBase : IHealthService
    {
        private readonly IFenixDbContext fenixDbContext;

        public ServicesHealthDataBase(IFenixDbContext fenixDbContext)
        {
            this.fenixDbContext = fenixDbContext;
        }
        public bool ProbarConexion()
        {
            return fenixDbContext.Creditos.Where(x => x.Id == 13000).Any();
        }
    }
}
