using DigitalTVBilling.Jobs;

namespace DigitalTVBilling
{
    public class DPlusAppStartup : System.Web.Hosting.IProcessHostPreloadClient
    {
        public void Preload(string[] parameters)
        {
            try
            {
                JobSheduler.Start();
            }
            catch
            {
            }
        }
    }
}