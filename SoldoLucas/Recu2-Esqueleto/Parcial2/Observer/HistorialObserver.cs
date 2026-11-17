using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parcial2.Services;

namespace Parcial2.Observer
{
    public class HistorialObserver : IPedidoObserver
    {
        public void Actualizar(EventoPedido evento)
        {
            Bitacora.Instancia.Registrar(evento.Descripcion());
        }
    }
}
