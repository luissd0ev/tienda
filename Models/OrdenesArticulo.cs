using System;
using System.Collections.Generic;

namespace Compras.Models;

public partial class OrdenesArticulo
{
    public int Idorder { get; set; }

    public int Idarticulo { get; set; }

    public int Cantidad { get; set; }

    public virtual Articulo IdarticuloNavigation { get; set; } = null!;

    public virtual Ordene IdorderNavigation { get; set; } = null!;
}
