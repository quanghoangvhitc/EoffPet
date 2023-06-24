using Newtonsoft.Json.Linq;
using Petrolimex.Bean;
using Petrolimex.Class;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;


namespace Petrolimex.DataProvider
{
    public class ProviderUser : ProviderBase
    {
        public BeanUser GetCurrentUserInfo()
        {
            BeanUser retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "CurrentUser"));
                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;

                JObject retData = GetJsonDataFromAPI(combieUrl + "/_layouts/15/VuThao.Petrolimex.API/ApiUser.ashx?func=CurrentUser", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<BeanUser>();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error - ProviderUser - GetCurrentUserInfo - Err:" + ex);

                return null;
            }
            return retValue;
        }

        public List<BeanSettings> GetAppSettings()
        {
            try
            {
                PAR par = new PAR(null, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=get&bname=BeanSettings", ref CmmVariable.M_AuthenticatedHttpClient, par, false);

                if (retData == null) return null;
                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("ERR"))
                    return null;
                if (strStatus.Equals("SUCCESS"))
                {
                    List<BeanSettings> lstRetValue = retData["data"].ToObject<List<BeanSettings>>();
                    return lstRetValue;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("ProviderUser - getCustomerAppVersion - Err:" + ex);

            }
            return null;
        }

        private class vaitro
        {
            public int ID { get; set; }
            public string Title { get; set; }
        }
    }
}

