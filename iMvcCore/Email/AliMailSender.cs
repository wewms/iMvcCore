using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using iMvcCore.Logging;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace iMvcCore.Email
{
    public class AliMailSender : IEmailSender
    {
        private const string AliMailCacheKeyPrefix = "ali_mail_code_";
        private readonly IDistributedCache _cache;
        private readonly AppLogger _logger;
        private readonly AliMailOptions _options;

        public AliMailSender(IOptions<AliMailOptions> options, AppLogger logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
            _options = options.Value;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, IEnumerable<string> cc = null)
        {
            // Verify
            if(string.IsNullOrEmpty(to)) throw new ArgumentNullException(nameof(to));
            if(string.IsNullOrEmpty(subject)) throw new ArgumentNullException(nameof(subject));
            if(string.IsNullOrEmpty(body)) throw new ArgumentNullException(nameof(body));

            var message = new MailMessage(new MailAddress(_options.AliMailAccount, _options.AliMailDisplayName), new MailAddress(to)) {Subject = subject, Body = body, IsBodyHtml = true};
            if(cc != null)
            {
                foreach(string s in cc)
                {
                    message.CC.Add(s);
                }
            }

            try
            {
                using(var smtpClient = new SmtpClient(_options.AliMailSmtp, _options.AliMailPort) {Credentials = new NetworkCredential(_options.AliMailAccount, _options.AliMailPassword), DeliveryMethod = SmtpDeliveryMethod.Network, EnableSsl = _options.AliMailEnableSsl})
                {
                    //smtpClient.Send(message);
                    await smtpClient.SendMailAsync(message);
                }

                return true;
            }
            catch(Exception ex)
            {
                _logger.EnqueueMessage($"{nameof(AliMailSender)}.{nameof(SendEmailAsync)} error. details: {ex.Message} stackTrace: {ex.StackTrace}");
            }

            return false;
        }

        public async Task<bool> VerifyCodeAsync(string to, string code)
        {
            if(string.IsNullOrEmpty(to)) throw new ArgumentNullException(nameof(to));
            if(string.IsNullOrEmpty(code)) throw new ArgumentNullException(nameof(code));

            var cached = await _cache.GetStringAsync($"{AliMailCacheKeyPrefix}{to}");
            return cached == code;
        }

        public Task<bool> SendCodeAsync(string to, string code)
        {
            _cache.SetAsync($"{AliMailCacheKeyPrefix}{to}", Encoding.UTF8.GetBytes(code), new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.AliMailExpireInSec)
            });

            return SendEmailAsync(to, "验证码", code);
        }
    }
}