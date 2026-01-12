window.userInteractionTimeout = null;
window.userInteractionInHTMLArea = false;
window.onBrowserHistoryButtonClicked = null;
var signalrConnection = null;

var locationUrl = '';

//$("#metismenu").metisMenu();

$(document).ready(function () {

    //3.kırılım
    var childSubLi = $('#metismenu .menu-item-expanded .childSub');
    $(childSubLi).each(function (i, el) {

        var liSiblings = $(el).siblings('ul');

        $(liSiblings.find('li')).each(function (j, sub) {

            $(liSiblings.find('li').find('ul')).each(function (j, sub1) {

                if ($(sub1).find('a').length == 0) {
                    $(sub1).parent().remove()
                }
            });
        });
    });

    //2.kırılım
    var childSubLi = $('#metismenu .menu-item-expanded .childSub');
    $(childSubLi).each(function (i, el) {

        var liSiblings = $(el).siblings('ul');
        $(liSiblings).each(function (j, sub) {

            if ($(sub).find('a').length == 0) {
                $(sub).parent().remove()
            }
        });
    });

    //1.kırılım
    var allMenuLi = $('#metismenu .menu-item .hasSub,#metismenu .menu-item .childSub');

    $(allMenuLi).each(function (i, el) {
        var liSiblings = $(el).siblings('ul');

        $(liSiblings).each(function (j, sub) {
            if ($(sub).find('a').length == 0) {
                $(sub).parent().remove();
            }
        });
    });


    if ($(window).width() >= 995) {
        if (typeof (Storage) !== "undefined") {
            if (localStorage.getItem("menu") == "close") {
                $('body').addClass('collapsed').removeClass('mopen');
                navbarClose();
            } else {
                $('body').removeClass('collapsed').addClass('mopen');
                navbarOpen();
            }
        }
    }

    //tableti yan çevirmede ekran yenileme için eklendi

    var lastWidth = $(window).width();
    $(window).resize(function () {
        if (lastWidth < 1100) {
            if ($(window).width() != lastWidth) {
                location.reload();
            }
        }
    });

    $(document).mousedown(function () {
        clearTimeout(window.userInteractionTimeout);
        window.userInteractionInHTMLArea = true;
        window.userInteractionTimeout = setTimeout(function () {
            window.userInteractionInHTMLArea = false;
        }, 500);
    });

    $(document).keydown(function () {
        clearTimeout(window.userInteractionTimeout);
        window.userInteractionInHTMLArea = true;
        window.userInteractionTimeout = setTimeout(function () {
            window.userInteractionInHTMLArea = false;
        }, 500);
    });

    if (window.history && window.history.pushState) {
        $(window).on('popstate', function () {
            if (!window.userInteractionInHTMLArea) {
                window.location = locationUrl;
            }
            if (window.onBrowserHistoryButtonClicked) {
                window.onBrowserHistoryButtonClicked();
            }
        });
    }
    selectControl();
    tabControl();
    menunoSub();
    LoadCustomer();

    var twStyle = $('#twStyle').attr('href').split('-');
    var styleUrl = twStyle[0] + '-';

    $('#nightmode').change(function () {
        if ($('#nightmode').is(':checked')) {
            $('#twStyle').attr('href', styleUrl + twStyle[1].replace('dark', 'light'));
            localStorage.setItem("nightmode", "close");
        } else {
            $('#twStyle').attr('href', styleUrl + twStyle[1].replace('light', 'dark'));
            localStorage.setItem("nightmode", "open");
        }
    })

    if (typeof (Storage) !== "undefined") {
        if (localStorage.getItem("nightmode") == undefined) {
            $('#twStyle').attr('href', styleUrl + twStyle[1].replace('dark', 'light'));
            $('#nightmode').prop('checked', true);
        } else {
            if (localStorage.getItem("nightmode") == "close") {
                $('#twStyle').attr('href', styleUrl + twStyle[1].replace('dark', 'light'));
                $('#nightmode').prop('checked', true);
            } else {
                $('#twStyle').attr('href', styleUrl + twStyle[1].replace('light', 'dark'));
                $('#nightmode').prop('checked', false);
            }
        }
    }

    var bgeffect = $('.bg-effect');
    var userOptions = $('.account-options');
    var moduleButton = $(".moduleButton");

    $('.userName').click(function () {
        if (userOptions.hasClass('d-none')) {
            userOptions.removeClass('d-none');
            moduleButton.css({ "z-index": "3" });
            $('.nightmodebox').css({ "z-index": "4" });
            bgeffect.fadeIn();
        } else {
            userOptions.addClass('d-none');
            bgeffect.fadeOut();
            setTimeout(function () {
                moduleButton.css({ "z-index": "" });
                $('.nightmodebox').css({ "z-index": "" });
            }, 300)
        }
    })

    bgeffect.click(function () {
        $('.modules').hide();
        moduleButton.removeClass('module-open');
        userOptions.addClass('d-none');
        moduleButton.css({ "z-index": "" });
        bgeffect.css({ "z-index": "", "background-color": "", "opacity": "", "display": "" });
        bgeffect.fadeOut();
    });

    //navbar Position
    var scrollNav = localStorage.getItem("navPosition");
    $(".sideNav").animate({ scrollTop: scrollNav }, 100);

    jQuery.fn.DataTable.ext.type.search.string = function (data) {
        var testd = !data ?
            '' :
            typeof data === 'string' ?
                data
                    .replace(/i/g, 'İ')
                    .replace(/ı/g, 'I') :
                data;
        return testd;
    };

    signalrConnection = new signalR.HubConnectionBuilder()
        .withUrl(`${apiAddress}hubs/advancedreport`, { withCredentials: true })
        .withAutomaticReconnect()
        .build();

    signalrConnection.start().then(function () {
        console.log("Bağlantı sağlandı.");
    }).catch(function (err) {
        console.log("Hata oluştu:" + err);
    });

    MesajlariYukle();
});


