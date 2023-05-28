function UserRegiser() {
    var userNMC = $("#UserNMC_ID").val();
    var phoneNo = $("#PhoneNo_ID").val();
    var password = $("#Password_ID").val();
    var repeatPassword = $("#RepeatPassword_ID").val();

    // 資料檢核
    if (password != repeatPassword) {
        alert("兩次輸入密碼不一致");
        return false;
    }

    var checkMsg = "";

    if (userNMC == undefined || userNMC == "")
        checkMsg += "【姓名】未輸入\r\n";
    if (phoneNo == undefined || phoneNo == "")
        checkMsg += "【手機號碼】未輸入\r\n";
    if (password == undefined || password == "")
        checkMsg += "【密碼】未輸入\r\n";

    var reg = "";

    reg = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/;
    if (reg.test(password) == false)
        checkMsg += "密碼需至少8個字元，且包含大小寫英文和數字";

    if (checkMsg != "") {
        alert(checkMsg);
        return false;
    }

    // 手機號碼驗證
    var data = {
        phoneNo: phoneNo
    }

    $.ajax({
        type: "POST",
        url: "/ObbRegister/CheckPhoneNo",
        data: data,
        success: function (data) {
            if (data.phoneNo == null) {
                // 資料庫無手機號碼資料，新增帳號
                var model = {
                    userNMC: userNMC,
                    phoneNo: phoneNo,
                    password: password,
                }

                $.ajax({
                    type: "POST",
                    url: "/ObbRegister/SaveData",
                    data: model,
                    success: function (data) {
                        alert("註冊成功，3秒後返回登入畫面");
                        setTimeout('window.location = "/ObbLogin/Index";', 3000);
                    },
                    error: function (xhr) {
                        alert(JSON.stringify(xhr));
                    }
                })
            } else {
                alert("帳號已存在");
            }
        },
        error: function (xhr) {
            alert(JSON.stringify(xhr));
        }
    })
}


