using System;
using System.Collections.Generic;

namespace Parcial2.Pagos
{
    // TODO 5 · Factory
    // La forma de pago se elige EN EJECUCIÓN y llega como TEXTO (del ComboBox o del JSON).
    // Este es el ÚNICO lugar del sistema que conoce las clases concretas de pago.
    public static class PagoFactory
    {
        public static IPagoStrategy Crear(string tipo)
        {
            switch (tipo)
            {
                case "Efectivo":
                    return new PagoEfectivo();
                case "Tarjeta":
                    return new PagoTarjeta();
                case "Cuenta empresa":
                    return new PagoCuentaEmpresa();
                case "Transferencia":
                    return new PagoTransferencia();
                default:
                    throw new ArgumentException("manera de pago no encontrada");
                    
            }
        }

        // La usa el ComboBox: la vista no escribe nombres a mano.
        public static List<string> TiposDisponibles()
        {
            return new List<string> { "Efectivo", "Tarjeta", "Cuenta empresa", "Transferencia" };
        }
    }
}