function MesajlariYukle() {
    signalrConnection.on("ReceiveMessage", function (userid, corpid, message) {
        if (userid == mainUserId) {
            alert(`${userid}, ${corpid}, ${message}`);
        }
    });
}

function menunoSub() {
    var menu = $('#metismenu');
    var activeli = menu.find('li.mm-active:not(.menu-item-expanded)');
    var hassub = menu.find('li.mm-active a:not(.menu-item-expanded a)').hasClass('hasSub');
    if (!hassub) {
        activeli.addClass('noSub');
    }
}

function navbarOpen() {
    $('.opacity-screen').fadeOut();
    $('.mm-active ul:not(.menu-item-expanded ul)').addClass('mm-show');
    $('.menu-open').show();
    $('.menu-closed').hide();
    subControl();
}

function navbarClose() {
    $('#metismenu li').removeClass('expitem');
    setTimeout(function () {
        $('.mm-active ul:not(.menu-item-expanded ul)').removeClass('mm-show');
    })
    $('.menu-open').hide();
    $('.menu-closed').show();
    //$('aside').css({ "z-index": "" });
    $('aside').removeProperty('z-index');
    subControl();
}
function subControl() {
    $('.menu-item-expanded').each(function (i, exp) {
        if ($(exp).find('ul li a').hasClass('active')) {
            $(exp).find('.childSub').addClass('active');
            $(exp).find('ul').addClass('mm-show');
        } else {
            $(exp).find('.childSub').removeClass('active');
            $(exp).find('ul').removeClass('mm-show');
        }
    })
}
if ($(window).width() <= 995) {

    $('.menu-closed').click(function (e) {
        e.preventDefault;
        var cvalue = "";
        if ($(window).width() <= 690) {
            cvalue = "67"
        } else if ($(window).width() <= 995) {
            cvalue = "33"
        }
        $('.opacity-screen').fadeIn('slow');
        $('aside').css({ "z-index": "24" });
        $('aside').css('left', '0%')
        $(this).hide();
        $('.menu-open').css('display', 'block');
        $('.collapseButton').css('display', 'none');
        setTimeout(function () {
            $(".collapseButton").fadeIn('slow');
            $('.collapseButton').css({ 'left': cvalue + '%' });
        }, 300);
    });

    $('.menu-open').click(function (e) {
        e.preventDefault;
        $('.opacity-screen').fadeOut();
        //$('aside').css({ "z-index": "2" });
        $('aside').css('left', '-100%')
        $(this).hide();
        $('.menu-closed').css('display', 'block');
        $('.collapseButton').css('display', 'none');
        setTimeout(function () {
            $(".collapseButton").fadeIn('slow');
            $('.collapseButton').css('left', '15px');
        }, 300);
    });
    $('.opacity-screen').click(function () {
        $('.opacity-screen').fadeOut();
        $('aside').css('left', '-100%')
        $('.menu-open').css('display', 'block');
        $('.menu-closed').css('display', 'none');
        $('.collapseButton').css('display', 'none');
        setTimeout(function () {
            $(".collapseButton").fadeIn('slow');
            $('.collapseButton').css('left', '15px');
        }, 300);

    });
} else {
    subControl();
    $('#metismenu .menu-item').on('click', function () {

        if ($(this).hasClass('mm-active')) {
            var itemTop = $(this).offset().top;
            if (itemTop >= 390) {
                setTimeout(function () {
                    $(".sideNav").animate({ scrollTop: itemTop }, 100);
                }, 100);
            }

            var currentTop = $('.sideNav').scrollTop();
            localStorage.setItem("navPosition", currentTop);
        }
        menunoSub();
        if ($('body').hasClass('collapsed')) {
            $('.childSub').click(function () {
                $(this).siblings('ul').toggleClass('mm-show');
                return false;
            })

            if (!$(this).hasClass('expitem')) {
                $('#metismenu li').removeClass('mm-active expitem');
                $(this).addClass('mm-active expitem');
                setTimeout(function () {
                    $('.mm-active ul:not(.menu-item-expanded ul)').addClass('mm-show');
                }, 50);
                $('.opacity-screen').show();
                $('aside').addClass('subOpen');
            } else {
                $('.opacity-screen').hide();
                $('aside').css({ "z-index": "24" });
                $(this).removeClass('expitem');
                $('.collapseButton svg path').css('fill', '');
                $('aside').removeClass('subOpen');
            }
            $('#metismenu li ul.mm-collapse').removeClass('mm-show');
            subControl();
        } else {
            $('#metismenu li').removeClass('expitem');
        }
    })
    $('.collapseButton a').click(function (e) {
        e.preventDefault;
        $('body').toggleClass('collapsed mopen');

        $('.menu-open').toggle();
        $('.menu-closed').toggle();

        $('aside').removeClass('subOpen');
        if ($('body').hasClass('collapsed')) {
            localStorage.setItem("menu", "close");
            navbarClose();
        } else {
            localStorage.setItem("menu", "open");
            navbarOpen();
        }
    });

    $('.opacity-screen').click(function () {
        $('.opacity-screen').hide();
        $('aside').removeClass('subOpen');
        $('#metismenu li').removeClass('expitem');
        $('.mm-active ul').removeClass('mm-show');
    })
}

