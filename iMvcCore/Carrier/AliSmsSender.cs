using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using iMvcCore.Logging;
using iMvcCore.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace iMvcCore.Carrier
{
    public class AliSmsSender : ISmsSender
    {
        private const string AliSmsCacheKeyPrefix = "ali_sms_";
        private const string Text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        private readonly IDistributedCache _cache;
        private readonly HttpClient _client;
        private readonly AppLogger _logger;
        private readonly AliSmsOptions _options;

        public AliSmsSender(AppLogger logger, IDistributedCache cache, IOptions<AliSmsOptions> options, HttpClient client)
        {
            _logger = logger;
            _cache = cache;
            _client = client;
            _options = options.Value;
        }

        public async Task<(bool IsCompleted, string Result)> SendAsync(string receiver, string paramString, string templateCode)
        {
            if(string.IsNullOrEmpty(receiver) || !RegexCache.MobilePattern.IsMatch(receiver)) throw new ArgumentException("Invalid receiver number", nameof(receiver));
            if(string.IsNullOrEmpty(paramString)) throw new ArgumentNullException(nameof(paramString));
            if(string.IsNullOrEmpty(templateCode)) throw new ArgumentNullException(nameof(templateCode));

            var timestamp = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            var nonce = Guid.NewGuid().ToString();

            StringBuilder builder = new StringBuilder();

            // system and business parameters, already order by asc.
            builder.Append($"AccessKeyId={_options.AliSmsAccessKeyId}&");
            builder.Append("Action=SendSms&");
            builder.Append("Format=XML&");
            builder.Append($"PhoneNumbers={PercentEncode(receiver)}&");
            builder.Append($"RegionId={PercentEncode("cn-hangzhou")}&");
            builder.Append($"SignName={PercentEncode(_options.AliSmsSignName)}&");
            builder.Append("SignatureMethod=HMAC-SHA1&");
            builder.Append($"SignatureNonce={nonce}&");
            builder.Append("SignatureVersion=1.0&");
            builder.Append($"TemplateCode={PercentEncode(templateCode)}&");
            builder.Append($"TemplateParam={PercentEncode(paramString)}&");
            builder.Append($"Timestamp={PercentEncode(timestamp)}&");
            builder.Append("Version=2017-05-25");

            // 计算签名
            var signString = $"GET&%2F&{Regex.Replace(Encode(builder.ToString()), @"%[a-f\d]{2}", m => m.Value.ToUpperInvariant())}";
            HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(_options.AliSmsAccessKeySecret + "&"));
            var sign = Convert.ToBase64String(hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(signString)));

            try
            {
                var url = $"http://dysmsapi.aliyuncs.com/?AccessKeyId={Encode(_options.AliSmsAccessKeyId)}&Action=SendSms&Format=XML&PhoneNumbers={Encode(receiver)}&RegionId={Encode("cn-hangzhou")}&SignatureMethod=HMAC-SHA1&SignatureNonce={Encode(nonce)}&SignatureVersion=1.0&SignName={Encode(_options.AliSmsSignName)}&TemplateCode={Encode(templateCode)}&TemplateParam={Encode(paramString)}&Timestamp={Encode(timestamp)}&Version={Encode("2017-05-25")}&Signature={Encode(sign)}";
                var result = await _client.GetStringAsync(url);

                return (result.EndsWith("<Code>OK</Code></SendSmsResponse>", StringComparison.OrdinalIgnoreCase), result);
            }
            catch(Exception ex)
            {
                _logger.EnqueueMessage($"{nameof(AliSmsSender)}.{nameof(SendAsync)} error. message: {ex.Message} stackTrace: {ex.StackTrace}");
            }

            return (false, "error");
        }

        public Task<bool> SendSigninCodeAsync(string receiver, string code)
        {
            return SendCodeInternalAsync(receiver, code, _options.AliSmsSigninTemplateCode, "sig_");
        }

        public Task<bool> SendRegisterCodeAsync(string receiver, string code)
        {
            return SendCodeInternalAsync(receiver, code, _options.AliSmsRegisterTemplateCode, "reg_");
        }

        public Task<bool> SendCommonCodeAsync(string receiver, string code)
        {
            return SendCodeInternalAsync(receiver, code, _options.AliSmsCommonTemplateCode, "com_");
        }

        public Task<bool> VerifySigninCodeAsync(string receiver, string inputCode)
        {
            return VerifyCodeInternalAsync(receiver, inputCode, "sig_");
        }

        public Task<bool> VerifyRegisterCodeAsync(string receiver, string inputCode)
        {
            return VerifyCodeInternalAsync(receiver, inputCode, "reg_");
        }

        public Task<bool> VerifyCommonCodeAsync(string receiver, string inputCode)
        {
            return VerifyCodeInternalAsync(receiver, inputCode, "com_");
        }

        private async Task<bool> SendCodeInternalAsync(string receiver, string code, string templateCode, string prefix = "")
        {
            var (isCompleted, _) = await SendAsync(receiver, $"{{code:\"{code}\"}}", templateCode);
            if(isCompleted)
            {
                await _cache.SetAsync($"{AliSmsCacheKeyPrefix}{prefix}{receiver}", Encoding.UTF8.GetBytes(code), new DistributedCacheEntryOptions {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.AliSmsExpireInSec)
                });
            }

            return isCompleted;
        }

        private async Task<bool> VerifyCodeInternalAsync(string receiver, string inputCode, string prefix = "")
        {
            if(string.IsNullOrEmpty(receiver) || !RegexCache.MobilePattern.IsMatch(receiver)) throw new ArgumentException("Invalid receiver number", nameof(receiver));
            if(string.IsNullOrEmpty(inputCode)) throw new ArgumentNullException(nameof(inputCode));

            var cached = await _cache.GetStringAsync($"{AliSmsCacheKeyPrefix}{prefix}{receiver}");
            return cached == inputCode;
        }

        #region Utils

        private static string Encode(string value)
        {
            return HttpUtility.UrlEncode(value, Encoding.UTF8);
        }

        public static string PercentEncode(string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            byte[] array = bytes;

            for(int i = 0; i < array.Length; i++)
            {
                char c = (char) array[i];
                if(Text.IndexOf(c) >= 0)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int) c));
                }
            }

            return stringBuilder.ToString();
        }

        #endregion
    }
}