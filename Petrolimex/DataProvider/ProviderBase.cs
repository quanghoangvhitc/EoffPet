using System;
using System.Collections.Generic;
using System.Text;
using Petrolimex.Bean;
using Petrolimex.Class;
using System.IO;
using SQLite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Petrolimex.DataProvider
{
    public class ProviderBase
    {
        public class PAR
        {
            public List<KeyValuePair<string, string>> lstGet { get; set; }
            public List<KeyValuePair<string, string>> lstPost { get; set; }
            public List<KeyValuePair<string, string>> lstFile { get; set; }

            public PAR()
            {
            }

            public PAR(List<KeyValuePair<string, string>> LstGet, List<KeyValuePair<string, string>> LstPost = null, List<KeyValuePair<string, string>> LstFile = null)
            {
                this.lstGet = LstGet;
                this.lstPost = LstPost;
                this.lstFile = LstFile;
            }
        }
        //public const string m_CrypterKey = "MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAIi34TUY2hKv92Viu9mOyixtU0PXmj1mm/pz5tNf40YrJWnrP42qXKrsVYaP5CxeVDxX+0NPBurioPy3HVt1QtcCAwEAAQ==";
        public const string m_CrypterKey = "";

        public string GetDataFromAPI2(string url, ConfigVariable config)
        {
            string retValue = "";
            try
            {
                HttpWebRequest endpointRequest = (HttpWebRequest)WebRequest.CreateHttp(url);
                endpointRequest.Credentials = new NetworkCredential(config.Title, config.LoginPassword, CmmVariable.M_Domain);
                endpointRequest.Method = "GET";
                using (HttpWebResponse response = (HttpWebResponse)endpointRequest.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            using (StreamReader sr = new StreamReader(responseStream))
                            {
                                retValue = sr.ReadToEnd();
                                sr.Dispose();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("ERR ProviderBase - gGetDataFromAPI : " + ex.Message);

            }

            return retValue;
        }

        /// <summary>
        /// Lấy dữ liệu dang Text từ API Server
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpClient"></param>
        /// <param name="parSendData"></param>
        /// <returns></returns>
        public async Task<string> GetStrDataFromAPI(string url, HttpClient httpClient, PAR parSendData = null)
        {
            string retValue = null;
            try
            {
                if (!string.IsNullOrEmpty(m_CrypterKey))
                {
                    if (parSendData.lstGet == null)
                    {
                        parSendData.lstGet = new List<KeyValuePair<string, string>>();
                    }
                    KeyValuePair<string, string> encKey = new KeyValuePair<string, string>("enc", "RSA");
                    if (!parSendData.lstGet.Contains(encKey))
                    {
                        parSendData.lstGet.Add(encKey);
                    }
                }
                if (parSendData != null && parSendData.lstGet != null)
                {
                    string para = "";
                    foreach (KeyValuePair<string, string> keyItem in parSendData.lstGet)
                    {
                        if (url.Contains("?" + keyItem.Key + "=") || url.Contains("&" + keyItem.Key + "="))
                        {
                            continue;
                        }
                        if (para != "") para += "&";
                        para += keyItem.Key + "=" + WebUtility.UrlEncode(keyItem.Value);
                    }

                    if (url.IndexOf("?") > 0)
                        para = "&" + para;
                    else
                        para = "?" + para;

                    url += para;
                }

                HttpContent content = null;

                MultipartFormDataContent contentM = null;
                if (parSendData == null)
                {
                    parSendData = new PAR();
                }
                if ((parSendData.lstPost != null && parSendData.lstPost.Count > 0) || (parSendData.lstFile != null && parSendData.lstFile.Count > 0))
                {
                    contentM = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString("yyyyMMddHHmmss") + "----");

                    if (parSendData.lstPost != null && parSendData.lstPost.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> postItem in parSendData.lstPost)
                        {
                            string encodeData = postItem.Value;
                            if (!string.IsNullOrEmpty(m_CrypterKey))
                            {
                                Crypter cry = new Crypter(m_CrypterKey);
                                encodeData = cry.EncryptStr(postItem.Value);
                            }
                            HttpContent contentP = new StringContent(encodeData, Encoding.UTF8);
                            contentM.Add(contentP, postItem.Key);
                        }
                    }
                    if (parSendData.lstFile != null && parSendData.lstFile.Count > 0)
                    {
                        for (int i = 0; i < parSendData.lstFile.Count; i++)
                        {
                            KeyValuePair<string, string> fileItem = parSendData.lstFile[i];
                            var streamContentF = new StreamContent(File.OpenRead(fileItem.Value));
                            contentM.Add(streamContentF, fileItem.Key, CmmFunction.Base64Encode(Path.GetFileName(fileItem.Value)));
                        }
                    }
                }
                else
                {
                    if (parSendData.lstPost == null) parSendData.lstPost = new List<KeyValuePair<string, string>>();
                    content = new FormUrlEncodedContent(parSendData.lstPost);
                }
                Console.WriteLine("REQUEST API : " + url);
                using (HttpResponseMessage response = await httpClient.PostAsync(url, contentM == null ? content : contentM).ConfigureAwait(false))
                {
                    //Console.WriteLine("API : " + url);

                    if (response.StatusCode != HttpStatusCode.OK) return retValue;
                    using (var responseStream = new StreamReader(response.Content.ReadAsStreamAsync().Result))
                    {
                        string texthtml = responseStream.ReadToEndAsync().Result;
                        responseStream.Dispose();
                        return texthtml;
                    }
                }
                //    var res = await httpClient.PostAsync(url, contentM == null ? content : contentM).ConfigureAwait(false);

                //Console.WriteLine("API : " + url);

                //if (res.StatusCode != HttpStatusCode.OK) return retValue;
                //using (var responseStream = new StreamReader(res.Content.ReadAsStreamAsync().Result))
                //{
                //    string texthtml = responseStream.ReadToEndAsync().Result;
                //    responseStream.Dispose();
                //    return texthtml;
                //}
            }
            catch (Exception ex)
            {

                Console.WriteLine("ERR ProviderBase - GetStrDataFromAPI : " + ex.Message);

            }
            return retValue;
        }

        /// <summary>
        /// Lấy dữ liệu dạng Json Object từ API Server
        /// </summary>
        /// <param name="url">URl lấy dữ liệu</param>
        /// <param name="httpClient">Đối httpClient đã Authern (Nếu yêu cầu) dùng để lấy dữ liệu</param>
        /// <param name="parSendData">Các Tham số gửi liên server</param>
        /// <param name="flgCheckLoginRequie">Biến kiểm tra login, Nếu chưa thì tự động login</param>
        /// <returns></returns>
        public JObject GetJsonDataFromAPI(string url, ref HttpClient httpClient, PAR parSendData = null, bool flgCheckLoginRequie = true)
        {
            JObject retValue = null;
            //kiem tra lai tinh nang ket noi lai khi mat ket noi
            if (httpClient == null) return null;
            string strData = GetStrDataFromAPI(url, httpClient, parSendData).Result;

            if (strData != null && strData != "")
            {
                bool flgReloginRequie = false;
                if (strData.IndexOf("DOCTYPE html") < 0)
                {
                    retValue = JObject.Parse(strData);
                }
                if (!CmmVariable.IsLoginApp)
                {
                    if (flgCheckLoginRequie)
                    {
                        // Kiểm tra hê thống có yêu cầu login ko? nếu có thì thực hiện login
                        if (retValue != null)
                        {
                            string strStatus = retValue.Value<string>("status");
                            if (strStatus.Equals("ERR"))
                            {
                                KeyValuePair<string, string> mess = retValue["mess"].ToObject<KeyValuePair<string, string>>();
                                // Nếu hệ thống yêu cầu tự động login và số lần login lỗi nhỏ hơn Max thì tiếp thực hiện lại relogin
                                if (mess.Key.Equals("998") && CmmVariable.M_AutoReLoginNum < CmmVariable.M_AutoReLoginNumMax)
                                {
                                    flgReloginRequie = true;
                                }
                            }
                        }

                        // Thực hiện relogin và tiếp tực thực hiện lấy dữ liệu

                        if (flgReloginRequie)
                        {
                            string loginUrl = CmmVariable.M_Domain;
                            HttpClient relogin;
                            if ((relogin = CmmFunction.Login(loginUrl + CmmVariable.M_ApiLogin, CmmVariable.sysConfig.LoginName, CmmVariable.sysConfig.LoginPassword, true, 2)) != null)
                            {
                                httpClient = relogin;
                                retValue = GetJsonDataFromAPI(url, ref httpClient, parSendData);
                            }
                        }
                    }
                }
            }
            return retValue;
        }

        public async Task<JObject> GetJsonDataFromAPIAsync(string url, HttpClient httpClient, PAR parSendData = null, bool flgCheckLoginRequie = true)
        {
            JObject retValue = null;
            //kiem tra lai tinh nang ket noi lai khi mat ket noi
            if (httpClient == null) return null;
            string strData = await GetStrDataFromAPI(url, httpClient, parSendData);

            if (strData != null && strData != "")
            {
                bool flgReloginRequie = false;
                if (strData.IndexOf("DOCTYPE html") < 0)
                {
                    retValue = JObject.Parse(strData);
                }
                if (!CmmVariable.IsLoginApp)
                {
                    if (flgCheckLoginRequie)
                    {
                        // Kiểm tra hê thống có yêu cầu login ko? nếu có thì thực hiện login
                        if (retValue != null)
                        {
                            string strStatus = retValue.Value<string>("status");
                            if (strStatus.Equals("ERR"))
                            {
                                KeyValuePair<string, string> mess = retValue["mess"].ToObject<KeyValuePair<string, string>>();
                                // Nếu hệ thống yêu cầu tự động login và số lần login lỗi nhỏ hơn Max thì tiếp thực hiện lại relogin
                                if (mess.Key.Equals("998") && CmmVariable.M_AutoReLoginNum < CmmVariable.M_AutoReLoginNumMax)
                                {
                                    flgReloginRequie = true;
                                }
                            }
                        }

                        // Thực hiện relogin và tiếp tực thực hiện lấy dữ liệu

                        if (flgReloginRequie)
                        {
                            string loginUrl = CmmVariable.M_Domain;
                            HttpClient relogin;
                            if ((relogin = CmmFunction.Login(loginUrl + CmmVariable.M_ApiLogin, CmmVariable.sysConfig.LoginName, CmmVariable.sysConfig.LoginPassword, true, 2)) != null)
                            {
                                httpClient = relogin;
                                retValue = GetJsonDataFromAPI(url, httpClient, parSendData);
                            }
                        }
                    }
                }
            }
            return retValue;
        }

        /// <summary>
        /// Lấy dữ liệu dang Json Object từ API Server
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpClient"></param>
        /// <param name="lstSendData"></param>
        /// <returns></returns>
        public JObject GetJsonDataFromAPI(string url, HttpClient httpClient, PAR parSendData = null)
        {
            JObject retValue = null;
            if (httpClient == null) return null;
            string strData = GetStrDataFromAPI(url, httpClient, parSendData).Result;

            if (strData != null && strData != "" && strData.IndexOf("DOCTYPE html") < 0)
            {
                retValue = JObject.Parse(strData);
            }
            return retValue;
        }

        public List<T> GetListItem<T>(SQLiteConnection con = null, PAR parSendData = null)
        {
            List<T> retValue = null;
            try
            {
                Type type = typeof(T);
                if (con == null)
                {
                    con = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, false);
                }
                BeanBase objMst = (BeanBase)(Activator.CreateInstance(type));
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;


                JObject retData = GetJsonDataFromAPI(combieUrl + objMst.GetServerUrl(), ref CmmVariable.M_AuthenticatedHttpClient, parSendData);
                if (retData == null) return retValue;

                string strData = retData.Value<string>("data");
                if (strData != null && strData.Length > 0)
                {
                    retValue = JsonConvert.DeserializeObject<List<T>>(strData);
                }

            }
            catch (Exception ex)
            {
                retValue = null;
                System.Diagnostics.Debug.Write("ERR GetListItem: " + ex.Message);
            }

            return retValue;
        }

        public bool UpdateAllMasterData(bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgReSyns = false)
        {
            bool retValue = true;
            try
            {
                if (flgChkUpdate)
                {
                    if ((UpdateMasterData<BeanItemDeleted>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                }

                if ((UpdateMasterData<BeanSettings>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
                if ((UpdateMasterData<BeanUser>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
                if ((UpdateMasterData<BeanNotify>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
                if ((UpdateMasterData<BeanCodeItem>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
                if ((UpdateMasterData<BeanVanBanBanHanh>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
                if ((UpdateMasterData<BeanVanBanDen>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
                if ((UpdateMasterData<BeanDepartment>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;

                // Những API này chưa xử lý Modified nên không cần UpdateMasterData
                if (flgGetAll)
                {
                    if ((UpdateMasterData<BeanBanLanhDao>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanMyGroup>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanCoQuanGui>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanNguoiKyVanBan>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanLoaiVanBan>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;

                    //Load Thêm data văn thư
                    //if (CmmVariable.sysConfig != null && CmmVariable.sysConfig.IsVanThu)
                    //{
                    if ((UpdateMasterData<BeanSoVanBanDen>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanLinhVuc>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanSoVanBanDi>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanUnitGroup>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanLoaiDinhKem>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    //}
                }
            }
            catch (Exception ex)
            {
                retValue = false;
                System.Diagnostics.Debug.Write("ERR UpdateMasterData: " + ex.Message);
            }

            return retValue;
        }

        public Task UpdateAllMasterDataUnitAsync(bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgReSyns = false)
        {
            var tasks = new List<Task>();
            try
            {
                if (flgChkUpdate)
                    tasks.Add(Task.Run(() => UpdateMasterData<BeanItemDeleted>(null, flgChkUpdate, dataLimitDay, flgGetAll)));

                //tasks.Add(Task.Run(() => UpdateMasterData<BeanSettings>(null, flgChkUpdate, dataLimitDay, flgGetAll)));
                //tasks.Add(Task.Run(() => UpdateMasterData<BeanUser>(null, flgChkUpdate, dataLimitDay, flgGetAll)));
                tasks.Add(Task.Run(() => UpdateMasterData<BeanNotify>(null, flgChkUpdate, dataLimitDay, flgGetAll)));
                //tasks.Add(Task.Run(() => UpdateMasterData<BeanCodeItem>(null, flgChkUpdate, dataLimitDay, flgGetAll)));
                tasks.Add(Task.Run(() => UpdateMasterData<BeanVanBanBanHanh>(null, flgChkUpdate, dataLimitDay, flgGetAll)));
                //tasks.Add(Task.Run(() => UpdateMasterData<BeanVanBanDen>(null, flgChkUpdate, dataLimitDay, flgGetAll)));
                //tasks.Add(Task.Run(() => UpdateMasterData<BeanDepartment>(null, flgChkUpdate, dataLimitDay, flgGetAll)));

                // Những API này chưa xử lý Modified nên không cần UpdateMasterData
                if (flgGetAll)
                {
                    tasks.Add(Task.Run(() => UpdateMasterData<BeanBanLanhDao>(null, flgChkUpdate, dataLimitDay, flgGetAll)));
                    tasks.Add(Task.Run(() => UpdateMasterData<BeanMyGroup>(null, flgChkUpdate, dataLimitDay, flgGetAll)));
                    tasks.Add(Task.Run(() => UpdateMasterData<BeanCoQuanGui>(null, flgChkUpdate, dataLimitDay, flgGetAll)));
                    tasks.Add(Task.Run(() => UpdateMasterData<BeanNguoiKyVanBan>(null, flgChkUpdate, dataLimitDay, flgGetAll)));
                    tasks.Add(Task.Run(() => UpdateMasterData<BeanLoaiVanBan>(null, flgChkUpdate, dataLimitDay, flgGetAll)));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("ERR UpdateAllMasterDataUnitAsync: " + ex.Message);
            }

            return Task.WhenAll(tasks);
        }

        public bool UpdateAllMasterDataAutoReload(bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgReSyns = false)
        {
            bool retValue = true;
            try
            {
                if (flgChkUpdate)
                {
                    if ((UpdateMasterData<BeanItemDeleted>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                }

                if ((UpdateMasterData<BeanSettings>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
                if ((UpdateMasterData<BeanUser>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
                if ((UpdateMasterData<BeanCodeItem>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
                if ((UpdateMasterData<BeanVanBanBanHanh>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
                if ((UpdateMasterData<BeanVanBanDen>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
                if ((UpdateMasterData<BeanDepartment>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;

                // Những API này chưa xử lý Modified nên không cần UpdateMasterData
                if (flgGetAll)
                {
                    if ((UpdateMasterData<BeanBanLanhDao>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanMyGroup>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanCoQuanGui>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanNguoiKyVanBan>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanLoaiVanBan>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;

                    //Load Thêm data văn thư
                    //if (CmmVariable.sysConfig != null && CmmVariable.sysConfig.IsVanThu)
                    //{
                    if ((UpdateMasterData<BeanSoVanBanDen>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanLinhVuc>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanSoVanBanDi>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanUnitGroup>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanLoaiDinhKem>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    //}
                }
            }
            catch (Exception ex)
            {
                retValue = false;
                System.Diagnostics.Debug.Write("ERR UpdateMasterData: " + ex.Message);
            }

            return retValue;
        }

        public Task UpdateAllMasterDataAsync(HttpClient client, string subSite, bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgReSyns = false)
        {
            List<Task> lstTask = new List<Task>();
            try
            {
                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanSettings>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanUser>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));

                if (CmmVariable.M_Wpf_DbSite.Equals(subSite))
                    lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanNotify>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));

                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanCodeItem>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanVanBanBanHanh>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanVanBanDen>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanDepartment>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanBanLanhDao>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanMyGroup>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanCoQuanGui>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanNguoiKyVanBan>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanLoaiVanBan>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));

                //Load Thêm data văn thư
                //if (CmmVariable.sysConfig != null && CmmVariable.sysConfig.IsVanThu)
                //{
                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanSoVanBanDen>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanLinhVuc>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanSoVanBanDi>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanUnitGroup>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => UpdateMasterDataAsync<BeanLoaiDinhKem>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll)));
                //}

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("ERR UpdateMasterData: " + ex.Message);
            }

            return Task.WhenAll(lstTask);
        }

        public async Task GetAllDataAsync(HttpClient client, bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgReSyns = false)
        {
            var stime = DateTime.Now;
            List<Task> lstTask = new List<Task>();
            try
            {
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanNotify>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSettings>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanUser>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanCodeItem>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanVanBanBanHanh>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanVanBanDen>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanDepartment>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanBanLanhDao>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanMyGroup>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanCoQuanGui>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanNguoiKyVanBan>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLoaiVanBan>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSoVanBanDen>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLinhVuc>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSoVanBanDi>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanUnitGroup>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLoaiDinhKem>(client, flgChkUpdate, dataLimitDay, flgGetAll)));


                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanNotify>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSettings>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanUser>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanCodeItem>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanVanBanBanHanh>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanVanBanDen>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanDepartment>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanBanLanhDao>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanMyGroup>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanCoQuanGui>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanNguoiKyVanBan>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLoaiVanBan>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSoVanBanDen>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLinhVuc>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSoVanBanDi>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanUnitGroup>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLoaiDinhKem>(client, flgChkUpdate, dataLimitDay, flgGetAll)));

                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanNotify>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSettings>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanUser>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanCodeItem>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanVanBanBanHanh>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanVanBanDen>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanDepartment>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanBanLanhDao>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanMyGroup>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanCoQuanGui>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanNguoiKyVanBan>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLoaiVanBan>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSoVanBanDen>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLinhVuc>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSoVanBanDi>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanUnitGroup>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLoaiDinhKem>(client, flgChkUpdate, dataLimitDay, flgGetAll)));

                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanNotify>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSettings>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanUser>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanCodeItem>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanVanBanBanHanh>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanVanBanDen>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanDepartment>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanBanLanhDao>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanMyGroup>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanCoQuanGui>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanNguoiKyVanBan>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLoaiVanBan>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSoVanBanDen>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLinhVuc>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSoVanBanDi>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanUnitGroup>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLoaiDinhKem>(client, flgChkUpdate, dataLimitDay, flgGetAll)));

                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanNotify>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSettings>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanUser>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanCodeItem>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanVanBanBanHanh>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanVanBanDen>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanDepartment>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanBanLanhDao>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanMyGroup>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanCoQuanGui>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanNguoiKyVanBan>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLoaiVanBan>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSoVanBanDen>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLinhVuc>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSoVanBanDi>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanUnitGroup>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLoaiDinhKem>(client, flgChkUpdate, dataLimitDay, flgGetAll)));

                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanNotify>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSettings>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanUser>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanCodeItem>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanVanBanBanHanh>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanVanBanDen>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanDepartment>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanBanLanhDao>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanMyGroup>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanCoQuanGui>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanNguoiKyVanBan>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLoaiVanBan>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSoVanBanDen>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLinhVuc>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanSoVanBanDi>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanUnitGroup>(client, flgChkUpdate, dataLimitDay, flgGetAll)));
                lstTask.Add(Task.Run(() => GetMasterDataAsync<BeanLoaiDinhKem>(client, flgChkUpdate, dataLimitDay, flgGetAll)));

            }
            catch (Exception ex)
            {

            }

            await Task.WhenAll(lstTask);

            Console.WriteLine($"Đã xong : {(DateTime.Now - stime).TotalMilliseconds}");
        }

        public Task GetAllMasterDataAsync(HttpClient client, string subSite, bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgReSyns = false)
        {
            List<Task> lstTask = new List<Task>();
            try
            {
                lstTask.Add(Task.Run(() =>
                {
                    if (CmmVariable.M_Wpf_DbSite.Equals(subSite))
                        UpdateMasterDataAsync<BeanNotify>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                    UpdateMasterDataAsync<BeanSettings>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                    UpdateMasterDataAsync<BeanUser>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                    UpdateMasterDataAsync<BeanCodeItem>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                    UpdateMasterDataAsync<BeanVanBanBanHanh>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                }));

                lstTask.Add(Task.Run(() =>
                {
                    UpdateMasterDataAsync<BeanVanBanDen>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                    UpdateMasterDataAsync<BeanDepartment>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                    UpdateMasterDataAsync<BeanBanLanhDao>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                    UpdateMasterDataAsync<BeanMyGroup>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                }));

                lstTask.Add(Task.Run(() =>
                {
                    UpdateMasterDataAsync<BeanCoQuanGui>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                    UpdateMasterDataAsync<BeanNguoiKyVanBan>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                    UpdateMasterDataAsync<BeanLoaiVanBan>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                    UpdateMasterDataAsync<BeanSoVanBanDen>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);

                }));

                lstTask.Add(Task.Run(() =>
                {
                    UpdateMasterDataAsync<BeanLinhVuc>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                    UpdateMasterDataAsync<BeanSoVanBanDi>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                    UpdateMasterDataAsync<BeanUnitGroup>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                    UpdateMasterDataAsync<BeanLoaiDinhKem>(client, subSite, flgChkUpdate, dataLimitDay, flgGetAll);
                }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("ERR UpdateMasterData: " + ex.Message);
            }

            return Task.WhenAll(lstTask);
        }

        public async Task<bool> GetMasterDataAsync<T>(HttpClient client, bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgDeleteOldData = false, PAR extPara = null)
        {
            Type type = typeof(T);
            string ID = type.Name;

            string Modified = "";

            try
            {
                if (flgGetAll)
                {
                    Modified = "";
                }

                BeanBase objMst = (BeanBase)(Activator.CreateInstance(type));

                string combieUrl = CmmVariable.M_Domain;

                if (extPara == null)
                    extPara = new PAR();

                List<KeyValuePair<string, string>> lstGet = extPara.lstGet;

                if (lstGet == null)
                    lstGet = new List<KeyValuePair<string, string>>();

                lstGet.Add(new KeyValuePair<string, string>("Modified", Modified));

                if (type.Name.Equals("BeanNotify")) // Luôn luôn lấy tất cả
                    lstGet.Add(new KeyValuePair<string, string>("status", "-1"));
                else
                    lstGet.Add(new KeyValuePair<string, string>("isFirst", flgGetAll ? "1" : "0"));

                extPara.lstGet = lstGet;
                string ApiServerUrl = objMst.GetServerUrl();

                ApiServerUrl = ApiServerUrl.Replace("<#SiteName#>","");

                //HttpClient client = CmmFunction.CreateHttpClient();
                //JObject retData = GetJsonDataFromAPI(combieUrl + ApiServerUrl, ref CmmVariable.M_AuthenticatedHttpClient, extPara);
                var sTime = DateTime.Now;
                Console.WriteLine($"Start API : - {type.Name} : {sTime.ToString("dd-MM-yyyy HH:mm:ss.fff")}");
                //Console.WriteLine($"Site : {(string.IsNullOrEmpty(subSite) ? "TĐ" : subSite)} - { type.Name} request {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.ffff")}");
                JObject retData = await GetJsonDataFromAPIAsync(combieUrl + ApiServerUrl, client, extPara);
                //Console.WriteLine($"Site : {(string.IsNullOrEmpty(subSite) ? "TĐ" : subSite)} - { type.Name} response {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.ffff")}");
                Console.WriteLine($"End API : - {type.Name} : {(DateTime.Now - sTime).TotalMilliseconds}");
                if (retData == null) return false;
                string strStatus = retData.Value<string>("status");
                if (strStatus == null || !strStatus.Equals("SUCCESS"))
                {
                    string err = retData.Value<JObject>("mess")["Key"].ToString();
                    if (err == "010")
                        return true;
                    else
                        return false;
                }


                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error : {type.Name} : {ex.Message} --- {ex.StackTrace}");
            }

            return false;
        }

        public bool UpdateMasterDataAsync<T>(HttpClient client, string subSite, bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgDeleteOldData = false, PAR extPara = null)
        {
            Type type = typeof(T);
            string ID = type.Name;
            //Console.WriteLine(type.Name + ":" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
            string Modified = "";
            SQLiteConnection con = new SQLiteConnection(System.IO.Path.Combine(CmmVariable.M_Wpf_DatabaseDir, "DB_sqlite_XamDocument" + (!string.IsNullOrEmpty(subSite) ? "_" + subSite : "") + ".db"), false);
            SQLite3.BusyTimeout(con.Handle, 30000);
            try
            {
                if (flgGetAll)
                {
                    Modified = "";
                }
                else
                {
                    TableQuery<DBVariable> table = con.Table<DBVariable>();
                    var items = from i in table
                                where i.Id == ID
                                select i;
                    if (items.Count() > 0)
                        Modified = items.First().Value;
                    else
                        Modified = DateTime.Now.AddDays(dataLimitDay * -1).ToString("yyyy-MM-dd HH:mm:ss");
                }

                string strCheckExists = string.Format("SELECT '' AS VALUE FROM sqlite_master WHERE type='table' AND name = '{0}'", type.Name);
                List<BeanSettings> lstCheckExists = con.Query<BeanSettings>(strCheckExists);

                if (lstCheckExists == null || lstCheckExists.Count == 0)
                {
                    CmmFunction.InstanceDb(CmmVariable.M_Wpf_SQLitePath, type);
                    Modified = "";
                }

                BeanBase objMst = (BeanBase)(Activator.CreateInstance(type));

                string combieUrl = CmmVariable.M_Domain;

                if (extPara == null)
                    extPara = new PAR();

                List<KeyValuePair<string, string>> lstGet = extPara.lstGet;

                if (lstGet == null)
                    lstGet = new List<KeyValuePair<string, string>>();

                lstGet.Add(new KeyValuePair<string, string>("Modified", Modified));

                if (type.Name.Equals("BeanNotify")) // Luôn luôn lấy tất cả
                    lstGet.Add(new KeyValuePair<string, string>("status", "-1"));
                else
                    lstGet.Add(new KeyValuePair<string, string>("isFirst", flgGetAll ? "1" : "0"));

                extPara.lstGet = lstGet;
                string ApiServerUrl = objMst.GetServerUrl();

                ApiServerUrl = ApiServerUrl.Replace("<#SiteName#>", string.IsNullOrEmpty(subSite) ? "" : ("/" + subSite));

                //HttpClient client = CmmFunction.CreateHttpClient();
                //JObject retData = GetJsonDataFromAPI(combieUrl + ApiServerUrl, ref CmmVariable.M_AuthenticatedHttpClient, extPara);
                var sTime = DateTime.Now;
                //Console.WriteLine($"Site : {(string.IsNullOrEmpty(subSite) ? "TĐ" : subSite)} - { type.Name} request {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.ffff")}");
                JObject retData = GetJsonDataFromAPI(combieUrl + ApiServerUrl, ref client, extPara);
                //Console.WriteLine($"Site : {(string.IsNullOrEmpty(subSite) ? "TĐ" : subSite)} - { type.Name} response {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.ffff")}");
                Console.WriteLine($"End API Site : {(string.IsNullOrEmpty(subSite) ? "TĐ" : subSite)} - {type.Name} : {(DateTime.Now - sTime).TotalMilliseconds}");
                if (retData == null) return false;
                string strStatus = retData.Value<string>("status");
                if (strStatus == null || !strStatus.Equals("SUCCESS"))
                {
                    string err = retData.Value<JObject>("mess")["Key"].ToString();
                    if (err == "010")
                        return true;
                    else
                        return false;
                }

                List<T> lstMstData = retData["data"].ToObject<List<T>>();
                String LocalKeyName = BeanBase.getPriKey(typeof(T))[0];
                String serverKeyName = BeanBase.getPriKeyS(typeof(T))[0];

                if (lstMstData == null || lstMstData.Count == 0)
                    return true;

                List<T> lstInsertItem = new List<T>();
                List<T> lstUpdateItem = new List<T>();


                List<string> lstHtmlDecode = BeanBase.getLstProName(type, typeof(HtmlEncodeAttribute));
                List<string> lstHtmlRemove = BeanBase.getLstProName(type, typeof(HtmlRemoveAttribute));
                List<string> lstRemoveAccentFrom = BeanBase.getLstProName(type, typeof(RemoveAccentFromAttribute));
                List<string> lstRemoveAccentFromMultiColumn = BeanBase.getLstProName(type, typeof(RemoveAccentFromMultiColumnAttribute));

                // Lấy danh sách các thuộc tính Bean kiểu DateTime
                PropertyInfo[] arrProDateTime = CmmFunction.GetPropertysWithType(type, typeof(DateTime?));

                if (flgChkUpdate)
                {
                    foreach (T item in lstMstData)
                    {
                        string sqlSel;

                        if (type.Name == "BeanItemDeleted")
                        {
                            BeanItemDeleted itemDelete = CmmFunction.ChangeToRealType<BeanItemDeleted>(item);

                            if (!string.IsNullOrEmpty(itemDelete.BeanName))
                            {
                                try
                                {
                                    sqlSel = string.Format("DELETE FROM {0} WHERE {1} = ?", itemDelete.BeanName, itemDelete.Key);
                                    var res = -1;
                                    if (itemDelete.BeanName.ToLower().Equals("beannotify"))
                                    {
                                        using (SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_Wpf_SqlPathNotify, false))
                                        {
                                            res = conn.Execute(sqlSel, itemDelete.Value.ToLower());
                                        }
                                    }
                                    else if (itemDelete.BeanName.ToLower().Equals("beangroup"))
                                    {
                                        using (SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_Wpf_SqlPathNotify, false))
                                        {
                                            sqlSel = string.Format("DELETE FROM {0} WHERE {1} = ?", "BeanMyGroup", itemDelete.Key);
                                            res = conn.Execute(sqlSel, itemDelete.Value.ToLower());
                                        }
                                    }
                                    else
                                        res = con.Execute(sqlSel, itemDelete.Value.ToLower());
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }
                            }
                            continue;
                        }

                        // Decode Dữ liệu bị encode html
                        foreach (string decodeFieldName in lstHtmlDecode)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, decodeFieldName);

                            object decodeValue = CmmFunction.GetPropertyValue(item, propInfo);
                            if (decodeValue != null)
                            {
                                decodeValue = WebUtility.HtmlDecode(decodeValue + string.Empty);
                                CmmFunction.SetPropertyValue(item, propInfo, decodeValue);
                            }
                        }

                        // Remove Dữ liệu có html tag
                        foreach (string decodeFieldName in lstHtmlRemove)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, decodeFieldName);

                            object decodeValue = CmmFunction.GetPropertyValue(item, propInfo);
                            if (decodeValue != null)
                            {
                                decodeValue = Regex.Replace(decodeValue + "", "<.*?>", String.Empty);
                                CmmFunction.SetPropertyValue(item, propInfo, decodeValue);
                            }
                        }

                        // Remove accent 1 column
                        foreach (string removeAccentFieldName in lstRemoveAccentFrom)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, removeAccentFieldName);

                            RemoveAccentFromAttribute cmmAttr = (RemoveAccentFromAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                object removeAccentValue = CmmFunction.GetPropertyValueByName(item, cmmAttr.ColFrom);
                                if (removeAccentValue != null)
                                {
                                    if (removeAccentValue.ToString().Contains(";#"))
                                    {
                                        //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                        removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                    }

                                    removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "").ToLowerInvariant();
                                    CmmFunction.SetPropertyValue(item, propInfo, removeAccentValue);
                                }
                            }
                        }

                        // Remove dấu lấy từ nhiều thuộc tính khác
                        string temp_khongdau = "";
                        foreach (string removeAccentFieldName in lstRemoveAccentFromMultiColumn)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, removeAccentFieldName);

                            RemoveAccentFromMultiColumnAttribute cmmAttr = (RemoveAccentFromMultiColumnAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromMultiColumnAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                if (cmmAttr.ColFrom.Count() > 0)
                                {
                                    foreach (var res in cmmAttr.ColFrom)
                                    {
                                        object removeAccentValue = CmmFunction.GetPropertyValueByName(item, res);
                                        if (removeAccentValue != null)
                                        {
                                            if (removeAccentValue.ToString().Contains(";#"))
                                            {
                                                //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                                removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                            }

                                            removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "");
                                            temp_khongdau += removeAccentValue + "^";
                                        }
                                    }
                                    CmmFunction.SetPropertyValue(item, propInfo, temp_khongdau.ToLowerInvariant());
                                }
                            }
                        }

                        //Kiểm tra Convert Datetime to UTC
                        if (arrProDateTime != null)
                        {
                            foreach (PropertyInfo pro in arrProDateTime)
                            {
                                object objValue = CmmFunction.GetPropertyValue(item, pro);
                                if (objValue != null)
                                {
                                    DateTime objDateTime = ((DateTime)objValue).AddHours(CmmVariable.M_DiffHours);
                                    CmmFunction.SetPropertyValueByName(item, pro.Name, objDateTime);
                                }
                            }
                        }

                        // Kiểm tra Table nếu tồn tại rồi thì Update
                        sqlSel = string.Format("SELECT {1} FROM {0} WHERE {1} = ?", type.Name, serverKeyName);
                        List<object> lstObjChk = con.Query(new TableMapping(type), sqlSel, CmmFunction.GetPropertyValueByName(item, serverKeyName));

                        if (lstObjChk.Count > 0)
                        {
                            T objChk = (T)lstObjChk[0];
                            CmmFunction.SetPropertyValueByName(item, LocalKeyName, CmmFunction.GetPropertyValueByName(objChk, LocalKeyName));

                            lstUpdateItem.Add(item);
                        }
                        else // Nếu không tồn tại thì Insert
                            lstInsertItem.Add(item);
                    }
                }
                else // Insert
                {
                    foreach (T item in lstMstData)
                    {
                        // Decode Dữ liệu bị encode html
                        foreach (string decodeFieldName in lstHtmlDecode)
                        {
                            object decodeValue = CmmFunction.GetPropertyValueByName(item, decodeFieldName);
                            if (decodeValue != null)
                            {
                                decodeValue = WebUtility.HtmlDecode(decodeValue + string.Empty);
                                CmmFunction.SetPropertyValueByName(item, decodeFieldName, decodeValue);
                            }
                        }

                        // Remove Dữ liệu có html tag
                        foreach (string decodeFieldName in lstHtmlRemove)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, decodeFieldName);

                            object decodeValue = CmmFunction.GetPropertyValue(item, propInfo);
                            if (decodeValue != null)
                            {
                                decodeValue = Regex.Replace(decodeValue + "", "<.*?>", String.Empty);
                                CmmFunction.SetPropertyValue(item, propInfo, decodeValue);
                            }
                        }

                        // Remove accent 1 column
                        foreach (string removeAccentFieldName in lstRemoveAccentFrom)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, removeAccentFieldName);

                            RemoveAccentFromAttribute cmmAttr = (RemoveAccentFromAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                object removeAccentValue = CmmFunction.GetPropertyValueByName(item, cmmAttr.ColFrom);
                                if (removeAccentValue != null)
                                {
                                    if (removeAccentValue.ToString().Contains(";#"))
                                    {
                                        //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                        removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                    }

                                    removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "").ToLowerInvariant();
                                    CmmFunction.SetPropertyValue(item, propInfo, removeAccentValue);
                                }
                            }
                        }

                        // Remove dấu lấy từ nhieu thuộc tính khác
                        string temp_khongdau = "";
                        foreach (string removeAccentFieldName in lstRemoveAccentFromMultiColumn)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, removeAccentFieldName);

                            RemoveAccentFromMultiColumnAttribute cmmAttr = (RemoveAccentFromMultiColumnAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromMultiColumnAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                if (cmmAttr.ColFrom.Count() > 0)
                                {
                                    foreach (var res in cmmAttr.ColFrom)
                                    {
                                        object removeAccentValue = CmmFunction.GetPropertyValueByName(item, res);
                                        if (removeAccentValue != null)
                                        {
                                            if (removeAccentValue.ToString().Contains(";#"))
                                            {
                                                //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                                removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                            }

                                            removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "");
                                            temp_khongdau += removeAccentValue + "^";
                                        }
                                    }
                                    CmmFunction.SetPropertyValue(item, propInfo, temp_khongdau.ToLowerInvariant());
                                }
                            }
                        }

                        //kiem tra convert datetime to UTC
                        if (arrProDateTime != null)
                        {
                            foreach (PropertyInfo pro in arrProDateTime)
                            {
                                object objValue = CmmFunction.GetPropertyValue(item, pro);

                                if (objValue != null)
                                {
                                    DateTime objDateTime = ((DateTime)objValue).AddHours(CmmVariable.M_DiffHours);
                                    CmmFunction.SetPropertyValueByName(item, pro.Name, objDateTime);
                                }
                            }
                        }
                    }
                    lstInsertItem = lstMstData;
                }

                string sysDateNow = retData.Value<string>("dateNow");
                if (string.IsNullOrEmpty(sysDateNow)) return false;

                con.BeginTransaction();

                if (lstInsertItem.Count > 0)
                    con.InsertAll(lstInsertItem);

                if (lstUpdateItem.Count > 0)
                    con.UpdateAll(lstUpdateItem);

                CmmFunction.UpdateDBVariable(new DBVariable(ID, sysDateNow), con);
                con.Commit();
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                con.Rollback();
                con.Commit();
                con.Close();

                Console.WriteLine("ERR - UpdateMasterData - " + type.Name + " - " + ex.Message + ex.StackTrace);
                System.Diagnostics.Debug.Write("ERR UpdateMasterData: " + ex.Message + ex.StackTrace);

            }
            finally
            {
                if (con != null)
                    con.Close();
            }
            return false;
        }

        /// <summary>
        /// Upload Dữ liệu 1 bảng Master từ server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="flgChkUpdate"></param>
        public bool UpdateMasterData<T>(SQLiteConnection con = null, bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgDeleteOldData = false, PAR extPara = null)
        {
            Type type = typeof(T);
            string ID = type.Name;
            string Modified = "";
            try
            {
                if (con == null)
                    con = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, false);

                if (type == typeof(BeanNotify) && !CmmVariable.M_Wpf_DbSite.Equals(CmmVariable.sysConfig.Subsite))
                    con = new SQLiteConnection(CmmVariable.M_Wpf_SqlPathNotify, false);

                if (flgGetAll)
                {
                    Modified = "";
                }
                else
                {
                    TableQuery<DBVariable> table = con.Table<DBVariable>();
                    var items = from i in table
                                where i.Id == ID
                                select i;
                    if (items.Count() > 0)
                        Modified = items.First().Value;
                    else
                        Modified = DateTime.Now.AddDays(dataLimitDay * -1).ToString("yyyy-MM-dd HH:mm:ss");
                }

                string strCheckExists = string.Format("SELECT '' AS VALUE FROM sqlite_master WHERE type='table' AND name = '{0}'", type.Name);
                List<BeanSettings> lstCheckExists = con.Query<BeanSettings>(strCheckExists);

                if (lstCheckExists == null || lstCheckExists.Count == 0)
                {
                    CmmFunction.InstanceDb(CmmVariable.M_Wpf_SQLitePath, type);
                    Modified = "";
                }

                BeanBase objMst = (BeanBase)(Activator.CreateInstance(type));

                string combieUrl = CmmVariable.M_Domain;

                if (extPara == null)
                    extPara = new PAR();

                List<KeyValuePair<string, string>> lstGet = extPara.lstGet;

                if (lstGet == null)
                    lstGet = new List<KeyValuePair<string, string>>();

                lstGet.Add(new KeyValuePair<string, string>("Modified", Modified));

                if (type.Name.Equals("BeanNotify")) // Luôn luôn lấy tất cả
                    lstGet.Add(new KeyValuePair<string, string>("status", "-1"));
                else
                    lstGet.Add(new KeyValuePair<string, string>("isFirst", flgGetAll ? "1" : "0"));

                extPara.lstGet = lstGet;
                string ApiServerUrl = objMst.GetServerUrl();

                if (CmmVariable.sysConfig.Subsite.Contains(";"))
                    CmmVariable.sysConfig.Subsite = CmmVariable.sysConfig.Subsite.Replace(";", " ").TrimStart();

                ApiServerUrl = ApiServerUrl.Replace("<#SiteName#>", string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite) ? "" : ("/" + CmmVariable.sysConfig.Subsite));

                JObject retData = GetJsonDataFromAPI(combieUrl + ApiServerUrl, ref CmmVariable.M_AuthenticatedHttpClient, extPara);
                if (retData == null) return false;
                string strStatus = retData.Value<string>("status");
                if (strStatus == null || !strStatus.Equals("SUCCESS"))
                {
                    string err = retData.Value<JObject>("mess")["Key"].ToString();
                    if (err == "010")
                        return true;
                    else
                        return false;
                }

                List<T> lstMstData = retData["data"].ToObject<List<T>>();
                String LocalKeyName = BeanBase.getPriKey(typeof(T))[0];
                String serverKeyName = BeanBase.getPriKeyS(typeof(T))[0];

                if (lstMstData == null || lstMstData.Count == 0)
                    return true;

                List<T> lstInsertItem = new List<T>();
                List<T> lstUpdateItem = new List<T>();


                List<string> lstHtmlDecode = BeanBase.getLstProName(type, typeof(HtmlEncodeAttribute));
                List<string> lstHtmlRemove = BeanBase.getLstProName(type, typeof(HtmlRemoveAttribute));
                List<string> lstRemoveAccentFrom = BeanBase.getLstProName(type, typeof(RemoveAccentFromAttribute));
                List<string> lstRemoveAccentFromMultiColumn = BeanBase.getLstProName(type, typeof(RemoveAccentFromMultiColumnAttribute));

                // Lấy danh sách các thuộc tính Bean kiểu DateTime
                PropertyInfo[] arrProDateTime = CmmFunction.GetPropertysWithType(type, typeof(DateTime?));

                if (flgChkUpdate)
                {
                    foreach (T item in lstMstData)
                    {
                        string sqlSel;

                        if (type.Name == "BeanItemDeleted")
                        {
                            BeanItemDeleted itemDelete = CmmFunction.ChangeToRealType<BeanItemDeleted>(item);

                            if (!string.IsNullOrEmpty(itemDelete.BeanName))
                            {
                                try
                                {
                                    sqlSel = string.Format("DELETE FROM {0} WHERE {1} = ?", itemDelete.BeanName, itemDelete.Key);
                                    var res = -1;
                                    if (itemDelete.BeanName.ToLower().Equals("beannotify"))
                                    {
                                        using (SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_Wpf_SqlPathNotify, false))
                                        {
                                            res = conn.Execute(sqlSel, itemDelete.Value.ToLower());
                                        }
                                    }
                                    else if (itemDelete.BeanName.ToLower().Equals("beangroup"))
                                    {
                                        using (SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_Wpf_SqlPathNotify, false))
                                        {
                                            sqlSel = string.Format("DELETE FROM {0} WHERE {1} = ?", "BeanMyGroup", itemDelete.Key);
                                            res = conn.Execute(sqlSel, itemDelete.Value.ToLower());
                                        }
                                    }
                                    else
                                        res = con.Execute(sqlSel, itemDelete.Value.ToLower());
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }
                            }
                            continue;
                        }

                        // Decode Dữ liệu bị encode html
                        foreach (string decodeFieldName in lstHtmlDecode)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, decodeFieldName);

                            object decodeValue = CmmFunction.GetPropertyValue(item, propInfo);
                            if (decodeValue != null)
                            {
                                decodeValue = WebUtility.HtmlDecode(decodeValue + string.Empty);
                                CmmFunction.SetPropertyValue(item, propInfo, decodeValue);
                            }
                        }

                        // Remove Dữ liệu có html tag
                        foreach (string decodeFieldName in lstHtmlRemove)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, decodeFieldName);

                            object decodeValue = CmmFunction.GetPropertyValue(item, propInfo);
                            if (decodeValue != null)
                            {
                                decodeValue = Regex.Replace(decodeValue + "", "<.*?>", String.Empty);
                                CmmFunction.SetPropertyValue(item, propInfo, decodeValue);
                            }
                        }

                        // Remove accent 1 column
                        foreach (string removeAccentFieldName in lstRemoveAccentFrom)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, removeAccentFieldName);

                            RemoveAccentFromAttribute cmmAttr = (RemoveAccentFromAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                object removeAccentValue = CmmFunction.GetPropertyValueByName(item, cmmAttr.ColFrom);
                                if (removeAccentValue != null)
                                {
                                    if (removeAccentValue.ToString().Contains(";#"))
                                    {
                                        //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                        removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                    }

                                    removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "").ToLowerInvariant();
                                    CmmFunction.SetPropertyValue(item, propInfo, removeAccentValue);
                                }
                            }
                        }

                        // Remove dấu lấy từ nhiều thuộc tính khác
                        string temp_khongdau = "";
                        foreach (string removeAccentFieldName in lstRemoveAccentFromMultiColumn)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, removeAccentFieldName);

                            RemoveAccentFromMultiColumnAttribute cmmAttr = (RemoveAccentFromMultiColumnAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromMultiColumnAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                if (cmmAttr.ColFrom.Count() > 0)
                                {
                                    foreach (var res in cmmAttr.ColFrom)
                                    {
                                        object removeAccentValue = CmmFunction.GetPropertyValueByName(item, res);
                                        if (removeAccentValue != null)
                                        {
                                            if (removeAccentValue.ToString().Contains(";#"))
                                            {
                                                //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                                removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                            }

                                            removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "");
                                            temp_khongdau += removeAccentValue + "^";
                                        }
                                    }
                                    CmmFunction.SetPropertyValue(item, propInfo, temp_khongdau.ToLowerInvariant());
                                }
                            }
                        }

                        //Kiểm tra Convert Datetime to UTC
                        if (arrProDateTime != null)
                        {
                            foreach (PropertyInfo pro in arrProDateTime)
                            {
                                object objValue = CmmFunction.GetPropertyValue(item, pro);
                                if (objValue != null)
                                {
                                    DateTime objDateTime = ((DateTime)objValue).AddHours(CmmVariable.M_DiffHours);
                                    CmmFunction.SetPropertyValueByName(item, pro.Name, objDateTime);
                                }
                            }
                        }

                        // Kiểm tra Table nếu tồn tại rồi thì Update
                        sqlSel = string.Format("SELECT {1} FROM {0} WHERE {1} = ?", type.Name, serverKeyName);
                        List<object> lstObjChk = con.Query(new TableMapping(type), sqlSel, CmmFunction.GetPropertyValueByName(item, serverKeyName));

                        if (lstObjChk.Count > 0)
                        {
                            T objChk = (T)lstObjChk[0];
                            CmmFunction.SetPropertyValueByName(item, LocalKeyName, CmmFunction.GetPropertyValueByName(objChk, LocalKeyName));

                            lstUpdateItem.Add(item);
                        }
                        else // Nếu không tồn tại thì Insert
                            lstInsertItem.Add(item);
                    }
                }
                else // Insert
                {
                    foreach (T item in lstMstData)
                    {
                        // Decode Dữ liệu bị encode html
                        foreach (string decodeFieldName in lstHtmlDecode)
                        {
                            object decodeValue = CmmFunction.GetPropertyValueByName(item, decodeFieldName);
                            if (decodeValue != null)
                            {
                                decodeValue = WebUtility.HtmlDecode(decodeValue + string.Empty);
                                CmmFunction.SetPropertyValueByName(item, decodeFieldName, decodeValue);
                            }
                        }

                        // Remove Dữ liệu có html tag
                        foreach (string decodeFieldName in lstHtmlRemove)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, decodeFieldName);

                            object decodeValue = CmmFunction.GetPropertyValue(item, propInfo);
                            if (decodeValue != null)
                            {
                                decodeValue = Regex.Replace(decodeValue + "", "<.*?>", String.Empty);
                                CmmFunction.SetPropertyValue(item, propInfo, decodeValue);
                            }
                        }

                        // Remove accent 1 column
                        foreach (string removeAccentFieldName in lstRemoveAccentFrom)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, removeAccentFieldName);

                            RemoveAccentFromAttribute cmmAttr = (RemoveAccentFromAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                object removeAccentValue = CmmFunction.GetPropertyValueByName(item, cmmAttr.ColFrom);
                                if (removeAccentValue != null)
                                {
                                    if (removeAccentValue.ToString().Contains(";#"))
                                    {
                                        //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                        removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                    }

                                    removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "").ToLowerInvariant();
                                    CmmFunction.SetPropertyValue(item, propInfo, removeAccentValue);
                                }
                            }
                        }

                        // Remove dấu lấy từ nhieu thuộc tính khác
                        string temp_khongdau = "";
                        foreach (string removeAccentFieldName in lstRemoveAccentFromMultiColumn)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, removeAccentFieldName);

                            RemoveAccentFromMultiColumnAttribute cmmAttr = (RemoveAccentFromMultiColumnAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromMultiColumnAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                if (cmmAttr.ColFrom.Count() > 0)
                                {
                                    foreach (var res in cmmAttr.ColFrom)
                                    {
                                        object removeAccentValue = CmmFunction.GetPropertyValueByName(item, res);
                                        if (removeAccentValue != null)
                                        {
                                            if (removeAccentValue.ToString().Contains(";#"))
                                            {
                                                //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                                removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                            }

                                            removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "");
                                            temp_khongdau += removeAccentValue + "^";
                                        }
                                    }
                                    CmmFunction.SetPropertyValue(item, propInfo, temp_khongdau.ToLowerInvariant());
                                }
                            }
                        }

                        //kiem tra convert datetime to UTC
                        if (arrProDateTime != null)
                        {
                            foreach (PropertyInfo pro in arrProDateTime)
                            {
                                object objValue = CmmFunction.GetPropertyValue(item, pro);

                                if (objValue != null)
                                {
                                    DateTime objDateTime = ((DateTime)objValue).AddHours(CmmVariable.M_DiffHours);
                                    CmmFunction.SetPropertyValueByName(item, pro.Name, objDateTime);
                                }
                            }
                        }
                    }
                    lstInsertItem = lstMstData;
                }

                string sysDateNow = retData.Value<string>("dateNow");
                if (string.IsNullOrEmpty(sysDateNow)) return false;

                con.BeginTransaction();

                if (type.Name.Equals("BeanNotify"))
                {

                }
                if (lstInsertItem.Count > 0)
                    con.InsertAll(lstInsertItem);

                if (lstUpdateItem.Count > 0)
                    con.UpdateAll(lstUpdateItem);

                CmmFunction.UpdateDBVariable(new DBVariable(ID, sysDateNow), con);
                con.Commit();
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                con.Rollback();
                con.Commit();
                con.Close();

                Console.WriteLine("ERR - UpdateMasterData - " + type.Name + " - " + ex.Message + ex.StackTrace);
                System.Diagnostics.Debug.Write("ERR UpdateMasterData: " + ex.Message + ex.StackTrace);

            }
            finally
            {
                if (con != null)
                    con.Close();
            }
            return false;
        }

        public BeanResult<T> UpdateMasterReturnData<T>(SQLiteConnection con = null, bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgDeleteOldData = false, PAR extPara = null)
        {
            Type type = typeof(T);
            string ID = type.Name;
            string Modified = "";
            try
            {
                if (con == null)
                    con = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, false);

                if (type == typeof(BeanNotify) && !CmmVariable.M_Wpf_DbSite.Equals(CmmVariable.sysConfig.Subsite))
                    con = new SQLiteConnection(CmmVariable.M_Wpf_SqlPathNotify, false);

                if (flgGetAll)
                {
                    Modified = "";
                }
                else
                {
                    TableQuery<DBVariable> table = con.Table<DBVariable>();
                    var items = from i in table
                                where i.Id == ID
                                select i;
                    if (items.Count() > 0)
                        Modified = items.First().Value;
                    else
                        Modified = DateTime.Now.AddDays(dataLimitDay * -1).ToString("yyyy-MM-dd HH:mm:ss");
                }

                string strCheckExists = string.Format("SELECT '' AS VALUE FROM sqlite_master WHERE type='table' AND name = '{0}'", type.Name);
                List<BeanSettings> lstCheckExists = con.Query<BeanSettings>(strCheckExists);

                if (lstCheckExists == null || lstCheckExists.Count == 0)
                {
                    CmmFunction.InstanceDb(CmmVariable.M_Wpf_SQLitePath, type);
                    Modified = "";
                }

                BeanBase objMst = (BeanBase)(Activator.CreateInstance(type));

                string combieUrl = CmmVariable.M_Domain;

                if (extPara == null)
                    extPara = new PAR();

                List<KeyValuePair<string, string>> lstGet = extPara.lstGet;

                if (lstGet == null)
                    lstGet = new List<KeyValuePair<string, string>>();

                lstGet.Add(new KeyValuePair<string, string>("Modified", Modified));

                if (type.Name.Equals("BeanNotify")) // Luôn luôn lấy tất cả
                    lstGet.Add(new KeyValuePair<string, string>("status", "-1"));
                else
                    lstGet.Add(new KeyValuePair<string, string>("isFirst", flgGetAll ? "1" : "0"));

                extPara.lstGet = lstGet;
                string ApiServerUrl = objMst.GetServerUrl();

                if (CmmVariable.sysConfig.Subsite.Contains(";"))
                    CmmVariable.sysConfig.Subsite = CmmVariable.sysConfig.Subsite.Replace(";", " ").TrimStart();

                ApiServerUrl = ApiServerUrl.Replace("<#SiteName#>", string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite) ? "" : ("/" + CmmVariable.sysConfig.Subsite));

                JObject retData = GetJsonDataFromAPI(combieUrl + ApiServerUrl, ref CmmVariable.M_AuthenticatedHttpClient, extPara);
                if (retData == null) return new BeanResult<T>(false);
                string strStatus = retData.Value<string>("status");
                if (strStatus == null || !strStatus.Equals("SUCCESS"))
                {
                    string err = retData.Value<JObject>("mess")["Key"].ToString();
                    if (err == "010")
                        return new BeanResult<T>(true);
                    else
                        return new BeanResult<T>(false);
                }

                List<T> lstMstData = retData["data"].ToObject<List<T>>();
                String LocalKeyName = BeanBase.getPriKey(typeof(T))[0];
                String serverKeyName = BeanBase.getPriKeyS(typeof(T))[0];

                if (lstMstData == null || lstMstData.Count == 0)
                    return new BeanResult<T>(true);

                List<T> lstInsertItem = new List<T>();
                List<T> lstUpdateItem = new List<T>();


                List<string> lstHtmlDecode = BeanBase.getLstProName(type, typeof(HtmlEncodeAttribute));
                List<string> lstHtmlRemove = BeanBase.getLstProName(type, typeof(HtmlRemoveAttribute));
                List<string> lstRemoveAccentFrom = BeanBase.getLstProName(type, typeof(RemoveAccentFromAttribute));
                List<string> lstRemoveAccentFromMultiColumn = BeanBase.getLstProName(type, typeof(RemoveAccentFromMultiColumnAttribute));

                // Lấy danh sách các thuộc tính Bean kiểu DateTime
                PropertyInfo[] arrProDateTime = CmmFunction.GetPropertysWithType(type, typeof(DateTime?));

                if (flgChkUpdate)
                {
                    foreach (T item in lstMstData)
                    {
                        string sqlSel;

                        if (type.Name == "BeanItemDeleted")
                        {
                            BeanItemDeleted itemDelete = CmmFunction.ChangeToRealType<BeanItemDeleted>(item);

                            if (!string.IsNullOrEmpty(itemDelete.BeanName))
                            {
                                try
                                {
                                    sqlSel = string.Format("DELETE FROM {0} WHERE {1} = ?", itemDelete.BeanName, itemDelete.Key);
                                    var res = -1;
                                    if (itemDelete.BeanName.ToLower().Equals("beannotify"))
                                    {
                                        using (SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_Wpf_SqlPathNotify, false))
                                        {
                                            res = conn.Execute(sqlSel, itemDelete.Value.ToLower());
                                        }
                                    }
                                    else if (itemDelete.BeanName.ToLower().Equals("beangroup"))
                                    {
                                        using (SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_Wpf_SqlPathNotify, false))
                                        {
                                            sqlSel = string.Format("DELETE FROM {0} WHERE {1} = ?", "BeanMyGroup", itemDelete.Key);
                                            res = conn.Execute(sqlSel, itemDelete.Value.ToLower());
                                        }
                                    }
                                    else
                                        res = con.Execute(sqlSel, itemDelete.Value.ToLower());
                                    //var res = con.Execute(sqlSel, itemDelete.Value.ToLower());
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }
                            }
                            continue;
                        }

                        // Decode Dữ liệu bị encode html
                        foreach (string decodeFieldName in lstHtmlDecode)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, decodeFieldName);

                            object decodeValue = CmmFunction.GetPropertyValue(item, propInfo);
                            if (decodeValue != null)
                            {
                                decodeValue = WebUtility.HtmlDecode(decodeValue + string.Empty);
                                CmmFunction.SetPropertyValue(item, propInfo, decodeValue);
                            }
                        }

                        // Remove Dữ liệu có html tag
                        foreach (string decodeFieldName in lstHtmlRemove)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, decodeFieldName);

                            object decodeValue = CmmFunction.GetPropertyValue(item, propInfo);
                            if (decodeValue != null)
                            {
                                decodeValue = Regex.Replace(decodeValue + "", "<.*?>", String.Empty);
                                CmmFunction.SetPropertyValue(item, propInfo, decodeValue);
                            }
                        }

                        // Remove accent 1 column
                        foreach (string removeAccentFieldName in lstRemoveAccentFrom)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, removeAccentFieldName);

                            RemoveAccentFromAttribute cmmAttr = (RemoveAccentFromAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                object removeAccentValue = CmmFunction.GetPropertyValueByName(item, cmmAttr.ColFrom);
                                if (removeAccentValue != null)
                                {
                                    if (removeAccentValue.ToString().Contains(";#"))
                                    {
                                        //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                        removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                    }

                                    removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "").ToLowerInvariant();
                                    CmmFunction.SetPropertyValue(item, propInfo, removeAccentValue);
                                }
                            }
                        }

                        // Remove dấu lấy từ nhiều thuộc tính khác
                        string temp_khongdau = "";
                        foreach (string removeAccentFieldName in lstRemoveAccentFromMultiColumn)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, removeAccentFieldName);

                            RemoveAccentFromMultiColumnAttribute cmmAttr = (RemoveAccentFromMultiColumnAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromMultiColumnAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                if (cmmAttr.ColFrom.Count() > 0)
                                {
                                    foreach (var res in cmmAttr.ColFrom)
                                    {
                                        object removeAccentValue = CmmFunction.GetPropertyValueByName(item, res);
                                        if (removeAccentValue != null)
                                        {
                                            if (removeAccentValue.ToString().Contains(";#"))
                                            {
                                                //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                                removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                            }

                                            removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "");
                                            temp_khongdau += removeAccentValue + "^";
                                        }
                                    }
                                    CmmFunction.SetPropertyValue(item, propInfo, temp_khongdau.ToLowerInvariant());
                                }
                            }
                        }

                        //Kiểm tra Convert Datetime to UTC
                        if (arrProDateTime != null)
                        {
                            foreach (PropertyInfo pro in arrProDateTime)
                            {
                                object objValue = CmmFunction.GetPropertyValue(item, pro);
                                if (objValue != null)
                                {
                                    DateTime objDateTime = ((DateTime)objValue).AddHours(CmmVariable.M_DiffHours);
                                    CmmFunction.SetPropertyValueByName(item, pro.Name, objDateTime);
                                }
                            }
                        }

                        // Kiểm tra Table nếu tồn tại rồi thì Update
                        sqlSel = string.Format("SELECT {1} FROM {0} WHERE {1} = ?", type.Name, serverKeyName);
                        List<object> lstObjChk = con.Query(new TableMapping(type), sqlSel, CmmFunction.GetPropertyValueByName(item, serverKeyName));

                        if (lstObjChk.Count > 0)
                        {
                            T objChk = (T)lstObjChk[0];
                            CmmFunction.SetPropertyValueByName(item, LocalKeyName, CmmFunction.GetPropertyValueByName(objChk, LocalKeyName));

                            lstUpdateItem.Add(item);
                        }
                        else // Nếu không tồn tại thì Insert
                            lstInsertItem.Add(item);
                    }
                }
                else // Insert
                {
                    foreach (T item in lstMstData)
                    {
                        // Decode Dữ liệu bị encode html
                        foreach (string decodeFieldName in lstHtmlDecode)
                        {
                            object decodeValue = CmmFunction.GetPropertyValueByName(item, decodeFieldName);
                            if (decodeValue != null)
                            {
                                decodeValue = WebUtility.HtmlDecode(decodeValue + string.Empty);
                                CmmFunction.SetPropertyValueByName(item, decodeFieldName, decodeValue);
                            }
                        }

                        // Remove Dữ liệu có html tag
                        foreach (string decodeFieldName in lstHtmlRemove)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, decodeFieldName);

                            object decodeValue = CmmFunction.GetPropertyValue(item, propInfo);
                            if (decodeValue != null)
                            {
                                decodeValue = Regex.Replace(decodeValue + "", "<.*?>", String.Empty);
                                CmmFunction.SetPropertyValue(item, propInfo, decodeValue);
                            }
                        }

                        // Remove accent 1 column
                        foreach (string removeAccentFieldName in lstRemoveAccentFrom)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, removeAccentFieldName);

                            RemoveAccentFromAttribute cmmAttr = (RemoveAccentFromAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                object removeAccentValue = CmmFunction.GetPropertyValueByName(item, cmmAttr.ColFrom);
                                if (removeAccentValue != null)
                                {
                                    if (removeAccentValue.ToString().Contains(";#"))
                                    {
                                        //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                        removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                    }

                                    removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "").ToLowerInvariant();
                                    CmmFunction.SetPropertyValue(item, propInfo, removeAccentValue);
                                }
                            }
                        }

                        // Remove dấu lấy từ nhieu thuộc tính khác
                        string temp_khongdau = "";
                        foreach (string removeAccentFieldName in lstRemoveAccentFromMultiColumn)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, removeAccentFieldName);

                            RemoveAccentFromMultiColumnAttribute cmmAttr = (RemoveAccentFromMultiColumnAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromMultiColumnAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                if (cmmAttr.ColFrom.Count() > 0)
                                {
                                    foreach (var res in cmmAttr.ColFrom)
                                    {
                                        object removeAccentValue = CmmFunction.GetPropertyValueByName(item, res);
                                        if (removeAccentValue != null)
                                        {
                                            if (removeAccentValue.ToString().Contains(";#"))
                                            {
                                                //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                                removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                            }

                                            removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "");
                                            temp_khongdau += removeAccentValue + "^";
                                        }
                                    }
                                    CmmFunction.SetPropertyValue(item, propInfo, temp_khongdau.ToLowerInvariant());
                                }
                            }
                        }

                        //kiem tra convert datetime to UTC
                        if (arrProDateTime != null)
                        {
                            foreach (PropertyInfo pro in arrProDateTime)
                            {
                                object objValue = CmmFunction.GetPropertyValue(item, pro);

                                if (objValue != null)
                                {
                                    DateTime objDateTime = ((DateTime)objValue).AddHours(CmmVariable.M_DiffHours);
                                    CmmFunction.SetPropertyValueByName(item, pro.Name, objDateTime);
                                }
                            }
                        }
                    }
                    lstInsertItem = lstMstData;
                }

                string sysDateNow = retData.Value<string>("dateNow");
                if (string.IsNullOrEmpty(sysDateNow)) return new BeanResult<T>(false);

                con.BeginTransaction();

                if (type.Name.Equals("BeanNotify"))
                {

                }
                if (lstInsertItem.Count > 0)
                    con.InsertAll(lstInsertItem);

                if (lstUpdateItem.Count > 0)
                    con.UpdateAll(lstUpdateItem);

                CmmFunction.UpdateDBVariable(new DBVariable(ID, sysDateNow), con);
                con.Commit();
                con.Close();
                return new BeanResult<T>(true)
                {
                    Data = lstInsertItem.Count > 0 ? lstInsertItem : lstUpdateItem.Count > 0 ? lstUpdateItem : null
                };
            }
            catch (Exception ex)
            {
                con.Rollback();
                con.Commit();
                con.Close();

                Console.WriteLine("ERR - UpdateMasterData - " + type.Name + " - " + ex.Message + ex.StackTrace);
                System.Diagnostics.Debug.Write("ERR UpdateMasterData: " + ex.Message + ex.StackTrace);

            }
            finally
            {
                if (con != null)
                    con.Close();
            }
            return new BeanResult<T>(false);
        }

        /// <summary>
        /// Download File ffrom server
        /// </summary>
        /// <param name="url"></param>
        /// <param name="localpath"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public bool DownloadFile(string url, string localpath, HttpClient httpClient)
        {
            try
            {
                Uri filepath = new Uri(url);
                if (httpClient != null)
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, filepath);
                    request.Headers.Add("ACCEPT", "*/*");
                    HttpResponseMessage response = httpClient.SendAsync(request).Result;
                    if (response.StatusCode != HttpStatusCode.OK) return false;
                    byte[] byteArray = response.Content.ReadAsByteArrayAsync().Result;
                    File.WriteAllBytes(localpath, byteArray);
                }
                return true;
            }
            catch (Exception ex)
            {
                if (File.Exists(localpath))
                {
                    try
                    {
                        File.Delete(localpath);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("ProviderBase - DownloadFile - Err: " + ex);
                    }
                }

                Console.WriteLine("ProviderBase - DownloadFile - Err: " + ex);

                return false;
            }
        }

        /// <summary>
        /// Load more data - neu trong DB da load het thi load them tu server
        /// </summary>
        /// <returns><c>true</c>, if more data was loaded, <c>false</c> otherwise.</returns>
        public List<T> LoadMoreData<T>(string querry, int limit, int offset, SQLiteConnection con = null)
        {
            // đóng tạm do SQLite
            ////            List<T> retValue = new List<T>();
            ////            try
            ////            {
            ////                if (con == null)
            ////                    con = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, false);

            ////                //string querry_more = string.Format(_querry + "LIMIT ? OFFSET ?");
            ////                //List<object> lstObjChk = con.Query(new TableMapping(type), querry_more, limit, offset);
            ////                string querryMore = string.Format(querry);
            ////                List<T> lstObjChk = con.Query<T>(querryMore, limit, offset);

            ////                if (lstObjChk != null && lstObjChk.Count > 0 && lstObjChk.Count <= limit) // load more from server
            ////                {
            ////                    retValue = lstObjChk.Cast<T>().ToList();
            ////                    //List<T> lstGetServer = GetMoreData<T>(null, limit);
            ////                    //lstObjChk.AddRange(lstGetServer);
            ////                }

            ////                return retValue;
            ////            }
            ////            catch (Exception ex)
            ////            {
            ////
            ////                Console.WriteLine("ERROR - LoadMoreData: " + ex);
            ////
            ////            }

            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="querry"></param>
        /// <param name="limit"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<T> LoadMoreDataT<T>(string querry, int limit, params object[] args)
        {
            // đóng tạm do SQLite
            List<T> retValue = new List<T>();
            ////            try
            ////            {
            ////                SQLiteConnection con = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, false);

            ////                string querryMore = string.Format(querry);
            ////                List<T> lstObjChk = con.Query<T>(querryMore, args);

            ////                if (lstObjChk != null && lstObjChk.Count > 0 && lstObjChk.Count <= limit) // load more from server
            ////                {
            ////                    retValue = lstObjChk.Cast<T>().ToList();
            ////                    //List<T> lstGetServer = GetMoreData<T>(null, limit);
            ////                    //lstObjChk.AddRange(lstGetServer);
            ////                }

            ////                return retValue;
            ////            }
            ////            catch (Exception ex)
            ////            {
            ////
            ////                Console.WriteLine("ERROR - LoadMoreData: " + ex);
            ////
            ////            }

            return null;
        }

        /// <summary>
        /// Tải thêm dữ liệu từ Server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<T> LoadMoreDataDocument<T>(int limit, int offset, SQLiteConnection conn)
        {
            List<T> retValue = new List<T>();
            try
            {
                if (conn == null)
                    conn = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, false);
                if (typeof(T) == typeof(BeanNotify))
                    conn = new SQLiteConnection(CmmVariable.M_Wpf_SqlPathNotify, false);

                List<T> lstGetServer = GetMoreDataDocument<T>(conn, limit, offset);
                if (lstGetServer != null && lstGetServer.Count > 0)
                {
                    List<T> lstObjChk = new List<T>(lstGetServer);
                    retValue = lstObjChk.Cast<T>().ToList();
                }

                return retValue;
            }
            catch (Exception ex)
            {

                Console.WriteLine("ERROR - LoadMoreDataFromServer: " + ex.Message + ex.StackTrace);

            }
            return new List<T>();
        }

        /// <summary>
        /// Tải thêm dữ liệu từ Server - Lấy giới hạn 6 tháng
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public List<T> GetMoreDataDocument<T>(SQLiteConnection con = null, int limit = 20, int offset = 0)
        {
            List<T> retValue = null;
            try
            {
                Type type = typeof(T);
                if (con == null)
                    con = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, false);
                if (typeof(T) == typeof(BeanNotify))
                    con = new SQLiteConnection(CmmVariable.M_Wpf_SqlPathNotify, false);

                BeanBase objMst = (BeanBase)(Activator.CreateInstance(type));

                string combieUrl = CmmVariable.M_Domain;
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();

                lstGet.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("offset", offset.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("Modified", DateTime.Now.AddDays(180 * -1).ToString("yyyy-MM-dd HH:mm:ss")));
                List<KeyValuePair<string, string>> plugPostPara = objMst.GetParaPost();
                string ApiServerUrl = objMst.GetServerUrl();
                ApiServerUrl = ApiServerUrl.Replace("<#SiteName#>", string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite) ? "" : ("/" + CmmVariable.sysConfig.Subsite));
                Console.WriteLine($"Start Get Api Loadmore {type.Name} - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff")}");
                JObject retData = GetJsonDataFromAPI(combieUrl + ApiServerUrl, ref CmmVariable.M_AuthenticatedHttpClient, new PAR(lstGet, plugPostPara));
                Console.WriteLine($"End Get Api Loadmore {type.Name} - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff")}");
                if (retData == null) return retValue;

                string strStatus = retData.Value<string>("status");
                if (strStatus == null || !strStatus.Equals("SUCCESS"))
                    return retValue;

                bool flgEndData = false;

                List<T> lstMstData = retData["data"].ToObject<List<T>>();
                //if (lstMstData == null) return retValue;

                // Dữ liệu trả về nhỏ hơn dữ liệu yêu cầu lấy --> Dữ liệu Server đã lấy hết --> Set cờ End Data
                if (lstMstData == null 
                    || (lstMstData != null && lstMstData.Count < limit))
                {
                    flgEndData = true;
                    CmmFunction.UpdateDBVariable(new DBVariable(type.Name + "Start", "-1"), con);

                    return retValue;
                }

                if (lstMstData.Count == 0)
                    return retValue;

                String serverKeyName = BeanBase.getPriKeyS(type)[0];
                List<T> lstInsertItem = new List<T>();
                List<string> lstHtmlDecode = BeanBase.getLstProName(type, typeof(HtmlEncodeAttribute));
                List<string> lstHtmlRemove = BeanBase.getLstProName(type, typeof(HtmlRemoveAttribute));
                List<string> lstRemoveAccentFrom = BeanBase.getLstProName(type, typeof(RemoveAccentFromAttribute));
                List<string> lstRemoveAccentFromMultiColumn = BeanBase.getLstProName(type, typeof(RemoveAccentFromMultiColumnAttribute));

                string sqlSelCheck = string.Format("SELECT ID FROM {0} WHERE {1} = ? ", type.Name, serverKeyName);

                foreach (T item in lstMstData)
                {
                    object serKeyValue = CmmFunction.GetPropertyValueByName(item, serverKeyName);

                    // Kiểm tra nếu tồn tại rồi thì bỏ qua không Insert vì đây là dữ liệu cũ hơn
                    List<object> lstObjChk = con.Query(new TableMapping(type), sqlSelCheck, serKeyValue);

                    if (lstObjChk.Count > 0)
                        continue;

                    // Decode Dữ liệu bị encode html
                    foreach (string decodeFieldName in lstHtmlDecode)
                    {
                        object decodeValue = CmmFunction.GetPropertyValueByName(item, decodeFieldName);
                        if (decodeValue != null)
                        {
                            decodeValue = WebUtility.HtmlDecode(decodeValue + string.Empty);
                            CmmFunction.SetPropertyValueByName(item, decodeFieldName, decodeValue);
                        }
                    }

                    // Remove Dữ liệu có html tag
                    foreach (string decodeFieldName in lstHtmlRemove)
                    {
                        PropertyInfo propInfo = CmmFunction.GetProperty(item, decodeFieldName);

                        object decodeValue = CmmFunction.GetPropertyValue(item, propInfo);
                        if (decodeValue != null)
                        {
                            decodeValue = Regex.Replace(decodeValue + "", "<.*?>", String.Empty);
                            CmmFunction.SetPropertyValue(item, propInfo, decodeValue);
                        }
                    }

                    // Remove accent 1 column
                    foreach (string removeAccentFieldName in lstRemoveAccentFrom)
                    {
                        PropertyInfo propInfo = CmmFunction.GetProperty(item, removeAccentFieldName);

                        RemoveAccentFromAttribute cmmAttr = (RemoveAccentFromAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromAttribute), true).FirstOrDefault();

                        if (cmmAttr != null)
                        {
                            object removeAccentValue = CmmFunction.GetPropertyValueByName(item, cmmAttr.ColFrom);
                            if (removeAccentValue != null)
                            {
                                removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "").ToLowerInvariant();
                                CmmFunction.SetPropertyValue(item, propInfo, removeAccentValue);
                            }
                        }
                    }

                    // Remove dấu lấy từ nhiều thuộc tính khác
                    string temp_khongdau = "";
                    foreach (string removeAccentFieldName in lstRemoveAccentFromMultiColumn)
                    {
                        PropertyInfo propInfo = CmmFunction.GetProperty(item, removeAccentFieldName);

                        RemoveAccentFromMultiColumnAttribute cmmAttr = (RemoveAccentFromMultiColumnAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromMultiColumnAttribute), true).FirstOrDefault();

                        if (cmmAttr != null)
                        {
                            if (cmmAttr.ColFrom.Count() > 0)
                            {
                                foreach (var res in cmmAttr.ColFrom)
                                {
                                    object removeAccentValue = CmmFunction.GetPropertyValueByName(item, res);
                                    if (removeAccentValue != null)
                                    {
                                        removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "");
                                        temp_khongdau += removeAccentValue + "^";
                                    }
                                }
                                CmmFunction.SetPropertyValue(item, propInfo, temp_khongdau.ToLowerInvariant());
                            }
                        }
                    }

                    lstInsertItem.Add(item);
                }

                string sysDateNow = retData.Value<string>("dateNow");
                if (string.IsNullOrEmpty(sysDateNow)) return retValue;

                con.BeginTransaction();
                if (lstInsertItem.Count > 0)
                    con.InsertAll(lstInsertItem);

                // Nếu chưa lấy hết dữ liệu từ Server thì cập nhật lại ModifiedStart của Table
                if (!flgEndData)
                {
                    string sqlGetMinModified = string.Format("SELECT MIN({0}) AS DateData FROM {1} ", objMst.GetModifiedColName(), type.Name);
                    GetMoreDate moreData = con.Query<GetMoreDate>(sqlGetMinModified).FirstOrDefault();

                    if (moreData != null)
                    {
                        sysDateNow = moreData.DateData.ToString("yyyy-MM-dd HH:mm:ss");
                        CmmFunction.UpdateDBVariable(new DBVariable(type.Name + "Start", sysDateNow), con);
                    }
                }

                con.Commit();
                con.Close();

                retValue = lstInsertItem;
                return retValue;

            }
            catch (Exception ex)
            {
                con.Rollback();

                System.Diagnostics.Debug.Write("ERR UpdateMasterData: " + ex.Message);

            }

            return retValue;
        }

        /// <summary>
        /// return true => het data
        /// </summary>
        /// <returns><c>true</c>, if end of data was checked, <c>false</c> otherwise.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static bool CheckEndOfData<T>()
        {
            bool retValue = false;

            SQLiteConnection con = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, false);
            if (typeof(T) == typeof(BeanNotify))
                con = new SQLiteConnection(CmmVariable.M_Wpf_SqlPathNotify, false);

            TableQuery<DBVariable> table = con.Table<DBVariable>();
            string temp = typeof(T).Name + "Start";
            var items = from i in table
                        where i.Id == temp
                        select i;

            if (items.Count() > 0)
            {
                string ModifiedStart = items.First().Value;
                retValue = (ModifiedStart == "-1");
            }

            return retValue;
        }

        public class GetMoreDate
        {
            public DateTime DateData { get; set; }
        }
    }

    public class BeanResult<T>
    {
        public bool Status { get; set; }
        public List<T> Data { get; set; }
        public string Message { get; set; }
        public BeanResult(bool _Status)
        {
            Status = _Status;
        }
    }
}
