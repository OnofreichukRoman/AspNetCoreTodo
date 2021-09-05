using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreTodo.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly ApplicationDbContext _dbContext;

        public TodoItemService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TodoItem[]> GetIncompleteItemsAsync()
        {
            return await _dbContext.Items
                .Where(i => i.IsDone == false)
                .ToArrayAsync();
        }

        public async Task<bool> TryAddItemAsync(TodoItem newItem)
        {
            newItem.Id = Guid.NewGuid();
            newItem.IsDone = false;

            await _dbContext.Items.AddAsync(newItem);

            var saveResult = await _dbContext.SaveChangesAsync();
            const int numberStateEntries = 1;
            return saveResult == numberStateEntries;
        }
    }
}
