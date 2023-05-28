function bookBorrow() {
    var userID = $("#UserID").val();
    var bookBorrow = [];
    $("[name=BookBorrow]:checkbox:checked").each(function () {
        bookBorrow.push($(this).val());
    });
    console.log(userID);
    // 資料檢核
    if (bookBorrow.length == 0) {
        alert("未選擇資料");
        return;
    }

    var data = {
        UserID: userID,
        bookBorrow: bookBorrow
    }

    $.ajax({
        type: "POST",
        url: "/ObbBorrow/BorrowBook",
        data: data,
        success: function (data) {
            alert("借閱成功");
            location.href = "/ObbBorrow/Index?userId=" + userID;
        },
        error: function (xhr) {
            alert(JSON.stringify(xhr));
        }
    })
}