$('#recordCreate').click(function (e) {
    e.preventDefault();
    var href = $(this).attr('href');
    var hash = $(this).data("data-hash");

    if (!hash) hash = 'create';

    $.ajax({
        url: href,
        type: 'get',
        success: function (data, status, xhr) {
            locationUrl = window.location.href;
            window.history.pushState('forward', null, '#' + hash);

            formControl(data, hash);
        },
    });
});

function recordEdit() {
    var id = $(this).data("data-id");
    var controller = $(this).data("data-controller");
    var action = $(this).data("data-action");
    var hash = $(this).data("data-hash");

    if (!hash) hash = 'edit';

    $.ajax({
        url: '/' + controller + '/' + action + '?id=' + id,
        type: 'get',
        success: function (data) {
            locationUrl = window.location.href;
            window.history.pushState('forward', null, '#' + hash);

            formControl(data, hash);
        }
    });
}

function recordDelete() {
    var id = $(this).data("data-id");
    var controller = $(this).data("data-controller");
    var action = $(this).data("data-action");

    Swal.fire({
        title: '<span class="ff-bold" style="color:rgb(237,27,46); font-size:21px">' + loc_DoYouApproveDeletion+'</span>',
        text: loc_CantUndoAction,
        showCancelButton: true,
        cancelButtonText: loc_Cancel,
        confirmButtonText: loc_Approve,
        confirmButtonColor: 'rgb(237,27,46)',
        imageUrl: "../../img/ghostQuestionDelete.svg",
        width: '760px'
    }).then(function (result) {
        if (result.value) {
            $.ajax({
                type: 'post',
                url: '/' + controller + '/' + action + '?id=' + id,
                success: function (result) {
                    if (result.data != null) {
                        if (result.data.result == false) {
                            toastr.error(result.data.message);
                        } else {
                            $(".recordTable").DataTable().ajax.reload();
                            toastr.success(result.data.message);
                        }
                    } else {
                        toastr.error(loc_NotAuthTransaction);
                    }
                },
                error: function (response) {
                    if (response.responseText != '') {
                        toastr["error"](response.responseText);
                    }
                }
            });
        }
    })
}

function BeforeSubmitForm() {
    var record_form = $('.recordForm');
    var validator = record_form.validate({
        focusInvalid: true,
        invalidHandler: function () {
            $(this).find(":input.error:first").focus();
        }
    });

    if (record_form.valid() == true) {
        $(".formSave").attr("disabled", true);
        return true;
    } else {
        validator.focusInvalid();
        return false;
    }
}

function SubmitSuccesful(result, statusText) {
    $(".formSave").attr("disabled", false);
    if (result.data.responseDataType == 1) {
        $(".recordTable").DataTable().ajax.reload();
        toastr["success"](result.data.message);

        $('#formContent').html('');
        $('#thinkContent').show();
    } else {

        if (result.data.result != null)
            formControl(result.data.result);

        if (result.data.message != null) {
            toastr["error"](result.data.message);
        }

    }
}

function AjaxError(result) {
    $(".formSave").attr("disabled", false);
    toastr["error"]('' + loc_UnexpectedError+'');
}

function tableOptionActions(table) {
    $(".table-search").on("keyup", function () {
        table.search(jQuery.fn.DataTable.ext.type.search.string(this.value)).draw();
    });

    $(".table-print").on("click", function (e) {
        //e.preventDefault(), table.button(0).trigger();
        window.print();
    });

    $(".table-excel").on("click", function (e) {
        e.preventDefault(), table.button(1).trigger();
    });

    $(".table-csv").on("click", function (e) {
        e.preventDefault(), table.button(2).trigger();
    });

    $(".table-pdf").on("click", function (e) {
        e.preventDefault(), table.button(3).trigger();
    });

    $(".table-refresh").on("click", function (e) {
        table.ajax.reload();
    });
}

