using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcial2.Pagos
{
    public class PagoTarjeta : IPagoStrategy
    {
        public string Nombre => "Tarjeta";

        public bool PermiteCuotas => true;

        public bool RequiereCuit => false;

        public decimal CalcularAjuste(decimal subtotal, int cuotas)
        {
            switch (cuotas)
            {
                case 1:
                    return 0m;
                case 2:
                    return subtotal * 0.05m;
                case 3:
                    return subtotal * 0.10m;
                default:
                    throw new ArgumentException("con tarjeta solo puede ser en 1,2 o 3 cuotas.");

            }
        }
    }
}
