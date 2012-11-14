using InternetExplorerExtension;
using Microsoft.Win32;
using mshtml;
using SHDocVw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TOSCheckerService.Contracts;

namespace BHO
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("B1E01E0D-1954-4EDA-BA08-DF1D1C988F87")]
    [ProgId("ToS;DR Internet Explorer Add-On")]
    public class TOSDRAddOn : IObjectWithSite
    {
        IWebBrowser2 browser;
        ChannelFactory<ITOSCheckerService> factory = new ChannelFactory<ITOSCheckerService>(new NetNamedPipeBinding(), Constants.ServiceUrl);

        void OnDocumentComplete(object pDisp, ref object URL)
        {
            var uri = new Uri(URL.ToString());
            var channel = factory.CreateChannel();
            System.Diagnostics.Debug.Print("Attempting to load " + uri.Host);
            try
            {
                channel.LoadDomain(uri.Host);
            }
            catch (EndpointNotFoundException)
            {
                System.Diagnostics.Debug.Print("Endpoint is not listening");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print("Couldn't load: " + e.Message);
            }
        }

        int IObjectWithSite.SetSite(object site)
        {
            if (site != null)
            {
                browser = (IWebBrowser2)site;
                ((DWebBrowserEvents2_Event)browser).DocumentComplete +=
                    new DWebBrowserEvents2_DocumentCompleteEventHandler(this.OnDocumentComplete);
            }
            else
            {
                ((DWebBrowserEvents2_Event)browser).DocumentComplete -=
                    new DWebBrowserEvents2_DocumentCompleteEventHandler(this.OnDocumentComplete);
                browser = null;
            }
            return 0;
        }
        int IObjectWithSite.GetSite(ref Guid guid, out IntPtr ppvSite)
        {
            IntPtr punk = Marshal.GetIUnknownForObject(browser);
            int hr = Marshal.QueryInterface(punk, ref guid, out ppvSite);
            Marshal.Release(punk);
            return hr;
        }

        public static string RegBHO = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Browser Helper Objects";

        [ComRegisterFunction]
        public static void RegisterBHO(Type type)
        {
            string guid = type.GUID.ToString("B");
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegBHO, true);
            if (registryKey == null)
                registryKey = Registry.LocalMachine.CreateSubKey(RegBHO);
            RegistryKey key = registryKey.OpenSubKey(guid);
            if (key == null)
                key = registryKey.CreateSubKey(guid);
            key.SetValue("NoExplorer", 1, RegistryValueKind.DWord);
            registryKey.Close();
            key.Close();
        }

        [ComUnregisterFunction]
        public static void UnregisterBHO(Type type)
        {
            string guid = type.GUID.ToString("B");
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegBHO, true);
            if (registryKey != null)
                registryKey.DeleteSubKey(guid, false);
        }
    }
}
