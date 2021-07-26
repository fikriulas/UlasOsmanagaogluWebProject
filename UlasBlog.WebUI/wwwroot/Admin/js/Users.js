$(document).on("click", ".open-editProfileModel", function () {
    //var name = $(this).data('role-name');
    var id = $(this).data('profile-id');
    //var name = $(this).data('profile-name');
    var name = $('#' + id).find('#name').html(); 
    var surname = $('#' + id).find('#surname').html(); 
    var username = $('#' + id).find('#username').html(); 
    var email = $('#' + id).find('#email').html(); 
    
    $(".modal-body #Name").val(name);
    $(".modal-body #Surname").val(surname);
    $(".modal-body #Email").val(email);
    $(".modal-body #UserName").val(username);
    $(".modal-body #Id").val(id);
});

$("#editProfileForm").submit(function (event) {
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
        success: function (user) {           
 
            $('#' + user.id).find('#name').text(user.name); 
            $('#' + user.id).find('#surname').text(user.surname); 
            $('#' + user.id).find('#username').text(user.userName); 
            $('#' + user.id).find('#email').text(user.email); 
            $("#editProfileModel").removeClass("in");
            $(".modal-backdrop").remove();
            $('body').removeClass('modal-open');
            $('body').css('padding-right', '');
            $("#editProfileModel").hide();
            $("#ajax-loading").hide();
            toastr.success("İşlem Başarılı");
        },
        error: function (ErrorMessage) {
            toastr.error(ErrorMessage.responseText);
        },
        cache: false,
        contentType: false,
        processData: false
    });
});

function Delete(url) {
    console.log("Delete içinde");
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
                    console.log("success içinde");
                    console.log(Id);
                    var m = document.getElementById(Id);
                    m.style.display = "none";
                    toastr.success("Kullanıcı Başarıyla Silindi");
                },
                error: function (errorMessage) {
                    console.log("error içinde");

                    toastr.error(errorMessage.responseText);
                }
            });
        }
    });
}

$(document).ready(function () {
    $('.js-example-basic-multiple').select2({
        placeholder: ' Select any Roles'
    });
});