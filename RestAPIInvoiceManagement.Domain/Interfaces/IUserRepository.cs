using RestAPIInvoiceManagement.Domain.Entities;

   namespace RestAPIInvoiceManagement.Domain.Interfaces;

   public interface IUserRepository
   {
       Task<User?> GetByUsernameAsync(string username);
       Task AddAsync(User user);
       Task SaveChangesAsync();
   }