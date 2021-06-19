
$(document).ready(function () {
    $('.js-example-basic-multiple').select2({
        placeholder: ' Select any Category'
    });
});
$(document).ready(function () {
    $('#example').DataTable();
});

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

$("#addBlog").submit(function (event) {
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
        success: function (blog) {
            const date = new Date();
            const formattedDate = date.toLocaleDateString('en-GB', {
                day: '2-digit', month: 'short', year: 'numeric'
            }).replace(/ /g, ' ');
            var trclass = "";
            var bosluk = " ";
            var count = $("#Blogs > tr").length;
            if (count % 2 == 0)
                trclass = "odd";
            else
                trclass = "even";
            var veri = '<tr id="' + blog.id + '" class="' + trclass + '">' +
                '<td style="width:40%"> ' + blog.title + '</td>' +
                '<td style="width:20%"> ' + blog.authorId + '</td>' +
                '<td style="width:20%"> ' + formattedDate + '</td> ' +
                '<td style="width:30%;text-align:center">' +
                '<a type="button" title="Düzenle" class="btn btn-warning btn-sm open-EditCategoryDialog" data-category-name="title 5" data-category-id="6" data-toggle="modal" data-target="#editCategoryModal">' +
                '<i class="fa fa-edit"></i>' +
                '</a>' +
                '<a style="margin-left:3.5px;" onclick=Delete("/Blog/Delete/' + blog.id + '") title="Sil" class="btn btn-danger btn-sm">' +
                '<i class="fa fa-trash-o"></i>' +
                '</a></td></tr>';            
            $("#Blogs").append(veri);
            jQuery.noConflict();
            $('#addBlogModal').modal('hide');
            $("#ajax-loading").hide();
            notie.alert({ type: 'success', text: "Ekleme İşlemi Başarılı", time: 3 }) // Hides after 2 seconds
        },
        error: function (ErrorMessage) {
            console.log(ErrorMessage)
            if (ErrorMessage.responseText != "") {
                console.log(ErrorMessage.responseText)
                jQuery.noConflict();
                $('#addBlogModal').modal('hide');
                $("#ajax-loading").hide();
                notie.alert({ type: 'error', text: "Ekleme başarısız, Bir Sorunla Karşılaşıldı. Yöneticiyle İletişime Geçin" })
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
                    notie.alert({ type: 'success', text: 'Silme İşlemi Başarılı!', time: 3 }) // Hides after 2 seconds
                },
                error: function (returnData) {
                    notie.alert({ type: 'error', text: returnData.responseText })
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
            jQuery.noConflict();
            $('#editCommentModal').modal('hide'); 
            $("#ajax-loading").hide();
            notie.alert({ type: 'success', text: "Silme İşlemi Başarılı", time: 3 }) // Hides after 2 seconds
        },
        error: function (ErrorMessage) {
            console.log(ErrorMessage)
            if (ErrorMessage.responseText != "") {
                console.log(ErrorMessage.responseText)
                jQuery.noConflict();
                $('#editCommentModal').modal('hide');
                $("#ajax-loading").hide();
                notie.alert({ type: 'error', text: "Silme İşlemi başarısız, Bir Sorunla Karşılaşıldı. Yöneticiyle İletişime Geçin" })
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