function formControl(data, hash = 'edit') {
    if (data.includes('l-login')) {
        location.reload();
    }
    else {
        if (hash == 'edit' || hash == 'create') {
            $('#formContent').html(data);
            $('#formContent').show();
            $('#thinkContent').hide();
        }
        else {
            $('#thinkContent').html(data);
            $('#thinkContent').show();
            $('#formContent').hide();
        }

        selectControl();

        $('.recordForm').ajaxForm({
            beforeSubmit: BeforeSubmitForm,
            success: SubmitSuccesful,
            error: AjaxError
        });

        tabControl();

        if (typeof (customFormControl) !== 'undefined') {
            customFormControl();
        }
    }
}

function tabControl() {

    $(".tab-ver li:first").addClass("active");

    $(".tab-ver li button ").click(function (event) {
        event.preventDefault();
        $(this).parent().addClass("active");
        $(this).parent().siblings().removeClass("active");
        var tab = $(this).attr("data-tab");
        $(".tab-content").css("display", "none");
        $('#' + tab).css("display", "block");
        $(tab).fadeIn();

        if ($(".tab-ver li.active button").hasClass('btn-secondary')) {
            return false;
        } else {
            $(".tab-ver li button").removeClass('btn-secondary').addClass('btn-passive');
            $(".tab-ver li.active button").toggleClass('btn-passive btn-secondary');
        }
        $(".tab-hor-inline .tab-hor li:first").addClass("active");
        $(".tab-hor-inline .tab-content:first").addClass("current");

        selectControl();

    });


    $(".tab-hor li button ").click(function () {
        event.preventDefault();
        $(this).parent().addClass("active");
        $(this).parent().siblings().removeClass("active");
        var tab = $(this).attr("data-tab");
        $(".tab-hor-inline .tab-content").css("display", "none");
        $(".tab-hor-inline .tab-content").removeClass("current");
        $('#' + tab).css("display", "block");
        $(tab).fadeIn();

        if ($(".tab-hor li.active button").hasClass('btn-secondary')) {
            return false;
        } else {
            $(".tab-hor li button").removeClass('btn-secondary').addClass('btn-passive');
            $(".tab-hor li.active button").toggleClass('btn-passive btn-secondary');
        }

        selectControl();

    });

}

function selectControl() {

    //görüntülemebutonuurl control
    var controlButton = $('.btnView');

    if (controlButton.length != 0) {

        $(controlButton).each(function (i, el) {
            var currentUrl = $(el).data('url');
            var urlControl = currentUrl.split('.').pop(-1);

            if (
                urlControl == 'png' || urlControl == 'svg' || urlControl == 'jpg' || urlControl == 'jpeg' ||
                urlControl == 'PNG' || urlControl == 'SVG' || urlControl == 'JPG' || urlControl == 'JPEG'
            ) {

                $(el).attr('data-lightbox', currentUrl);
            }
        })


    }


    //ortak toggle
    $('.toggleBtn').on('click', function () {
        var parent = $(this).parents('.white-bg');
        $(parent).find('.toggleContent').stop().slideToggle();
        $(this).find('i').toggleClass('fa-chevron-up fa-chevron-down');
    });
    //form boşsa silme
    var allForms = $('.row.white-bg:not(".row.white-bg.questionCard"):not(".row.dynamicForm")');
    $(allForms).each(function (j, form) {
        if ($(form).find('input').length == 0 && $(form).find('select').length == 0) {
            $(this).remove();
        }
    });

    var readURL = function (input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('.profile-pic').attr('src', e.target.result);
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

    $(".file-upload").on('change', function () {
        readURL(this);
    });

    $(".upload-button").off("click").click(function () {
        $(".file-upload").click();
    });

    $('.only-text').keypress(function (e) {
        var regex = new RegExp("[(a-z)(A-Z)(\ğ\Ğ\\ç\Ç\ş\Ş\ü\Ü\ö\Ö\ı\İ)]\d*$");
        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }
        e.preventDefault();
        return false;
    });

    getOnlyNumeric();

    var cssStartDate = $('.start-date');
    var cssEndDate = $('.end-date');

    cssStartDate.datepicker({
        language: "tr-TR",
        autoHide: true,
        format: "dd.mm.yyyy",
        firstDay: 1
    });
    cssEndDate.datepicker({
        language: "tr-TR",
        autoHide: true,
        format: "dd.mm.yyyy",
        firstDay: 1,
        startDate: cssStartDate.datepicker('getDate'),
    });

    cssStartDate.on('change', function () {
        cssEndDate.datepicker('setStartDate', cssStartDate.datepicker('getDate'));
    });

    if (document.URL.indexOf("formworkflow#edit") != -1) {
        $("section").css("display", "table");
    } else if (document.URL.indexOf("leave/leavegroup") != -1) {
        $("tr.group").css("background-color", "#ddd !important");
        $("tr.group:hover").css("background-color", "#ddd !important");
    }

    $('.twselect').chosen({
        no_results_text: "" + loc_Choose +"...",
        placeholder_text_single: "" + loc_Choose +"...",
        placeholder_text_multiple: "" + loc_Choose +"..."
    });

    $('.twselect').each(function () {
        $(this).next().append('<label>' + $(this).prev().html() + '</label>');
    });

    $(".custdatepicker").datepicker({
        language: "tr-TR",
        autoHide: true,
        format: "dd.mm.yyyy",
        firstDay: 1
    });


    triggerTimePicker();

    $('.timefol_button').unbind('click');
    $('.timefol_button').on('click', function (e) {
        $('.timefol_button_open').html("Gizle");
        $('.timefol_button_close').html("Göster");
        e.preventDefault();
        var button = $(this).find('i');
        var span = $(this).find('span');
        var content = $('.time_col');
        content.slideToggle(200);
        span.toggleClass('timefol_button_open').toggleClass('timefol_button_close');
        button.toggleClass('fa-chevron-up').toggleClass('fa-chevron-down');
    });

    $(":input").inputmask();
    $(".phone-mask").inputmask({
        "mask": "(999) 999-9999",
        "removeMaskOnSubmit": true
    });

    //datepicker mask
    $('.custdatepicker').val(function (i, n) {
        var result = n;
        if (n != '' || n == null || n == undefined) {
            if (n.length < 10) {
                return "0" + result;
            } else {
                return result;
            }
        }

    }).mask('00.00.0000');


    //select disable özelliği
    $('.twselect').each(function (i, tw) {
        if ($(tw).siblings('.chosen-container').hasClass('chosen-disabled')) {
            $(tw).parent().addClass('disableInput')
        }
    });
    //input disable
    $('input[disabled]').each(function (i, input) {
        $(input).parent().addClass('disableInput');
    });



    $.validator.setDefaults({ ignore: ":hidden:not(select)" });

    $.fn.dataTable.ext.errMode = function (settings, helpPage, message) {
        if (helpPage == 1) {
            location.reload();
        }
    };
}


