
function myFunction() {
    var m = document.getElementById("0");
    m.style.display = "none";
}

$(document).on("click", ".open-editRoleModal", function () {
    //var name = $(this).data('role-name');
    var id = $(this).data('role-id');
    var name = $('#' + id).find('td:first').html();    
    $(".modal-body #Name").val(name);
    $(".modal-body #Id").val(id);
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
            var trclass = "";
            var bosluk = " ";
            var count = $("#Roles > tr").length;
            if (count % 2 == 0)
                trclass = "odd";
            else
                trclass = "even";
            var veriki = '<tr id="' + role.id + '" class="even">' +
                '<td style="width:40%" class="dtr-control sorting_1" tabindex="0">' + role.name + '</td>' +
                '<td style="width:15%;text-align:center">' +
                '<a type="button" title="Düzenle" class="btn btn-warning btn-sm mr-1 open-editRoleModal" data-role-id="' + role.id + '" data-role-name="' + role.name + '" data-toggle="modal" data-target="#editRoleModal">' +
                '<i class="fas fa-pencil-alt">' +
                '</i>Edit</a>' +
                '<a onclick=Delete("/User/DeleteRole/' + role.id + '") title="Sil" class="btn btn-danger btn-sm">' +
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
                    toastr.success("Rol Başarıyla Silindi");
                },
                error: function (errorMessage) {
                    toastr.error(errorMessage.responseText);
                }
            });
        }
    });
}


$("#editRoleForm").submit(function (event) {
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
            var trclass = "";
            var bosluk = " ";
            var count = $("#Roles > tr").length;
            if (count % 2 == 0)
                trclass = "odd";
            else
                trclass = "even";       
            $('#' + role.id).find('td:first').text(role.name);
            $("#editRoleModal").removeClass("in");
            $(".modal-backdrop").remove();
            $('body').removeClass('modal-open');
            $('body').css('padding-right', '');
            $("#editRoleModal").hide();
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

