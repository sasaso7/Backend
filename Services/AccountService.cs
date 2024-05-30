using Amazon.S3;
using Amazon.S3.Transfer;
using BankBackend.Database.Models;
using EFGetStarted.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BankBackend.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAccountsAsync();
        Task<bool> UploadAccountPictureAsync(string accountId, IFormFile file);
        Task<Account?> GetAccountByIdAsync(string id);
        Task<Account> CreateAccountAsync(CreateAccountRequest account);
        Task<bool> UpdateAccountAsync(string id, Account account);
        Task<bool> DeleteAccountAsync(string id);
    }
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public AccountService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            _s3Client = new AmazonS3Client(
                _configuration["AWS:AccessKey"],
                _configuration["AWS:SecretKey"],
                Amazon.RegionEndpoint.GetBySystemName(_configuration["AWS:Region"]));
            _bucketName = _configuration["AWS:BucketName"];
        }

        public async Task<IEnumerable<Account>> GetAccountsAsync()
        {
            return await _context.Accounts.ToListAsync();
        }

        public async Task<Account?> GetAccountByIdAsync(string id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        public async Task<bool> UploadAccountPictureAsync(string accountId, IFormFile file)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return false;
            }

            if (file != null && file.Length > 0)
            {
                var fileTransferUtility = new TransferUtility(_s3Client);

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    var fileName = Path.GetFileName(file.FileName);
                    var fileExtension = Path.GetExtension(fileName);

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = stream,
                        Key = $"{accountId}/ProfilePicture{fileExtension}",
                        BucketName = _bucketName,
                        CannedACL = S3CannedACL.PublicRead
                    };

                    await fileTransferUtility.UploadAsync(uploadRequest);

                    var fileUrl = $"https://{_bucketName}.s3.{_configuration["AWS:Region"]}.amazonaws.com/{uploadRequest.Key}";
                    account.Picture = fileUrl;
                    _context.Accounts.Update(account);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<Account> CreateAccountAsync(CreateAccountRequest account)
        {

            var user = await _context.Users.Include(u => u.Accounts).FirstOrDefaultAsync(u => u.Id == account.UserId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            // Add the account to the user's list of accounts
            if (user.Accounts == null)
            {
                user.Accounts = new List<Account>();
            }

            Account newAccount = new Account
            {
                Name = account.Name,
                UserId = account.UserId,
            };
            user.Accounts.Add(newAccount);

            // Add the account to the Accounts DbSet
            _context.Accounts.Add(newAccount);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return newAccount;
        }

        public async Task<bool> UpdateAccountAsync(string id, Account account)
        {
            if (id != account.Id)
            {
                return false;
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AccountExistsAsync(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAccountAsync(string id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return false;
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> AccountExistsAsync(string id)
        {
            return await _context.Accounts.AnyAsync(e => e.Id == id);
        }
    }
}
