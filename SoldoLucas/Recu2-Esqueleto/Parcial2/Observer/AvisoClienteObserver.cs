using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parcial2.Models;
using System.IO;

namespace Parcial2.Observer
{
    public class AvisoClienteObserver : IPedidoObserver
    {
        private readonly String _ruta;
        public AvisoClienteObserver(string ruta)
        {
            _ruta = ruta;
        }
        public void Actualizar(EventoPedido evento)
        {
            {
                OrdenPedido o = evento.Pedido;
                string texto = null;

                if (evento.Tipo == TipoEvento.Alta)
                    texto = $"Hola {o.Cliente},recibimos tu pedido #{o.Id}.";
                else if ( o.Estado == EstadoPedido.ParaRetirar)
                    texto = $"Tu pedido {o.Id} está listo para retirar. Total: {o.Total:C0}.";
                else if (evento.Tipo == TipoEvento.Cancelacion)
                    texto = $"Tu pedido #{o.Id} fue cancelada.";

                if (texto == null)   
                    return;

                string linea = $"[{evento.Fecha:dd/MM HH:mm}] WhatsApp a {o.Cliente} ({o.Telefono}): {texto}";
                File.AppendAllText(_ruta, linea + Environment.NewLine);
            }
        }
    }
}
