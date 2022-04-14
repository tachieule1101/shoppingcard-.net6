var dtable;
$(document).ready(function () {
    dtable = $('#tableProduct').DataTable({
        "ajax": { "url": "/Admin/Product/AllProducts" },
        method: "GET",
        xhrFields: {
            withCredentials: true
        },
        "columns":[
            { data : "name" },
            { data : "description" },
            { data : "price" },
            { data : "category.name" },
            {
                data: "id",
                "render": function (data) {
                    return `<a href="/Admin/Product/CreateUpdate?id=${data}"><i class="bi bi-pencil-square"></i></a>
                            <a onclick=RemoveProduct("/Admin/Product/CreateUpdate?id=${data}")><i class="bi bi-trash"></i></a>`
                }
            }
        ]
    });

});
function RemoveProduct(url) {
    Swal.fire({
        title: 'Are you sure ? ',
        text: " you won't be..",
        icon: 'warning',
        showCancelButton: 'true',
        confirmButtonColor: '#3085d6',
        cancelButton: '#d33',
        confirmButtonText: 'Yes, delete it '
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                typel: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dtable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}