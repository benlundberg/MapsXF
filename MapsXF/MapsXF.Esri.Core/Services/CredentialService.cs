using Esri.ArcGISRuntime.Security;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Esri.Core.Services
{
    public static class CredentialService
    {
        public static async Task<Credential> CreateCredential(CredentialRequestInfo info)
        {
            Credential credential = null;

            try
            {
                credential = new ArcGISNetworkCredential
                {
                    Credentials = new System.Net.NetworkCredential(ServiceConfig.Username, ServiceConfig.Password),
                    ServiceUri = info.ServiceUri
                };
            }
            catch (TaskCanceledException)
            {
                return credential;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            return credential;
        }

        public static async Task<Credential> CreateEsriCredential(CredentialRequestInfo info)
        {
            Credential credential = null;

            try
            {
                credential = await AuthenticationManager.Current.GenerateCredentialAsync(info.ServiceUri, ServiceConfig.EsriUsername, ServiceConfig.EsriPassword);
            }
            catch (TaskCanceledException)
            {
                return credential;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            return credential;
        }
    }
}
