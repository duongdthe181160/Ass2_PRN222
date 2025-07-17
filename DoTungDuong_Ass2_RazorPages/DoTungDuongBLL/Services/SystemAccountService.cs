using DoTungDuongDAL.Models;
using DoTungDuongDAL.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DoTungDuongBLL.Services
{
    public class SystemAccountService
    {
        private readonly IRepository<SystemAccount> _repository;
        private readonly IConfiguration _configuration;

        public SystemAccountService(IRepository<SystemAccount> repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public IEnumerable<SystemAccount> GetAllAccounts()
        {
            return _repository.GetAll();
        }

        public SystemAccount? GetAccountById(short id)
        {
            return _repository.GetById(id);
        }

        public SystemAccount? GetAccountByEmail(string email)
        {
            return _repository.Search(a => a.AccountEmail == email).FirstOrDefault();
        }

        public void AddAccount(SystemAccount account)
        {
            _repository.Add(account);
        }

        public void UpdateAccount(SystemAccount account)
        {
            _repository.Update(account);
        }

        public void DeleteAccount(short id)
        {
            var account = GetAccountById(id);
            if (account != null)
            {
                _repository.Delete(account);
            }
        }

        public IEnumerable<SystemAccount> SearchAccounts(string keyword)
        {
            return _repository.Search(a => (a.AccountName != null && a.AccountName.Contains(keyword)) || 
                                          (a.AccountEmail != null && a.AccountEmail.Contains(keyword)));
        }

        public ClaimsPrincipal? Authenticate(string email, string password)
        {
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (email == adminEmail && password == adminPassword)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, "Admin")
            };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                return new ClaimsPrincipal(identity);
            }

            var account = _repository.Search(a => a.AccountEmail == email && a.AccountPassword == password).FirstOrDefault();
            if (account != null)
            {
                var role = account.AccountRole == 1 ? "Staff" : "Lecturer";
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                return new ClaimsPrincipal(identity);
            }

            return null;
        }

        public void UpdateProfile(short id, SystemAccount updated)
        {
            var account = GetAccountById(id);
            if (account != null)
            {
                account.AccountName = updated.AccountName;
                account.AccountEmail = updated.AccountEmail;
                if (!string.IsNullOrEmpty(updated.AccountPassword)) 
                    account.AccountPassword = updated.AccountPassword;  // Hash if needed
                UpdateAccount(account);
            }
        }
    }
}