using System;
using System.Collections.Generic;

namespace Compras.Models;

public partial class Articulo
{
    public int Idart { get; set; }

    public string Nameart { get; set; } = null!;

    public decimal Priceart { get; set; }

    public int Quantityart { get; set; }

    public virtual ICollection<Carritoscompra> Carritoscompras { get; set; } = new List<Carritoscompra>();

    public virtual ICollection<OrdenesArticulo> OrdenesArticulos { get; set; } = new List<OrdenesArticulo>();
}
