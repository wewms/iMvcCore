using System.Threading.Tasks;

namespace iMvcCore.Carrier
{
    public interface ISmsSender
    {
        Task<(bool IsCompleted, string Result)> SendAsync(string receiver, string paramString, string templateCode);

        Task<bool> SendSigninCodeAsync(string receiver, string code);
        Task<bool> SendRegisterCodeAsync(string receiver, string code);
        Task<bool> SendCommonCodeAsync(string receiver, string code);

        Task<bool> VerifySigninCodeAsync(string receiver, string inputCode);
        Task<bool> VerifyRegisterCodeAsync(string receiver, string inputCode);
        Task<bool> VerifyCommonCodeAsync(string receiver, string inputCode);
    }
}