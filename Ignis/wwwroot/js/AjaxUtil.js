// The folowing library are required:
// jquery.js

'use strict';
var Ax = function (
    vars = {
        url: '',
        errorMessage: { title: '', message: '' },
        data: null,
        f: null,
        divId: ''
    }
) {
    // inside the class

    // define root
    var root = this;
    // constructor
    this.construct = function (options) {
        $.extend(vars, options);
    };

    // local variable
    var loadingSpinner = '<div class="d-flex align-items-center text-danger"><strong> Loading...</strong><div class="spinner-border ms-auto" role="status" aria-hidden="true"></div></div> ';
    //var loadingSpinner = '<div class="d-flex align-items-center" style="color:#007D88 !important"><strong> Loading...</strong><div class="spinner-border ms-auto" role="status" aria-hidden="true"></div></div > ';
    var loadingDiv = $('div[data-name="pageLoading"]');
    // Ajax Get
    this.AxGet = function () {

        var req = false;
        if (!req) {
            req = true;
            // render spinner inside div before server callback is received
            $('#' + vars.divId).html(loadingSpinner);
            $(loadingDiv).show();
            $.ajax({
                url: vars.url,
                data: vars.data,
                type: 'GET',
                cache: false,
                success: function (result) {
                    if (result === 'error') {
                        // handle error via ajax req                        
                        new Notify({ title: 'Page Request', message: 'Unable to complete request.  Please try reload page.' }).initError();
                    } else {
                        $('#' + vars.divId).html(result);
                    }
                    // trigger pass through function if not null
                    if (typeof vars.f === 'function') {
                        vars.f(result);
                    }
                },
                complete: function () {
                    req = false;
                    $(loadingDiv).hide();
                },
                error: function () {
                    req = false;
                    $(loadingDiv).hide();
                    // handle error messaging to the user
                }
            })
        }
    };

    this.AxGetTrad = function () {

        var req = false;
        if (!req) {
            req = true;
            // render spinner inside div before server callback is received
            $('#' + vars.divId).html(loadingSpinner);
            $(loadingDiv).show();
            $.ajax({
                url: vars.url,
                data: vars.data,
                type: 'GET',
                cache: false,
                traditional:true,
                success: function (result) {
                    if (result === 'error') {
                        // handle error via ajax req                        
                        new Notify({ title: 'Page Request', message: 'Unable to complete request.  Please try reload page.' }).initError();
                    } else {
                        $('#' + vars.divId).html(result);
                    }
                    // trigger pass through function if not null
                    if (typeof vars.f === 'function') {
                        vars.f(result);
                    }
                },
                complete: function () {
                    req = false;
                    $(loadingDiv).hide();
                },
                error: function () {
                    req = false;
                    $(loadingDiv).hide();
                    // handle error messaging to the user
                }
            })
        }
    };
    // Ajax POST
    this.AxPost = function () {

        var token = $("[name=__RequestVerificationToken]").val();
        var req = false;
        if (!req) {
            req = true;
            $(loadingDiv).show();
            $.ajax({
                url: vars.url,
                data: vars.data,
                type: 'POST',
                headers: { '__RequestVerificationToken': token },
                success: function (result) {
                    if (result === 'error') {
                        // handle error via ajax req
                        new Notify({ title: 'Page Request', message: 'Unable to complete request.  Please try reload page.' }).initError();

                    } else {
                        $('#' + vars.divId).html(result);
                    }
                    // trigger pass through function if not null
                    if (typeof vars.f === 'function') {
                        vars.f();
                    }
                },
                complete: function () {
                    req = false;
                    $(loadingDiv).hide();
                },
                error: function () {
                    req = false;
                    // handle error messaging to the user
                }
            })
        }
    };

    // Ajax POST
    this.AxPostJson = function () {
        var token = $('input[name="__RequestVerificationToken"]').val();
        vars.data.__RequestVerificationToken = token;
        $(loadingDiv).show();
        var req = false;
        if (!req) {
            req = true;
            $.ajax({
                url: vars.url,
                data: vars.data,
                type: 'POST',
                success: function (result) {
                    if (result === 'error') {
                        // handle error via ajax req
                        new Notify({ title: 'Page Request', message: 'Unable to complete request.  Please try reload page.' }).initError();

                    } else {
                        $('#' + vars.divId).html(result);
                    }
                },
                complete: function () {
                    // trigger pass through function if not null
                    if (typeof vars.f === 'function') {
                        vars.f();
                    }
                    req = false;
                    $(loadingDiv).hide();
                },
                error: function (err) {
                    req = false;
                    $(loadingDiv).hide();
                    // handle error messaging to the user
                    console.log(err);
                }
            })
        }
    };

    // initialize constructor
    this.construct(vars);

}