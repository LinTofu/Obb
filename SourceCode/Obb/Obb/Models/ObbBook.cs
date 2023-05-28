using System.ComponentModel.DataAnnotations;

namespace Obb.Models
{
    public class ObbBook
    {
        [Key]
        public string ISBN { get; set; }
        public string BookName { get; set; }
        public string BookAuthor { get; set; }
        public string BookIntroduction { get; set; }
        public string[] BookBorrow { get; set; }
        public string[] BookReturn { get; set; }
        public string UserID { get; set; }
        public string Status { get; set; }
    }

    public class ObbBorrow
    {
        public string UserID { get; set; }
        public string InventoryID { get; set; }
        public string BorrowDateTime { get; set; }
        public string ReturnDateTime { get; set; }
    }
    
    public class ObbInventory
    {
        public string InventoryID { get; set; }
        public string ISBN { get; set; }
        public string StoreTime { get; set; }
        public string Status { get; set; }
    }
}
