using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Petrolimex.Bean;
using Petrolimex.Class;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Petrolimex.DataProvider
{
    public class ProviderDoc : ProviderBase
    {
        /// <summary>
        /// Submit Action VanBanDi/HoSoTaiLieu
        /// </summary>
        /// <param name="Action">ID của Action</param>
        /// <param name="_beanCodeItem"> Bean dữ liệu truyền vào</param>
        /// <param name="lstExtent"></param>
        /// <returns></returns>
        public bool ActionHoSoTaiLieu(int Action, BeanCodeItem _beanCodeItem, List<KeyValuePair<string, string>> lstExtent = null)
        {
            bool result = false;
            string jsonFilter = string.Empty;
            string ApiServerUrl = string.Empty;
            try
            {
                dynamic obj = new JObject();
                obj.ID = _beanCodeItem.ID;
                obj.ModuleId = _beanCodeItem.ModuleId;

                switch (Action)
                {
                    case (int)WorkflowAction.Action.Reject:
                    case (int)WorkflowAction.Action.Return:
                    case (int)WorkflowAction.Action.Approve:
                    case (int)WorkflowAction.Action.Next:
                        {
                            obj.CommentValue = _beanCodeItem.CommentValue;
                            //obj.CCForm = _beanCodeItem.CCForm;
                            obj.UserCC = _beanCodeItem.UserCC;
                            obj.Users = _beanCodeItem.UserCC;
                        }
                        break;
                    case (int)WorkflowAction.Action.RequestInformation:
                        {
                            obj.CommentValue = _beanCodeItem.CommentValue;
                            obj.ChooseUserValue = _beanCodeItem.ChooseUserValue;
                        }
                        break;
                    case (int)WorkflowAction.Action.Forward:
                        {
                            obj.CommentValue = _beanCodeItem.CommentValue;
                            obj.ChooseUserValue = _beanCodeItem.ChooseUserValue;
                        }
                        break;
                    case (int)WorkflowAction.Action.Recall:
                        {
                            obj.CommentValue = _beanCodeItem.CommentValue;
                        }
                        break;
                    default:
                        break;
                }

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "submit"));
                lstGet.Add(new KeyValuePair<string, string>("action", Action.ToString()));

                jsonFilter = JsonConvert.SerializeObject(obj);
                lstPost.Add(new KeyValuePair<string, string>("data", jsonFilter));

                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;

                PAR par = new PAR(lstGet, lstPost, null);

                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiDocMan.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return false;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    result = true;
            }
            catch (Exception ex)
            {

                result = false;
                Console.WriteLine("Error - ProviderDoc - ActionHoSoTaiLieu - Err:" + ex.Message + ex.StackTrace);

            }
            return result;
        }

        /// <summary>
        /// Lấy tất cả danh sách Tổ chức thực hiện bao gồm: Phòng/ban,Đơn vị, Task con
        /// </summary>
        /// <param name="_beanVanBanDen">Truyền từ chi tiết vào</param>
        /// <returns></returns>
        public string GetAllToChucThucHien(BeanVanBanDen _beanVanBanDen)
        {
            string retValue = null;
            try
            {
                dynamic obj = new JObject();
                obj.ID = _beanVanBanDen.ID;
                obj.ModuleId = _beanVanBanDen.ModuleId;

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getTasks"));
                lstGet.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(obj)));
                lstGet.Add(new KeyValuePair<string, string>("rid", _beanVanBanDen.ID.ToString()));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;
                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetAllToChucThucHien_VanBanDen - Err:" + ex);
            }
            return retValue;
        }

        public string GetAllTCTH_VBBH(BeanVanBanBanHanh _VBBHItem)
        {
            string retValue = null;
            try
            {
                dynamic obj = new JObject();
                obj.ID = _VBBHItem.ID;
                obj.ModuleId = _VBBHItem.ModuleId;

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getTasks"));
                lstGet.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(obj)));
                lstGet.Add(new KeyValuePair<string, string>("rid", _VBBHItem.ID.ToString()));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanBanHanhMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;
                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetAllToChucThucHien_VanBanDen - Err:" + ex);
            }
            return retValue;
        }

        /// <summary>
        /// Submit Action VanBanDen
        /// </summary>
        /// <param name="Action">ID của Action (Recall,Submit,...) </param>
        /// <param name="_beanVanbanden">Dữ liệu truyền vào</param>
        /// <returns></returns>
        public bool ActionVanBanDen(int Action, BeanVanBanDen _beanVanbanden)
        {
            bool result = false;
            string jsonFilter = string.Empty;
            string ApiServerUrl = string.Empty;
            try
            {
                dynamic obj = new JObject();
                obj.ID = _beanVanbanden.ID;
                obj.ModuleId = _beanVanbanden.ModuleId;

                switch (Action)
                {
                    case (int)VanBanDenAction.Action.CommentBOD:
                    case (int)VanBanDenAction.Action.Recall:
                    case (int)VanBanDenAction.Action.Completed:
                    case (int)VanBanDenAction.Action.Comment:
                    case (int)VanBanDenAction.Action.Forward:
                    case (int)VanBanDenAction.Action.RecallBOD:
                    case (int)VanBanDenAction.Action.FowardArchives:
                        {
                            obj.YKien = _beanVanbanden.YKien;
                        }
                        break;
                    case (int)VanBanDenAction.Action.Assignment:
                        {
                            obj.NguoiChuyen = _beanVanbanden.NguoiChuyen;
                            obj.DueDate = _beanVanbanden.DueDate;
                            obj.YKien = _beanVanbanden.YKien;
                            obj.chkPhanCong = _beanVanbanden.chkPhanCong;
                            obj.chkCVP_TPTH = _beanVanbanden.chkCVP_TPTH;
                            obj.chkLanhDao = _beanVanbanden.chkLanhDao;
                            obj.NguoiNhan = _beanVanbanden.NguoiNhan;
                            obj.ListAssignmentUsers = _beanVanbanden.ListAssignmentUsers;
                            obj.ListAssignmentDept = _beanVanbanden.ListAssignmentDept;
                            obj.UserCC = _beanVanbanden.UserCC;
                        }
                        break;
                    case (int)VanBanDenAction.Action.SubmitBOD:
                        {
                            if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite))
                            {
                                obj.YKien = _beanVanbanden.YKien;
                                obj.chkLanhDaoValue = _beanVanbanden.chkLanhDaoValue;
                                obj.UserCC = _beanVanbanden.UserCC;
                            }
                            else
                            {
                                obj.YKien = _beanVanbanden.YKien;
                                obj.chkLanhDao = _beanVanbanden.chkLanhDao;
                                obj.chkCVP_TPTH = _beanVanbanden.chkCVP_TPTH;
                                obj.chkPhanCong = _beanVanbanden.chkPhanCong;
                                obj.chkLanhDaoValue = _beanVanbanden.chkLanhDaoValue;
                                obj.chkCVP_TPTHValue = _beanVanbanden.chkCVP_TPTHValue;
                                obj.UserCC = _beanVanbanden.UserCC;
                                obj.UuTien = _beanVanbanden.UuTien;
                            }
                        }
                        break;
                    case (int)VanBanDenAction.Action.ReAssignment:
                        {
                            obj.NguoiDanhGiaUserValue = _beanVanbanden.NguoiDanhGiaUserValue;
                            obj.YKien = _beanVanbanden.YKien;
                            obj.ListIDDelete = _beanVanbanden.ListIDDelete;
                            obj.ListAssignmentUsers = _beanVanbanden.ListAssignmentUsers;
                        }
                        break;
                    case (int)VanBanDenAction.Action.RecallOrForward:
                        {
                            obj.YKien = _beanVanbanden.YKien;
                            obj.actionType = _beanVanbanden.actionType;
                            obj.ListAssignmentDept = _beanVanbanden.ListAssignmentDept;
                            obj.ListIDDelete = _beanVanbanden.ListIDDelete;
                        }
                        break;
                    default:
                        break;
                }

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "submit"));
                lstGet.Add(new KeyValuePair<string, string>("action", Action.ToString()));

                jsonFilter = JsonConvert.SerializeObject(obj);
                lstPost.Add(new KeyValuePair<string, string>("data", jsonFilter));

                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;

                PAR par = new PAR(lstGet, lstPost, null);

                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return false;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    result = true;
            }
            catch (Exception ex)
            {

                result = false;
                Console.WriteLine("Error - ProviderDoc - ActionVanBanDen - Err:" + ex.ToString());

            }
            return result;
        }

        public bool ActionVBBanHanh(int Action, BeanVanBanBanHanh _beanVBBH)
        {
            bool result = false;
            string jsonFilter = string.Empty;
            string ApiServerUrl = string.Empty;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "submit"));
                lstGet.Add(new KeyValuePair<string, string>("action", Action.ToString()));

                jsonFilter = JsonConvert.SerializeObject(_beanVBBH);
                lstPost.Add(new KeyValuePair<string, string>("data", jsonFilter));

                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;

                PAR par = new PAR(lstGet, lstPost, null);

                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanBanHanhMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return false;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    result = true;
            }
            catch (Exception ex)
            {

                result = false;
                Console.WriteLine("Error - ProviderDoc - ActionVBBanHanh - Err:" + ex.ToString());

            }
            return result;
        }

        /// <summary>
        /// Submit Action TaskVanBanDen
        /// </summary>
        /// <param name="Action">ID của Action Task Văn bản đến (Recall,Submit,...) </param>
        /// <param name="_beanTaskVanbanden">Dữ liệu truyền vào</param>
        /// <returns></returns>
        public bool ActionTaskVanBanDen(int Action, BeanTaskDocument _beanTaskVanbanden)
        {
            bool result = false;
            try
            {
                dynamic obj = new JObject();
                obj.ID = _beanTaskVanbanden.ID;
                switch (_beanTaskVanbanden.Role)
                {
                    case (int)TaskVanBanDenAction.Role.Assignor:
                    case (int)TaskVanBanDenAction.Role.AssignorPBan:
                        {
                            switch (Action)
                            {
                                case (int)TaskVanBanDenAction.Action.Feedback:
                                    {
                                        obj.YKienChiDao = _beanTaskVanbanden.YKienChiDao;
                                        break;
                                    }
                                case (int)TaskVanBanDenAction.Action.Save:
                                    {
                                        obj.YKienChiDao = _beanTaskVanbanden.YKienChiDao;
                                        if (_beanTaskVanbanden.DueDate.HasValue)
                                            obj.DueDate = _beanTaskVanbanden.DueDate.Value.ToString("yyyy/MM/dd");
                                        obj.AssignedToUserValue = _beanTaskVanbanden.AssignedToUserValue;
                                        break;
                                    }
                                case (int)TaskVanBanDenAction.Action.Assignment:
                                    {
                                        obj.NguoiChuyen = _beanTaskVanbanden.NguoiChuyen;
                                        obj.ListAssignmentUsers = _beanTaskVanbanden.ListAssignmentUsers;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }
                    default: // Người Nhận Task
                        {
                            switch (Action)
                            {
                                case (int)TaskVanBanDenAction.Action.Completed:
                                case (int)TaskVanBanDenAction.Action.Feedback:
                                    {
                                        obj.YKienCuaNguoiGiaiQuyet = _beanTaskVanbanden.YKienCuaNguoiGiaiQuyet;
                                        break;
                                    }
                                case (int)TaskVanBanDenAction.Action.Save:
                                    {
                                        obj.YKienCuaNguoiGiaiQuyet = _beanTaskVanbanden.YKienCuaNguoiGiaiQuyet;
                                        obj.TrangThai = _beanTaskVanbanden.TrangThai;
                                        obj.Percent = _beanTaskVanbanden.Percent;
                                        break;
                                    }
                                case (int)TaskVanBanDenAction.Action.Assignment:
                                    {
                                        obj.NguoiChuyen = _beanTaskVanbanden.NguoiChuyen;
                                        obj.YKienCuaNguoiGiaiQuyet = _beanTaskVanbanden.YKienCuaNguoiGiaiQuyet;
                                        obj.ListAssignmentUsers = _beanTaskVanbanden.ListAssignmentUsers;
                                        obj.Percent = _beanTaskVanbanden.Percent;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                }

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstPost.Add(new KeyValuePair<string, string>("func", "taskSubmit"));
                lstPost.Add(new KeyValuePair<string, string>("action", Action.ToString()));
                lstPost.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(obj)));

                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                PAR par = new PAR(lstGet, lstPost, null);
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return false;
                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    result = true;
            }
            catch (Exception ex)
            {
#if DEBUG
                result = false;
                Console.WriteLine("Error - ProviderDoc - ActionTaskVanBanDen - Err:" + ex.ToString());
#endif
            }
            return result;
        }

        public bool ActionTaskVanBanBanHanh(int Action, BeanTaskDocument _beanTaskVanbanden)
        {
            bool result = false;
            try
            {
                dynamic obj = new JObject();
                obj.ID = _beanTaskVanbanden.ID;
                switch (_beanTaskVanbanden.Role)
                {
                    case (int)TaskVanBanDenAction.Role.Assignor:
                    case (int)TaskVanBanDenAction.Role.AssignorPBan:
                        {
                            switch (Action)
                            {
                                case (int)TaskVanBanDenAction.Action.Feedback:
                                    {
                                        obj.YKienChiDao = _beanTaskVanbanden.YKienChiDao;
                                        obj.TrangThai = _beanTaskVanbanden.TrangThai;
                                        break;
                                    }
                                case (int)TaskVanBanDenAction.Action.Save:
                                    {
                                        obj.YKienChiDao = _beanTaskVanbanden.YKienChiDao;
                                        if (_beanTaskVanbanden.DueDate.HasValue)
                                            obj.DueDate = _beanTaskVanbanden.DueDate.Value.ToString("yyyy/MM/dd");
                                        obj.TrangThai = _beanTaskVanbanden.TrangThai;
                                        obj.AssignedToUserValue = _beanTaskVanbanden.AssignedToUserValue;
                                        break;
                                    }
                                case (int)TaskVanBanDenAction.Action.Assignment:
                                    {
                                        obj.NguoiChuyen = _beanTaskVanbanden.NguoiChuyen;
                                        obj.ListAssignmentUsers = _beanTaskVanbanden.ListAssignmentUsers;
                                        obj.YKienCuaNguoiGiaiQuyet = string.IsNullOrEmpty(_beanTaskVanbanden.YKienCuaNguoiGiaiQuyet) ? "" : _beanTaskVanbanden.YKienCuaNguoiGiaiQuyet;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }
                    default: // Người Nhận Task
                        {
                            switch (Action)
                            {
                                case (int)TaskVanBanDenAction.Action.Completed:
                                case (int)TaskVanBanDenAction.Action.Feedback:
                                    {
                                        obj.TrangThai = _beanTaskVanbanden.TrangThai;
                                        obj.YKienCuaNguoiGiaiQuyet = string.IsNullOrEmpty(_beanTaskVanbanden.YKienCuaNguoiGiaiQuyet) ? "" : _beanTaskVanbanden.YKienCuaNguoiGiaiQuyet;
                                        break;
                                    }
                                case (int)TaskVanBanDenAction.Action.Save:
                                    {
                                        obj.YKienCuaNguoiGiaiQuyet = string.IsNullOrEmpty(_beanTaskVanbanden.YKienCuaNguoiGiaiQuyet) ? "" : _beanTaskVanbanden.YKienCuaNguoiGiaiQuyet;
                                        obj.TrangThai = _beanTaskVanbanden.TrangThai;
                                        obj.Percent = _beanTaskVanbanden.Percent;
                                        break;
                                    }
                                case (int)TaskVanBanDenAction.Action.Assignment:
                                    {
                                        obj.NguoiChuyen = _beanTaskVanbanden.NguoiChuyen;
                                        obj.YKienCuaNguoiGiaiQuyet = string.IsNullOrEmpty(_beanTaskVanbanden.YKienCuaNguoiGiaiQuyet) ? "" : _beanTaskVanbanden.YKienCuaNguoiGiaiQuyet;
                                        obj.ListAssignmentUsers = _beanTaskVanbanden.ListAssignmentUsers;
                                        obj.Percent = _beanTaskVanbanden.Percent;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                }

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstPost.Add(new KeyValuePair<string, string>("func", "taskSubmit"));
                lstPost.Add(new KeyValuePair<string, string>("action", Action.ToString()));
                lstPost.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(obj)));

                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                PAR par = new PAR(lstGet, lstPost, null);
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanBanHanhMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return false;
                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    result = true;
            }
            catch (Exception ex)
            {
#if DEBUG
                result = false;
                Console.WriteLine("Error - ProviderDoc - ActionTaskVanBanDen - Err:" + ex.ToString());
#endif
            }
            return result;
        }

        /// <summary>
        /// Lấy chi tiết văn bản đến bằng ID
        /// </summary>
        /// <param name="ID">ID của văn bản đến</param>
        /// <returns></returns>
        public BeanVanBanDen GetVanBanDenByID(long ID,bool isSave = true)
        {
            BeanVanBanDen retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();

                lstGet.Add(new KeyValuePair<string, string>("func", "get"));
                lstGet.Add(new KeyValuePair<string, string>("rid", ID.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("actionPer", "1"));
                lstGet.Add(new KeyValuePair<string, string>("cmt", "1"));
                lstGet.Add(new KeyValuePair<string, string>("modified", DateTime.Now.ToString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))));

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;

                string ApiServerUrl = "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx";

                JObject retData = GetJsonDataFromAPI(combieUrl + ApiServerUrl, ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
               
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToObject<BeanVanBanDen>();
                    if (retValue.ID == 0)
                    {
                        string Params = "\n" + "func:get" + "\n" + "rid:" +ID.ToString() + "\n" + "actionPer:1" + "\n" + "modified:" + DateTime.Now.ToString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        retValue.UrlApiErr = combieUrl + ApiServerUrl + Params;
                    }
                    if (isSave)
                    {
                        List<string> lstRemoveAccentFrom = BeanBase.getLstProName(retValue.GetType(), typeof(RemoveAccentFromAttribute));
                        List<string> lstRemoveAccentFromMultiColumn = BeanBase.getLstProName(retValue.GetType(), typeof(RemoveAccentFromMultiColumnAttribute));

                        foreach (string removeAccentFieldName in lstRemoveAccentFrom)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(retValue, removeAccentFieldName);

                            RemoveAccentFromAttribute cmmAttr = (RemoveAccentFromAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                object removeAccentValue = CmmFunction.GetPropertyValueByName(retValue, cmmAttr.ColFrom);
                                if (removeAccentValue != null)
                                {
                                    if (removeAccentValue.ToString().Contains(";#"))
                                    {
                                        //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                        removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                    }

                                    removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "").ToLowerInvariant();
                                    CmmFunction.SetPropertyValue(retValue, propInfo, removeAccentValue);
                                }
                            }
                        }

                        // Remove dấu lấy từ nhieu thuộc tính khác
                        string temp_khongdau = "";
                        foreach (string removeAccentFieldName in lstRemoveAccentFromMultiColumn)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(retValue, removeAccentFieldName);

                            RemoveAccentFromMultiColumnAttribute cmmAttr = (RemoveAccentFromMultiColumnAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromMultiColumnAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                if (cmmAttr.ColFrom.Count() > 0)
                                {
                                    foreach (var res in cmmAttr.ColFrom)
                                    {
                                        object removeAccentValue = CmmFunction.GetPropertyValueByName(retValue, res);
                                        if (removeAccentValue != null)
                                        {
                                            if (removeAccentValue.ToString().Contains(";#"))
                                            {
                                                //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                                removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                            }

                                            removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "");
                                            temp_khongdau += removeAccentValue + "^";
                                            //CmmFunction.SetPropertyValue(item, propInfo, temp_khongdau);
                                        }
                                    }
                                    CmmFunction.SetPropertyValue(retValue, propInfo, temp_khongdau.ToLowerInvariant());
                                }
                            }
                        }

                        // Cập nhật data xuống db
                        SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, false);
                        string query = string.Format("SELECT ID FROM BeanVanBanDen WHERE ID = ?");
                        List<BeanVanBanDen> lst_vbdenTCT = conn.Query<BeanVanBanDen>(query, ID);

                        if (lst_vbdenTCT != null && lst_vbdenTCT.Count > 0)
                            conn.Update(retValue);
                        else
                            conn.Insert(retValue);
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error - ProviderDoc - GetVanBanDenByID - Err:" + ex.ToString());

            }
            return retValue;
        }

        /// <summary>
        /// Lấy chi tiết của Văn bản ban hành
        /// </summary>
        /// <param name="ID">ID của văn bản ban hành</param>
        /// <returns></returns>
        public BeanVanBanBanHanh GetVanBanBanHanhByID(float ID, bool IsSave = true)
        {
            BeanVanBanBanHanh retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();

                lstGet.Add(new KeyValuePair<string, string>("func", "get"));
                lstGet.Add(new KeyValuePair<string, string>("rid", ID.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("actionPer", "1"));
                lstGet.Add(new KeyValuePair<string, string>("task", "1"));

                PAR par = new PAR(lstGet, lstPost, null);

                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;

                string ApiServerUrl = "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanBanHanhMobile.ashx";

                JObject retData = GetJsonDataFromAPI(combieUrl + ApiServerUrl, ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToObject<BeanVanBanBanHanh>();

                    if (IsSave)
                    {
                        List<string> lstRemoveAccentFrom = BeanBase.getLstProName(retValue.GetType(), typeof(RemoveAccentFromAttribute));
                        List<string> lstRemoveAccentFromMultiColumn = BeanBase.getLstProName(retValue.GetType(), typeof(RemoveAccentFromMultiColumnAttribute));

                        foreach (string removeAccentFieldName in lstRemoveAccentFrom)
                        {
                            System.Reflection.PropertyInfo propInfo = CmmFunction.GetProperty(retValue, removeAccentFieldName);

                            RemoveAccentFromAttribute cmmAttr = (RemoveAccentFromAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                object removeAccentValue = CmmFunction.GetPropertyValueByName(retValue, cmmAttr.ColFrom);
                                if (removeAccentValue != null)
                                {
                                    if (removeAccentValue.ToString().Contains(";#"))
                                    {
                                        //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                        removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                    }

                                    removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "").ToLowerInvariant();
                                    CmmFunction.SetPropertyValue(retValue, propInfo, removeAccentValue);
                                }
                            }
                        }

                        // Remove dấu lấy từ nhieu thuộc tính khác
                        string temp_khongdau = "";
                        foreach (string removeAccentFieldName in lstRemoveAccentFromMultiColumn)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(retValue, removeAccentFieldName);

                            RemoveAccentFromMultiColumnAttribute cmmAttr = (RemoveAccentFromMultiColumnAttribute)propInfo.GetCustomAttributes(typeof(RemoveAccentFromMultiColumnAttribute), true).FirstOrDefault();

                            if (cmmAttr != null)
                            {
                                if (cmmAttr.ColFrom.Count() > 0)
                                {
                                    foreach (var res in cmmAttr.ColFrom)
                                    {
                                        object removeAccentValue = CmmFunction.GetPropertyValueByName(retValue, res);
                                        if (removeAccentValue != null)
                                        {
                                            if (removeAccentValue.ToString().Contains(";#"))
                                            {
                                                //removeAccentValue = removeAccentValue.ToString().Split(";#")[1];
                                                removeAccentValue = removeAccentValue.ToString().Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                            }

                                            removeAccentValue = CmmFunction.RemoveSignVietnamese(removeAccentValue + "");
                                            temp_khongdau += removeAccentValue + "^";
                                            //CmmFunction.SetPropertyValue(item, propInfo, temp_khongdau);
                                        }
                                    }
                                    CmmFunction.SetPropertyValue(retValue, propInfo, temp_khongdau.ToLowerInvariant());
                                }
                            }
                        }

                        //cap nhat data xuong database
                        SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, false);
                        string query = string.Format("SELECT * FROM BeanVanBanBanHanh WHERE ID = ?");
                        List<BeanVanBanBanHanh> lst_vbdenTCT = conn.Query<BeanVanBanBanHanh>(query, ID);

                        if (lst_vbdenTCT != null && lst_vbdenTCT.Count > 0)
                        {
                            BeanVanBanBanHanh _item = lst_vbdenTCT[0];
                            if (string.IsNullOrEmpty(retValue.AuthorID) && !string.IsNullOrEmpty(_item.AuthorID))
                                retValue.AuthorID = _item.AuthorID;

                            conn.Update(retValue);
                        }
                        else
                            conn.Insert(retValue);
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error - ProviderDoc - GetVanBanBanHanhByID - Err:" + ex.ToString());

            }
            return retValue;
        }

        /// <summary>
        /// Lấy thông tin luân chuyển của Văn bản ban hành
        /// </summary>
        /// <param name="_beanWorkflow">BeanVanBanBanHanh</param>
        /// <returns></returns>
        public List<BeanQuaTrinhLuanChuyenWorkflow> GetTrinhLuanChuyenVBBH(BeanVanBanBanHanh _beanWorkflow)
        {
            if (_beanWorkflow == null)
                return null;

            List<BeanQuaTrinhLuanChuyenWorkflow> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getWorkflowHistory"));
                lstGet.Add(new KeyValuePair<string, string>("rid", _beanWorkflow.ID.ToString()));

                PAR par = new PAR(lstGet, lstPost, null);

                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;

                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanBanHanhMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<List<BeanQuaTrinhLuanChuyenWorkflow>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetTrinhLuanChuyenVBBH - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy File của Văn bản ban hành
        /// </summary>
        /// <param name="WorkflowItem"></param>
        /// <returns></returns>
        public List<BeanAttachFile> GetAttFileByVBBH(BeanVanBanBanHanh WorkflowItem)
        {
            List<BeanAttachFile> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getAttachFiles"));
                lstGet.Add(new KeyValuePair<string, string>("rid", WorkflowItem.ID.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("type", "2"));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanBanHanhMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<List<BeanAttachFile>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetAttFileByVBBH - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy File của văn bản đến
        /// </summary>
        /// <param name="WorkflowItem">BeanVanBanDen</param>
        /// <returns></returns>
        public List<BeanAttachFile> GetAttFileVanBanDen(BeanVanBanDen WorkflowItem)
        {
            List<BeanAttachFile> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getAttachFiles"));
                lstGet.Add(new KeyValuePair<string, string>("rid", WorkflowItem.ID.ToString()));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToObject<List<BeanAttachFile>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc- GetAttFileVanBanDen - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy danh sách người xem của văn bản ban hành
        /// </summary>
        /// <param name="WorkflowItem">BeanVanBanBanHanh</param>
        /// <returns></returns>
        public List<BeanQuaTrinhLuanChuyenWorkflow> GetViewersVBBH(BeanVanBanBanHanh WorkflowItem)
        {
            if (WorkflowItem == null)
                return null;

            List<BeanQuaTrinhLuanChuyenWorkflow> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getViewer"));
                lstGet.Add(new KeyValuePair<string, string>("rid", WorkflowItem.ID.ToString()));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanBanHanhMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<List<BeanQuaTrinhLuanChuyenWorkflow>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetViewersVBBH - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy quá trình luân chuyển tại đơn vị khác của văn bản ban hành
        /// </summary>
        /// <param name="WorkflowItem">BeanVanBanBanHanh</param>
        /// <returns></returns>
        public List<BeanQuaTrinhLuanChuyenWorkflow> GetQuaTrinhLuanChuyenTaiDonViKhac(BeanVanBanBanHanh WorkflowItem)
        {
            if (WorkflowItem == null)
                return null;

            List<BeanQuaTrinhLuanChuyenWorkflow> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getWorkflowHistoryOtherDepartment"));
                lstGet.Add(new KeyValuePair<string, string>("rid", WorkflowItem.ID.ToString()));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanBanHanhMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<List<BeanQuaTrinhLuanChuyenWorkflow>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetQuaTrinhLuanChuyenTaiDonViKhac - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy chi tiết quá trình luân chuyển tại đơn vị khác của văn bản ban hành
        /// </summary>
        /// <param name="WorkflowItem">BeanQuaTrinhLuanChuyenWorkflow</param>
        /// <returns></returns>
        public List<BeanQuaTrinhLuanChuyenWorkflow> GetDetailQuaTrinhLuanChuyenTaiDonViKhac(BeanQuaTrinhLuanChuyenWorkflow data)
        {
            List<BeanQuaTrinhLuanChuyenWorkflow> retValue = null;
            try
            {
                dynamic obj = new JObject();
                obj.VBId = data.VBId;
                obj.LookupId = data.LookupId;

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "ExpandThongTinLuanChuyen"));
                lstPost.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(obj)));

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanBanHanhMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<List<BeanQuaTrinhLuanChuyenWorkflow>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetDetailQuaTrinhLuanChuyenTaiDonViKhac - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Update trạng thái đã đọc của văn bản
        /// </summary>
        /// <param name="workflowID">ID văn bản</param>
        /// <param name="docmentSite">Site văn bản</param>
        /// <param name="docmentType">Loại văn bản</param>
        /// <returns> true : Đã đọc,  false : lỗi</returns>
        public bool IsReadedDocument(string workflowID, string docmentSite, string docmentType)
        {
            bool retValue = false;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "read"));
                lstPost.Add(new KeyValuePair<string, string>("rid", workflowID));
                lstPost.Add(new KeyValuePair<string, string>("listname", docmentType));

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/" + docmentSite + "/_layouts/15/VuThao.Petrolimex.API/ApiPublish.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);

                if (retData == null) return false;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - IsReadedDocument - Err:" + ex);
                return false;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy Danh sách thông tin ý kiến lãnh đạo của văn bản đến
        /// </summary>
        /// <param name="WorkflowItem">BeanVanBanDen</param>
        /// <returns></returns>
        public List<BeanVanBanDen> GetDanhSachThongTinYKienLanhDao_VanBanDen(BeanVanBanDen WorkflowItem)
        {
            List<BeanVanBanDen> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getIdeaOfBOD"));
                lstGet.Add(new KeyValuePair<string, string>("rid", WorkflowItem.ID.ToString()));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<List<BeanVanBanDen>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetDanhSachThongTinYKienLanhDao_VanBanDen - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy Danh sách thông tin ý kiến lãnh đạo của văn bản đi
        /// </summary>
        /// <param name="WorkflowItem">BeanVanBanDi</param>
        /// <returns></returns>
        public List<BeanVanBanDen> GetDanhSachThongTinYKienLanhDao_VanBanDi(BeanCodeItem WorkflowItem)
        {
            List<BeanVanBanDen> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getIdeaOfBOD"));
                lstGet.Add(new KeyValuePair<string, string>("rid", WorkflowItem.ID.ToString()));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiDocMan.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;
                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<List<BeanVanBanDen>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetDanhSachThongTinYKienLanhDao_VanBanDi - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy thông tin luân chuyển của văn bản đến
        /// </summary>
        /// <param name="_beanWorkflow">BeanVanBanDen</param>
        /// <returns></returns>
        public List<BeanQuaTrinhLuanChuyenWorkflow> GetTrinhLuanChuyen_VanBanDen(BeanVanBanDen _beanWorkflow)
        {
            List<BeanQuaTrinhLuanChuyenWorkflow> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getWorkflowHistory"));
                lstGet.Add(new KeyValuePair<string, string>("rid", _beanWorkflow.ID.ToString()));

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;

                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<List<BeanQuaTrinhLuanChuyenWorkflow>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetTrinhLuanChuyen_VanBanDen - Err:" + ex.Message + ex.StackTrace);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy danh sách người xem văn bản đến
        /// </summary>
        /// <param name="WorkflowItem">BeanVanBanDen</param>
        /// <returns></returns>
        public List<BeanQuaTrinhLuanChuyenWorkflow> GetViewers_VanBanDen(BeanVanBanDen WorkflowItem)
        {
            List<BeanQuaTrinhLuanChuyenWorkflow> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getViewer"));
                lstGet.Add(new KeyValuePair<string, string>("rid", WorkflowItem.ID.ToString()));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToObject<List<BeanQuaTrinhLuanChuyenWorkflow>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetViewers_VanBanDen - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy quá trình luân chuyển tại đơn vị khác của văn bản đến
        /// </summary>
        /// <param name="WorkflowItem">BeanVanBanDen</param>
        /// <returns></returns>
        public List<BeanQuaTrinhLuanChuyenWorkflow> GetQuaTrinhLuanChuyenTaiDonViKhac_VanBanDen(BeanVanBanDen WorkflowItem)
        {
            List<BeanQuaTrinhLuanChuyenWorkflow> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getWorkflowHistoryOtherDepartment"));
                lstGet.Add(new KeyValuePair<string, string>("rid", WorkflowItem.ID.ToString()));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToObject<List<BeanQuaTrinhLuanChuyenWorkflow>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetQuaTrinhLuanChuyenTaiDonViKhac_VanBanDen - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy Danh sách Tổ chức thực hiện Phòng/Ban của văn bản đến
        /// </summary>
        /// <param name="WorkflowItem">BeanVanBanDen</param>
        /// <returns></returns>
        public List<BeanTaskDocument> GetToChucThucHien_VanBanDen(BeanVanBanDen WorkflowItem)
        {
            List<BeanTaskDocument> retValue = null;
            try
            {
                dynamic obj = new JObject();
                obj.ID = WorkflowItem.ID;
                obj.ModuleId = WorkflowItem.ModuleId;

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getTasks"));
                lstGet.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(obj)));
                lstGet.Add(new KeyValuePair<string, string>("rid", WorkflowItem.ID.ToString()));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"]["TaskJson"].ToObject<List<BeanTaskDocument>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetToChucThucHien_VanBanDen - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy Danh sách Tổ chức thực hiện đơn vị của văn bản đến
        /// </summary>
        /// <param name="WorkflowItem">BeanVanBanDen</param>
        /// <returns></returns>
        public List<BeanTaskDocument> GetToChucThucHien_DonVi_VanBanDen(BeanVanBanDen WorkflowItem)
        {
            List<BeanTaskDocument> retValue = null;
            try
            {
                dynamic obj = new JObject();
                obj.ID = WorkflowItem.ID;
                obj.ModuleId = WorkflowItem.ModuleId;

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getTasks"));
                lstGet.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(obj)));
                lstGet.Add(new KeyValuePair<string, string>("rid", WorkflowItem.ID.ToString()));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"]["NhiemVuXuLyJson"].ToObject<List<BeanTaskDocument>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetToChucThucHien_DonVi_VanBanDen - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy phân công xử lý của văn bản đến
        /// </summary>
        /// <param name="WorkflowItem">BeanVanBanDen</param>
        /// <returns></returns>
        public List<BeanTaskDocument> GetPhanCongXuLy_VanBanDen(BeanVanBanDen WorkflowItem)
        {
            List<BeanTaskDocument> retValue = null;
            try
            {
                dynamic obj = new JObject();
                obj.ID = WorkflowItem.ID;
                obj.ModuleId = WorkflowItem.ModuleId;

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getTasks"));
                lstGet.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(obj)));
                lstGet.Add(new KeyValuePair<string, string>("rid", WorkflowItem.ID.ToString()));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"]["TaskJson"].ToObject<List<BeanTaskDocument>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetPhanCongXuLy_VanBanDen - Err:" + ex);
                return null;
            }
            return retValue;
        }

        public List<BeanTaskDocument> GetPhanCongXuLy_VBBH(BeanVanBanBanHanh _VBBHItem)
        {
            List<BeanTaskDocument> retValue = null;
            try
            {
                dynamic obj = new JObject();
                obj.ID = _VBBHItem.ID;
                obj.ModuleId = _VBBHItem.ModuleId;

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getTasks"));
                lstGet.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(obj)));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanBanHanhMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"]["TaskJson"].ToObject<List<BeanTaskDocument>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetPhanCongXuLy_VanBanDen - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy chi tiết Task văn bản đến
        /// </summary>
        /// <param name="ID_TaskVanBanDen">ID Task văn bản đến</param>
        /// <returns></returns>
        public BeanTaskDocument GetDetailTaskVanBanDen(long ID)
        {
            BeanTaskDocument retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getTaskById"));
                lstGet.Add(new KeyValuePair<string, string>("rid", ID.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("modified", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                lstGet.Add(new KeyValuePair<string, string>("actionPer", "1"));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<BeanTaskDocument>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetDetailTaskVanBanDen - Err:" + ex);
                return null;
            }
            return retValue;
        }

        public BeanTaskDocument GetDetailTaskVanBanBanHanh(long ID)
        {
            BeanTaskDocument retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getTaskById"));
                lstGet.Add(new KeyValuePair<string, string>("rid", ID.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("modified", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                lstGet.Add(new KeyValuePair<string, string>("actionPer", "1"));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanBanHanhMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<BeanTaskDocument>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetDetailTaskVanBanBanHanh - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy chi tiết Văn bản đi
        /// </summary>
        /// <param name="ID">ID Văn bản đi</param>
        /// <returns></returns>
        public BeanCodeItem GetDetailVanBanDiByID(float ID, bool IsSave = true)
        {
            BeanCodeItem retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "get"));
                lstGet.Add(new KeyValuePair<string, string>("rid", ID.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("actionPer", "1"));
                lstGet.Add(new KeyValuePair<string, string>("modified", DateTime.Now.ToString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiDocMan.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToObject<BeanCodeItem>();

                    if (retValue.ID == 0)
                    {
                        string Params = "\n" + "func:get" + "\n" + "rid:" + ID.ToString() + "\n" + "actionPer:1" + "\n" + "modified:" + DateTime.Now.ToString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        retValue.UrlApiErr = combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiDocMan.ashx" + Params;
                    }
                    if (IsSave)
                    {
                        var conn = new SQLiteConnection(CmmVariable.M_Wpf_SQLitePath, false);
                        var _queryCodeItem = "SELECT * FROM BeanCodeItem WHERE ID = ? ";
                        var _codeItems = conn.Query<BeanCodeItem>(_queryCodeItem, retValue.ID.ToString());
                        if (_codeItems != null && _codeItems.Count > 0)
                        {
                            _codeItems[0] = retValue;
                            conn.Update(_codeItems[0]);
                        }
                        else
                            conn.Insert(retValue);
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetDetailVanBanDiByID - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy nhiệm vụ đã phân công của Task Văn bản đến
        /// </summary>
        /// <param name="_beanTask">Bean Task văn bản đến</param>
        /// <returns></returns>
        public List<BeanTaskDocument> GetNhiemVuDaPhanCong_TaskVanBanDen(BeanTaskDocument _beanTask)
        {
            List<BeanTaskDocument> retValue = null;
            try
            {
                dynamic obj = new JObject();
                obj.ID = _beanTask.ID;
                obj.VBId = _beanTask.VBId;

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getChildTasks"));
                lstGet.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(obj)));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<List<BeanTaskDocument>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetNhiemVuDaPhanCong_TaskVanBanDen - Err:" + ex);
                return null;
            }
            return retValue;
        }


        /// <summary>
        /// Lấy thông tin luân chuyển đơn vị khác của văn bản đến
        /// </summary>
        /// <param name="_beanTaskDocument"></param>
        /// /// <param name="wname">domain của đơn vị đó</param>
        /// <returns></returns>
        public List<BeanQuaTrinhLuanChuyenWorkflow> DetailWfHistoryOtherDeparment_VanBanDen(BeanQuaTrinhLuanChuyenWorkflow _WfHistory)
        {
            List<BeanQuaTrinhLuanChuyenWorkflow> retValue = null;
            try
            {
                dynamic obj = new JObject();
                obj.VBId = _WfHistory.VBId;
                obj.LookupId = _WfHistory.LookupId;


                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "expandThongTinLuanChuyenOtherDepartment"));
                lstGet.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(obj)));

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;

                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<List<BeanQuaTrinhLuanChuyenWorkflow>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderDoc - GetTrinhLuanChuyen_VanBanDen - Err:" + ex.Message + ex.StackTrace);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Tìm kiếm văn bản
        /// </summary>
        /// <param name="limit">số lượng văn bản cần lấy</param>
        /// <param name="offset">lấy theo index mỗi lần load more</param>
        /// <returns></returns>
        public List<BeanSearchProperty> SearchDocument(BeanSearchProperty searchProperty, int limit, int offset)
        {
            List<BeanSearchProperty> retValue = new List<BeanSearchProperty>();
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "searchVB"));
                lstGet.Add(new KeyValuePair<string, string>("Limit", limit.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("Offset", offset.ToString()));
                lstPost.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(searchProperty)));

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;

                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanBanHanhMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = retData["data"].ToObject<List<BeanSearchProperty>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Func SearchDocument ERROR + " + ex.ToString() + ex.StackTrace);
            }
            return retValue;
        }

        public object GetCountVB()
        {
            var retValue = new
            {
                VBCanXuLy = 0,
                VBPhoiHop = 0,
                ThongBao = 0
            };

            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getCount"));

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;

                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanBanHanhMobile.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = JsonConvert.DeserializeAnonymousType(retData["data"].ToString(), retValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Func GetCountVB ERROR + " + ex.ToString() + ex.StackTrace);
            }
            return retValue;
        }

        public List<T> LoadMoreDataVBPhoiHop<T>(int limit, int offset)
        {
            List<T> retValue = new List<T>();
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getVBPH"));
                lstGet.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("Offset", offset.ToString()));

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite;

                JObject retData = GetJsonDataFromAPI(combieUrl + "/vanban/_layouts/15/VuThao.Petrolimex.API/ApiPublish.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                    retValue = JsonConvert.DeserializeAnonymousType(retData["data"].ToString(), retValue);

                return retValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Func LoadMoreDataVBPhoiHop ERROR + " + ex.Message + ex.StackTrace);
            }
            return new List<T>();
        }
    }
}
