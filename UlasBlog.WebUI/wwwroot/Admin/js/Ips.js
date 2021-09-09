

$("#AddBlackListIp").submit(function (event) {
    console.log("javascript devrede");
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
        success: function (ip) {
            console.log("succes girildi");
            var trclass = "";
            var bosluk = " ";
            var count = $("#ips > tr").length;
            if (count % 2 == 0)
                trclass = "odd";
            else
                trclass = "even";

            /*var verii = '<tr id="' + user.id + '" class="' + trclass + '">' +
                '<td id="username" style="width:20%" class="dtr-control sorting_1" tabindex="0">' + user.userName + '</td>' +
                '<td id="name" style="width:20%">' + user.name + '</td>' +
                '<td id="surname" style="width:20%">' + user.surname + '</td>' +
                '<td>'+user.email+'</td>' +
                '<td style="width:30%;text-align:center">' +
                '<a type="button" title="Düzenle" class="btn btn-warning btn-sm" href="/User/Edit/' + user.id + '"><i class="fas fa-pencil-alt"></i> Edit</a>' +
                '<a style="margin-left:3.5px;" onclick=Delete("/User/DeleteUser/' + user.id + '") title="Sil" class="btn btn-danger btn-sm">' +
                '<i class="fas fa-trash"> Delete</i>' +
                '</a></td></tr>';      */
            var verii = '<tr id="' + ip.id + '" class="' + trclass + '">' +
                '<td id="ip" style="width:20%" class="dtr-control sorting_1" tabindex="0">' + ip.ip + '</td>' +
                '<td id="note" style="width:20%"></td>' +
                '<td id="block" style="width:20%">False</td>' +
                '<td style="width:20%;text-align:center">' +
                '<a type="button" title="Düzenle" class="btn btn-warning btn-sm" href="/User/Edit/4"><i class="fas fa-pencil-alt"></i>Edit</a>' +
                '<a style="margin-left:3.5px;" onclick=Delete("/User/DeleteIpList/' + ip.ip + '") title="Sil" class="btn btn-danger btn-sm"><i class="fas fa-trash"></i>Delete</a></td></tr>';


            $("#ips").append(verii);
            $("#AddBlackListIpModal").removeClass("in");
            $(".modal-backdrop").remove();
            $('body').removeClass('modal-open');
            $('body').css('padding-right', '');
            $("#AddBlackListIpModal").hide();
            $("#ajax-loading").hide();
            toastr.success("İşlem Başarılı");
        },
        error: function (ErrorMessage) {
            console.log(ErrorMessage)
            if (ErrorMessage.responseText != "") {             

                toastr.error(ErrorMessage.responseText);
            }

        },
        cache: false,
        contentType: false,
        processData: false
    });
});

