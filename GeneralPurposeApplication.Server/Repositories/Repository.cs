using GeneralPurposeApplication.Server.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context) => _context = context;

        public async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().AsNoTracking().ToListAsync();
        public IQueryable<T> GetQueryable() => _context.Set<T>().AsNoTracking();
        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            
            if(entity != null)
                _context.Set<T>().Remove(entity);
        }
    }
}