function triggerTimePicker() {
    $('.timepicker-input').each(function (ix, el) {
        $(el).timepicker({
            minuteStep: 1,
            template: 'dropdown',
            appendWidgetTo: 'body',
            showSeconds: false,
            showMeridian: false,
            defaultTime: false,
            icons: {
                up: 'fa fa-chevron-up',
                down: 'fa fa-chevron-down'
            }
        });
    })
}


function getOnlyNumeric() {
    $('.only-numeric').keypress(function (e) {
        var regex = new RegExp("[(0-9)(\.)(\,)]\d*$");
        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }
        e.preventDefault();
        return false;
    });
}

function dataTableRenderTime(data, type, dataToSet) {
    var hour = '';
    var minute = '';

    if (data.hours != undefined) {
        if (data.hours.toString().length == 1) {
            hour = '0' + data.hours.toString();
        }
        else {
            hour = data.hours;
        }

        if (data.minutes.toString().length == 1) {
            minute = '0' + data.minutes.toString();
        }
        else {
            minute = data.minutes;
        }

        return hour + ':' + minute;
    } else {
        return '';
    }
}

function dataTableRenderDate(data, type, dataToSet) {
    if (data != undefined) {
        var newDate = new Date(data);
        var day = newDate.getDate().toString();
        var month = (newDate.getMonth() + 1).toString();
        var year = newDate.getFullYear().toString();

        day = day.length == 1 ? '0' + day : day;
        month = month.length == 1 ? '0' + month : month;

        return day + '.' + month + '.' + year;
    } else {
        return '';
    }
}

function dataTableRenderHourDecimalToHourStr(data) {
    var hour = '';
    var minute = '';
    if (data != undefined) {
        data = data.toFixed(2);
        var time = data.toString().split('.');
        if (time[0].length == 1) {
            hour = '0' + time[0];
        }
        else {
            hour = time[0];
        }

        minute = Math.round(time[1] * 6 / 10).toString();
        if (minute.length == 1) {
            minute = '0' + minute;
        }

        return hour + ':' + minute;
    }
    else {
        return '';
    }
}

function dataTableRenderTimeForNullable(data, type, dataToSet) {
    var hour = '';
    var minute = '';
    var v_data = data.value;
    if (v_data != undefined) {

        if (v_data.hours.toString().length == 1) {
            hour = '0' + v_data.hours.toString();
        }
        else {
            hour = v_data.hours;
        }

        if (v_data.minutes.toString().length == 1) {
            minute = '0' + v_data.minutes.toString();
        }
        else {
            minute = v_data.minutes;
        }

        return hour + ':' + minute;
    }
    else {
        return '';
    }
}
function dataTableRenderTimeFromDate(data, type, dataToSet) {
    if (data != undefined) {
        var newDate = new Date(data);

        var hour = '';
        var minute = '';

        if (newDate.getHours().toString().length == 1) {
            hour = '0' + newDate.getHours().toString();
        }
        else {
            hour = newDate.getHours();
        }

        if (newDate.getMinutes().toString().length == 1) {
            minute = '0' + newDate.getMinutes().toString();
        }
        else {
            minute = newDate.getMinutes();
        }

        return hour + ':' + minute;
    } else {
        return '';
    }
}

