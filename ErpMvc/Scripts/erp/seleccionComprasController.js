angular.module('erp').controller('seleccionComprasController', [
            '$scope', '$http', function ($scope, $http) {
                $scope.loading = true;
                $scope.addMode = false;
                $scope.tieneError = false;
                $scope.error = "";
                $scope.select_changed = 0;

                $scope.comprasIds = [];
                $scope.compras = 0.00;
                $scope.ventas = 0.00;
                $scope.porciento = 0;

                $scope.fetchVentas = function() {
                    $scope.ventas = $('#ventas').attr("data-importe") * 1;
                }

                $scope.toggleCompraSelection = function (id,importe) {
                    
                    var esta = $scope.comprasIds.find(function (a) {
                        if (id === a) {
                            return true;
                        }
                        return false;
                    });
                    if (esta) {
                        var index = $scope.comprasIds.indexOf(id);
                        $scope.comprasIds.splice(index, 1);

                        $scope.compras -= importe;
                        $scope.porciento = $scope.compras * 100 / $scope.ventas;
                    } else {
                        $scope.comprasIds.push(id);
                        $scope.compras += importe;
                        $scope.porciento = $scope.compras * 100 / $scope.ventas;
                    }
                }

                $scope.closeAlert = function () {
                    $scope.tieneError = false;
                };

            }
]);
