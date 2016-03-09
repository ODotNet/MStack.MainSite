$(function () {
    $(document).on('pjax:start', function () {
        //$.blockUI({
        //    overlayCSS: {
        //        backgroundColor: '#000',
        //        opacity: 0.2,
        //        cursor: 'wait'
        //    },
        //    message: "<h2>Paging Loadding...</h2>",
        //    css: {
        //        border: 'none',
        //        padding: '15px',
        //        backgroundColor: '#000',
        //        '-webkit-border-radius': '10px',
        //        '-moz-border-radius': '10px',
        //        opacity: .5,
        //        color: '#fff'
        //    },
        //    baseZ: 11000,
        //});
        NProgress.start();
    });
    $(document).on('pjax:end', function () {
        //$.unblockUI();
        NProgress.done();
    });
    if ($(document)["pjax"] != undefined)
        $(document).pjax('a[data-pjax]', '#pjax-container')
});