
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
    console.log("Script'e girdi.")
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

            var veri = '<tr id=' + data.id + ' class=' + trclass + '><td class="sorting_1">' + data.id + '</td><td>' + data.name + '</td><td><a  type="button" title="Düzenle" class="btn btn-warning btn-sm open-EditCategoryDialog" data-category-name="' + data.name + '" data-category-id="' + data.id + '" data-toggle="modal" data-target="#editCategoryModal"><i class="fa fa-edit"></i></a><a style="margin-left:3.5px;" onclick=Delete("/Category/Delete/' + data.id + '") title="Sil" class="btn btn-danger btn-sm"><i class="fa fa-trash-o"></i></a></td></tr>';

            $("#Categories").append(veri);
            $("#addCategoryModal").removeClass("in");
            $(".modal-backdrop").remove();
            $("#addCategoryModal").hide();
            $("#ajax-loading").hide();
            $('#addCategoryModal').modal('hide');
        },
        error: function () {
            console.log("Eklenmedi")
        }
    });
});

function Delete(url) {
    console.log("giriş yapıldu");
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
                    console.log("silme işlemi başarlı")
                    //notie.alert({ type: 'success', text: 'Silme İşlemi Başarılı!', time: 3 }) // Hides after 2 seconds
                },
                error: function () {
                    notie.alert({ type: 'error', text: 'İşlem Başarısız!' })
                    //console.log("İşlem başarısız")
                }
            });
        }
    });
}