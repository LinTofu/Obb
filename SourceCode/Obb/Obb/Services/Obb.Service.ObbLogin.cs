using Microsoft.Data.SqlClient;
using Obb.Data;
using Obb.Models;
using System.Data;
using System.Text;
using XSystem.Security.Cryptography;

namespace Obb.Services
{
    public interface IObbLoginService
    {
        IList<ObbUser> ReadUserData();
        ObbUser SavaData(ObbContext obbContext, ObbUser obbUser);
        ObbUser CheckPhoneNo(ObbUser obbUser);

        ObbUser LoginCheck(ObbUser obbUser);

    }
    public class ObbLoginService: IObbLoginService
    {
        private readonly IObbMethod obbMethod;

        public ObbLoginService(IObbMethod obbMethod)
        {
            this.obbMethod = obbMethod;
        }

        public IList<ObbUser> ReadUserData()
        {
            return new List<ObbUser>();
        }

        public ObbUser SavaData(ObbContext obbContext, ObbUser obbUser)
        {
            // 資料處理
            obbUser.UserID = Guid.NewGuid().ToString();
            obbUser.RegistrationDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");

            // 密碼加密
            string sPassword = obbUser.Password;
            string sSalt = Guid.NewGuid().ToString();

            // SHA256加密
            byte[] bPasswordSalt = Encoding.UTF8.GetBytes(sPassword + sSalt);
            byte[] bHashBytes = new SHA256Managed().ComputeHash(bPasswordSalt);
            string sHash = Convert.ToBase64String(bHashBytes);
            obbUser.Password = sHash;
            obbUser.PassSalt = sSalt;

            // 資料庫存取
            this.obbMethod.InsertObbUser(obbUser);

            return obbUser;
        }

        public ObbUser CheckPhoneNo(ObbUser obbUser)
        {
            ObbUser Result = new ObbUser();
            DataTable SourceTable = this.obbMethod.ByPhoneNoGetUserData(obbUser);
            if(SourceTable.Rows.Count == 0) // 防呆
            {
                return Result;
            }

            foreach(DataRow ARow in SourceTable.Rows)
            {
                Result.UserID = ARow["UserID"].ToString();
                Result.UserNMC = ARow["UserNMC"].ToString();
                Result.PhoneNo = ARow["PhoneNo"].ToString();
                Result.RegistrationDateTime = ARow["RegistrationDateTime"].ToString();
                Result.LoginDateTime = ARow["LoginDateTime"].ToString();
                Result.LastLoginDateTime = ARow["LastLoginDateTime"].ToString();
            }


            return Result;
        }

        public ObbUser LoginCheck(ObbUser obbUser)
        {
            ObbUser Result = new ObbUser();
            string sSalt = "";
            string sPassword = "";
            DataTable SourceTable = this.obbMethod.ByPhoneNoGetUserData(obbUser);
            if(SourceTable.Rows.Count == 0) // 防呆
            {
                return Result;
            }

            foreach(DataRow ARow in SourceTable.Rows)
            {
                // 取得該用戶經加鹽雜湊密碼
                sSalt = ARow["PassSalt"].ToString();
                sPassword = ARow["Password"].ToString();
                Result.UserID = ARow["UserID"].ToString();
            }

            // 比對密碼是否正確
            byte[] bPasswordSalt = Encoding.UTF8.GetBytes(obbUser.Password + sSalt);
            byte[] bHashBytes = new SHA256Managed().ComputeHash(bPasswordSalt);
            string sHash = Convert.ToBase64String(bHashBytes);

            if(sHash != sPassword)
            {
                // 密碼驗證失敗
                Result.ErrorMsg = "密碼輸入錯誤，請重新輸入!";
            }
           

            return Result;
        }
    }
}
