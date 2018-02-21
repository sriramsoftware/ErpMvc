angular.module('erp').controller('seleccionComprasController', [
            '$scope', '$http', function ($scope, $http) {
                $scope.loading = true;
                $scope.addMode = false;
                $scope.tieneError = false;
                $scope.error = "";
                $scope.select_changed = 0;

                $scope.comprasIds = [];
                $scope.compras = 0.00;
                $scope.comprasComprobante = 0.00;
                $scope.comprasSinComprobante = 0.00;
                $scope.ventas = 0.00;
                $scope.porciento = 0;
                $scope.porcientoComprobante = 0;
                $scope.porcientoSinComprobante = 0;

                $scope.fetchComprasSeleccionadas = function(id) {
                    $http.get("/Seleccion/ComprasSeleccionadas/" + id).then(function (result) {
                        $scope.comprasIds = result.data.comprasIds;
                        $scope.compras = result.data.importe.toFixed(2);
                        $scope.comprasComprobante = result.data.importeComprobante;
                        $scope.comprasSinComprobante = result.data.importeSinComprobante;
                        $scope.porciento = ($scope.compras / $scope.ventas * 100).toFixed(2);
                        $scope.porcientoComprobante = ($scope.comprasComprobante / $scope.compras * 100).toFixed(2);
                        $scope.porcientoSinComprobante = ($scope.comprasSinComprobante / $scope.compras * 100).toFixed(2);
                    });
                }

                $scope.fetchVentas = function() {
                    $scope.ventas = $('#ventas').attr("data-importe") * 1;
                }

                $scope.toggleCompraSelection = function (id,importe,tieneComprobante) {
                    
                    var esta = $scope.comprasIds.find(function (a) {
                        if (id === a) {
                            return true;
                        }
                        return false;
                    });
                    if (esta) {
                        var index = $scope.comprasIds.indexOf(id);
                        $scope.comprasIds.splice(index, 1);

                        $scope.compras = (parseFloat($scope.compras) - importe).toFixed(2);
                        $scope.porciento = ($scope.compras * 100 / $scope.ventas).toFixed(2);
                        if (tieneComprobante) {
                            $scope.comprasComprobante = parseFloat($scope.comprasComprobante) - importe;
                            $scope.porcientoComprobante = ($scope.comprasComprobante / $scope.compras * 100).toFixed(2);
                            $scope.porcientoSinComprobante = ($scope.comprasSinComprobante / $scope.compras * 100).toFixed(2);
                        } else {
                            $scope.comprasSinComprobante = parseFloat($scope.comprasSinComprobante) - importe;
                            $scope.porcientoComprobante = ($scope.comprasComprobante / $scope.compras * 100).toFixed(2);
                            $scope.porcientoSinComprobante = ($scope.comprasSinComprobante / $scope.compras * 100).toFixed(2);
                        }
                    } else {
                        $scope.comprasIds.push(id);
                        var newImporte = (parseFloat($scope.compras) + importe);
                        $scope.compras = parseFloat(newImporte).toFixed(2);
                        $scope.porciento = ($scope.compras * 100 / $scope.ventas).toFixed(2);
                        if (tieneComprobante) {
                            $scope.comprasComprobante = parseFloat($scope.comprasComprobante) + importe;
                            $scope.porcientoComprobante = ($scope.comprasComprobante / $scope.compras * 100).toFixed(2);
                            $scope.porcientoSinComprobante = ($scope.comprasSinComprobante / $scope.compras * 100).toFixed(2);
                        } else {
                            $scope.comprasSinComprobante = parseFloat($scope.comprasSinComprobante) + importe;
                            $scope.porcientoComprobante = ($scope.comprasComprobante / $scope.compras * 100).toFixed(2);
                            $scope.porcientoSinComprobante = ($scope.comprasSinComprobante / $scope.compras * 100).toFixed(2);
                        }
                    }
                    
                }

                $scope.buscarCompras = function (id) {
                    return $scope.comprasIds.find(function (a) {
                        if (id === a) {
                            return a;
                        }
                        return null;
                    });
                }

                $scope.isCheckedCompra = function (id) {
                    if ($scope.comprasIds === undefined) {
                        return false;
                    }
                    var a = $scope.buscarCompras(id);
                    if (a == null) {
                        return false;
                    }
                    return true;
                }

                $scope.closeAlert = function () {
                    $scope.tieneError = false;
                };

            }
]);
