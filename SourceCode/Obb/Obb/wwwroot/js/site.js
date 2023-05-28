// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function loginOut() {
    $.ajax({
        type: "POST",
        url: "/ObbLogin/LoginOut",
        success: function (data) {
            
        },
        error: function (xhr) {
            alert(JSON.stringify(xhr));
        }
    })
}