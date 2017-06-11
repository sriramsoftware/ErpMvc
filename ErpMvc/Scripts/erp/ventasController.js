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
                            if ($scope.detalleActual.Id != undefined) {
                                $http.get('/Ventas/DisminuirAgregado?detalleId=' + $scope.detalleActual.Id + '&agregadoId=' + ag.Id).then(function (result) {
                                    if (result.data) {
                                        var precio = ag.Precio * $scope.detalleActual.Cantidad;
                                        ag.Cantidad--;
                                        $scope.importeTotal -= precio;
                                        $scope.detalleActual.ImporteTotal -= precio;
                                        $scope.tieneError = false;
                                    } else {
                                        $scope.error = "Error al disminuir la cantidad del menu.";
                                        $scope.tieneError = true;
                                    }
                                });
                            } else {
                                var precio = ag.Precio * $scope.detalleActual.Cantidad;
                                ag.Cantidad--;
                                $scope.importeTotal -= precio;
                                $scope.detalleActual.ImporteTotal -= precio;
                            }
                        } if (ag.Cantidad == 0) {
                            var index = $scope.detalleActual.Agregados.indexOf(ag);
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
                        $scope.detalleActual.ImporteTotal = ($scope.detalleActual.ImporteTotal * 1) +(precio * 1);
                        ag = agregado;
                        $scope.detalleActual.Agregados.push(agregado);
                    } else {
                        var precio = ag.Precio * $scope.detalleActual.Cantidad;
                        $scope.importeTotal += precio;
                        $scope.detalleActual.ImporteTotal = ($scope.detalleActual.ImporteTotal * 1) + (precio * 1);
                        ag.Cantidad++;
                    }
                    if ($scope.detalleActual.Id != undefined) {
                        $http.get('/Ventas/AumentarAgregado?detalleId=' + $scope.detalleActual.Id + '&agregadoId=' + ag.Id).then(function (result) {
                            if (result.data) {
                                $scope.tieneError = false;
                            } else {
                                $scope.error = "Error al aumentar la cantidad del menu.";
                                var precio = ag.Precio * $scope.detalleActual.Cantidad;
                                $scope.importeTotal -= precio;
                                $scope.detalleActual.ImporteTotal -= (precio * 1);
                                ag.Cantidad--;
                                $scope.tieneError = true;
                            }
                        });
                    } else {
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
                                $scope.detalleActual.ImporteTotal -= (precio * 1);
                                ag.Cantidad--;
                                $scope.tieneError = true;
                            }
                        });
                    }
                }

                $scope.fetchMenus();

                $scope.detallesVenta = [];
                $scope.newDetalle = {};

                $scope.toggleEdit = function () {
                    this.detalle.editMode = !this.detalle.editMode;
                };

                $scope.mostrarAgregados = function (detalle) {
                    $scope.fetchAgregados(detalle.Elaboracion.Id);
                    $scope.detalleActual = detalle;
                    if ($scope.detalleActual.Agregados == null) {
                        $scope.detalleActual.Agregados = [];
                    } else {
                        
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
                        detalleNuevo.ElaboracionId = $scope.newDetalle.Elaboracion.Id;
                        detalleNuevo.Elaboracion = $scope.newDetalle.Elaboracion;
                        detalleNuevo.Cantidad = $scope.newDetalle.Cantidad;
                        detalleNuevo.ImporteTotal = ($scope.newDetalle.Elaboracion.Precio * $scope.newDetalle.Cantidad);
                    }
                    if ($("#Id").val() == undefined) {
                        var url = "/Ventas/SePuedeVender/";
                        var data = {
                            PuntoDeVentaId: $("#PuntoDeVentaId").val(),
                            Detalles: $scope.detallesVenta,
                            NuevoDetalle: detalleNuevo
                        }
                        $http.post(url, data).then(function (result) {
                            valido = result.data;
                            if (valido) {
                                $scope.importeTotal += ($scope.newDetalle.Elaboracion.Precio * $scope.newDetalle.Cantidad);
                                $scope.detallesVenta.push(detalleNuevo);
                                $("#save-aprob-btn").removeClass('disabled');
                                $scope.newDetalle = {};
                                $scope.tieneError = false;
                                $scope.fetchMenus();
                            } else {
                                $scope.error = "No hay la cantidad de producto requerido para la cantidad seleccionada del menu.";
                                $scope.tieneError = true;
                            }
                            $scope.ocupado = false;
                        });
                    } else {
                        var url = "/Ventas/AgregarMenuAVenta/";
                        detalleNuevo.VentaId = $("#Id").val();
                        $http.post(url, detalleNuevo).then(function (result) {
                            valido = result.data.Result;
                            if (valido) {
                                $scope.importeTotal += ($scope.newDetalle.Elaboracion.Precio * $scope.newDetalle.Cantidad);
                                detalleNuevo.Id = result.data.DetalleId;
                                $scope.detallesVenta.push(detalleNuevo);
                                $("#save-aprob-btn").removeClass('disabled');
                                $scope.newDetalle = {};
                                $scope.tieneError = false;
                                $scope.fetchMenus();
                            } else {
                                $scope.error = "No hay la cantidad de producto requerido para la cantidad seleccionada del menu.";
                                $scope.tieneError = true;
                            }
                            $scope.ocupado = false;
                        });
                    }
                };

                $scope.disminuirCantidad = function (detalle) {
                    if (detalle.Cantidad > 1) {
                        if (detalle.Id != undefined) {
                            $http.get('/Ventas/DisminuirMenu/' + detalle.Id).then(function (result) {
                                if (result.data) {
                                    $scope.tieneError = false;
                                } else {
                                    $scope.error = "Error al disminuir la cantidad del menu.";
                                    $scope.tieneError = true;
                                }
                            });
                        }
                        if (!$scope.tieneError) {
                            var precio = detalle.ImporteTotal / detalle.Cantidad;
                            detalle.Cantidad--;
                            $scope.importeTotal -= precio;
                            detalle.ImporteTotal -= precio;
                            $scope.tieneError = false;
                        }
                    } 
                }

                $scope.aumentarCantidad = function (detalle) {
                    var precio = detalle.ImporteTotal / detalle.Cantidad;
                    if (detalle.Id == undefined) {
                        $scope.importeTotal += precio;
                        detalle.ImporteTotal += precio;
                        detalle.Cantidad++;
                        var url = "/Ventas/SePuedeVender/";
                        var data = {
                            PuntoDeVentaId: $("#PuntoDeVentaId").val(),
                            Detalles: $scope.detallesVenta
                        }
                        $http.post(url, data).then(function(result) {
                            if (result.data) {
                                $scope.tieneError = false;
                            } else {
                                $scope.error = "No hay la cantidad de producto requerido para la cantidad seleccionada del menu.";                                
                                $scope.importeTotal -= precio;
                                detalle.ImporteTotal -= precio;
                                detalle.Cantidad--;
                                $scope.tieneError = true;
                            }
                        });
                    } else {
                        $http.get('/Ventas/AumentarMenu/' + detalle.Id).then(function (result) {
                            if (result.data) {
                                $scope.importeTotal += precio;
                                detalle.ImporteTotal += precio;
                                detalle.Cantidad++;
                                $scope.tieneError = false;
                            } else {
                                $scope.error = "Error al aumentar la cantidad del menu.";
                                $scope.tieneError = true;
                            }
                        });
                    }
                }

                $scope.closeAlert = function () {
                    $scope.tieneError = false;
                };

                //Used to edit a record
                $scope.borrarDetalle = function (index) {
                    $scope.loading = true;
                    var detalle = this.detalle;
                    var valido = true;
                    if (detalle.Id != undefined) {
                        $http.get('/Ventas/EliminarDetalle/' + detalle.Id).then(function (result) {
                            if (result.data) {
                                $scope.tieneError = false;
                            } else {
                                $scope.error = "Error, o se pudo eliminar el menu.";
                                valido = false;
                                $scope.tieneError = true;
                            }
                        });
                    }
                    if (valido) {
                        $scope.importeTotal -= (detalle.ImporteTotal);
                        $scope.detallesVenta.splice(index, 1);
                    }
                };

                $scope.menuCuentaCasa = function(detalle) {
                    $scope.loading = true;
                    var valido = true;
                    if (detalle.Id != undefined) {
                        $http.get('/Ventas/MenuPorCuentaCasa/' + detalle.Id).then(function (result) {
                            if (result.data) {
                                $scope.tieneError = false;
                            } else {
                                $scope.error = "Error, este menu no puede ser cuenta casa.";
                                valido = false;
                                $scope.tieneError = true;
                            }
                        });
                    }
                    if (valido) {
                        $scope.importeTotal -= detalle.ImporteTotal;
                        detalle.ImporteTotal = 0;
                    }
                }

                $scope.calculaImporteTotal = function () {
                    $scope.importeTotal = 0.00;
                    $scope.detallesVenta.forEach(function(a) {
                        $scope.importeTotal += (a.ImporteTotal * 1);
                    });
                }
            }
]);
