using System.ComponentModel;

namespace FTWContracts
{
    public partial class FTWSmithNep17
    {
        [DisplayName("Fault")]
        public static event OnFaultDelegate OnFault;
        public delegate void OnFaultDelegate(string message, object data);
    }
}
