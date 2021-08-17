
$(document).ready(function () {
    $('.js-example-basic-multiple').select2({
        placeholder: ' Select any Category'
    });
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
    document.getElementById("HtmlContent").value = tinymce.activeEditor.getContent();  
    event.preventDefault();
    var form = $(this);
    var formData = new FormData(this);
    formData.append('HtmlContent', 'deger');
    console.log("Form Data: ");
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
                '<a class="btn btn-info btn-sm" type="button" title="Düzenle" class="btn btn-warning btn-sm" href="/Blog/Edit/'+blog.id+'"><i class="fas fa-pencil-alt"></i>Edit</a>'+                                            
                '<a style="margin-left:3.5px;" onclick=Delete("/Blog/Delete/' + blog.id + '") title="Sil" class="btn btn-danger btn-sm">' +
                '<i class="fas fa-trash"> Delete</i>' +
                '</a></td></tr>';      
            $("#Blogs").append(veri);        
            $("#addBlogModal").removeClass("in");
            $(".modal-backdrop").remove();
            $('body').removeClass('modal-open');
            $('body').css('padding-right', '');
            $("#addBlogModal").hide();           
            $("#ajax-loading").hide();
            toastr.success("İşlem Başarılı");            
        },
        error: function (ErrorMessage) {
            console.log(ErrorMessage)
            if (ErrorMessage.responseText != "") {
                console.log(ErrorMessage.responseText)
                jQuery.noConflict();
                $('#addBlogModal').modal('hide');
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

function HtmlSave() {
    console.log("html save");
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
                '<a class="btn btn-info btn-sm" type="button" title="Düzenle" class="btn btn-warning btn-sm" href="/Blog/Edit/' + blog.id + '"><i class="fas fa-pencil-alt"></i>Edit</a>' +
                '<a style="margin-left:3.5px;" onclick=Delete("/Blog/Delete/' + blog.id + '") title="Sil" class="btn btn-danger btn-sm">' +
                '<i class="fas fa-trash"> Delete</i>' +
                '</a></td></tr>';
            $("#Blogs").append(veri);
            $("#addBlogModal").removeClass("in");
            $(".modal-backdrop").remove();
            $('body').removeClass('modal-open');
            $('body').css('padding-right', '');
            $("#addBlogModal").hide();
            $("#ajax-loading").hide();
            toastr.success("İşlem Başarılı");
        },
        error: function (ErrorMessage) {
            console.log(ErrorMessage)
            if (ErrorMessage.responseText != "") {
                console.log(ErrorMessage.responseText)
                jQuery.noConflict();
                $('#addBlogModal').modal('hide');
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
}


