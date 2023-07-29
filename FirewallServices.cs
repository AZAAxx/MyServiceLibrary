using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebKurulum.Services
{
    public class FirewallServices
    {


        WebKurulum report = new WebKurulum();





        public void CreateFirewallRule(int[] ports)
        {
            NetFwTypeLib.NET_FW_RULE_DIRECTION_[] directions = { NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT };
            int[] protocols = { (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP, (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP };

            foreach (var direction in directions)
            {
                foreach (var protocol in protocols)
                {
                    foreach (var port in ports)
                    {
                        try
                        {
                            //create a new rule
                            INetFwRule2 Rule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));

                            Rule.Direction = direction;
                            Rule.Enabled = true;
                            Rule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                            Rule.Protocol = protocol;
                            Rule.LocalPorts = port.ToString();
                            Rule.Name = string.Format("{0}{1}", protocol, port);
                            Rule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;
                            Rule.Description = string.Format("Direction : {0}\r\n Protocol : {1} \r\n Port : {2}", direction, protocol, port);

                            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                            firewallPolicy.Rules.Add(Rule);
                        }
                        catch (Exception ex)
                        {
                            report.Report(string.Format("Exception creating the rule with protocol {0} and port {1}", protocol, port));
                            report.Report(ex.ToString());
                        }
                    }
                }
            }
        }
    }
}
