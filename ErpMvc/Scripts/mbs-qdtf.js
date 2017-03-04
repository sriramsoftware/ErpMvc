/*! mbs-dynamic-tables 1.0
 * ©Matanzas Bay Software -
 */

/**
 * @summary     Quick DataTables formatting
 * @description Quick applies formatting to DataTables with class 'dynamic'
 * @version     1.0
 * @file        mbs-qdtf.js
 * @author      Dariel J. Vicedo <darielvicedo@gmail.com>
 * @copyright   Copyright 2015 Matanzas Bay Software
 * @type        {jQuery.length|*|jQuery}
 *
 * This source file is free software, available under the following license:
 *   MIT license - https://github.com/MatanzasBay/utils/blob/master/LICENSE
 *
 * This source file is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the license files for details.
 *
 * For details please refer to: http://www.datatables.net
 */

var colums = $('.dynamic thead th').length;
$(document).ready(function () {
    var dynamicTable = $(".dynamic").DataTable({
        "lengthMenu": [ [20, 25, 30, 50, 100, -1], [20, 25, 30, 50, 100, "Todos"] ],
        "language": {
            "paginate": {
                "first": "Primera",
                "previous": "Anterior",
                "next": "Siguiente",
                "last": "&Uacute;ltima"
            },
            "info": "Mostrando resultados del _START_ al _END_ de un total de _TOTAL_",
            "lengthMenu": "Mostrando _MENU_ resultados",
            "search": "Buscar: ",
            "infoEmpty": "Mostrando 0 resultados",
            "infoFiltered": "(filtrado de un total de _MAX_ resultados)",
            "sZeroRecords": "No se encontraron resultados con este criterio"
        },
        "pagingType": "full_numbers",
        "columnDefs": [
            { "orderable": false, "targets": colums - 1 }
        ]
    });
    dynamicTable.on('draw', function() {
        var body = $(dynamicTable.table().body());

        body.unhighlight();
        body.highlight(dynamicTable.search());
    });
});