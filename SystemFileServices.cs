using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace WebKurulum.Services
{
    public class SystemFileServices
    {


        WebKurulum report = new WebKurulum();





        //copy the contesnt of the sourceDir into destDir
        public void copy_dir(string sourceDir, string destDir)
        {
            //Check if sourceDir and destDir exist
            if (!Directory.Exists(sourceDir))
            {
                report.Report(string.Format("Source directory {0} doesn't exist.", sourceDir));
                return;
            }
            if (!Directory.Exists(destDir))
            {
                report.Report(string.Format("Source directory {0} doesn't exist.", destDir));
                return;
            }

            //get all directories from sourceDir and create them in destDir
            try
            {
                var allDirectories = Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories);
                foreach (var directory in allDirectories)
                {
                    //report.Report(directory);
                    String dir_name = directory.Substring(sourceDir.Length + 1);
                    Directory.CreateDirectory(Path.Combine(destDir, dir_name));
                    if (!Directory.Exists(directory))
                    {
                        report.Report(string.Format("Unable to create {0}.", directory));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                report.Report(ex.Message);
                report.Report(string.Format("Unable to copy the directories from {}", sourceDir));
            }


            //get all files from sourceDir and create them in destDir
            try
            {
                var allFiles = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
                foreach (var file_path in allFiles)
                {
                    try
                    {
                        String file_name = file_path.Substring(sourceDir.Length + 1);
                        File.Copy(Path.Combine(sourceDir, file_name), Path.Combine(destDir, file_name), true);
                    }
                    catch (Exception ex)
                    {
                        report.Report(string.Format("Unable to copy the file at {0}", file_path));
                        report.Report(ex.Message);
                    }

                }
            }
            catch (Exception ex)
            {
                report.Report(ex.Message);
                report.Report(string.Format("Unable to copy the files from directory {}", sourceDir));
            }
        }

















        //create a directory with path=path
        public void create_dir(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    report.Report(string.Format("Unable create {0}. Directory already exists.", path));
                    return;
                }

                Directory.CreateDirectory(path);

                if (!Directory.Exists(path))
                {
                    report.Report(string.Format("Unable create {0}.", path));
                    return;
                }
            }
            catch (Exception ex)
            {
                report.Report(ex.Message);
            }
        }
















        //alter the security property of the files
        public void FolderPermission(String accountName, String folderPath)
        {
            try
            {
                FileSystemRights Rights;

                //What rights are we setting? Here accountName is == "IIS_IUSRS"

                Rights = FileSystemRights.FullControl;
                bool modified;
                var none = new InheritanceFlags();
                none = InheritanceFlags.None;

                //set on dir itself
                var accessRule = new FileSystemAccessRule(accountName, Rights, none, PropagationFlags.NoPropagateInherit, AccessControlType.Allow);
                var dInfo = new DirectoryInfo(folderPath);
                var dSecurity = dInfo.GetAccessControl();
                dSecurity.ModifyAccessRule(AccessControlModification.Set, accessRule, out modified);

                //Always allow objects to inherit on a directory 
                var iFlags = new InheritanceFlags();
                iFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;

                //Add Access rule for the inheritance
                var accessRule2 = new FileSystemAccessRule(accountName, Rights, iFlags, PropagationFlags.InheritOnly, AccessControlType.Allow);
                dSecurity.ModifyAccessRule(AccessControlModification.Add, accessRule2, out modified);

                dInfo.SetAccessControl(dSecurity);
            }
            catch (Exception ex)
            {
                report.Report(ex.Message);
            }
        }













        //edit json file
        public void alter_connectionString(String path, String connectionString)
        {
            connectionString = connectionString.Replace("\\\\", "\\");
            try
            {
                string json = File.ReadAllText(path);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                jsonObj["ConnectionStrings"]["DefaultConnection"] = connectionString;
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(path, output);
            }
            catch (Exception ex)
            {
                report.Report(ex.ToString());
            }
        }












        public void alter_BaseUrl(String path, String BaseUrl)
        {
            try
            {
                string json = File.ReadAllText(path);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                jsonObj["ConfigurationManager"]["BaseUrl"] = BaseUrl;
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(path, output);
            }
            catch (Exception ex)
            {
                report.Report(ex.Message);
            }
        }
    }
}
