;
var Main = Main || {};

layui.use('table', function () {
    Main.layTable = layui.table;
});

Main.Widgets = {
    address: null,
    table: null
}

Main.table = function (id) {
    this.id = id;
    return this;
};

Main.table.prototype = {
    render: function () {
        let _this = this;
        $.post("/clear/getData/" + Main.Widgets.address, function (res) {
            if (res.code !== 0) {
                alert(res.msg);
                return;
            }

            Main.layTable.render({
                elem: '#' + _this.id,
                cols: [[
                    {type: 'checkbox'},
                    {field: 'Node', title: '节点', sort: true},
                    {field: 'ServiceID', title: '服务ID'},
                    {field: 'ServiceName', title: '服务名称', sort: true}
                ]],
                page: true,
                data: JSON.parse(res.msg)
            });
        });
    },
    getChecked() {
        return Main.layTable.checkStatus(this.id).data;
    }
};

$(function () {
    Main.Widgets.table = new Main.table("table");

    $("#btn-query").click(function () {
        let _ip = $("#in-ip").val();
        if (!_ip) {
            alert("请先输入服务地址");
            return;
        }

        let _port = $("#in-port").val();
        Main.Widgets.address = _port ? _ip + ":" + _port : _ip;

        Main.Widgets.table.render();
    });

    $("#btn-clear").click(function () {
        let _ids = Main.Widgets.table.getChecked().map(x => x.ServiceID);
        if (!_ids || _ids.length === 0) {
            alert("请先选中数据");
            return;
        }
        $.ajax({
            type: "POST",
            contentType: 'application/json',
            data: JSON.stringify(_ids),
            url: "/clear/clearData/" + Main.Widgets.address,
            success: function (res) {
                if (res.code !== 0) {
                    alert(res.msg);
                    return;
                }

                alert("删除了 " + res.data + " 条数据");
                Main.Widgets.table.render();
            }
        })
    });
})