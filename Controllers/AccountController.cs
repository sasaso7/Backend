using BankBackend.Database.Models;
using BankBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankBackend.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // GET: api/Accounts
        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            var accounts = await _accountService.GetAccountsAsync();
            return Ok(accounts);
        }

        // GET: api/Account/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(string id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        // PUT: api/Account/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(string id, Account account)
        {
            if (id != account.Id)
            {
                return BadRequest();
            }

            var result = await _accountService.UpdateAccountAsync(id, account);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Account
        [HttpPost]

        public async Task<IActionResult> AddAccountToUser(Account account)
        {
            try
            {
                var createdAccount = await _accountService.CreateAccountAsync(account.UserId, account);
                return CreatedAtAction(nameof(GetAccount), new { id = createdAccount.Id }, createdAccount);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/Account/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var result = await _accountService.DeleteAccountAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
