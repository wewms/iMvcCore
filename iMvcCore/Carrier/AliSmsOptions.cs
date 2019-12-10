namespace iMvcCore.Carrier
{
    public class AliSmsOptions
    {
        public string AliSmsAccessKeyId { get; set; }
        public string AliSmsAccessKeySecret { get; set; }

        public string AliSmsSignName { get; set; }

        public string AliSmsSigninTemplateCode { get; set; }
        public string AliSmsRegisterTemplateCode { get; set; }
        public string AliSmsCommonTemplateCode { get; set; }

        public int AliSmsExpireInSec { get; set; } = 300;
    }
}