using Microsoft.EntityFrameworkCore;
   using RestAPIInvoiceManagement.Domain.Entities;
   using RestAPIInvoiceManagement.Domain.Interfaces;
   using RestAPIInvoiceManagement.Infrastructure.Data;

   namespace RestAPIInvoiceManagement.Infrastructure.Repositories;

   public class UserRepository(AppDbContext context) : IUserRepository
   {
       public async Task<User?> GetByUsernameAsync(string username)
       {
           return await context.Users.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
       }

       public async Task AddAsync(User user)
       {
           await context.Users.AddAsync(user);
       }

       public async Task SaveChangesAsync()
       {
           await context.SaveChangesAsync();
       }
   }