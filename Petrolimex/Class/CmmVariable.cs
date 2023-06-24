﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Petrolimex.Bean;

namespace Petrolimex.Class
{
    public class CmmVariable
    {
        //public static string M_Domain = "http://petrolimex.vuthao.com.vn"; //site dev
        public static string M_Domain = "https://daotaoeoffice.petrolimex.com.vn"; //site đào tạo
        //public static string M_Domain = "http://eoffice.petrolimex.com.vn"; //Site live

        //public static string M_Domain_dev = "https://daotaoeoffice.petrolimex.com.vn"; //site đào tạo
        //public static string M_Domain_live = "http://eoffice.petrolimex.com.vn"; //site đào tạo

        public static string M_DomainSubsite
        {
            get
            {
                string result = M_Domain;
                if (sysConfig != null)
                {
                    if (!string.IsNullOrEmpty(sysConfig.Subsite))
                        result += "/" + sysConfig.Subsite;
                }
                return result;
            }
        }

        public static bool IsLoginApp = false;
        public static string M_Wpf_DbSite = string.Empty;
        public static string M_Wpf_RootDir = "EOffice/Data";                      // Folder Root của App  
        public static string M_Wpf_DatabaseDir = "Database";                      // Folder lưu Database
        private static string _M_Wpf_AttachDir = "File";                            // Folder lưu File đính kèm
        public static string M_Wpf_AttachDir
        {
            get
            {
                if(sysConfig != null)
                {
                    if(!string.IsNullOrEmpty(sysConfig.Subsite))
                        return _M_Wpf_AttachDir + "/" + sysConfig.Subsite.ToLower();
                }
                return _M_Wpf_AttachDir;
            }
            set
            {
                _M_Wpf_AttachDir = value;
            }
        }
        public static string M_Wpf_LogDir = "Log";                                // Folder lưu File Log

        //public static string M_Wpf_SQLitePath = "DB_sqlite_XamDocument.db";       // Folder Root của App  
        public static string M_Wpf_SQLitePath
        {
            get
            {
                if (sysConfig == null)
                    return System.IO.Path.Combine(M_Wpf_DatabaseDir, "DB_sqlite_XamDocument.db");
                else
                    return System.IO.Path.Combine(M_Wpf_DatabaseDir, "DB_sqlite_XamDocument" + (!string.IsNullOrEmpty(sysConfig.Subsite) ? "_" + sysConfig.Subsite : "") + ".db");
            }
        }
        public static string M_Wpf_SqlPathNotify
        {
            get
            {
                return System.IO.Path.Combine(M_Wpf_DatabaseDir, "DB_sqlite_XamDocument"+(!string.IsNullOrEmpty(M_Wpf_DbSite) ? "_" + M_Wpf_DbSite : "")+".db");
            }
        }
        public static bool IsFirstSite
        {
            get
            {
                if (sysConfig != null)
                    return M_Wpf_DbSite.Equals(sysConfig.Subsite);
                return true;
            }
        }

        public static string AliasDBName
        {
            get
            {
                if (!IsFirstSite)
                    return "td.";
                return string.Empty;
            }
        }

        public static string M_DeviceScanFolder = @"C:\Program Files (x86)\fiScanner\PaperStream Capture";
        public static string M_ScanFolder = "FileScan";
        public static int M_TimeScanFile = 5;
        public static int M_MaxTimeScanFile = 180;

        public static string M_Wpf_SettingFileName = "setting.ini";               // Tên File Config

        public static bool M_RenewDB = true;

        public static string Prefix = string.Empty; // PrefixMainBoardCode

        public readonly static bool M_flgSysDebug = true;
        public static Dictionary<string, string> M_LangData = null;     //Dictionary
        public static ConfigVariable sysConfig = null;                  // Dữ liệu lưu dữ lại ghi xuống file như: site, subsite
        public static HttpClient M_AuthenticatedHttpClient = null;      // httpClient sử dụng chung khi kết nối thành công server
        public static short M_AutoReLoginNum = 0;                       // Số Auto login bị lỗi liên tiếp
        public static short M_AutoReLoginNumMax = 1;                    // Số tối đa được phép Auto login lỗi liên tiếp, nếu > Max sẽ yêu cầu User đăng nhập lại

        public static string M_ApiLogin = "/API/ApiUser.ashx"; // api user lấy thông tin về user

        public static string M_WorkDateCustomCalendarFormatDateVN = "dd/MM/yyyy";
        public static string M_WorkDateFormatDayVN = "dd/MM/yy";
        public static string M_WorkDateFormatTimeVN = "HH:mm";
        public static string M_WorkDateFormatVN = M_WorkDateFormatDayVN + " " + M_WorkDateFormatTimeVN;

        public static string M_WorkDateFormatDay = "MM/dd/yy";
        public static string M_WorkDateFormatTime = "HH:mm";
        //public static string M_LogPath = "logPetrolimex.txt";
        public static int M_DiffHours = 0;

        public static int M_LoadMoreCount_Local = 30; // số lượng load more từ Local
        public static int M_LimitLvItem = 100;

        public static string M_WorkDateFormat = M_WorkDateFormatDay + " " + M_WorkDateFormatTime; //Định dạng ngày giờ làm việc
        public static int M_DelayGetDataTime = 3; // Độ trễ của mạng khi lấy dữ liệu, đơn vị Giây


        // Dùng để giải mã
        public readonly static string M_RSACodePublicKey = "MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAIHCO6tw3Lbfpa6JWJvEW+CNH7JneKlFXPVEXvrzsk+ipU+C/vg4nJkTuWgii0FD1sX5sKll/WG8mCitNIn3ow0CAwEAAQ==";

        //Dùng để mã hóa
        public readonly static string M_RSACodePrivateKey = "LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlCT2dJQkFBSkJBSUhDTzZ0dzNMYmZwYTZKV0p2RVcrQ05IN0puZUtsRlhQVkVYdnJ6c2sraXBVK0Mvdmc0Cm5Ka1R1V2dpaTBGRDFzWDVzS2xsL1dHOG1DaXROSW4zb3cwQ0F3RUFBUUpBQVBTamwxSmVXWE9DNWVYbDVWcU4KSDc5dUE4RHJaNXdLYjBINzFzZjlYazZ2blJmVjg0R2hGTzdmNlJUZTlBMUxUZzRheCtCcnI5bGdOdVFwTzk4MwpzUUloQUw3TFltUU1zdGt3WEYyTW5oSTF5bDBCNEoyaHlla294c3RkR3Qwb3hMRzFBaUVBcmhyVUx2eFBWSUIyCjc5ZVlIcmF1NEpTZEprd2JzRE5yT3NzV1BpWFI0dmtDSUF4ZmNDQUEydEJPM0k2TWdldTRaVWtteUFRdzY4RWQKRGRjK1VIR3JKU1pKQWlBMFJnL2hENVpOKzhnNkdOUXVuSkVERXZ2Z0RNZmZib0RLaFVGblpYbVFHUUloQUsrVQpNcVZPdzcyODJENkRDVTNISlBBb2dLZDRPUVl0ejFsM0ZWb3dMNG5XCi0tLS0tRU5EIFJTQSBQUklWQVRFIEtFWS0tLS0t";
    }
}