function dataTableRenderDateAndTime(data, type, dataToSet) {
    if (data != undefined) {
        var newDate = new Date(data);
        var day = newDate.getDate().toString();
        var month = (newDate.getMonth() + 1).toString();
        var year = newDate.getFullYear().toString();

        day = day.length == 1 ? '0' + day : day;
        month = month.length == 1 ? '0' + month : month;


        var hour = '';
        var minute = '';

        if (newDate.getHours().toString().length == 1) {
            hour = '0' + newDate.getHours().toString();
        }
        else {
            hour = newDate.getHours();
        }

        if (newDate.getMinutes().toString().length == 1) {
            minute = '0' + newDate.getMinutes().toString();
        }
        else {
            minute = newDate.getMinutes();
        }

        return day + '.' + month + '.' + year + ' ' + hour + ':' + minute;
    } else {
        return '';
    }
}

function getDateAndTimeStr(newDate, type, dataToSet) {
    if (newDate != undefined) {
        var day = newDate.getDate().toString();
        var month = (newDate.getMonth() + 1).toString();
        var year = newDate.getFullYear().toString();

        day = day.length == 1 ? '0' + day : day;
        month = month.length == 1 ? '0' + month : month;


        var hour = '';
        var minute = '';

        if (newDate.getHours().toString().length == 1) {
            hour = '0' + newDate.getHours().toString();
        }
        else {
            hour = newDate.getHours();
        }

        if (newDate.getMinutes().toString().length == 1) {
            minute = '0' + newDate.getMinutes().toString();
        }
        else {
            minute = newDate.getMinutes();
        }

        return year + '-' + month + '-' + day + 'T' + hour + ':' + minute + ':00';
    } else {
        return '';
    }
}

function dataTableRenderDateAndTime2(data) {
    if (data != undefined) {
        var h = data.hours;
        var m = data.minutes;
        if (h < 10)
            h = '0' + h;
        if (m < 10)
            m = '0' + m;
        return h + ':' + m;
    }
    else {
        return '';
    }

}

function renderTime(data, type, dataToSet) {
    var hour = '';
    var minute = '';

    if (data.getHours() != undefined) {
        if (data.getHours().toString().length == 1) {
            hour = '0' + data.getHours().toString();
        }
        else {
            hour = data.getHours();
        }

        if (data.getMinutes().toString().length == 1) {
            minute = '0' + data.getMinutes().toString();
        }
        else {
            minute = data.getMinutes();
        }

        return hour + ':' + minute;
    } else {
        return '';
    }
}

function mediaUpload(id) {
    $("#" + id).click();
    $("#" + id).on('change', function () {
        $("#fileplace_" + id).attr("placeholder", $('#' + id).val().replace(/.*(\/|\\)/, ''));
    });
}

jQuery.ajaxSetup({
    beforeSend: function (jqXHR, settings) {
        $('body').removeClass('loaded');
        $('#loader').html('<img src="../img/loading.gif"/><h6>' + loc_pleaseWait + '</h6>');
    },
    complete: function () {
        $('#loader').html('');
        $('body').addClass('loaded');

        popoverTrigger();
    },
    success: function (result, status, xhr) {
        if (result != undefined) {
            if (result.includes("l-login")) {
                location.reload();
            }
        }
    },
    error: function (result, status, xhr) {
        handleAjaxError(result, status, xhr);
    }
});

$('a[data-person]').on('click', function () {
    var id = $(this).data("person");
    if ($('#' + id).css('display') === 'none') {

        $('#' + id).hide();
        currr = $(this).closest("div.mini-c");
        currr.removeClass("bottom-line");

    } else if ($('#' + id).css('display') === 'flex') {
        $('#' + id).show();
        $('.mini-c').removeClass("bottom-line");
        currr = $(this).closest("div.mini-c");
        currr.addClass("bottom-line");
    }
});

function randomString() {
    var length = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : 32;
    var result = '';
    var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var charactersLength = characters.length;

    for (var i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }

    return result;
}

String.prototype.includes = function (str) {
    var returnValue = false;

    if (this.indexOf(str) !== -1) {
        returnValue = true;
    }

    return returnValue;
}

Date.prototype.getWeek = function () {
    var onejan = new Date(this.getFullYear(), 0, 1);
    return Math.ceil((((this - onejan) / 86400000) + onejan.getDay() + 1) / 7);
}

