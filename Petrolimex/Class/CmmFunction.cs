using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using Petrolimex.Bean;
using Petrolimex.DataProvider;
using SQLite;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Management;

namespace Petrolimex.Class
{
    public class CmmFunction
    {
        public static bool DoesCurrentWorkFlowFinished(string strStatus)
        {
            bool ReturnValue = false;

            strStatus = strStatus.ToLower();
            if (strStatus == "phê duyệt" || strStatus == "chuyển phát hành" ||
            strStatus == "đã phát hành" || strStatus == "chờ phát hành" || strStatus == "từ chối" || strStatus == "chờ đóng dấu")
            {
                ReturnValue = true;
            }
            return ReturnValue;
        }
        /// <summary>
        /// Cập nhật biến SQL
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        public static bool UpdateDBVariable(DBVariable variable, SQLiteConnection con = null)
        {
            try
            {
                if (con == null)
                {
                    con = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath);
                }
                if(variable.Id.Equals("BeanNotifyStart"))
                {
                    con = new SQLiteConnection(CmmVariable.M_Wpf_SqlPathNotify, false);
                }    
                TableQuery<DBVariable> table = con.Table<DBVariable>();
                var items = from i in table
                            where i.Id == variable.Id
                            select i;
                if (items.Count() > 0)
                {
                    con.Update(variable);
                }
                else
                {
                    con.Insert(variable);
                }
                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine("UpdateDBVariable ERR: " + ex.Message + ex.StackTrace);

            }
            return false;
        }

        public static bool UpdateDBVariable(DBVariable variable, SQLiteAsyncConnection con)
        {
            try
            {
                if (con == null)
                {
                    con = new SQLiteAsyncConnection(CmmVariable.M_Wpf_SQLitePath);
                }
                AsyncTableQuery<DBVariable> table = con.Table<DBVariable>();
                var items = from i in table
                            where i.Id == variable.Id
                            select i;
                if (items.CountAsync().Result > 0)
                {
                    con.UpdateAsync(variable);
                }
                else
                {
                    con.InsertAsync(variable);
                }
                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine("UpdateDBVariable ERR: " + ex.Message + ex.StackTrace);

            }
            return false;
        }

        /// <summary>
        /// Lấy tiêu đề
        /// </summary>
        /// <param name="fieldId"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetTitle(string fieldId, string defaultValue = "")
        {
            string retValue = defaultValue;
            try
            {
                if (CmmVariable.M_LangData == null)
                {
                    CmmVariable.M_LangData = new Dictionary<string, string>();
                    if (File.Exists(CmmVariable.M_Wpf_SQLitePath))
                    {
                        using (var conn = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, SQLiteOpenFlags.ReadOnly, false))
                        {
                            List<BeanSettings> lstLang = conn.Query<BeanSettings>("SELECT * FROM BeanSettings");
                            foreach (BeanSettings langItem in lstLang)
                            {
                                CmmVariable.M_LangData.Add(langItem.KEY, langItem.VALUE);
                            }
                        }
                    }
                }

                //Nếu trong dictionary không có thì lấy từ dư liệu sqlLite
                string outValue;
                if (CmmVariable.M_LangData.TryGetValue(fieldId, out outValue))
                {
                    retValue = outValue;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmFunction - getTitle - ERR: " + ex.ToString());
            }

            return retValue;
        }
        public static HttpClient CreateHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            string loginPass = new Crypter(CmmVariable.M_RSACodePublicKey, true)
                                    .Decrypt(CmmVariable.sysConfig.LoginPassword).Replace(CmmVariable.Prefix, "");

            Cookie cookie = GetAuthCookie(CmmVariable.M_Domain, CmmVariable.sysConfig.LoginName, loginPass);

            if (cookie == null)
                return null;

