angular.module('erp').controller('convertirProductoController', [
            '$scope', '$http', function ($scope, $http) {
                $scope.loading = true;
                $scope.addMode = false;
                $scope.tieneError = false;
                $scope.error = "";
                $scope.select_changed = 0;
                $scope.centroId = 0;
                $scope.origenId = {};
                $scope.destinoId = 0;
                $scope.productosDestino = [];

                $scope.productos = [];
                $scope.fetchProductos = function (id) {
                    $http.get('/CentrosDeCostos/Existencias/' + id).then(function (result) {
                        $scope.productos = result.data;
                        $scope.select_changed++;
                    });
                }

                $scope.productos = [];
                $scope.fetchProductosDestino = function (id) {
                    $http.get('/CentrosDeCostos/ExistenciasMismaUnidad/' + id).then(function (result) {
                        $scope.productosDestino = result.data;
                        $scope.select_changed++;
                    });
                }

                $scope.unidades = [];
                $scope.fetchUnidades = function (id) {
                    $http.get('/Productos/ListaUnidadesDeMedida/' + id).then(function (result) {
                        $scope.unidades = result.data;
                        $scope.select_changed++;
                    });
                }

                $scope.cambiarCentro = function () {
                    $scope.fetchProductos($scope.centroId);
                }
                
                $scope.closeAlert = function () {
                    $scope.tieneError = false;
                };

                $scope.cambiarProducto = function () {
                    var id = $scope.origenId.Id;
                    $scope.fetchUnidades(id);
                    $scope.fetchProductosDestino(id);
                    //$scope.fetchProductos($scope.centroId);
                    //$scope.productosDestino = $scope.productos;
                    //var index = $scope.productosDestino.findIndex(function (a) {
                    //    if (id == a.Id) {
                    //        return true;
                    //    }
                    //    return false;
                    //});
                    //$scope.productosDestino.splice(index, 1);
                    $scope.select_changed++;
                };
            }
]);
