using AspNetCoreTodo.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreTodo.Services
{
    public interface ITodoItemService
    {
        Task<TodoItem[]> GetIncompleteItemsAsync(IdentityUser user);
        Task<bool> TryAddItemAsync(TodoItem newItem, IdentityUser user);
        Task<bool> TryMarkDoneAsync(Guid id, IdentityUser user);
    }
}
