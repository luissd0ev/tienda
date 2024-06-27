using System;
using System.Collections.Generic;

namespace Compras.Models;

public partial class Usuario
{
    public int Iduser { get; set; }

    public string Nameuser { get; set; } = null!;

    public string? Secondname { get; set; }

    public int? Edad { get; set; }

    public string Email { get; set; } = null!;

    public string Passworduser { get; set; } = null!;

    public virtual ICollection<Carritoscompra> Carritoscompras { get; set; } = new List<Carritoscompra>();

    public virtual ICollection<Ordene> Ordenes { get; set; } = new List<Ordene>();
}
