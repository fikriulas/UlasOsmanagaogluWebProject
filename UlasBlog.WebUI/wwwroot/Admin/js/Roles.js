
function myFunction() {
    var m = document.getElementById("0");
    m.style.display = "none";
}

$(document).on("click", ".open-EditCommentDialog", function () {
    var commentName = $(this).data('comment-name');
    var commentId = $(this).data('comment-id');
    var commentEmail = $(this).data('comment-email');
    var commentDate = $(this).data('comment-date');
    var commentMessage = $(this).data('comment-message');
    $(".modal-body #Name").val(commentName);
    $(".modal-body #Id").val(commentId);
    $(".modal-body #Email").val(commentEmail);
    $(".modal-body #dateAdded").val(commentDate);
    $(".modal-body #Message").val(commentMessage);
});

$("#addRole").submit(function (event) {
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
        success: function (role) {
            console.log("başarılıya girdi");
            var trclass = "";
            var bosluk = " ";
            var count = $("#Roles > tr").length;
            if (count % 2 == 0)
                trclass = "odd";
            else
                trclass = "even";

            var veri = "aa";
            var anv = '<tr id="' + role.id + '" class="' + trclass + '">' +
                'td style="width:40%" class="dtr-control sorting_1" tabindex="0">' + role.name + '</td>' +
                'td style="width:15%;text-align:center">' +
                '<a type="button" title="Düzenle" class="btn btn-warning btn-sm open-addRoleModal" data-role-name="' + role.name + '" data-toggle="modal" data-target="#addRoleModal"><i class="fas fa-pencil-alt"></i>Edit</a>' +
                '<a onclick=Delete("/User/RoleDelete/' + role.id + '") title="Sil" class="btn btn-danger btn-sm"><i class="fas fa-trash"></i>Delete</a></td></tr>';
            var veriki = '<tr id="' + role.id + '" class="even">' +
                '<td style="width:40%" class="dtr-control sorting_1" tabindex="0">' + role.name + '</td>' +
                '<td style="width:15%;text-align:center">' +
                '<a type="button" title="Düzenle" class="btn btn-warning btn-sm mr-1 open-addRoleModal" data-role-name="' + role.name + '" data-toggle="modal" data-target="#addRoleModal">' +
                '<i class="fas fa-pencil-alt">' +
                '</i>Edit</a>' +
                '<a onclick="Delete("/User/RoleDelete/' + role.id + '")" title="Sil" class="btn btn-danger btn-sm">' +
                '<i class="fas fa-trash"></i> Delete </a> </td> </tr>';

            $("#Roles").append(veriki);
            $("#addRoleModal").removeClass("in");
            $(".modal-backdrop").remove();
            $('body').removeClass('modal-open');
            $('body').css('padding-right', '');
            $("#addRoleModal").hide();
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
                    toastr.success("İşlem Başarılı");
                },
                error: function (returnData) {
                    toastr.error("İşlem Başarısız, Yönetici ile iletişime geçin.");
                }
            });
        }
    });
}


$("#deleteComment").submit(function (event) {

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
        success: function (Id) {
            var m = document.getElementById(Id);
            m.style.display = "none";
            $("#editCommentModal").removeClass("in");
            $(".modal-backdrop").remove();
            $('body').removeClass('modal-open');
            $('body').css('padding-right', '');
            $("#editCommentModal").hide();
            $("#ajax-loading").hide();
            toastr.success("İşlem Başarılı");
        },
        error: function (ErrorMessage) {
            console.log(ErrorMessage)
            if (ErrorMessage.responseText != "") {
                console.log(ErrorMessage.responseText)
                jQuery.noConflict();
                $('#editCommentModal').modal('hide');
                $("#ajax-loading").hide();
                toastr.error("İşlem Başarısız, Yönetici ile iletişime geçin.");
            }
            else {
                console.log("Else girildi.")
            }
        },
        cache: false,
        contentType: false,
        processData: false
    });
});


