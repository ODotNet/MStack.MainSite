$(function () {
    var TopicList = function () {

        var handleDataTable = function (options) {
            if ($().dataTable) {
                var table;
                var defaults = {
                    "pageLength": 10,
                    "pagingType": "simple",
                    "searching": false,
                    "ordering": true,
                    "language": {
                        "sProcessing": "处理中...",
                        "sLengthMenu": "每页 _MENU_ 条记录",
                        "sZeroRecords": "没有匹配结果",
                        "sInfo": "第 _PAGE_ 页 ( 共 _PAGES_ 页 )",
                        "sInfoEmpty": "无记录",
                        "sInfoFiltered": "(从 _MAX_ 条记录过滤)",
                        "sInfoPostFix": "",
                        "sSearch": "搜索:",
                        "sUrl": "",
                        "sEmptyTable": "表中数据为空",
                        "sLoadingRecords": "载入中...",
                        "sInfoThousands": ",",
                        "oPaginate": {
                            "sFirst": "首页",
                            "sPrevious": "上页",
                            "sNext": "下页",
                            "sLast": "末页"
                        },
                        "oAria": {
                            "sSortAscending": ": 升序",
                            "sSortDescending": ": 降序"
                        }
                    },
                    "order": [[5, "desc"]]
                };
                if ($.fn.dataTable.isDataTable('.table-data')) {
                    table = $('.table-data').DataTable();
                }
                else {
                    options = $.extend(true, {}, defaults, options);
                    table = $('.table-data').dataTable(options);
                }
            }
        };
        return {
            init: function () {
                handleDataTable();
            },
            initDataTable: function (options) {
                handleDataTable(options);
            }
        }
    }();

    TopicList.init();
});