using Microsoft.Data.SqlClient;
using Obb.Models;
using System.Data;
using XAct;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Obb.Data
{
    public interface IObbMethod
    {
        string GetConnectionString();
        void InsertObbUser(ObbUser obbUser);
        DataTable ByPhoneNoGetUserData(ObbUser obbUser);
        DataTable ReadBookData();
        void BorrowBook(ObbBook obbBook);
        DataTable ReadBorrowBookData(string userID);
        void ReturnBook(ObbBook obbBook);

        void UpdateLastLoginDateTime(string userID);
    }

    public class ObbDBMethod : IObbMethod
    {
        // 取得 appsetttings.json 連線字串
        public string GetConnectionString()
        {
            var builder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json");
            var config = builder.Build();
            var connectionString = config.GetConnectionString("ObbContext");

            return connectionString;
        }

        // INSERT OBBUSER
        public void InsertObbUser(ObbUser obbUser)
        {
            IDbDataParameter Parameter;
            IDbCommand NewCommand;
            string sCommandText = "";

            IDbConnection NewConnection = new SqlConnection();
            NewConnection.ConnectionString = GetConnectionString();

            NewConnection.Open();
            using (IDbTransaction NewTransaction = NewConnection.BeginTransaction())
            {
                try
                {
                    #region ReadData

                    sCommandText = @"INSERT INTO OBBUSER
                                    (UserID, UserNMC, PhoneNo, Password, RegistrationDateTime, LoginDateTime, LastLoginDateTime, PassSalt)
                                    VALUES
                                    (@UserID, @UserNMC, @PhoneNo, @Password, @RegistrationDateTime, @LoginDateTime, @LastLoginDateTime, @PassSalt)";
                    NewCommand = NewConnection.CreateCommand();
                    NewCommand.CommandText = sCommandText;
                    NewCommand.Transaction = NewTransaction;

                    Parameter = NewCommand.CreateParameter();
                    Parameter.ParameterName = "@UserID";
                    Parameter.Value = obbUser.UserID;
                    NewCommand.Parameters.Add(Parameter);

                    Parameter = NewCommand.CreateParameter();
                    Parameter.ParameterName = "@UserNMC";
                    Parameter.Value = obbUser.UserNMC;
                    NewCommand.Parameters.Add(Parameter);

                    Parameter = NewCommand.CreateParameter();
                    Parameter.ParameterName = "@PhoneNo";
                    Parameter.Value = obbUser.PhoneNo;
                    NewCommand.Parameters.Add(Parameter);

                    Parameter = NewCommand.CreateParameter();
                    Parameter.ParameterName = "@Password";
                    Parameter.Value = obbUser.Password;
                    NewCommand.Parameters.Add(Parameter);

                    Parameter = NewCommand.CreateParameter();
                    Parameter.ParameterName = "@RegistrationDateTime";
                    Parameter.Value = obbUser.RegistrationDateTime;
                    NewCommand.Parameters.Add(Parameter);

                    Parameter = NewCommand.CreateParameter();
                    Parameter.ParameterName = "@LoginDateTime";
                    Parameter.Value = obbUser.LoginDateTime == null ? "" : obbUser.LoginDateTime;
                    NewCommand.Parameters.Add(Parameter);

                    Parameter = NewCommand.CreateParameter();
                    Parameter.ParameterName = "@LastLoginDateTime";
                    Parameter.Value = obbUser.LastLoginDateTime == null ? "" : obbUser.LoginDateTime;
                    NewCommand.Parameters.Add(Parameter);

                    Parameter = NewCommand.CreateParameter();
                    Parameter.ParameterName = "@PassSalt";
                    Parameter.Value = obbUser.PassSalt == null ? "" : obbUser.PassSalt;
                    NewCommand.Parameters.Add(Parameter);


                    NewCommand.ExecuteNonQuery();

                    #endregion

                    NewTransaction.Commit();
                }
                catch
                {
                    NewTransaction.Rollback();
                }
                finally
                {
                    NewConnection.Close();
                }
            }
        }

        // 驗證手機號碼
        public DataTable ByPhoneNoGetUserData(ObbUser obbUser)
        {
            ObbUser Result = new ObbUser();
            IDbDataParameter Parameter;
            IDbCommand NewCommand;
            DataSet NewDataSet = new DataSet();
            DataTable SourceTable = null;
            string sCommandText = "";

            IDbConnection NewConnection = new SqlConnection();
            NewConnection.ConnectionString = GetConnectionString();

            NewConnection.Open();

            sCommandText = @"SELECT *
                             FROM OBBUSER
                             WHERE PhoneNo = @PhoneNo;";
            SqlDataAdapter NewDataAdapter = new SqlDataAdapter(sCommandText, NewConnection.ConnectionString);
            NewDataAdapter.SelectCommand.Parameters.AddWithValue("@PhoneNo", obbUser.PhoneNo);
            NewDataAdapter.Fill(NewDataSet, "ObbUser");

            SourceTable = NewDataSet.Tables["ObbUser"];

            return SourceTable;
        }

        // 取得所有書籍
        public DataTable ReadBookData()
        {
            IDbDataParameter Parameter;
            IDbCommand NewCommand;
            DataSet NewDataSet = new DataSet();
            DataTable SourceTable = null;
            string sCommandText = "";

            IDbConnection NewConnection = new SqlConnection();
            NewConnection.ConnectionString = GetConnectionString();

            NewConnection.Open();

            sCommandText = @"SELECT OBBBOOK.*, OBBINVENTORY.Status
                             FROM OBBBOOK
                             LEFT OUTER JOIN OBBINVENTORY ON (OBBINVENTORY.ISBN = OBBBOOK.ISBN)";
            SqlDataAdapter NewDataAdapter = new SqlDataAdapter(sCommandText, NewConnection.ConnectionString);
            NewDataAdapter.Fill(NewDataSet, "ObbBook");

            SourceTable = NewDataSet.Tables["ObbBook"];


            return SourceTable;
        }

        // 新增借閱紀錄 & 更新書籍狀態(借書)
        public void BorrowBook(ObbBook obbBook)
        {
            IDbDataParameter Parameter;
            IDbCommand NewCommand;
            DataSet NewDataSet = new DataSet();
            DataTable SourceTable = null;
            string sCommandText = "";
            string[] sInventoryID = new string[obbBook.BookBorrow.Length];

            IDbConnection NewConnection = new SqlConnection();
            NewConnection.ConnectionString = GetConnectionString();


            using (NewConnection)
            {
                NewConnection.Open();

                foreach (var item in obbBook.BookBorrow)
                {
                    #region ReadInventoryData

                    NewDataSet = new DataSet();

                    sCommandText = @"SELECT *
                                        FROM OBBINVENTORY
                                        WHERE ISBN = @ISBN";
                    SqlDataAdapter NewDataAdapter = new SqlDataAdapter(sCommandText, NewConnection.ConnectionString);
                    NewDataAdapter.SelectCommand.Parameters.AddWithValue("@ISBN", item);
                    NewDataAdapter.Fill(NewDataSet, "ObbInventory");

                    SourceTable = NewDataSet.Tables["ObbInventory"];

                    sInventoryID[obbBook.BookBorrow.IndexOf(item)] = SourceTable.Rows[0]["InventoryID"].ToString();

                    #endregion
                }

                IDbTransaction NewTransaction = NewConnection.BeginTransaction();

                NewCommand = NewConnection.CreateCommand();
                NewCommand.Connection = NewConnection;
                NewCommand.Transaction = NewTransaction;

                try
                {
                    foreach (var item in obbBook.BookBorrow)
                    {
                        // 新增借閱紀錄
                        #region INSERT OBBBORROWRECORD

                        sCommandText = @"INSERT INTO OBBBORROWRECORD
                                    (UserID, InventoryID, BorrowDateTime, ReturnDateTime)
                                    VALUES
                                    (@UserID, @InventoryID, @BorrowDateTime, @ReturnDateTime)";
                        NewCommand = NewConnection.CreateCommand();
                        NewCommand.Connection = NewConnection;
                        NewCommand.Transaction = NewTransaction;
                        NewCommand.CommandText = sCommandText;

                        Parameter = NewCommand.CreateParameter();
                        Parameter.ParameterName = "@UserID";
                        Parameter.Value = obbBook.UserID;
                        NewCommand.Parameters.Add(Parameter);

                        Parameter = NewCommand.CreateParameter();
                        Parameter.ParameterName = "@InventoryID";
                        Parameter.Value = sInventoryID[obbBook.BookBorrow.IndexOf(item)];
                        NewCommand.Parameters.Add(Parameter);

                        Parameter = NewCommand.CreateParameter();
                        Parameter.ParameterName = "@BorrowDateTime";
                        Parameter.Value = DateTime.Now.ToString("yyyyMMddHHmmss");
                        NewCommand.Parameters.Add(Parameter);

                        Parameter = NewCommand.CreateParameter();
                        Parameter.ParameterName = "@ReturnDateTime";
                        Parameter.Value = "";
                        NewCommand.Parameters.Add(Parameter);

                        NewCommand.ExecuteNonQuery();

                        #endregion

                        // 更新書籍狀態
                        #region UPDATE OBBINVENTORY

                        sCommandText = @"UPDATE OBBINVENTORY
                                     SET Status = @Status
                                     WHERE InventoryID = @InventoryID2";
                        NewCommand = NewConnection.CreateCommand();
                        NewCommand.Connection = NewConnection;
                        NewCommand.Transaction = NewTransaction;
                        NewCommand.CommandText = sCommandText;

                        Parameter = NewCommand.CreateParameter();
                        Parameter.ParameterName = "@Status";
                        Parameter.Value = "已借閱";
                        NewCommand.Parameters.Add(Parameter);

                        Parameter = NewCommand.CreateParameter();
                        Parameter.ParameterName = "@InventoryID2";
                        Parameter.Value = sInventoryID[obbBook.BookBorrow.IndexOf(item)];
                        NewCommand.Parameters.Add(Parameter);

                        NewCommand.ExecuteNonQuery();

                        #endregion
                    }

                    NewTransaction.Commit();

                }
                catch
                {
                    NewTransaction.Rollback();
                }
                finally
                {
                    NewConnection.Close();
                }
            }
        }
        // 取得使用者借閱書籍
        public DataTable ReadBorrowBookData(string userID)
        {
            IDbDataParameter Parameter;
            IDbCommand NewCommand;
            DataSet NewDataSet = new DataSet();
            DataTable SourceTable = null;
            string sCommandText = "";

            IDbConnection NewConnection = new SqlConnection();
            NewConnection.ConnectionString = GetConnectionString();

            NewConnection.Open();

            sCommandText = @"SELECT OBBBOOK.*, OBBBORROWRECORD.BorrowDateTime, OBBBORROWRECORD.ReturnDateTime
                             FROM OBBBORROWRECORD 
                             LEFT OUTER JOIN OBBINVENTORY ON (OBBBORROWRECORD.InventoryID = OBBINVENTORY.InventoryID)
                             LEFT OUTER JOIN OBBBOOK ON (OBBINVENTORY.ISBN = OBBBOOK.ISBN)
                             WHERE OBBBORROWRECORD.UserID = @UserID AND OBBBORROWRECORD.ReturnDateTime = ''
                             ORDER BY OBBBORROWRECORD.BorrowDateTime DESC";
            SqlDataAdapter NewDataAdapter = new SqlDataAdapter(sCommandText, NewConnection.ConnectionString);
            NewDataAdapter.SelectCommand.Parameters.AddWithValue("@UserID", userID);
            NewDataAdapter.Fill(NewDataSet, "ObbBook");

            SourceTable = NewDataSet.Tables["ObbBook"];


            return SourceTable;
        }
        // 更新借閱紀錄 & 更新書籍狀態(還書)
        public void ReturnBook(ObbBook obbBook)
        {
            IDbDataParameter Parameter;
            IDbCommand NewCommand;
            DataSet NewDataSet = new DataSet();
            DataTable SourceTable = null;
            string sCommandText = "";
            string[] sInventoryID = new string[obbBook.BookReturn.Length];

            IDbConnection NewConnection = new SqlConnection();
            NewConnection.ConnectionString = GetConnectionString();


            using (NewConnection)
            {
                NewConnection.Open();

                foreach (var item in obbBook.BookReturn)
                {
                    #region ReadInventoryData

                    NewDataSet = new DataSet();

                    sCommandText = @"SELECT *
                                        FROM OBBINVENTORY
                                        WHERE ISBN = @ISBN";
                    SqlDataAdapter NewDataAdapter = new SqlDataAdapter(sCommandText, NewConnection.ConnectionString);
                    NewDataAdapter.SelectCommand.Parameters.AddWithValue("@ISBN", item);
                    NewDataAdapter.Fill(NewDataSet, "ObbInventory");

                    SourceTable = NewDataSet.Tables["ObbInventory"];

                    sInventoryID[obbBook.BookReturn.IndexOf(item)] = SourceTable.Rows[0]["InventoryID"].ToString();

                    #endregion
                }

                IDbTransaction NewTransaction = NewConnection.BeginTransaction();

                NewCommand = NewConnection.CreateCommand();
                NewCommand.Connection = NewConnection;
                NewCommand.Transaction = NewTransaction;

                try
                {
                    foreach (var item in obbBook.BookReturn)
                    {
                        // 更新借閱紀錄
                        #region INSERT OBBBORROWRECORD

                        sCommandText = @"UPDATE OBBBORROWRECORD
                                         SET ReturnDateTime = @ReturnDateTime
                                         WHERE InventoryID = @InventoryID AND UserID = @UserID";
                        NewCommand = NewConnection.CreateCommand();
                        NewCommand.Connection = NewConnection;
                        NewCommand.Transaction = NewTransaction;
                        NewCommand.CommandText = sCommandText;

                        Parameter = NewCommand.CreateParameter();
                        Parameter.ParameterName = "@ReturnDateTime";
                        Parameter.Value = DateTime.Now.ToString("yyyyMMddHHmmss");
                        NewCommand.Parameters.Add(Parameter);

                        Parameter = NewCommand.CreateParameter();
                        Parameter.ParameterName = "@InventoryID";
                        Parameter.Value = sInventoryID[obbBook.BookReturn.IndexOf(item)];
                        NewCommand.Parameters.Add(Parameter);

                        Parameter = NewCommand.CreateParameter();
                        Parameter.ParameterName = "@UserID";
                        Parameter.Value = obbBook.UserID;
                        NewCommand.Parameters.Add(Parameter);

                        NewCommand.ExecuteNonQuery();

                        #endregion

                        // 更新書籍狀態
                        #region UPDATE OBBINVENTORY

                        sCommandText = @"UPDATE OBBINVENTORY
                                     SET Status = @Status
                                     WHERE InventoryID = @InventoryID2";
                        NewCommand = NewConnection.CreateCommand();
                        NewCommand.Connection = NewConnection;
                        NewCommand.Transaction = NewTransaction;
                        NewCommand.CommandText = sCommandText;

                        Parameter = NewCommand.CreateParameter();
                        Parameter.ParameterName = "@Status";
                        Parameter.Value = "可借閱";
                        NewCommand.Parameters.Add(Parameter);

                        Parameter = NewCommand.CreateParameter();
                        Parameter.ParameterName = "@InventoryID2";
                        Parameter.Value = sInventoryID[obbBook.BookReturn.IndexOf(item)];
                        NewCommand.Parameters.Add(Parameter);

                        NewCommand.ExecuteNonQuery();

                        #endregion
                    }

                    NewTransaction.Commit();

                }
                catch
                {
                    NewTransaction.Rollback();
                }
                finally
                {
                    NewConnection.Close();
                }
            }
        }

        // 更新使用者最後登入時間
        public void UpdateLastLoginDateTime(string userID)
        {
            IDbDataParameter Parameter;
            IDbCommand NewCommand;
            DataSet NewDataSet = new DataSet();
            DataTable SourceTable = null;
            string sCommandText = "";

            IDbConnection NewConnection = new SqlConnection();
            NewConnection.ConnectionString = GetConnectionString();


            using (NewConnection)
            {
                NewConnection.Open();

                IDbTransaction NewTransaction = NewConnection.BeginTransaction();

                NewCommand = NewConnection.CreateCommand();
                NewCommand.Connection = NewConnection;
                NewCommand.Transaction = NewTransaction;

                try
                {
                    // 更新使用者最後登入時間
                    #region UPDATE OBBINVENTORY

                    sCommandText = @"UPDATE OBBUser
                                    SET LastLoginDateTime = @LastLoginDateTime
                                    WHERE UserID = @UserID";
                    NewCommand = NewConnection.CreateCommand();
                    NewCommand.Connection = NewConnection;
                    NewCommand.Transaction = NewTransaction;
                    NewCommand.CommandText = sCommandText;

                    Parameter = NewCommand.CreateParameter();
                    Parameter.ParameterName = "@LastLoginDateTime";
                    Parameter.Value = DateTime.Now.ToString("yyyyMMddHHmmss");
                    NewCommand.Parameters.Add(Parameter);

                    Parameter = NewCommand.CreateParameter();
                    Parameter.ParameterName = "@UserID";
                    Parameter.Value = userID;
                    NewCommand.Parameters.Add(Parameter);

                    NewCommand.ExecuteNonQuery();

                    #endregion


                    NewTransaction.Commit();

                }
                catch
                {
                    NewTransaction.Rollback();
                }
                finally
                {
                    NewConnection.Close();
                }
            }
        }
    }
}
