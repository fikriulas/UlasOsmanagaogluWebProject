$(document).on("click", ".open-EditBlackListIpModal", function () {
    var id = $(this).data('iplist-id');    
    var ip = $('#' + id).find('#ip').html();    
    var note = $(this).data('iplist-note');    
    $(".modal-body #Ip").val(ip);
    $(".modal-body #Id").val(id); 
    $(".modal-body #Note").val(note);    
});

$("#AddBlackListIp").submit(function (event) {
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
            var trclass = "";
            var bosluk = " ";
            var count = $("#ips > tr").length;
            if (count % 2 == 0)
                trclass = "odd";
            else
                trclass = "even";
            var verii = '<tr id="' + ip.id + '" class="' + trclass + '">' +
                '<td id="ip" style="width:20%" class="dtr-control sorting_1" tabindex="0">' + ip.ip + '</td>' +
                '<td id="note" style="width:20%"></td>' +
                '<td id="block" style="width:20%"><a id="status.' + ip.id + '" onclick=ChangeStatu("/Admin/IpListChangeStatus/' + ip.id + '") title="Sil" class="btn btn-danger btn-sm ' + ip.ip +'">Engellendi</a></td>' +
                '<td style="width:20%;text-align:center">' +
                '<a type="button" title="Düzenle" class="btn btn-warning btn-sm open-EditBlackListIpModal" data-iplist-id="' + ip.id + '" data-iplist-ip="' + ip.ip + '" data-iplist-note="' + ip.note + '" data-iplist-block="'+ip.block+'" data-toggle="modal" data-target="#EditBlackListIpModal"><i class="fas fa-pencil-alt"></i>Edit </a>' +
                '<a style="margin-left:3.5px;" onclick=Delete("/Admin/DeleteIpList/' + ip.id + '") title="Sil" class="btn btn-danger btn-sm"><i class="fas fa-trash"></i>Delete</a></td></tr>';
                

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
            if (ErrorMessage.responseText != "") {             

                toastr.error(ErrorMessage.responseText);
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
                    toastr.success("Ip Adresi Listeden Kaldırıldı");
                },
                error: function (errorMessage) {
                    toastr.error(errorMessage.responseText);
                }
            });
        }
    });
}

$("#EditBlackListIp").submit(function (event) {    
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
        success: function (item) {
            $('#' + item.id).find('#ip').text(item.ip);
            $('#' + item.id).find('#note').text(item.note);            
            $("#EditBlackListIpModal").removeClass("in");
            $(".modal-backdrop").remove();
            $('body').removeClass('modal-open');
            $('body').css('padding-right', '');
            $("#EditBlackListIpModal").hide();
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

function ChangeStatu(url) {
    event.preventDefault();    
    $.ajax({
        url: url,
        type: "Post",
        data: $(this).serialize(),
        beforeSend: function () {
            $("#ajax-loading").show();
        },
        complete: function () {
            $("#ajax-loading").hide();
        },
        success: function (item) {
            //var el = document.getElementsByClassName("btn.btn-danger.btn-sm.13");
            var el = document.getElementById("status." + item.id);
            if (item.block) {
                el.textContent = "Engellendi";
                el.className = "btn btn-danger btn-sm "+item.id;
            }
            else {
                el.textContent = "İzin Verildi";
                el.className = "btn btn-success btn-sm " + item.id;
            }            
            toastr.success("Ip adresinin durumu değiştirilmiştir.");
        },
        error: function (errorMessage) {
            toastr.error(errorMessage.responseText);
        }
    });
}