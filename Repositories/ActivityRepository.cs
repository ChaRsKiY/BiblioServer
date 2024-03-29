using System;
using System.Diagnostics;
using BiblioServer.Context;
using BiblioServer.Interfaces;
using BiblioServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BiblioServer.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly ApplicationDbContext _context;

        public ActivityRepository(ApplicationDbContext context)
        {
            _context = context;   
        }

        public async Task AddActivityAsync(ActivityModel model)
        {
            await _context.Activities.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public void AddActivity(ActivityModel model)
        {
            _context.Activities.Add(model);
            _context.SaveChanges();
        }

        public async Task<object> GetActivitiesAsync(int page, int pageSize)
        {
            var query = _context.Activities.AsQueryable();
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize); 

            var activities = await query
                .OrderByDescending(a => a.Time)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new { TotalItems = totalPages, Activities = activities };
        }
    }
}

