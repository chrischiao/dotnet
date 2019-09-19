using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.Management;


namespace dotnet
{
    public class ComputerInfo
    {
        public ComputerInfo()
        {

        }

        public string Output()
        {
            StringBuilder builder = new StringBuilder();

            // cpu
            string[] properties = new[] { "AddressWidth", "Architecture", "AssetTag", "Caption", "DataWidth", "Description", "Family", "Level", "Manufacturer", "MaxClockSpeed", "Name", "NumberOfCores", "PartNumber", "ProcessorId", "Revision", "SerialNumber", "Version" };
            builder.Append(GetInfo("CPU", "Win32_Processor", properties));

            // bios
            builder.Append(GetBIOSInfo());

            // MotherBoard
            properties = new string[] { "Caption", "Description", "Manufacturer", "Name", "Product", "SerialNumber", "Tag", "Version" };
            builder.Append(GetInfo("MB", "Win32_BaseBoard", properties));

            // Hard Disk Drive
            properties = new string[] { "Caption", "FirmwareRevision", "Model", "SerialNumber", "Size" };
            builder.Append(GetInfo("HDD", "Win32_DiskDrive", properties));

            // Network Card
            builder.Append(GetNetworkInterfaceInfo());

            // OS
            properties = new string[] { "Caption", "OSArchitecture", "Version", "ServicePackMajorVersion", "CSName" };
            builder.Append(GetInfo("OS", "Win32_OperatingSystem", properties));

            // RAM
            builder.Append(GetPhysicalMemoryInfo());

            // ip
            builder.Append(GetIPInfo());

            // UserName
            builder.Append(GetWindowsUserInfo());

            return builder.ToString();
        }

        private string GetProcessorInfo()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(HeadLine("CPU"));

            ManagementClass mc = new ManagementClass("Win32_Processor");
            string[] properties = new string[] { "AddressWidth", "Architecture", "AssetTag", "Caption", "DataWidth", "Description", "Family", "Level", "Manufacturer", "MaxClockSpeed", "Name", "NumberOfCores", "PartNumber", "ProcessorId", "Revision", "SerialNumber", "Version" };

            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                foreach (string property in properties)
                {
                    builder.Append($"{property} : { mo[property]}\r\n");
                }
                builder.Append("\r\n");
            }

