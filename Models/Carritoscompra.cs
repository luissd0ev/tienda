using System;
using System.Collections.Generic;

namespace Compras.Models;

public partial class Carritoscompra
{
    public int Idcarrito { get; set; }

    public int Idarticulo { get; set; }

    public decimal Price { get; set; }

    public virtual Articulo IdarticuloNavigation { get; set; } = null!;

    public virtual Usuario IdcarritoNavigation { get; set; } = null!;
}