            handler.CookieContainer.Add(cookie);
            handler.AllowAutoRedirect = false;
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri(CmmVariable.M_Domain, UriKind.RelativeOrAbsolute);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            return client;
        }
        /// <summary>
        /// Login tới server
        /// </summary>
        /// <param name="loginUrl">Địa chỉ Url API thực hiện login</param>
        /// <param name="loginName">tài khoản</param>
        /// <param name="passWord">Pass hoặc mã OTP</param>
        /// <param name="flgTickEventLogin"></param>
        /// <param name="loginType">Phân loại login: 1: login thông thường, 2: auto login, 3: login ghi đè DeviceInfo</param>
        public static HttpClient Login(string loginUrl, string loginName, string passWord, bool flgTickEventLogin = false, int loginType = 1)
        {
            try
            {
                Console.WriteLine($"Begin login : {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.ffff")}");
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                string userName = loginName.Contains('@') ? loginName.Split('@')[0] : loginName;
                Cookie cookie = GetAuthCookie(CmmVariable.M_Domain, userName, passWord);

                if (cookie == null)
                {
                    CmmEvent.ReloginRequest_Performence(null, null);
                    return null;
                }

                handler.CookieContainer.Add(cookie);
                handler.AllowAutoRedirect = false;
                HttpClient client = new HttpClient(handler);
                client.BaseAddress = new Uri(CmmVariable.M_Domain, UriKind.RelativeOrAbsolute);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "login"));
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                //lstPost.Add(new KeyValuePair<string, string>("deviceInfo", string.IsNullOrEmpty(CmmVariable.sysConfig.DeviceInfo) ? "" : CmmVariable.sysConfig.DeviceInfo));
                lstPost.Add(new KeyValuePair<string, string>("loginType", loginType.ToString()));

                CmmVariable.M_AuthenticatedHttpClient = client;

                ProviderBase pro = new ProviderBase();
                JObject retData = pro.GetJsonDataFromAPI(loginUrl, ref client, new ProviderBase.PAR(lstGet, lstPost), false);
                Console.WriteLine($"End login : {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.ffff")}");
                if (retData == null)
                {
                    //if (flgTickEventLogin)
                    //    CmmEvent.ReloginRequest_Performence(null, null);
                    
                    return null;
                }
                string strStatus = retData.Value<string>("status");
                if (strStatus == null)
                {
                    //if (flgTickEventLogin)
                    //    CmmEvent.ReloginRequest_Performence(null, null);

                    return null;
                }

                if (strStatus.Equals("SUCCESS"))
                {
                    if (flgTickEventLogin)
                    {
                        //BeanUser userInfo = retData["data"].ToObject<BeanUser>();
                        //CmmEvent.ReloginRequest_Performence(null, new CmmEvent.LoginEventArgs(true, loginName, passWord, userInfo));
                    }
                }
                ServicePointManager.FindServicePoint(new Uri(CmmVariable.M_Domain)).ConnectionLimit = 1000;
                return client;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmFunction - Login - ERROR: " + ex);
                if (ex.Message == "Error: NameResolutionFailure")// chua mo vpn
                    CmmEvent.ReloginRequest_Performence(null, new CmmEvent.LoginEventArgs(false, loginName, passWord, null, "NameResolutionFailure"));
                else if (ex.Message == "Unable to read data from the transport connection: Connection timed out.")
                    CmmEvent.ReloginRequest_Performence(null, new CmmEvent.LoginEventArgs(false, loginName, passWord, null, "Connection timed out"));
                else if (ex.Message.Contains("Connection timed out"))
                    CmmEvent.ReloginRequest_Performence(null, new CmmEvent.LoginEventArgs(false, loginName, passWord, null, "Connection timed out"));
                return null;
            }
        }

        /// <summary>
        /// Format Ykien Lãnh đạo từ Html To Json
        /// </summary>
        /// <param name="inputValue">y kiến định dạng html</param>
        /// <returns></returns>
        public static string FormatYkienLd(string inputValue)
        {
            if (string.IsNullOrEmpty(inputValue)) return "";
            string retValue = "";
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(inputValue);

            List<HtmlNode> lstNode = htmlDoc.DocumentNode
            .Descendants("span")
            .ToList();

            if (lstNode != null && lstNode.Count > 0)
            {
                List<string> lstYkienTitle = new List<string>();
                foreach (HtmlNode node in lstNode)
                {
                    lstYkienTitle.Add(node.InnerText);
                }

                lstNode = htmlDoc.DocumentNode
                .Descendants("div")
                .Where(d =>
                   d.Attributes.Contains("class")
                   && d.Attributes["class"].Value.Contains("noidung")
                ).ToList();

                if (lstNode != null && lstNode.Count > 0 && lstNode.Count <= lstYkienTitle.Count)
                {
                    List<BeanYKien> lstYkien = new List<BeanYKien>();
                    for (int i = 0; i < lstNode.Count; i++)
                    {
                        lstYkien.Add(new BeanYKien(lstYkienTitle[i], lstNode[i].InnerText));
                    }

                    retValue = JsonConvert.SerializeObject(lstYkien);
                }

            }
            return retValue;
        }

        /// <summary>
        /// Lấy giá trị từ Html
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static string GetvalueFromHtml(string inputValue)
        {
            if (string.IsNullOrEmpty(inputValue)) return "";
            string retValue = "";
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(inputValue);

            List<HtmlNode> lstNode = htmlDoc.DocumentNode
            .Descendants("span")
            .ToList();

            if (lstNode != null && lstNode.Count > 0)
            {
                List<string> lstYkienTitle = new List<string>();
                foreach (HtmlNode node in lstNode)
                {
                    lstYkienTitle.Add(node.InnerText);
                }

                lstNode = htmlDoc.DocumentNode
                .Descendants("div")
                .Where(d =>
                   d.Attributes.Contains("class")
                   && d.Attributes["class"].Value.Contains("noidung")
                ).ToList();

                if (lstNode != null && lstNode.Count > 0 && lstNode.Count <= lstYkienTitle.Count)
                {
                    List<BeanYKien> lstYkien = new List<BeanYKien>();
                    for (int i = 0; i < lstNode.Count; i++)
                    {
                        lstYkien.Add(new BeanYKien(lstYkienTitle[i], lstNode[i].InnerText));
                    }

                    retValue = JsonConvert.SerializeObject(lstYkien);
                }
                else
                    retValue = lstYkienTitle[0];

            }
            return retValue;
        }

        /// <summary>
        /// Convert Attach file thành chuỗi Json
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static string ConvertAttachFileToStrJson(string strInput)
        {
            if (string.IsNullOrEmpty(strInput)) return "";
            List<BeanAttachFile> lstAttach = new List<BeanAttachFile>();
            string[] arrItem = strInput.Split(new[] { ";#" }, StringSplitOptions.None);
            if (arrItem.Length > 0)
            {
                foreach (string strItem in arrItem)
                {
                    string[] itemDetail = strItem.Split('|');
                    if (itemDetail.Length > 2)
                    {
                        BeanAttachFile bAttach = new BeanAttachFile();
                        bAttach.Url = itemDetail[0];
                        bAttach.Title = itemDetail[1];
                        bAttach.Type = itemDetail[2];

                        lstAttach.Add(bAttach);
                    }
                }

            }
            return JsonConvert.SerializeObject(lstAttach);

        }

        public static Cookie GetAuthCookie(String url, String uname, String pswd)
        {
            string envelope =
                        "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">"
                        + "<soap:Body>"
                        + "<Login xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">"
                        + "<username>{0}</username>"
                        + "<password>{1}</password>"
                        + "</Login>" + "</soap:Body>"
                        + "</soap:Envelope>";

            CookieContainer cookieJar = new CookieContainer();
            // Android: TrungTD
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => { return true; };

            Uri authServiceUri = new Uri(url + "/_vti_bin/authentication.asmx");
            HttpWebRequest spAuthReq = HttpWebRequest.Create(authServiceUri) as HttpWebRequest;
            spAuthReq.CookieContainer = cookieJar;
            spAuthReq.Headers["SOAPAction"] = "http://schemas.microsoft.com/sharepoint/soap/Login";
            spAuthReq.ContentType = "text/xml; charset=utf-8";
            spAuthReq.Method = "POST";

            string userName = uname;
            string password = pswd;
            envelope = string.Format(envelope, WebUtility.HtmlEncode(userName), WebUtility.HtmlEncode(password));
            StreamWriter streamWriter = new StreamWriter(spAuthReq.GetRequestStream());
            streamWriter.Write(envelope);
            streamWriter.Close();
            HttpWebResponse response = spAuthReq.GetResponse() as HttpWebResponse;
            if (response != null && response.Cookies.Count > 0)
            {
                Cookie returnValue = response.Cookies[0];
                response.Close();
                return returnValue;
            }
            else
                return null;
        }
        /// <summary>
        /// Lấy Danh sách Name trong chuỗi dữ liệu lookup của Sharepoint
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static List<string> GetNameFromLookupData(string strData)
        {
            List<string> retvalue = new List<string>();
            if (string.IsNullOrEmpty(strData)) return retvalue;
            if (!strData.Contains(";#")) return new List<string>() { strData };

            string[] lstData = strData.Split(new[] { ";#" }, StringSplitOptions.None);

            for (int i = 0; i < lstData.Length - 1; i += 2)
            {
                retvalue.Add(lstData[i + 1]);
            }

            return retvalue;
        }

        /// <summary>
        /// Khởi tạo Dữ liệu ban đâu khi chạy lần đầu tiên
        /// </summary>
        /// <param name="dataFilePath">Đường dẫn file DataBase sqlite</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool InstanceDb(string dataFilePath, Type type = null)
        {
            try
            {
                if (!File.Exists(dataFilePath))
                {
                    Console.WriteLine("Database does not exist, create new DB.");
                    //using (var conn = new SQLiteConnection(dataFilePath, false))
                    using (var conn = new SQLiteConnection(dataFilePath)) // bỏ false cho WPF
                    {
                        conn.CreateTable<DBVariable>();
                        conn.CreateTable<BeanSettings>();
                        conn.CreateTable<BeanUser>();
                        conn.CreateTable<BeanCodeItem>();
                        conn.CreateTable<BeanNotify>();
                        conn.CreateTable<BeanVanBanBanHanh>();
                        conn.CreateTable<BeanVanBanDen>();
                        conn.CreateTable<BeanDepartment>();
                        conn.CreateTable<BeanBanLanhDao>();
                        conn.CreateTable<BeanMyGroup>();

                        conn.CreateTable<BeanCoQuanGui>();
                        conn.CreateTable<BeanNguoiKyVanBan>();
                        conn.CreateTable<BeanLoaiVanBan>();

                        //if(CmmVariable.sysConfig != null && CmmVariable.sysConfig.IsVanThu)
                        //{
                            conn.CreateTable<BeanSoVanBanDen>();
                            conn.CreateTable<BeanLinhVuc>();
                            conn.CreateTable<BeanSoVanBanDi>();
                            conn.CreateTable<BeanUnitGroup>();
                            conn.CreateTable<BeanLoaiDinhKem>();
                        //}
                        // Thêm biến danh sách ID Văn Bản đến
                        UpdateDBVariable(new DBVariable("VBDListID", ""));
                        conn.Close();
                    }
                }
                else if (type != null)
                {
                    Console.WriteLine("Extent table :" + type.Name);
                    using (var conn = new SQLiteConnection(dataFilePath, false))
                    {
                        conn.CreateTable(type);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR - CmmFunction - instanceDB: " + ex);
            }
            return false;
        }

        /// <summary>
        /// Lấy biến từ DB
        /// </summary>
        /// <param name="id">Id field muốn lấy</param>
        /// <param name="con">connect nếu đã có trước</param>
        /// <returns></returns>
        public static DBVariable GetVariableFromDb(string id, SQLiteConnection con = null)
        {
            DBVariable retValue = null;
            try
            {
                if (con == null)
                {
                    con = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, false);
                }
                TableQuery<DBVariable> table = con.Table<DBVariable>();
                var items = from i in table
                            where i.Id == id
                            select i;
                if (items.Count() > 0)
                {
                    return items.First();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("ERR getVariableFromDB: " + ex.Message);
            }
            return retValue;
        }

        /// <summary>
        /// Lấy Danh sách các văn bản hiện có trên App
        /// </summary>
        /// <param name="con">connection Sqlite nếu có</param>
        /// <returns></returns>
        public static string GetListIdVanBanDen(SQLiteConnection con = null)
        {
            if (con == null)
            {
                con = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, false);
            }
            con = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, false);
            TableQuery<DBVariable> table = con.Table<DBVariable>();
            var items = from i in table
                        where i.Id == "VBDListID"
                        select i;
            if (items.Count() > 0)
            {
                return items.First().Value;
            }
            return "";
        }

        /// <summary>
        /// Thêm biến ID vào trong danh sách id văn bản
        /// </summary>
        /// <param name="strLstIdvbd"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        public static bool ExtentListIdVbd(string strLstIdvbd, SQLiteConnection con = null)
        {
            if (string.IsNullOrEmpty(strLstIdvbd)) return true;
            if (con == null)
            {
                con = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, false);
            }
            string sql = "UPDATE DBVariable Set Value = (Value || ?) WHERE ID = ?";
            con.Execute(sql, strLstIdvbd, "VBDListID");
            return true;
        }

        /// <summary>
        /// Kiểm tra phiên bản App trên Server và Client
        /// </summary>
        /// <param name="applicationVer"></param>
        /// <param name="configVariableVer"></param>
        /// <returns></returns>
        public static bool CheckIsNewVer(string applicationVer, string configVariableVer)
        {
            bool res = false;

            var versionConfig = new Version(configVariableVer);
            var versionApplication = new Version(applicationVer);

            var result = versionApplication.CompareTo(versionConfig);
            if (result > 0)
            {
                Console.WriteLine("version_application is greater");
                res = true;
            }
            else if (result < 0)
            {
                Console.WriteLine("version_config is greater");
            }
            else
            {
                Console.WriteLine("versions are equal");
            }

            return res;
        }
        public static object GetPropertyValueByName(object obj, string key)
        {
            object retValue = null;
            Type type = obj.GetType();
            PropertyInfo perInfo = type.GetProperty(key);
            if (perInfo != null)
            {
                retValue = perInfo.GetValue(obj);
            }
            return retValue;
        }
        public static object GetPropertyValue(object obj, PropertyInfo perInfo)
        {
            object retValue = null;
            retValue = perInfo.GetValue(obj);

            return retValue;
        }
        public static PropertyInfo[] GetPropertysWithType(Type objType, Type proType)
        {
            PropertyInfo[] retValue = null;
            PropertyInfo[] arrPro = objType.GetProperties();
            List<PropertyInfo> lstPro = new List<PropertyInfo>();
            foreach (PropertyInfo pro in arrPro)
            {
                if (pro.PropertyType == proType)
                {
                    lstPro.Add(pro);
                }
            }

            if (lstPro.Count > 0) return lstPro.ToArray();

            return retValue;
        }

        /// <summary>
        /// Lấy giá trị trong String nếu Null thì trả về Empty ""
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static string GetTextFromStr(string strInput)
        {
            if (string.IsNullOrEmpty(strInput)) return "";

            return strInput;
        }

        /// <summary>
        /// Kiểm tra hành động được phép trên văn bản không
        /// </summary>
        /// <param name="btnIdAction">Id của action muốn kiểm tra</param>
        /// <param name="vanbanBtnAction"> thuộc tính BtnAction trong đối tượng văn bản</param>
        /// <returns>true: được phép, false: không được phép</returns>
        public static bool CheckExistAction(int btnIdAction, int vanbanBtnAction)
        {
            return ((vanbanBtnAction & btnIdAction) > 0);
        }
        public static bool GetDataMstDataFromServer(bool flgFirst = false)
        {
            try
            {
                /*
                //get Department
                DepartmentProvider getDepartment = new DepartmentProvider();
                getDepartment.getDepartment(CmmVariable.M_getDepartmentLastTime.Value, !flgFirst);

                // get User/Group
                ContactProvider getUserGroup = new ContactProvider();
                getUserGroup.getContactInfolist(CmmVariable.M_getUserGroupLastTime.Value, !flgFirst);

                // get Document Data
                DocumentProvider getDoc = new DocumentProvider();
                GetDataStatus docGetResult = getDoc.getVBDFromList(CmmVariable.M_getDocumentLastTime.Value, !flgFirst);

                if (docGetResult == GetDataStatus.SUCC) {
                    System.Diagnostics.Debug.Write("MESS: Láy dữ liệu văn bản thành công - Có dữ liệu");
                }else if(docGetResult == GetDataStatus.SUCC_NO_DATA){
                    System.Diagnostics.Debug.Write("MESS: Láy dữ liệu văn bản thành công - Không có dữ liệu");
                }else {
                    System.Diagnostics.Debug.Write("ERR: Láy dữ liệu văn bản không thành công");
                }
                */
                return true;

            }
            catch (Exception)
            {
                //
            }
            return false;
        }
        public static bool GetDBtime(string dataFilePath)
        {
            try
            {

                //ProviderBase basePrd = new ProviderBase();
                //using (var conn = new SQLiteConnection(dataFilePath))
                //{
                //    getValue = basePrd.getVariableFromDB(CmmVariable.M_getBinUser.Id, conn);
                //    if (getValue != null) CmmVariable.M_getBinUser = getValue;
                //}
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("ERR getDBtime: " + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Đọc config cửa chương trình từ file config lên
        /// </summary>
        /// <returns></returns>
        public static bool ReadSetting()
        {
            try
            {
                if (File.Exists(CmmVariable.M_Wpf_SettingFileName))
                {
                    FileStream strm = new FileStream(CmmVariable.M_Wpf_SettingFileName, FileMode.Open, FileAccess.Read);
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter biforInfor = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    CmmVariable.sysConfig = (ConfigVariable)biforInfor.Deserialize(strm);
                    strm.Close();

                    // Add thêm serial vào sysConfig: Username password
                    string _prefix = GetPrefixMainBoardCode();
                    if (!String.IsNullOrEmpty(_prefix))
                    {
                        // encrypt RSA Password lại
                        Crypter crypter = new Crypter(CmmVariable.M_RSACodePublicKey, true);
                        CmmVariable.sysConfig.LoginPassword = crypter.Decrypt(CmmVariable.sysConfig.LoginPassword);

                        if (CmmVariable.sysConfig.LoginPassword.StartsWith(_prefix))
                        {
                            CmmVariable.sysConfig.LoginPassword = CmmVariable.sysConfig.LoginPassword.Replace(_prefix, "");
                        }
                        else // không trùng prefix -> logout
                            return false;
                    }
                    return true;
                }
                else
                {
                    string dirPath = Path.GetDirectoryName(CmmVariable.M_Wpf_SettingFileName);
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath ?? throw new InvalidOperationException());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("ERR readSetting: " + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Ghi thông tin config xuống file
        /// </summary>
        /// <returns></returns>
        public static bool WriteSetting(bool IsFirstLogin = true)
        {
            try
            {
                FileStream strm = new FileStream(CmmVariable.M_Wpf_SettingFileName, FileMode.OpenOrCreate, FileAccess.Write);
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter biforSetting = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                // Add thêm serial vào sysConfig: Username password
                if (IsFirstLogin)
                {
                    string _prefix = GetPrefixMainBoardCode();
                    if (!String.IsNullOrEmpty(_prefix))
                    {
                        if (!CmmVariable.sysConfig.LoginPassword.StartsWith(_prefix))
                        {
                            CmmVariable.sysConfig.LoginPassword = _prefix + CmmVariable.sysConfig.LoginPassword;

                            // encrypt RSA Password lại
                            string encryptKey = CmmVariable.M_RSACodePrivateKey;
                            encryptKey = CmmFunction.Base64Decode(encryptKey);
                            Crypter cry = new Crypter(encryptKey, false);
                            CmmVariable.sysConfig.LoginPassword = cry.EncryptStr(CmmVariable.sysConfig.LoginPassword);
                        }

                    }
                }

                biforSetting.Serialize(strm, CmmVariable.sysConfig);
                strm.Close();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("ERR readSetting: " + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Lấy số ID để gán vào mã hóa username, password
        /// </summary>
        /// <returns></returns>
        public static string GetPrefixMainBoardCode()
        {
            string _res = "";

            // "SELECT Product, SerialNumber FROM Win32_BaseBoard"
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Product FROM Win32_BaseBoard");

            ManagementObjectCollection information = searcher.Get();
            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                {
                    _res = data.Value.ToString();
                }
            }

            // [Lấy 8 ký tự đầu tiên của MainBoardCode, nếu rỗng tự động gắn ký tự 0 cho đủ 8 ký tự] + [Password]
            if (_res.Length > 8)
                _res = _res.Substring(0, 7);
            else
            {
                int _tempLength = _res.Length;
                for (int i = 0; i < 8 - _tempLength; i++)
                    _res += "0";
            }

            searcher.Dispose();
            return _res;
        }

        private static readonly string[] VietnameseSigns = new string[]{
                                                            "aAeEoOuUiIdDyY",
                                                            "áàạảãâấầậẩẫăắằặẳẵ",
                                                            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                                                            "éèẹẻẽêếềệểễ",
                                                            "ÉÈẸẺẼÊẾỀỆỂỄ",
                                                            "óòọỏõôốồộổỗơớờợởỡ",
                                                            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                                                            "úùụủũưứừựửữ",
                                                            "ÚÙỤỦŨƯỨỪỰỬỮ",
                                                            "íìịỉĩ",
                                                            "ÍÌỊỈĨ",
                                                            "đ",
                                                            "Đ",
                                                            "ýỳỵỷỹ",
                                                            "ÝỲỴỶỸ"
                                                            };

        /// <summary>
        /// Bỏ dấu tiếng việt
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSignVietnamese(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

            }
            return str;

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="strPropName"></param>
        /// <param name="value"></param>
        public static void SetPropertyValueByName(object obj, string strPropName, object value)
        {
            PropertyInfo propInfo = GetProperty(obj, strPropName);
            if (propInfo != null)
            {
                //Nullable<System.DateTime>
                Type t = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

                object safeValue = (value == null) ? null : Convert.ChangeType(value, t);

                propInfo.SetValue(obj, safeValue, null);
            }

        }

        /// <summary>
        /// Phân luồng Task để tăng tốc độ chạy App
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flgUpdate"></param>
        /// <param name="flgGetAll"></param>
        /// <returns></returns>
        public static Task GetDataLogin<T>(bool flgUpdate, bool flgGetAll)
        {
            new ProviderBase().UpdateMasterData<T>(null, flgUpdate, CmmVariable.sysConfig.DataLimitDay, flgGetAll);
            return Task.CompletedTask;
        }

        /// <summary>
        ///  Set giá trị cho thuộc tính của Object
        /// </summary>
        /// <param name="obj">Object muốn set giá trị</param>
        /// <param name="propInfo">Thuộc tính propertyInfo thuộc Class Object</param>
        /// <param name="value">Giá trị muốn set</param>
        /// <returns></returns>
        public static void SetPropertyValue(object obj, PropertyInfo propInfo, object value)
        {
            Type t = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
            object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
            propInfo.SetValue(obj, safeValue, null);


        }

        /// <summary>
        /// Giải mã chuỗi Json thành Object và bỏ qua Catch
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng muốn chuyển đổi thành</typeparam>
        /// <param name="value">Giá trị chuỗi Json</param>
        /// <returns></returns>
        public static T TryDeserializeObject<T>(string value)
        {
            T retValue = default(T);
            try
            {
                retValue = JsonConvert.DeserializeObject<T>(value);
            }
            catch
            {
                //
            }
            return retValue;
        }

        /// <summary>
        /// Lấy Danh sách các Id trong chuỗi dữ liệu lookup của Sharepoint
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static List<int> GetIdFromLookupData(string strData)
        {
            List<int> retvalue = new List<int>();
            if (string.IsNullOrEmpty(strData)) return retvalue;
            string[] lstData = strData.Split(new[] { ";#" }, StringSplitOptions.None);

            for (int i = 0; i < lstData.Length - 1; i += 2)
            {
                int tempData;
                if (int.TryParse(lstData[i], out tempData))
                {
                    retvalue.Add(tempData);
                }
            }

            return retvalue;
        }
        /// <summary>
        /// Map phần Data field giữa 2 object
        /// </summary>
        /// <param name="objFrom">object chứa dữ liệu nguồn</param>
        /// <param name="objTo">object muốn map dữ liệu tới</param>
        /// <param name="lstColsFilter">Danh sách các cột muốn lọc lấy dự liệu</param>
        /// <returns></returns>
        public static object MapData(object objFrom, object objTo, List<string> lstColsFilter = null)
        {

            Type objFromType = objFrom.GetType();
            PropertyInfo[] arrProperty = objFromType.GetProperties();
            foreach (PropertyInfo prop in arrProperty)
            {

                // Nếu Property không tồn lại trong List lọc thì bỏ qua
                if (lstColsFilter != null && !lstColsFilter.Contains(prop.Name)) continue;

                object fieldValue = prop.GetValue(objFrom);
                SetPropertyValueByName(objTo, prop.Name, fieldValue);

            }

            return objTo;
        }
        public static PropertyInfo GetProperty(object obj, string strPropName)
        {
            Type type = obj.GetType();
            return type.GetProperty(strPropName);
        }
        public static object GetPropertyValue(object obj, string strPropName)
        {
            Type type = obj.GetType();
            return type.GetProperty(strPropName)?.GetValue(obj, null);
        }
        public static T ChangeToRealType<T>(object readData)
        {
            if (readData is T)
            {
                return (T)readData;
            }
            else
            {
                try
                {
                    return (T)Convert.ChangeType(readData, typeof(T));
                }
                catch (InvalidCastException)
                {
                    return default(T);
                }
            }
        }
        public static string GetValue(Dictionary<string, string> keyValuePairs, string key)
        {
            string value = "";
            try
            {
                if (keyValuePairs != null && !string.IsNullOrEmpty(key))
                {
                    if (keyValuePairs.ContainsKey(key))
                    {
                        value = (keyValuePairs[key]);
                        return value;
                    }
                }
            }
            catch (Exception)
            {
                //
            }
            return value;
        }
        public static List<ClassActionTasks> GetLstMyAction(List<ClassActionTasks> lstButtonAction, int actionPermission)
        {
            return lstButtonAction.FindAll(i => (i.ID & actionPermission) > 0);
        }
        public static List<ClassActionTasks> GetColorMyAction(List<ClassActionTasks> lstButtonAction, Dictionary<string, string> dictionary)
        {
            foreach (ClassActionTasks item in lstButtonAction)
            {
                string value = GetValue(dictionary, item.ID.ToString());
                if (!string.IsNullOrEmpty(value))
                {
                    item.Color = value;
                }
            }
            return lstButtonAction;
        }
        /// <summary>
        /// Format Dynamics the control data value.
        /// </summary>
        /// <returns>The control data value.</returns>
        /// <param name="controlType">Loại dynamic control (text, datetime, user ...).</param>
        /// <param name="controlValue">Giá trị gốc của control .</param>
        public static string FormatDynamicControlDataValue(int controlType, string controlValue)
        {
            string value;
            if (string.IsNullOrEmpty(controlValue)) return controlValue;

            switch (controlType)
            {
                case (int)ControlType.FieldDatetime:
                    value = DateTime.Parse(controlValue).ToString(CmmVariable.M_WorkDateFormatVN);
                    break;
                case (int)ControlType.FieldDate:
                    value = DateTime.Parse(controlValue).ToString(CmmVariable.M_WorkDateFormatDayVN);
                    break;
                case (int)ControlType.FieldTime:
                    value = DateTime.Parse(controlValue).ToString(CmmVariable.M_WorkDateFormatTimeVN);
                    break;
                case (int)ControlType.FieldComboboxDrop:
                case (int)ControlType.FieldMultichoice:
                case (int)ControlType.FieldUser:
                    if (controlValue.Contains(";#"))
                        value = controlValue.Split(new[] { ";#" }, StringSplitOptions.None)[1];
                    else
                        value = controlValue;
                    break;
                case (int)ControlType.FieldTextNum:
                    CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
                    CultureInfo culVn = CultureInfo.GetCultureInfo("vi-VN");
                    value = double.Parse(controlValue, cul).ToString("N0", culVn);
                    break;
                case (int)ControlType.FieldChoice:
                    value = controlValue;
                    break;
                case (int)ControlType.FieldBool:
                    if (controlValue == "true")
                        value = "Yes";
                    else if (controlValue == "false")
                        value = "No";
                    else
                        value = controlValue;
                    break;
                case (int)ControlType.FieldText:
                    {
                        if (controlValue.Equals("0"))
                            value = "";
                        else
                            value = controlValue;
                    }
                    break;
                default:
                    value = controlValue;
                    break;
            }
            return value;
        }
        public static void TrackLogAppCenter(string _cate, string _content)
        {
            Analytics.TrackEvent(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - " + _cate + " - " + _content);
        }
        public static void TrackErrorAppCenter(Exception ex, string key, string value)
        {
            key = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - " + key;
            var properties = new Dictionary<string, string>
            {
                { key, value }
            };
            Crashes.TrackError(ex, properties);
        }

        /// <summary>
        /// Xóa các thuộc tính không cần dùng trong chuỗi json
        /// </summary>
        /// <param name="json"></param>
        /// <param name="regexes"></param>
        /// <returns></returns>
        public static string RemoveSensitivePropertiesFromString(string json)
        {
            var regexe = new Regex("^.*UpdateFields.*$", RegexOptions.IgnoreCase);
            JToken token = JToken.Parse(json);
            RemoveSensitivePropertiesFromJToken(token, regexe);
            return token.ToString();
        }

        public static void RemoveSensitivePropertiesFromJToken(JToken token, Regex regex)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (JProperty prop in token.Children<JProperty>().ToList())
                {
                    bool removed = false;
                    if (regex.IsMatch(prop.Name))
                    {
                        prop.Remove();
                        removed = true;
                        break;
                    }
                    if (!removed)
                    {
                        RemoveSensitivePropertiesFromJToken(prop.Value, regex);
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (JToken child in token.Children())
                {
                    RemoveSensitivePropertiesFromJToken(child, regex);
                }
            }
        }

        public static string DisplayUnreadCount(int unreadCount)
        {
            if (unreadCount == 0)
                return string.Empty;
            else if (unreadCount > 99)
                return "99+";
            else
                return unreadCount.ToString();
        }

        /// <summary>
        /// Get Avatar sort name
        /// </summary>
        /// <param name="name">full name</param>
        /// <returns></returns>
        public static string GetAvatarName(string name)
        {
            string res = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(name) && name.Contains(' '))
                {
                    name = CmmFunction.RemoveSignVietnamese(name);
                    string[] lst_char = name.Split(' ');

                    var first = lst_char[0].Substring(0, 1).ToUpper(); ;
                    var second = lst_char.Last().Substring(0, 1).ToUpper();
                    res = first + second;
                }
                else
                    res = name.Substring(0, 1).ToUpper();

                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Table_Document_CellCustom - ShowAvatar - Err:" + ex.Message + ex.StackTrace);
                return res;
            }

        }

        /// <summary>
        /// Lấy giá trị từ Settings theo KEY
        /// </summary>
        /// <param name="key">tên giá trị cần lấy</param>
        /// <returns></returns>
        public static string GetAppSetting(string key)
        {
            string retValue = "";
            SQLiteConnection con = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath);
            try
            {
                string sqlSel = "SELECT VALUE FROM BeanSettings WHERE KEY = ?";
                List<BeanSettings> lstObjChk = con.Query<BeanSettings>(sqlSel, key);
                if (lstObjChk != null && lstObjChk.Count > 0)
                    retValue = lstObjChk[0].VALUE;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmFunction - GetAppSetting - ERROR: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return retValue;
        }

        /// <summary>
        /// Lấy danh sách Action Task văn bản đến theo ActionPermission
        /// </summary>
        /// <param name="ActionPermission">Id của Action</param>
        /// <returns></returns>
        public static List<ClassActionTasks> GetActionTaskVBDen(int ActionPermission)
        {
            List<ClassActionTasks> actions = new List<ClassActionTasks>();
            try
            {
                var json_Action = "[{\"ID\":2,\"Value\":\"Update\",\"Title\":\"Lưu\",\"TitleEN\":\"Update\",\"Index\":3}," +
                    "{\"ID\":4,\"Value\":\"Delete\",\"Title\":\"Xóa\",\"TitleEN\":\"Delete\",\"Index\":0}," +
                    "{\"ID\":8,\"Value\":\"CreateSubTask\",\"Title\":\"Phân công\",\"TitleEN\":\"CreateSubTask\",\"Index\":1}," +
                    "{\"ID\":16,\"Value\":\"Complete\",\"Title\":\"Kết thúc\",\"TitleEN\":\"Complete\",\"Index\":2}," +
                    "{\"ID\":128,\"Value\":\"Confirm\",\"Title\":\"Xác nhận\",\"TitleEN\":\"Confirm\",\"Index\":0}," +
                    "{\"ID\":256,\"Value\":\"RequestAdjustment\",\"Title\":\"Yêu cầu hiệu chỉnh\",\"TitleEN\":\"RequestAdjustment\",\"Index\":0}," +
                    "{\"ID\":512,\"Value\":\"Comments\",\"Title\":\"Trao đổi lại\",\"TitleEN\":\"Comments\",\"Index\":4}]";
                actions = CmmFunction.GetLstMyAction(JsonConvert.DeserializeObject<List<ClassActionTasks>>(json_Action), ActionPermission);
                return actions;
            }
            catch (Exception ex)
            {
                actions = new List<ClassActionTasks>();
                Console.WriteLine("CmmFunction - GetActionTaskVBDen - ERROR: " + ex.ToString());
            }
            return actions;
        }

        /// <summary>
        /// Giải mã chuỗi string thành base 64
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Giải mã chuỗi base 64 thành string
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public struct DynamicColorAvatar
        {
            public const string A = "#4D004D";
            public const string B = "#0000B3";
            public const string C = "#CC3300";
            public const string D = "#CC6600";
            public const string E = "#B16E4B";
            public const string F = "#002DB3";
            public const string G = "#248F24";
            public const string H = "#CC0000";
            public const string I = "#990099";
            public const string J = "#9900CC";
            public const string K = "#330099";
            public const string L = "#B30000";
            public const string M = "#007399";
            public const string N = "#AF6E4E";
            public const string O = "#7A00CC";
            public const string P = "#0000FF";
            public const string Q = "#196619";
            public const string R = "#38908F";
            public const string S = "#003399";
            public const string T = "#662200";
            public const string U = "#5900B3";
            public const string V = "#196666";
            public const string W = "#005C99";
            public const string X = "#00994D";
            public const string Y = "#009900";
            public const string Z = "#009973";
        }

        public static class FileSizeFormatter
        {
            // Load all suffixes in an array
            static readonly string[] suffixes =
            { "Bytes", "KB", "MB", "GB", "TB", "PB" };
            public static string FormatSize(Int64 bytes)
            {
                int counter = 0;
                decimal number = (decimal)bytes;
                while (Math.Round(number / 1024) >= 1)
                {
                    number /= 1024;
                    counter++;
                }
                return string.Format("{0:n1} {1}", number, suffixes[counter]);
            }
        }
    }
}