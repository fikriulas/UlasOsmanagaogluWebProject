$("#settingForm").submit(function (event) {
    event.preventDefault();
    var form = $(this);
    var formData = new FormData(this);
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
            toastr.success("İşlem Başarılı");            
        },
        error: function (ErrorMessage) {            
            if (ErrorMessage.responseText != "") {
                toastr.error("İşlem Başarısız, Yönetici ile iletişime geçin.");
            }
            else {                
                toastr.error("Kontrol Edip Tekrar Deneyin");
            }
        },
        cache: false,
        contentType: false,
        processData: false
    });
});
