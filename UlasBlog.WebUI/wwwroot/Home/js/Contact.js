$("#ContactForm").submit(function (event) {
    event.preventDefault();
    var form = $(this);
    var formData = new FormData(this);
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
        success: function () {            
            document.getElementById("ContactForm").reset();
            iziToast.show({
                title: 'Başarılı!',
                message: 'Mesajınız iletildi.',
                color: 'green', // blue, red, green, yellow
                icon: 'fa fa-check'
            }); 
        },
        error: function (errorMessage) {
            iziToast.show({
                title: 'Başarısız!',
                message: errorMessage.responseText,
                color: 'red', // blue, red, green, yellow
                icon: 'fa fa-times'
            });  
        },
        cache: false,
        contentType: false,
        processData: false
    });
});
