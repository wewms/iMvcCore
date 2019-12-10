namespace iMvcCore.Email
{
    public class AliMailOptions
    {
        public string AliMailAccount { get; set; }
        public string AliMailPassword { get; set; }
        public string AliMailDisplayName { get; set; }

        public string AliMailSmtp { get; set; }
        public int AliMailPort { get; set; }
        public bool AliMailEnableSsl { get; set; }

        public int AliMailExpireInSec { get; set; }
    }
}