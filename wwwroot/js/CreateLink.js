﻿$(document).ready(function () {
    let fullUrl = $("#fullUrl");
    let fullUrl2 = $("#fullUrl2");
    let btnShort = $(".btnShort");
    let form = $("#formFullUrl");


    btnShort.click(function () {
        let link = fullUrl.val();
        let link2 = fullUrl2.val();
        if(link.length === 0){
            fullUrl.val(link2);
            link = link2;
        }
        else{
            fullUrl2.val(link);
            link2 = link;
        }
        if (form.valid()) {
            jQuery.ajax({
                method: "POST",
                url: "Links/Create",
                data: {
                    "FullUrl": link
                },
                success: function (id) {
                    let shortLink = window.location.protocol+"//"+window.location.host +"/"+ id;
                    fullUrl.val(shortLink);
                    fullUrl2.val(shortLink);
                    btnShort.html("Copy");
                    btnShort.off("click");
                    btnShort.click(function () {
                        copyToClipboard(shortLink);
                        btnShort.html("Copied");
                        btnShort.attr("disabled");
                    });
                },
                error: function (error) {
                    var obj = JSON.parse(error.responseText);
                }
            });
        }
    });

    function copyToClipboard(Value) {
        var aux = document.createElement("input");
        aux.setAttribute("value", Value);
        document.body.appendChild(aux);
        aux.select();
        document.execCommand("copy");
        document.body.removeChild(aux);
    }

});