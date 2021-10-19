using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreTodo.UnitTests
{
    public class TodoItemServiceShould
    {
        [Fact]
        public async Task Add_new_item_as_incomplete_with_due_date()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_AddNewItem").Options;

            using (var contex = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(contex);

                var fakeUser = new IdentityUser
                {
                    Id = "fake-000",
                    UserName = "fake@example.com"
                };

                await service.TryAddItemAsync(
                    new TodoItem
                    {
                        Title = "Test",
                        DueAt = DateTimeOffset.Now.AddDays(3)
                    }, fakeUser);
            }

            using (var context = new ApplicationDbContext(options))
            {
                var itemsInDb = await context.Items.CountAsync();
                Assert.Equal(1, itemsInDb);

                var item = await context.Items.FirstOrDefaultAsync();
                Assert.Equal("Test", item.Title);
                Assert.False(item.IsDone);

                var dateDifference = DateTimeOffset.Now.AddDays(3) - item.DueAt;
                Assert.True(dateDifference < TimeSpan.FromSeconds(1));
            }
        }

        [Fact]
        public async Task Return_false_if_nonexistent_id_is_passed_to_mark_done()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_MarkDone").Options;

            using (var context = new ApplicationDbContext(options))
            {
                var fakeUser = new IdentityUser
                {
                    Id = "fake-000",
                    UserName = "fake@example.com"
                };

                var fakeItem = new TodoItem
                {
                    Id = Guid.NewGuid(),
                    Title = "Test",
                    UserId = fakeUser.Id
                };

                await context.Items.AddAsync(fakeItem);
                await context.SaveChangesAsync();

                var service = new TodoItemService(context);
                var isDone = await service.TryMarkDoneAsync(id: Guid.Empty, fakeUser);
                Assert.False(isDone);
            }
        }

        [Fact]
        public async Task Return_true_when_mark_done_makes_valid_item_as_complete()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_MarkDone").Options;

            using (var context = new ApplicationDbContext(options))
            {
                var fakeUser = new IdentityUser
                {
                    Id = "fake-000",
                    UserName = "fake@example.com"
                };

                var fakeItem = new TodoItem
                {
                    Id = Guid.NewGuid(),
                    Title = "Test",
                    UserId = fakeUser.Id
                };

                await context.Items.AddAsync(fakeItem);
                await context.SaveChangesAsync();

                var service = new TodoItemService(context);
                var isDone = await service.TryMarkDoneAsync(fakeItem.Id, fakeUser);
                Assert.True(isDone);
            }
        }

        [Fact]
        public async Task Return_only_the_items_owned_by_a_particular_user()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_GetIncompleteItems").Options;

            using (var context = new ApplicationDbContext(options))
            {
                var fakeUser = new IdentityUser
                {
                    Id = "fake-000",
                    UserName = "fake@example.com"
                };

                var fakeItems = new TodoItem []
                {
                    new TodoItem
                    {
                        Id = Guid.NewGuid(),
                        Title = "Test",
                        UserId = fakeUser.Id
                    },
                    new TodoItem
                    {
                        Id = Guid.NewGuid(),
                        Title = "Test",
                        UserId = fakeUser.Id
                    }
                };

                await context.Items.AddRangeAsync(fakeItems);
                await context.SaveChangesAsync();

                var service = new TodoItemService(context);
                var items = await service.GetIncompleteItemsAsync(fakeUser);
                var fakeUserItems = items.Where(i => i.UserId == fakeUser.Id).ToArray();
                Assert.True(items.Length == fakeUserItems.Length);
            }
        }
    }
}
