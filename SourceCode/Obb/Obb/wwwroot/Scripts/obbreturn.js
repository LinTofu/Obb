function bookReturn() {
    var userID = $("#UserID").val();
    var bookReturnArray = [];
    $("[name=BookReturn]:checkbox:checked").each(function () {
        bookReturnArray.push($(this).val());
    });
    console.log(bookReturnArray);
    // 資料檢核
    if (bookReturnArray.length == 0) {
        alert("未選擇資料");
        return;
    }

    var data = {
        userID: userID,
        bookReturn: bookReturnArray
    }

    $.ajax({
        type: "POST",
        url: "/ObbReturn/ReturnBook",
        data: data,
        success: function (data) {
            alert("還書成功");
            location.href = "/ObbReturn/Index?userID=" + userID;
        },
        error: function (xhr) {
            alert(JSON.stringify(xhr));
        }
    })
}