using System.Collections.Generic;
using System.Threading.Tasks;

namespace iMvcCore.Email
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, IEnumerable<string> cc = null);
        Task<bool> VerifyCodeAsync(string to, string code);
        Task<bool> SendCodeAsync(string to, string code);
    }
}