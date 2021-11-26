$(document).on("click", ".open-editContactModal", function () {
    var name = $(this).data('contact-name');
    var id = $(this).data('contact-id');
    var date = $(this).data('contact-date');
    var email = $(this).data('contact-email');
    var message = $(this).data('contact-message');
    $(".modal-body #Name").val(name);
    $(".modal-body #Id").val(id);
    $(".modal-body #dateAdded").val(date);
    $(".modal-body #Message").val(message);
    $(".modal-body #Email").val(email);
});

$("#replyMessage").submit(function (event) {
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
            document.getElementById("replyMessage").reset();
            $("#editContactModal").removeClass("in");
            $(".modal-backdrop").remove();
            $('body').removeClass('modal-open');
            $('body').css('padding-right', '');
            $("#editContactModal").hide();
            $("#ajax-loading").hide();      
            $('.note-editable').empty()
            toastr.success("Mesaj iletildi.");
        },
        error: function () {            
            toastr.error("Bir sorun oluştu");
        },
        cache: false,
        contentType: false,
        processData: false
    });
});

function Delete(url) {
    event.preventDefault(); // prevent form submit
    var form = event.target.form; // storing the form
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    var data = { Id: $("#Id").val() };
    var dataWithToken = $.extend(data, { '__RequestVerificationToken': token });
    swal({
        title: "Are you sure you want to Delete?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "Delete",
                data: $(this).serialize(),
                beforeSend: function () {
                    $("#ajax-loading").show();
                },
                complete: function () {
                    $("#ajax-loading").hide();
                },
                success: function (Id) {                    
                    var m = document.getElementById(Id);
                    m.style.display = "none";                    

                    toastr.success("İşlem Başarılı");
                },
                error: function (errorMessage) {
                    toastr.error(errorMessage.responseText);
                }
            });
        }
    });
}

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