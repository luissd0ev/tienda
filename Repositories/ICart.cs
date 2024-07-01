﻿using Compras.Models;

namespace Compras.Repositories
{
    public interface ICart
    {
        public Task<Carritoscompra> Get(int idUsuario);

        public Task<bool> AddArticleToCart(int idUsuario, int idArticulo, decimal price, int cantidad); 

        
    }
}