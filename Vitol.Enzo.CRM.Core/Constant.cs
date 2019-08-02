using System;
using System.Collections.Generic;
using System.Text;

namespace Vitol.Enzo.CRM.Core
{
    public class Constant
    {
        public const string JsonContentType = "application/json";
        public const string CommandType = "Command Type";
        public const string CommandText = "Command Text";
        public const string ExceptionData = "Exception Data: ";
        public const string ExceptionMessage = "Exception Message: ";
        public const string InnerExceptionMessage = "Inner Exception Message: ";
        public const string ExceptionStackTrace = "Exception StackTrace: ";
        public const string InnerExceptionStackTrace = "Inner Exception StackTrace: ";
        //public const string RestPasswordSubject = "Reset Password";
        //public const string RestPasswordUrl = "http://localhost:4200/ResetPassword";
        public const string AlphanumericCaps = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()_+";
        public const int DefaultPasswordLength = 20;

        public const char GeneralDelimiter = ',';
        public const char ParentDelimiter = ';';
        //public const byte AppointmentSlotSize = 30;

        //public const ushort DefaultTrimLevelStartYear = 1950;
        //public const string SortByDistance = "Distance";
        //public const string ExternalStoreDirectory = "inspection";
        //public const string LocalTimeZone = "Pakistan Standard Time";// "Turkey Standard Time"; // "Pakistan Standard Time";
        //public const double UTCOffsetInMinutes = 300;

        //public const string DefaultLanguage = "en-UK";
        //public const string SecondaryLanguage = "tr-TR";
        public const string TurkeyName = "Turkey";

        public const uint DefaultUserId = 1;
    }
}
