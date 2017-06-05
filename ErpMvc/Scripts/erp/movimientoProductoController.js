angular.module('erp').controller('movimientoProductoController', [
            '$scope', '$http', function ($scope, $http) {
                $scope.loading = true;
                $scope.addMode = false;
                $scope.tieneError = false;
                $scope.error = "";
                $scope.select_changed = 0;
                $scope.origenId = 0;
                $scope.destinoId = 0;
                $scope.centrosDestino = [];

                $scope.centros = [];
                $scope.fetchCentros = function () {
                    $http.get('/CentrosDeCostos/ListaCentrosDeCosto/').then(function (result) {
                        $scope.centros = result.data;
                        $scope.select_changed++;
                    });
                }
                $scope.fetchCentros();

                $scope.productos = [];
                $scope.fetchProductos = function (id) {
                    $http.get('/CentrosDeCostos/Existencias/' + id).then(function (result) {
                        $scope.productos = result.data;
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

                $scope.cambiarOrigen = function () {
                    $scope.fetchProductos($scope.origenId);
                    $scope.fetchCentros();
                    $scope.centrosDestino = $scope.centros;
                    var index = $scope.centrosDestino.findIndex(function (a) {
                        if ($scope.origenId == a.Id) {
                            return true;
                        }
                        return false;
                    });
                    $scope.centrosDestino.splice(index, 1);
                }

                $scope.detalles = [];
                $scope.newDetalle = {};

                $scope.agregarDetalle = function () {
                    $scope.show = false;
                    var detalleNuevo = {};
                    var valido = true;
                    if ($scope.newDetalle.Cantidad == null || $scope.newDetalle.Producto.Id === "") {
                        $scope.error = "Debe insertar los datos del producto.";
                        $scope.tieneError = true;
                        valido = false;
                    }

                    if (valido && $scope.detalles.find(function (a) {
                        if ($scope.newDetalle.Producto.Id === a.Producto.Id) {
                            return true;
                    }
                        return false;
                    })) {
                        $scope.error = "Ya agrego este producto.";
                        $scope.tieneError = true;
                        valido = false;
                    }
                    if (valido) {
                        var data = {
                            ProductoId: $scope.newDetalle.Producto.Id,
                            Cantidad: $scope.newDetalle.Cantidad,
                            UnidadId: $scope.newDetalle.Unidad.Id,
                            CentroCostoId: $scope.origenId
                        }
                        $http.post('/CentrosDeCostos/SePuedeDarSalida/', data).then(function (result) {
                            if (!result.data) {
                                $scope.error = "No existe la cantidad que quiere dar salida, solo hay " + $scope.newDetalle.Producto.Cantidad + " " + $scope.newDetalle.Producto.Unidad;
                                $scope.tieneError = true;
                                valido = false;
                            }
                            if (valido) {
                                detalleNuevo.Producto = $scope.newDetalle.Producto;
                                detalleNuevo.Cantidad = $scope.newDetalle.Cantidad;
                                detalleNuevo.Unidad = $scope.newDetalle.Unidad;
                                $scope.detalles.push(detalleNuevo);
                                $("#save-aprob-btn").removeClass('disabled');
                                $scope.newDetalle = {};
                                $("#Aprobacion_Fecha").val("");
                                $scope.fetchProductos();
                                $scope.tieneError = false;
                            }
                        });
                    }
                };

                $scope.closeAlert = function () {
                    $scope.tieneError = false;
                };

                $scope.borrarCarga = function (index) {
                    $scope.loading = true;
                    var detalle = this.detalle;
                    $scope.detalles.splice(index, 1);
                    detalle.editMode = false;
                    $scope.loading = false;
                    $scope.fetchProductos();
                };

                $scope.cambiarProducto = function () {
                    $scope.fetchUnidades($scope.newDetalle.Producto.Id);
                };
            }
]);
