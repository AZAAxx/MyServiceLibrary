using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace WebKurulum.Services
{
    public class ServerManagerServices
    {


        WebKurulum report = new WebKurulum();






        public void CreateApplicationPool(String poolName)
        {
            ServerManager server = new ServerManager();
            ApplicationPool myApplicationPool = null;
            try
            {
                //create a new ApplicationPool
                myApplicationPool = server.ApplicationPools.Add(poolName);

                if (myApplicationPool != null)
                {
                    //set the runtime version
                    myApplicationPool.ManagedRuntimeVersion = "";  /*"No Managed Code";*/

                    //set the Idle Time-out(minutes)
                    myApplicationPool.ProcessModel.IdleTimeout = TimeSpan.Zero;

                    //save our new ApplicationPool
                    server.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                report.Report(ex.Message);
            }
        }












        public void AddWebsite(String siteName, String physicalPath, int port, String poolName, String IP)
        {
            try
            {
                ServerManager serverManager = new ServerManager();

                //IP = ipEntry.AddressList[1].toString();
                string bindingInfo = string.Format(@"{0}:{1}:{2}", IP, port.ToString(), "");

                //add the new Site to the Sites collection
                Site mySite = serverManager.Sites.Add(siteName, "http", bindingInfo, physicalPath);

                //Site mySite = serverManager.Sites.Add(siteName, physicalPath, port);
                mySite.ServerAutoStart = true;
                mySite.Applications[0].ApplicationPoolName = poolName;

                serverManager.CommitChanges();
            }
            catch (Exception ex)
            {
                report.Report(ex.Message);
            }
        }












        public IPAddress[] GetIPs()
        {
            ServerManager serverManager = new ServerManager();
 
            // Get the Name of HOST  
            string hostName = Dns.GetHostName();

            //GetHostEntry
            IPHostEntry ipEntry = Dns.GetHostEntry(hostName);

            //ipEntry.AddressList holds the IP address list for Host hostName
            IPAddress[] addr = ipEntry.AddressList;

            return addr;
        }
    }
}
