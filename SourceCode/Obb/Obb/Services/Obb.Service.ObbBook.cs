using Obb.Models;
using Obb.Data;
using System.Data;

namespace Obb.Services
{
    public interface IObbBookService
    {
        IList<ObbBook> ReadBookData(string userID);
        ObbBook BorrowBook(ObbBook obbBook);
        IList<ObbBook> ReadBorrowBookData(string userID);
        ObbBook ReturnBook(ObbBook obbBook);

        void UpdateLastLoginDateTime(string userID);
    }

    public class ObbBookService: IObbBookService
    {
        private readonly IObbMethod obbMethod;

        public ObbBookService(IObbMethod obbMethod)
        {
            this.obbMethod = obbMethod;
        }

        public IList<ObbBook> ReadBookData(string userID)
        {
            IList<ObbBook> Result = new List<ObbBook>();
            DataTable SourceTable = this.obbMethod.ReadBookData();

            foreach (DataRow ARow in SourceTable.Rows)
            {
                ObbBook obbBook = new ObbBook();
                obbBook.ISBN = ARow["ISBN"].ToString();
                obbBook.BookName = ARow["BookName"].ToString();
                obbBook.BookAuthor = ARow["BookAuthor"].ToString();
                obbBook.BookIntroduction = ARow["BookIntroduction"].ToString();
                obbBook.Status = ARow["Status"].ToString();
                obbBook.UserID = userID;
                Result.Add(obbBook);
            }

            return Result;
        }

        public ObbBook BorrowBook(ObbBook obbBook)
        {
            ObbBook Result = new ObbBook();
            this.obbMethod.BorrowBook(obbBook);


            return Result;
        }

        public IList<ObbBook> ReadBorrowBookData(string userID)
        {
            IList<ObbBook> Result = new List<ObbBook>();
            DataTable SourceTable = this.obbMethod.ReadBorrowBookData(userID);

            foreach (DataRow ARow in SourceTable.Rows)
            {
                ObbBook obbBook = new ObbBook();
                obbBook.ISBN = ARow["ISBN"].ToString();
                obbBook.BookName = ARow["BookName"].ToString();
                obbBook.BookAuthor = ARow["BookAuthor"].ToString();
                obbBook.BookIntroduction = ARow["BookIntroduction"].ToString();
                obbBook.UserID = userID;
                Result.Add(obbBook);
            }

            return Result;
        }

        public ObbBook ReturnBook(ObbBook obbBook)
        {
            ObbBook Result = new ObbBook();
            this.obbMethod.ReturnBook(obbBook);


            return Result;
        }

        public void UpdateLastLoginDateTime(string userID)
        {
            this.obbMethod.UpdateLastLoginDateTime(userID);
        }
    }
}
