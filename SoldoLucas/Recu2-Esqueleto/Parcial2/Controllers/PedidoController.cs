using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Parcial2.Models;
using Parcial2.Observer;
using Parcial2.Pagos;
using Parcial2.Repository;

namespace Parcial2.Controllers
{
    // PATRÓN OBSERVER — este controller es el Notificador (Publisher).
    // Hace su trabajo (validar, calcular, guardar) y AVISA. No sabe quién escucha.
    public class PedidoController : IPedidoPublisher
    {
        public const int MaximoObservaciones = 100;

        private readonly IRepository<Pedido> _pedidos;
        private readonly IRepository<Prenda> _prendas;

        // TODO 7 · Observer
        // Este controller tiene que ser el Notificador: guardar la lista de suscriptores,
        // permitir el alta y la baja, y avisarles a todos cuando algo pasa.
        private readonly List<IPedidoObserver> _observadores = new List<IPedidoObserver>();
        public PedidoController(IRepository<Pedido> pedidos, IRepository<Prenda> prendas)
        {
            _pedidos = pedidos;
            _prendas = prendas;
        }

        public void Suscribir(IPedidoObserver observador)
        {
            if (!_observadores.Contains(observador))
                _observadores.Add(observador);

        }

        public void Desuscribir(IPedidoObserver observador)
        {
            _observadores.Remove(observador);
        }

        private void Notificar(EventoPedido evento)
        {
            foreach (IPedidoObserver o in _observadores.ToList())
                o.Actualizar(evento);

        }

        // ------------------------------------------------------------------ CONSULTAS

        public List<Prenda> ObtenerCatalogo()
        {
            return _prendas.LeerTodos().OrderBy(p => p.Categoria).ThenBy(p => p.Nombre).ToList();
        }

        public List<Pedido> ObtenerPedidos()
        {
            return _pedidos.LeerTodos().OrderByDescending(p => p.Id).ToList();
        }

        // Lo que tiene que mirar la persona de los lavarropas: lo que espera y lo que se está lavando.
        public List<Pedido> PedidosEnProceso()
        {
            return _pedidos.LeerTodos()
                .Where(p => p.Estado == EstadoPedido.EnEspera || p.Estado == EstadoPedido.Lavando)
                .OrderBy(p => p.Id)
                .ToList();
        }

        public int Contar(EstadoPedido estado)
        {
            return _pedidos.LeerTodos().Count(p => p.Estado == estado);
        }

        public decimal Facturacion()
        {
            return _pedidos.LeerTodos().Where(p => p.Estado != EstadoPedido.Cancelado).Sum(p => p.Total);
        }

        public List<string> FormasDePago()
        {
            return PagoFactory.TiposDisponibles();
        }

        // La vista lo usa para habilitar "Cuotas" y "CUIT" según la forma de pago elegida.
        public IPagoStrategy InfoPago(string tipo)
        {
            return PagoFactory.Crear(tipo);
        }

        // ------------------------------------------------------------------ ARMADO DEL PEDIDO

        public Pedido NuevoBorrador()
        {
            return new Pedido();
        }

        public void AgregarItem(Pedido borrador, int idPrenda, int cantidad)
        {
            Prenda prenda = _prendas.BuscarPorId(idPrenda);
            borrador.AgregarItem(prenda, cantidad);
        }

        // TODO 6 · Controller
        // CotizarPago: usá la factory para obtener la estrategia y delegale el cálculo (redondeado a 0 decimales).
        // ConfirmarPedido: validar, completar el borrador, calcular ajuste y total, guardar
        //                  y notificar el alta (TODO 7). Reglas en el README.

        public decimal CotizarPago(string tipoPago, decimal subtotal, int cuotas)
        {
            IPagoStrategy estrategia = PagoFactory.Crear(tipoPago);
            decimal ajuste = estrategia.CalcularAjuste(subtotal, cuotas);
            return Math.Round(ajuste, 0);
        }

        public Pedido ConfirmarPedido(Pedido borrador, string cliente, string telefono,
                                      string observaciones, string cuit, int cuotas, string tipoPago)
        {
            if (string.IsNullOrWhiteSpace(cliente))
                throw new ArgumentException("Cliente no puede quedar vacio.");
            if (borrador.Items.Count == 0)
                throw new ArgumentException("El pedido debe tener al menos una prenda.");
            
        }

        // ------------------------------------------------------------------ OPERACIONES

        public void AvanzarEstado(int id)
        {
            Pedido pedido = BuscarOFallar(id);
            EstadoPedido anterior = pedido.Estado;
            pedido.AvanzarEstado();
            _pedidos.Actualizar(pedido);

            Notificar(new EventoPedido(TipoEvento.CambioEstado, pedido, anterior));
        }

        public void Cancelar(int id)
        {
            Pedido pedido = BuscarOFallar(id);
            EstadoPedido anterior = pedido.Estado;
            pedido.Cancelar();
            _pedidos.Actualizar(pedido);

            Notificar(new EventoPedido(TipoEvento.Cancelacion, pedido, anterior));
        }

        private Pedido BuscarOFallar(int id)
        {
            return _pedidos.BuscarPorId(id) ?? throw new InvalidOperationException($"No existe el pedido #{id}.");
        }
    }
}