function dTableStatusFieldControl(item) {

    if (item == true) {
        item = "" + loc_Active + ""
    } else if (item == false) {
        item = "" + loc_Passive + ""
    }

    switch (item) {
        case "" + loc_Approvedd + "":
            return '<div class="badge badge-xs badge-success">' + item + '</div>'
            break;
        case "" + loc_Denied + "":
            return '<div class="badge badge-xs badge-danger">' + item + '</div>'
            break;
        case "" + loc_WaitingForApproval + "":
            return '<div class="badge badge-xs badge-warning">' + item + '</div>'
            break;
        case "" + loc_Active + "":
            return '<div class="badge badge-xs badge-success">' + item + '</div>'
            break;
        case "" + loc_Passive + "":
            return '<div class="badge badge-xs badge-lightgray">' + item + '</div>'
            break;
        default:
            return '<div class="badge badge-xs badge-danger">' + item + '</div>'
            break;
    }
}
function dTableStatusExistFieldControl(item) {
    if (item == true)
        return '<div class="badge badge-xs badge-success">' + loc_Exist + '</div>';
    else if (item == false)
        return '<div class="badge badge-xs badge-lightgray">' + loc_Absent + '</div>';
}
function dTableStatusYesNoFieldControl(item) {
    if (item == true)
        return '<div class="badge badge-xs badge-success">' + loc_Yes + '</div>';
    else if (item == false)
        return '<div class="badge badge-xs badge-lightgray">' + loc_No + '</div>';
}
function dTableStatusReverseYesNoFieldControl(item) {
    if (item == true)
        return '<div class="badge badge-xs badge-lightgray">' + loc_No + '</div>';
    else if (item == false)
        return '<div class="badge badge-xs badge-success">' + loc_Yes + '</div>';
}


function getFormatedDate(date) {
    var dayOfWeek = date.getDay() == 0 ? 7 : date.getDay();
    var month = date.getMonth() + 1;
    var day = date.getDate();

    var dayOfWeekStr = '';
    var monthOfStr = '';

    switch (dayOfWeek) {
        case 1:
            dayOfWeekStr = '' + loc_Monday+'';
            break;

        case 2:
            dayOfWeekStr = '' + loc_Tuesday+'';
            break;

        case 3:
            dayOfWeekStr = '' + loc_Wednesday+'';
            break;

        case 4:
            dayOfWeekStr = '' + loc_Thursday+'';
            break;

        case 5:
            dayOfWeekStr = '' + loc_Friday+'';
            break;

        case 6:
            dayOfWeekStr = '' + loc_Saturday+'';
            break;

        case 7:
            dayOfWeekStr = '' + loc_Sunday+'';
            break;

        default:
            dayOfWeekStr = '';
            break;
    }


    switch (month) {
        case 1:
            monthOfStr = '' + loc_January+'';
            break;

        case 2:
            monthOfStr = ''+loc_February+'';
            break;

        case 3:
            monthOfStr = '' + loc_March+'';
            break;

        case 4:
            monthOfStr = '' + loc_April+'';
            break;

        case 5:
            monthOfStr = '' + loc_May+'';
            break;

        case 6:
            monthOfStr = '' + loc_June+'';
            break;

        case 7:
            monthOfStr = '' + loc_July+'';
            break;

        case 8:
            monthOfStr = '' +loc_August+'';
            break;

        case 9:
            monthOfStr = '' + loc_September+'';
            break;

        case 10:
            monthOfStr = '' + loc_October+'';
            break;

        case 11:
            monthOfStr = '' + loc_November +'';
            break;

        case 12:
            monthOfStr = ''+loc_December+'';
            break;

        default:
            monthOfStr = '';
            break;
    }

    return day + ' ' + monthOfStr + ' ' + dayOfWeekStr;
}
function getFormatedMonth(month) {
    var monthOfStr = '';
    switch (month) {
        case 1:
            monthOfStr = '' + loc_January + '';
            break;

        case 2:
            monthOfStr = '' + loc_February+'';
            break;

        case 3:
            monthOfStr = '' + loc_March + '';
            break;

        case 4:
            monthOfStr = '' + loc_April + '';
            break;

        case 5:
            monthOfStr = '' + loc_May + '';
            break;

        case 6:
            monthOfStr = '' + loc_June + '';
            break;

        case 7:
            monthOfStr = '' + loc_July + '';
            break;

        case 8:
            monthOfStr = '' + loc_August + '';
            break;

        case 9:
            monthOfStr = '' + loc_September + '';
            break;

        case 10:
            monthOfStr = '' + loc_October + '';
            break;

        case 11:
            monthOfStr = '' + loc_November + '';
            break;

        case 12:
            monthOfStr = '' + loc_December + '';
            break;

        default:
            monthOfStr = '';
            break;
    }

    return monthOfStr;
}

