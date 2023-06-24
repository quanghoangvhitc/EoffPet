using System;
using System.Collections.Generic;
using System.Text;
using Petrolimex.Bean;

namespace Petrolimex.Class
{
    public static class CmmEvent
    {
        public class UpdateEventArgs : EventArgs
        {
            public bool IsSuccess { get; set; }
            public string LangSelected { get; set; }
            public string ErrMess { get; set; }

            public UpdateEventArgs()
            {
                LangSelected = "";
                ErrMess = "";
            }
            public UpdateEventArgs(bool isSuccess, string langSelected = "", string errMess = "")
            {
                IsSuccess = isSuccess;
                LangSelected = langSelected;
                ErrMess = errMess;
            }
        }

        public class LoginEventArgs : EventArgs
        {
            public bool IsSuccess { get; set; }
            public string UserName { get; set; }
            public string ErrCode { get; set; }
            public string Pass { get; set; }
            public BeanUser UserInfo { get; set; }

            public LoginEventArgs()
            {
                UserName = "";
                Pass = "";
            }
            public LoginEventArgs(bool isSuccess, string userName = "", string pass = "", BeanUser userInfo = null, string errCode = "")
            {
                IsSuccess = isSuccess;
                UserName = userName;
                Pass = pass;
                UserInfo = userInfo;
                ErrCode = errCode;
            }
        }

        public static event EventHandler UpdateCount;
        public static event EventHandler<UpdateEventArgs> UpdateLangComplete;
        public static event EventHandler<LoginEventArgs> ReloginRequest;
        public static event EventHandler SyncDataBackGroundRequest;
        public static event EventHandler UpdateUSerTctRequest;
        //public static event EventHandler PinCodeRequest;
        public static event EventHandler SyncDataRequest;

        public static void UpdateCount_Performence(object sender, EventArgs e)
        {
            if (UpdateCount == null)
            {
                UpdateCount(sender, e);
            }
        }

        public static void UpdateLangComplete_Performence(object sender, UpdateEventArgs e)
        {

            if (UpdateLangComplete != null)
            {
                UpdateLangComplete(sender, e);
            }
        }

        public static void ReloginRequest_Performence(object sender, LoginEventArgs e)
        {

            if (ReloginRequest != null)
            {
                ReloginRequest(sender, e);
            }
        }

        public static void SyncDataBackgroundRequest_Performence(object sender, LoginEventArgs e)
        {

            if (SyncDataBackGroundRequest != null)
            {
                SyncDataBackGroundRequest(sender, e);
            }
        }

        public static void SyncDataRequest_Performence(object sender, EventArgs e)
        {
            if (SyncDataRequest != null)
            {
                SyncDataRequest(sender, e);
            }
        }

        public static void UpdateUserTct_Performence(object sender, EventArgs e)
        {
            if (UpdateUSerTctRequest != null)
                UpdateUSerTctRequest(sender, e);
        }
        public static event EventHandler<DownloadedEventArgs> DownloadedFile;

        public class DownloadedEventArgs : EventArgs
        {
            public string path;
            public DownloadedEventArgs(string path)
            {
                this.path = path;
            }
        }

        public static void DownloadedFile_EventArgs(object sender, DownloadedEventArgs e)
        {
            if (DownloadedFile != null)
            {
                DownloadedFile(sender, e);
            }
        }
    }
}
