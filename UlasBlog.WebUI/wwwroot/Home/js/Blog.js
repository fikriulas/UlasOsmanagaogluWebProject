$("#AddComment").submit(function (event) {
    event.preventDefault();
    var form = $(this);
    var formData = new FormData(this);
    $.ajax({
        url: form.attr("action"),
        type: 'POST',
        data: formData,
        beforeSend: function () {
           // $("#ajax-loading").show();
        },
        complete: function () {
            //$("#ajax-loading").hide();
        },
        success: function (comment) {       
            var today = new Date();
            var dd = String(today.getDate()).padStart(2, '0');
            var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
            var yyyy = today.getFullYear();
            today = mm + ' ' + dd + ' ' + yyyy;
            $("#noComment").hide();
            var number = parseInt($('#totalComment').text());
            number += 1;

            var veri = '<li id="'+comment.id+'" class="comment">' +
                '<div class="vcard">' +
                '<img src="/Home/images/person_1.jpg" alt="Image placeholder"></div>' +
                '<div class="comment-body">' +
                '<h3>' + comment.name + '</h3>' +
                '<div class="meta">' + today + '</div>' +
                '<p>'+comment.message+'</p></div></li>';
            $("#CommentList").append(veri);
            console.log("Success Girildi")     
            document.getElementById("AddComment").reset();
            iziToast.show({
                title: 'Başarılı!',
                message: 'Yorum Eklendi',
                color: 'green', // blue, red, green, yellow
                icon: 'fa fa-check'
            });            
            var scrollDiv = document.getElementById(comment.id).offsetTop;
            window.scrollTo({ top: scrollDiv, behavior: 'smooth' }); // yorum yazıldığında smooth şekilde yrumun yapıldığı yere sayfayı götürecektir.
        },
        error: function (ErrorMessage) {
            iziToast.show({
                title: 'Başarısız!',
                message: 'Kontrol Edip, Tekrar Deneyin',
                color: 'red', // blue, red, green, yellow
                icon: 'fa fa-times'
            });     
        },
        cache: false,
        contentType: false,
        processData: false
    });
});
var maxchar = 500;
var i = document.getElementById("Comment_Message");
var c = document.getElementById("remainingC");
c.innerHTML = maxchar;
i.addEventListener("keydown", count);
function count(e) {
    var len = i.value.length;
    if (len >= maxchar) {
        e.preventDefault();
    } else {
        c.innerHTML = maxchar - len - 1;
    }
}
