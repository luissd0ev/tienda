using System;
using System.Collections.Generic;

namespace Compras.Models;

public partial class Ordene
{
    public int Idorder { get; set; }

    public int Iduser { get; set; }

    public DateTime Fecha { get; set; }

    public decimal Total { get; set; }

    public virtual Usuario IduserNavigation { get; set; } = null!;

    public virtual ICollection<OrdenesArticulo> OrdenesArticulos { get; set; } = new List<OrdenesArticulo>();
}
