$("#settingForm").submit(function (event) {
    event.preventDefault();
    var form = $(this);
    var formData = new FormData(this);
    formData.append('fileDeleteInfo', 'fileDeleteInfo');
    console.log(formData);
    $.ajax({
        url: form.attr("action"),
        type: 'POST',
        data: formData,
        beforeSend: function () {
            $("#ajax-loading").show();
        },
        complete: function () {
            $("#ajax-loading").hide();
        },
        success: function (setting) {       
            document.getElementById("fileDeleteInfo").value = " ";
            document.getElementById('logFileSizeTd').innerHTML = "0 Byte";
            console.log("girildi");
            toastr.success("İşlem Başarılı");            
        },
        error: function (ErrorMessage) {
            console.log(ErrorMessage)
            console.log(ErrorMessage.responseText)
            if (ErrorMessage.responseText == "") {
                toastr.error("İşlem Başarısız, Yönetici ile iletişime geçin.");
            }
            else {
                toastr.error(ErrorMessage.responseText);
            }
        },
        cache: false,
        contentType: false,
        processData: false
    });
});

