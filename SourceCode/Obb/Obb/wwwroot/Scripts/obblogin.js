function logincheck() {
    var phoneNo = $("#PhoneNo_Id").val();
    var password = $("#Pass_Id").val();

    var data = {
        phoneNo: phoneNo,
        password: password
    }

    $.ajax({
        type: "POST",
        url: "/ObbLogin/LoginCheck",
        data: data,
        success: function (data) {
            if (data.errorMsg != null) {
                alert(data.errorMsg);
                return;
            } else {
                window.location = "/ObbBorrow/Index";
            }
        },
        error: function (xhr) {
            alert(JSON.stringify(xhr));
        }
    })
}