            return builder.ToString();
        }

        private string GetBIOSInfo()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(HeadLine("BIOS"));

            ManagementClass mc = new ManagementClass("Win32_BIOS");
            string[] properties = new string[] { "Caption", "Description", "Manufacturer", "Name", "ReleaseDate", "SerialNumber", "SMBIOSBIOSVersion", "Version" };

            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                var availableProperties = new List<string>();
                foreach (PropertyData property in mo.Properties)
                    availableProperties.Add(property.Name);

                string version = "";
                if (availableProperties.Contains("BIOSVersion"))
                {
                    string[] biosVersion = mo["BIOSVersion"] as string[];
                    version = string.Join("; ", biosVersion);
                }
                builder.Append($"BIOSVersion : {version}\r\n");

                foreach (string property in properties)
                {
                    string propertyValue = "";
                    if (availableProperties.Contains(property))
                        propertyValue = mo[property].ToString();

                    builder.Append($"{property} : {propertyValue}\r\n");
                }
                builder.Append("\r\n");
            }

            return builder.ToString();
        }

        private string GetBaseBoardInfo()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(HeadLine("MB"));

            ManagementClass mc = new ManagementClass("Win32_BaseBoard");
            string[] properties = new string[] { "Caption", "Description", "Manufacturer", "Name", "Product", "SerialNumber", "Tag", "Version" };

            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                foreach (string property in properties)
                {
                    builder.Append($"{property} : { mo[property]}\r\n");
                }
                builder.Append("\r\n");
            }

            return builder.ToString();
        }

        private string GetDiskDriveInfo()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(HeadLine("HDD"));

            ManagementClass mc = new ManagementClass("Win32_DiskDrive");
            string[] properties = new string[] { "Caption", "FirmwareRevision", "Model", "SerialNumber", "Size" };

            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                foreach (string property in properties)
                {
                    builder.Append($"{property} : { mo[property]}\r\n");
                }
                builder.Append("\r\n");
            }

            return builder.ToString();
        }

        private string GetNetworkInterfaceInfo()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(HeadLine("Network Card"));

            NetworkInterface[] fNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in fNetworkInterfaces)
            {
                //builder.Append($"Id : {adapter.Id}\r\n"); 
                builder.Append($"Name : {adapter.Name}\r\n");
                builder.Append($"Description : {adapter.Description}\r\n");
                builder.Append($"InterfaceType : {adapter.NetworkInterfaceType}\r\n");
                builder.Append($"PhysicalAddress : {adapter.GetPhysicalAddress().ToString()}\r\n");
                builder.Append("\r\n");
            }

            return builder.ToString();
        }

        private string GetNetworkAdapterInfo()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(HeadLine("Network Card"));

            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            string[] properties = new string[] { "Description", "MACAddress" };

            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                foreach (string property in properties)
                {
                    builder.Append($"{property} : { mo[property]}\r\n");
                }
                builder.Append("\r\n");
            }

            return builder.ToString();
        }

        private string GetOSInfo()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(HeadLine("OS"));

            ManagementClass mc = new ManagementClass("Win32_OperatingSystem");
            string[] properties = new string[] { "Caption", "OSArchitecture", "Version", "ServicePackMajorVersion", "CSName" };

            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                foreach (string property in properties)
                {
                    builder.Append($"{property} : { mo[property]}\r\n");
                }
                builder.Append("\r\n");
            }

            return builder.ToString();
        }

        private string GetPhysicalMemoryInfo()
        {
            string result = HeadLine("RAM");

            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                var availableProperties = new List<string>();
                foreach (PropertyData property in mo.Properties)
                    availableProperties.Add(property.Name);

                string memory = "";
                if (availableProperties.Contains("TotalPhysicalMemory"))
                    memory = Math.Round(((ulong)mo["TotalPhysicalMemory"]) * 1.0 / 1024 / 1024 / 1024, 2).ToString();

                result += $"TotalPhysicalMemory : {memory}\r\n";
                break;
            }

            return result;
        }

        private string GetIPInfo()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(HeadLine("IP"));

            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                var availableProperties = new List<string>();
                foreach (PropertyData property in mo.Properties)
                    availableProperties.Add(property.Name);

                if (availableProperties.Contains("IPEnabled"))
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        builder.Append($"IPAddress : { ((string[])mo["IPAddress"])[0]}\r\n");
                        builder.Append($"IPSubnet : { ((string[])mo["IPSubnet"])[0]}\r\n");
                        builder.Append($"DefaultIPGateway : { ((string[])mo["DefaultIPGateway"])[0]}\r\n");
                        builder.Append("\r\n");
                    }
                }
            }

            return builder.ToString();
        }

        private string GetWindowsUserInfo()
        {
            string result = HeadLine("Windows User");
            result += $"UserName : {System.Environment.UserName}\r\n";
            return result;
        }

        private string HeadLine(string caption)
        {
            return $"------------------------------ {caption} ------------------------------\r\n";
        }

        private string GetInfo(string header, string path, string[] properties)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(HeadLine(header));

            ManagementClass mc = new ManagementClass(path);
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                var availableProperties = new List<string>();
                foreach (PropertyData property in mo.Properties)
                    availableProperties.Add(property.Name);

                foreach (string property in properties)
                {
                    string propertyValue = "";
                    if (availableProperties.Contains(property))
                        propertyValue = mo[property].ToString();

                    builder.Append($"{property} : {propertyValue}\r\n");
                }
                builder.Append("\r\n");
            }

            return builder.ToString();
        }
    }
}
