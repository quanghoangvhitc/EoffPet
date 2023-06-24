using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Petrolimex.Bean;
using Petrolimex.Class;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.DataProvider
{
    public class ProviderControlDynamic : ProviderBase
    {
        /// <summary>
        /// lấy data cols của văn bản đến
        /// </summary>
        /// <param name="WorkflowItem"></param>
        /// <param name="LstparaCols">các cột cần lấy dữ liệu</param>
        /// <returns></returns>
        public string GetTicketRequestById_VanBanDen(BeanVanBanDen WorkflowItem, List<string> LstparaCols)
        {
            string retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();

                lstGet.Add(new KeyValuePair<string, string>("func", "getV2"));
                lstGet.Add(new KeyValuePair<string, string>("rid", WorkflowItem.ID.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("bname", "BeanVanBanDen"));
                lstGet.Add(new KeyValuePair<string, string>("actionPer", "1"));
                lstGet.Add(new KeyValuePair<string, string>("obj", "true"));

                lstPost.Add(new KeyValuePair<string, string>("cols", JsonConvert.SerializeObject(LstparaCols)));

                PAR par = new PAR(lstGet, lstPost, null);
                string CombieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) CombieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(CombieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par, true);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - GetTicketRequestById_VanBanDen - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// lấy data cols 
        /// </summary>
        /// <param name="WorkflowItem"></param>
        /// <param name="LstparaCols">các cột cần lấy dữ liệu</param>
        /// <returns></returns>
        public string GetTicketRequestByWorkflowItemId(BeanCodeItem WorkflowItem, List<string> LstparaCols)
        {
            string retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getV2"));
                lstGet.Add(new KeyValuePair<string, string>("rid", WorkflowItem.ID.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("bname", "BeanCodeItem"));
                lstGet.Add(new KeyValuePair<string, string>("type", "2"));
                lstGet.Add(new KeyValuePair<string, string>("actionPer", "1"));

                PAR par = new PAR(lstGet, lstPost, null);
                string CombieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) CombieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(CombieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiDocMan.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par, true);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - GetTicketRequestByWorkflowItemId - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy File
        /// </summary>
        /// <param name="WorkflowItem"></param>
        /// <returns></returns>
        public List<BeanAttachFile> GetAttFileByWorkflowItem(BeanCodeItem WorkflowItem)
        {
            List<BeanAttachFile> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getAttachFiles"));
                lstGet.Add(new KeyValuePair<string, string>("rid", WorkflowItem.ID.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("type", "1"));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiDocMan.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<List<BeanAttachFile>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - GetDinhKemFromRequestTicket - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy quá trình luân chuyển
        /// </summary>
        /// <param name="JsonFilter"></param>
        /// <param name="Limit"></param>
        /// <param name="Offset"></param>
        /// <returns></returns>
        public List<BeanQuaTrinhLuanChuyenWorkflow> GetTrinhLuanChuyenWorkflows(BeanCodeItem _beanCodeItem)
        {
            List<BeanQuaTrinhLuanChuyenWorkflow> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getWorkflowHistory"));
                lstGet.Add(new KeyValuePair<string, string>("rid", _beanCodeItem.ID.ToString()));

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;

                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiDocMan.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<List<BeanQuaTrinhLuanChuyenWorkflow>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - GetTrinhLuanChuyenWorkflows - Err:" + ex);
                return null;
            }
            return retValue;
        }

        public string GetTicketRequestVBBH(BeanVanBanBanHanh WorkflowItem)
        {
            string retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getV2"));
                lstGet.Add(new KeyValuePair<string, string>("rid", WorkflowItem.ID.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("bname", "BeanVanBanBanHanh"));
                lstGet.Add(new KeyValuePair<string, string>("type", "2"));
                lstGet.Add(new KeyValuePair<string, string>("obj", "true"));

                PAR par = new PAR(lstGet, null, null);
                string CombieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) CombieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(CombieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanBanHanhMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par, true);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - GetTicketRequestVBBH - Err:" + ex);
                return null;
            }
            return retValue;
        }
    }
}
