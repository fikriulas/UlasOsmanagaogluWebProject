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
                    toastr.success("Kullanıcı Başarıyla Silindi");
                },
                error: function (errorMessage) {
                    toastr.error(errorMessage.responseText);
                }
            });
        }
    });
}

$(document).ready(function () {
    $('.js-AddUser-basic-multiple').select2({
        placeholder: ' Select any Roles'
    });
});
$(document).ready(function () {
    $('.js-examplet-basic-multiple').select2({
        placeholder: ' Select any Roles'
    });
});

$("#AddUserForm").submit(function (event) {    
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
            var trclass = "";
            var bosluk = " ";
            var count = $("#Users > tr").length;
            if (count % 2 == 0)
                trclass = "odd";
            else
                trclass = "even";

            var verii = '<tr id="' + user.id + '" class="' + trclass + '">' +
                '<td id="username" style="width:20%" class="dtr-control sorting_1" tabindex="0">' + user.userName + '</td>' +
                '<td id="name" style="width:20%">' + user.name + '</td>' +
                '<td id="surname" style="width:20%">' + user.surname + '</td>' +
                '<td>'+user.email+'</td>' +
                '<td style="width:30%;text-align:center">' +
                '<a type="button" title="Düzenle" class="btn btn-warning btn-sm" href="/User/Edit/' + user.id + '"><i class="fas fa-pencil-alt"></i> Edit</a>' +
                '<a style="margin-left:3.5px;" onclick=Delete("/User/DeleteUser/' + user.id + '") title="Sil" class="btn btn-danger btn-sm">' +
                '<i class="fas fa-trash"> Delete</i>' +
                '</a></td></tr>';      



            $("#Users").append(verii);
            $("#AddUserModal").removeClass("in");
            $(".modal-backdrop").remove();
            $('body').removeClass('modal-open');
            $('body').css('padding-right', '');
            $("#AddUserModal").hide();
            $("#ajax-loading").hide();
            toastr.success("İşlem Başarılı");
        },
        error: function (ErrorMessage) {
            if (ErrorMessage.responseText != "") {       

                toastr.error(ErrorMessage.responseText);
            }

        },
        cache: false,
        contentType: false,
        processData: false
    });
});

