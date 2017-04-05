angular.module('erp').controller('ventasController', [
            '$scope', '$http', function ($scope, $http) {
                $scope.loading = true;
                $scope.addMode = false;
                $scope.tieneError = false;
                $scope.error = "";
                $scope.select_changed = 0;
                $scope.precioDeVentaDeMenu = 0;
                $scope.importeTotal = 0.00;
                $scope.detalleActual = {};
                $scope.ocupado = false;

                $scope.menus = [];
                $scope.urlMenus = '/Menus/Menus';

                $scope.fetchMenus = function () {
                    $http.get($scope.urlMenus).then(function (result) {
                        $scope.menus = result.data;
                        $scope.select_changed++;
                    });
                }

                $scope.agregados = [];
                $scope.fetchAgregados = function (id) {
                    $http.get("/Menus/AgregadosData/" + id).then(function (result) {
                        $scope.agregados = result.data;
                    });
                }

                $scope.cantidadAgregado = function (agregado) {
                    var ag = $scope.detalleActual.Agregados.find(function (a) {
                        if (agregado.Id === a.Id) {
                            return a;
                        }
                        return null;
                    });
                    if (ag == null) {
                        return 0;
                    } else {
                        return ag.Cantidad;
                    }
                };

                $scope.menosADisabled = false;

                $scope.disminuirAgregado = function (agregado) {
                    var ag = $scope.detalleActual.Agregados.find(function (a) {
                        if (agregado.Id === a.Id) {
                            return a;
                        }
                        return null;
                    });
                    if (ag != null) {
                        if (ag.Cantidad > 0) {
                            var precio = ag.Precio * $scope.detalleActual.Cantidad;
                            ag.Cantidad--;
                            $scope.importeTotal -= precio;
                            $scope.detalleActual.ImporteTotal -= precio;
                        } if (ag.Cantidad <= 0) {
                            var index = $scope.detalleActual.Agregados.indexOf(agregado);
                            $scope.detalleActual.Agregados.splice(index, 1);
                        }
                    }
                }

                $scope.aumentarAgregado = function (agregado) {
                    var ag = $scope.detalleActual.Agregados.find(function (a) {
                        if (agregado.Id === a.Id) {
                            return a;
                        }
                        return null;
                    });
                    if (ag == null) {
                        agregado.Cantidad = 1;
                        var precio = agregado.Precio * $scope.detalleActual.Cantidad;
                        $scope.importeTotal += precio;
                        $scope.detalleActual.ImporteTotal += precio;
                        ag = agregado;
                        $scope.detalleActual.Agregados.push(agregado);
                    } else {
                        var precio = ag.Precio * $scope.detalleActual.Cantidad;
                        $scope.importeTotal += precio;
                        $scope.detalleActual.ImporteTotal += precio;
                        ag.Cantidad++;
                    }
                    //var url = "@Url.Action("SePuedeVender","VentasController")";
                    var url = "/Ventas/SePuedeVender";
                    var data = {
                        PuntoDeVentaId: $("#PuntoDeVentaId").val(),
                        Detalles: $scope.detallesVenta
                    }
                    $http.post(url, data).then(function (result) {
                        if (result.data) {
                            $scope.tieneError = false;
                        } else {
                            $scope.error = "No hay la cantidad de producto requerido para la cantidad seleccionada del menu.";
                            var precio = ag.Precio * $scope.detalleActual.Cantidad;
                            $scope.importeTotal -= precio;
                            $scope.detalleActual.ImporteTotal -= precio;
                            ag.Cantidad--;
                            $scope.tieneError = true;
                        }
                    });
                }

                $scope.fetchMenus();

                $scope.detallesVenta = [];
                $scope.newDetalle = {};

                $scope.toggleEdit = function () {
                    this.detalle.editMode = !this.detalle.editMode;
                };

                $scope.mostrarAgregados = function (detalle) {
                    $scope.fetchAgregados(detalle.ElaboracionId);
                    $scope.detalleActual = detalle;
                    if ($scope.detalleActual.Agregados == null) {
                        $scope.detalleActual.Agregados = [];
                    }
                    $("#modal-agregados").modal('show');
                }

                $scope.agregarDetalle = function () {
                    $scope.show = false;
                    $scope.ocupado = true;
                    var detalleNuevo = {};
                    var valido = true;
                    if ($scope.newDetalle.Cantidad == null || $scope.newDetalle.ElaboracionId === "") {
                        $scope.error = "Debe insertar los datos del menu.";
                        $scope.tieneError = true;
                        valido = false;
                        $scope.ocupado = false;
                    }
                    if (valido) {
                        detalleNuevo.ElaboracionId = $scope.newDetalle.ElaboracionId.Id;
                        detalleNuevo.Cantidad = $scope.newDetalle.Cantidad;
                        detalleNuevo.ImporteTotal = ($scope.newDetalle.ElaboracionId.Precio * $scope.newDetalle.Cantidad);
                        var menuNombre = $("#ElaboracionId option:selected").html();
                        detalleNuevo.NombreMenu = menuNombre;
                    }
                    //var url = "@Url.Action("SePuedeVender","VentasController")";
                    var url = "/Ventas/SePuedeVender/";
                    var data = {
                        PuntoDeVentaId: $("#PuntoDeVentaId").val(),
                        Detalles: $scope.detallesVenta,
                        NuevoDetalle: detalleNuevo
                    }
                    $http.post(url, data).then(function (result) {
                        valido = result.data;
                        if (valido) {
                            $scope.importeTotal += ($scope.newDetalle.ElaboracionId.Precio * $scope.newDetalle.Cantidad);
                            $scope.detallesVenta.push(detalleNuevo);
                            $("#save-aprob-btn").removeClass('disabled');
                            $scope.newDetalle = {};
                            $("#Aprobacion_Fecha").val("");
                            $scope.tieneError = false;
                            $scope.fetchMenus();
                        } else {
                            $scope.error = "No hay la cantidad de producto requerido para la cantidad seleccionada del menu.";
                            $scope.tieneError = true;
                        }
                        $scope.ocupado = false;
                    });
                };

                $scope.disminuirCantidad = function (detalle) {
                    if (detalle.Cantidad > 1) {
                        var precio = detalle.ImporteTotal / detalle.Cantidad;
                        detalle.Cantidad--;
                        $scope.importeTotal -= precio;
                        detalle.ImporteTotal -= precio;
                        $scope.tieneError = false;
                    } 
                }

                $scope.aumentarCantidad = function (detalle) {
                    var precio = detalle.ImporteTotal / detalle.Cantidad;
                    $scope.importeTotal += precio;
                    detalle.ImporteTotal += precio;
                    detalle.Cantidad++;
                    //var url = "@Url.Action("SePuedeVender","VentasController")";
                    var url = "/Ventas/SePuedeVender/";
                    var data = {
                        PuntoDeVentaId: $("#PuntoDeVentaId").val(),
                        Detalles: $scope.detallesVenta
                    }
                    $http.post(url, data).then(function (result) {
                        if (result.data) {
                            $scope.tieneError = false;
                        } else {
                            $scope.error = "No hay la cantidad de producto requerido para la cantidad seleccionada del menu.";
                            var precio = detalle.ImporteTotal / detalle.Cantidad;
                            $scope.importeTotal -= precio;
                            detalle.ImporteTotal -= precio;
                            detalle.Cantidad--;
                            $scope.tieneError = true;
                        }

                    });
                }

                $scope.closeAlert = function () {
                    $scope.tieneError = false;
                };

                //Used to edit a record
                $scope.borrarDetalle = function (index) {
                    $scope.loading = true;
                    var detalle = this.detalle;
                    $scope.importeTotal -= (detalle.ImporteTotal);
                    $scope.detallesVenta.splice(index, 1);
                    detalle.editMode = false;
                    $scope.loading = false;
                };
            }
]);
