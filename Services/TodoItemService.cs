using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using Microsoft.AspNetCore.Identity;
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

        public async Task<TodoItem[]> GetIncompleteItemsAsync(IdentityUser user)
        {
            return await _dbContext.Items
                .Where(i => i.IsDone == false && i.UserId == user.Id)
                .ToArrayAsync();
        }

        public async Task<bool> TryMarkDoneAsync(Guid id, IdentityUser user)
        {
            var item = await _dbContext.Items
                .Where(i => i.Id == id && i.UserId == user.Id)
                .SingleOrDefaultAsync();

            if (item == null)
            {
                return false;
            }

            item.IsDone = true;

            var saveResult = await _dbContext.SaveChangesAsync();
            const int numberStateEntries = 1;
            return saveResult == numberStateEntries;
        }

        public async Task<bool> TryAddItemAsync(TodoItem newItem, IdentityUser user)
        {
            newItem.Id = Guid.NewGuid();
            newItem.IsDone = false;
            newItem.UserId = user.Id;

            await _dbContext.Items.AddAsync(newItem);

            var saveResult = await _dbContext.SaveChangesAsync();
            const int numberStateEntries = 1;
            return saveResult == numberStateEntries;
        }

    }
}