function permissionFieldControl(viewName, fields) {
    var result = null;

    $.ajax({
        type: "POST",
        url: "/user/getViewPermissionFieldControl",
        data: { "viewName": viewName, "fields": fields },
        async: false,
        beforeSend: function (xhr, settings) { }, //loader çalışmaması için before ve comp. metotları ezildi
        complete: function () {
            popoverTrigger();
        },
        success: function success(res) {
            result = res.data;
        }
    });

    return result;
}

function handleAjaxError(result, status, xhr) {

    if (result.status == 401) 
        window.location.href = '/auth/login'

    else if (result.status == 403) 
        toastr.error(loc_NotAuthTransaction);

    else
        toastr.error(result.responseText);
}

function bottomModal(header, content, footer, center, fbuttoncount) {
    var footerButtons;
    if (center) {
        center = 'center';
    } else {
        center = '';
    }

    if (fbuttoncount == 1 && footer == '') {
        footerButtons =
            ' <button type="button" class="btn btn-secondary w-100" onclick="$.modal.close();">' + loc_OK+'</button>';
    } else {
        footerButtons = footer +
            '<button type="button" class="btn btn-secondary modal-cancel" onclick="$.modal.close();">' + loc_Close+'</button>';

    }

    var modal =
        '<div class="modal-header ' + center + '">'
        + header +
        '</div>' +
        '<div class="modal-content">'
        + content +
        '</div>' +
        '<div class="modal-footer">'
        + footerButtons +
        '</div>';

    return modal;

}

function popoverTrigger() {
    if ($(window).width() >= 995) {
        $('[data-title][data-title!=""]').each(function (ix, el) {
            var content = $(el).data('title')
            $(el).popover({
                content: content,
                autoPlace: true,
                trigger: 'hover',
                placement: 'top',
                delay: 100
            });
        });
    }
}

function roundN(num, n) {
    num = num.replace(',', '.');
    return parseFloat(Math.round(num * Math.pow(10, n)) / Math.pow(10, n)).toFixed(n);
}


$('.dateFilterSelect').on('change', function () {
    var _val = $(this).val();
    if (_val == -1) {
        $(this).parents('.white-bg').find('.dateFilterInput').slideDown();
    } else {
        $(this).parents('.white-bg').find('.dateFilterInput').slideUp();
    }
})


//loader
function loader(element, status) {
    if (status) {
        element.style.position = 'relative';
        element.setAttribute('disabled', true);
        var wrapper = document.createElement('div');
        wrapper.classList.add('buttonLoader');

        var loader = document.createElement('div');
        loader.classList.add('loading');

        wrapper.appendChild(loader);

        element.appendChild(wrapper);
    } else {

        element.querySelector('.buttonLoader').remove();
        element.style.position = '';
        element.removeAttribute('disabled');
    }

}

//dropdown
$(document).on('click', '.dropdown-btn', function (e) {
    $('.dropdown-content').hide()
    $(this).parent().find('.dropdown-content').show()
})

$(document).on('click', function (e) {
    if (!e.target.matches('.dropdown-btn') && !e.target.matches('.dropdown-content')) {
        if ($(e.target).parents('.dropdown-clickable .dropdown-content').length < 1) {
            $('.dropdown-clickable .dropdown-content').hide()
        }
    }   
})

function LoadCustomer() {
    var loadSelectId = "#LayoutCustomer";

    if (isCorpAdmin == 1 || isHrRole == 1) {
        $.ajax({
            url: '/customer/data',
            type: "get",
            success: function (result) {
                $(loadSelectId).empty();
                var optionText = loc_All + '...';
                let defaultOption = new Option(optionText,
                    "", false, false);
                $(loadSelectId).append(defaultOption);

                if (result.data.length > 0) {
                    let selected = localStorage.getItem("filter.customerid");

                    $.each(result.data, function (key, value) {
                        let newOption = new Option(value.name,
                            value.id, false, value.id == selected ? true : false);
                        newOption.setAttribute("data-customerlogo", value.logo);
                        $(loadSelectId).append(newOption);
                    });
                }

                GetModulePic(loadSelectId);
                $(loadSelectId).trigger("chosen:updated");
            },
            error: function (response) {
                if (response.responseText != '') {
                    toastr["error"](response.responseText);
                }
            }
        });
    } else {
        $.ajax({
            url: '/customer/getbyid/' + userBrand,
            type: "get",
            success: function (result) {
                localStorage.setItem("filter.customerid", result.data.id);
                $('.currentModule').attr("src", result.data.logoUrl)
                $('.brandName').append(result.data.name);
            },
            error: function (response) {
                if (response.responseText != '') {
                    toastr["error"](response.responseText);
                }
            }
        });
    }
}

function GetModulePic(d) {
    var logoUrl = $(d).find(':selected').data('customerlogo');
    localStorage.setItem("filter.customerid", $(d).val());

    if (logoUrl != undefined) {
        $('.currentModule').attr("src", logoUrl)
        $('.currentModule').removeClass('moduleFilter');
    } else {
        $('.currentModule').attr("src", "/img/avatar.png")
        $('.currentModule').addClass('moduleFilter');
    }
}