using System;
namespace Petrolimex.Bean
{
    public class DeviceInfo
    {
        public string DeviceId { get; set; }
        public string DevicePushToken { get; set; }
        public short DeviceOS { get; set; } // 1: Android   2: IOS  4: WindowPhone
        public string AppVersion { get; set; }
    }
}
