$(function () {
    var guid = $("#Id").val().replace(/-/g, '');
    var $pimg = $("#imgPreview");
    var $simg = $("#imgBox");
    var $pcnt = $("#imgContainer");
    var xsize = $pcnt.width();
    var ysize = $pcnt.height();
    var $msgBox = $('#myModal');
    var $state = $("#AvatarState");
    var $avatar = $("#Avatar");
    var jcropApi, boundx, boundy;
    $("#fileUpload").change(function () {
        $.ajaxFileUpload({
            url: '/Manage/UploadAvatar/' + guid,
            type: 'post',
            secureuri: false, //一般设置为false
            fileElementId: 'fileUpload', // 上传文件的id、name属性名
            dataType: 'json', //返回值类型，一般设置为json、application/json
            success: function (data, status) {
                if (data.success) {
                    var path = data.savePath;
                    $avatar.val(path);
                    $simg.attr("src", path);
                    $pimg.attr("src", path);
                    $msgBox.modal();
                    $simg.Jcrop({
                        onChange: function (c) {
                            if (parseInt(c.w) > 0) {
                                $state.val([c.x, c.y, c.w, c.h].join(','));
                                var rx = xsize / c.w;
                                var ry = ysize / c.h;
                                $pimg.css({
                                    width: Math.round(rx * boundx) + 'px',
                                    height: Math.round(ry * boundy) + 'px',
                                    marginLeft: '-' + Math.round(rx * c.x) + 'px',
                                    marginTop: '-' + Math.round(ry * c.y) + 'px'
                                });
                            }
                        },
                        onRelease: function () {

                        },
                        aspectRatio: 1
                    }, function () {
                        var bounds = this.getBounds();
                        boundx = bounds[0];
                        boundy = bounds[1];
                        jcropApi = this;
                        jcropApi.animateTo([50, 50, 200, 200])
                    });
                    $pcnt.dblclick(function () {
                        $msgBox.modal();
                    });
                }
            },
            error: function (data, status, e) {
                console.error(e);
            }
        });
    });
});