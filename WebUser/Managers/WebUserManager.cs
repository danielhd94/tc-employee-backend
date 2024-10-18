using System;
using System.IO;
using System.Xml.Linq;

namespace WebUser.Managers
{
	public class WebUserManager
	{

        public void EnsureFileExists()
        {
            string directoryPath = Path.GetDirectoryName("/app/WebUser/WebUser.xml");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Check if the file exists and create it if not
            if (!File.Exists("/app/WebUser/WebUser.xml"))
            {
                // Create a new file (or handle the initial state as needed)
                XElement xElement = new XElement("Root");
                xElement.Save("/app/WebUser/WebUser.xml");
            }
        }
    }
}

