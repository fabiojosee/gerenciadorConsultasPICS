﻿using gerenciadorConsultasPICS.Data;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gerenciadorConsultasPICS.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<T> ObterPorIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> ObterTodosAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task AdicionarAsync(T entidade)
        {
            await _context.Set<T>().AddAsync(entidade);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(T entidade)
        {
            _context.Set<T>().Update(entidade);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var entidade = await ObterPorIdAsync(id);
            _context.Set<T>().Remove(entidade);
            await _context.SaveChangesAsync();
        }
    }
}
