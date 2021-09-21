
$(document).ready(function () {
    $('#example').DataTable();
});

function myFunction() {
    var m = document.getElementById("0");
    m.style.display = "none";

}

$(document).on("click", ".open-EditCategoryDialog", function () {
    var categoryName = $(this).data('category-name');
    var categoryId = $(this).data('category-id');
    $(".modal-body #Name").val(categoryName);
    $(".modal-body #Id").val(categoryId);
});

$("#addCategoryf").submit(function (event) {
    event.preventDefault();
    var form = $(this);
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    var data = { Name: $("#Name").val() };
    var dataWithToken = $.extend(data, { '__RequestVerificationToken': token });
    $.ajax({
        url: form.attr("action"),
        type: "POST",
        data: dataWithToken,
        beforeSend: function () {
            $("#ajax-loading").show();
        },
        complete: function () {
            $("#ajax-loading").hide();
        },
        success: function (data) {
            var trclass = "";
            var bosluk = " ";
            var count = $("#Categories > tr").length;
            if (count % 2 == 0)
                trclass = "odd";
            else
                trclass = "even";

            var veri = '<tr id=' + data.id + ' class=' + trclass + '><td class="sorting_1">' + data.id + '</td><td>' + data.name + '</td><td><a  type="button" title="Düzenle" class="btn btn-info btn-sm open-EditCategoryDialog" data-category-name="' + data.name + '" data-category-id="' + data.id + '" data-toggle="modal" data-target="#editCategoryModal"><i class="fas fa-pencil-alt"></i> Edit</a><a style="margin-left:3.5px;" onclick=Delete("/Category/Delete/' + data.id + '") title="Sil" class="btn btn-danger btn-sm"><i class="fas fa-trash"></i> Delete</a></td></tr>';
            $("#Categories").append(veri);
            $("#addCategoryModal").removeClass("in");
            $(".modal-backdrop").remove();
            $('body').removeClass('modal-open');
            $('body').css('padding-right', '');
            $("#addCategoryModal").hide();
            $("#ajax-loading").hide();
            $("#ajax-loading").hide();
            toastr.success("İşlem Başarılı");            
        },
        error: function () {
            toastr.danger("İşlem Başarısız");    
        }
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
                error: function () {
                    toastr.danger("İşlem Başarısız");            
                }
            });
        }
    });
}