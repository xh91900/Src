using System.Text;

namespace SuperProducer.Core.Utility.Encrypt
{
    public class EncryptBase
    {
        public virtual Encoding DefaultEncode { get; set; }

        public EncryptBase()
        {
            if (this.DefaultEncode == null)
            {
                this.DefaultEncode = InternalConstant.DefaultEncode;
            }
        }
    }
}
