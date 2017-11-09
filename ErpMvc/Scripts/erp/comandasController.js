angular.module('erp').controller('comandasController', [
            '$scope', '$http', function ($scope, $http) {
                $scope.loading = true;
                $scope.addMode = false;
                $scope.tieneError = false;
                $scope.error = "";
                $scope.select_changed = 0;
                $scope.cantidadComensales = 0;

                $scope.detalleActual = {};
                $scope.ocupado = false;

                $scope.ordenes = [];
                $scope.ordenActual = {};
                $scope.ordenes_detalle = {};

                $scope.comensales = [{ Id: 1, Nombre: "Niña" }, { Id: 2, Nombre: "Niño" }, { Id: 3, Nombre: "Mujer" }, { Id: 4, Nombre: "Hombre" }];

                $scope.buscarOrden = function (numero) {
                    return $scope.ordenes.find(function (a) {
                        if (numero === a.Numero) {
                            return a;
                        }
                        return null;
                    });
                }

                $scope.buscarOrdenDetalle = function (detalle, orden) {
                    return detalle.Ordenes.find(function (a) {
                        if (orden.Numero === a.Orden.Numero) {
                            return a;
                        }
                        return null;
                    });
                }


                $scope.buscarAnotacion = function (id) {
                    return $scope.anotaciones.find(function (a) {
                        if (id === a.Id) {
                            return a;
                        }
                        return null;
                    });
                }

                $scope.buscarAnotacionOrdenDetalle = function (anotacion) {
                    if ($scope.ordenActual.Anotaciones === undefined) {
                        return null;
                    }
                    return $scope.ordenActual.Anotaciones.find(function (a) {
                        if (anotacion.Id === a.Id) {
                            return a;
                        }
                        return null;
                    });
                }

                $scope.isCheckedAnotacion = function (anotacion) {
                    if ($scope.ordenActual.Anotaciones === undefined) {
                        return false;
                    }
                    var a = $scope.buscarAnotacionOrdenDetalle(anotacion);
                    if (a == null) {
                        return false;
                    }
                    return true;
                }
                //no hace nada se edita
                $scope.cambiarCantidadDeComensales = function () {
                    var cantidadAnterior = $scope.ordenes.length;
                    var dif = cantidadAnterior - $scope.cantidadComensales;

                    if (dif >= 0) {
                        for (var j = $scope.cantidadComensales * 1 + 1; j <= cantidadAnterior; j++) {
                            var ag = $scope.buscarOrden(j);
                            if (ag != null) {
                                $scope.detalles.forEach(function (detalle) {
                                    if (detalle.Ordenes == undefined) {
                                        detalle.Ordenes = [];
                                    }
                                    var det = $scope.buscarOrdenDetalle(detalle, ag);
                                    var index = detalle.Ordenes.indexOf(det);
                                    if (index > -1) {
                                        detalle.Ordenes.splice(index, 1);
                                    }
                                });

                                var index = $scope.ordenes.indexOf(ag);
                                if (index > -1) {
                                    $scope.ordenes.splice(index, 1);
                                }
                            }
                        }
                    }
                    for (var i = 1; i <= $scope.cantidadComensales; i++) {
                        var ag = $scope.buscarOrden(i);
                        if (ag == null) {
                            $scope.ordenes.push({ Numero: i, Comensal: {} });
                        }
                    }
                }

                $scope.toggleOrdenSelection = function (orden) {
                    var det = $scope.buscarOrdenDetalle($scope.detalleActual, orden);
                    var index = $scope.detalleActual.Ordenes.indexOf(det);
                    if (index > -1) {
                        $scope.detalleActual.Ordenes.splice(index, 1);
                        $scope.ordenActual = {};
                        if (det.Id != undefined) {
                            $http.get('/Comandas/EliminarOrdenEnDetalle/' + det.Id).then(function (result) {
                                if (result.data) {
                                    $scope.tieneError = false;
                                } else {
                                    $scope.error = "Error al eliminar el orden.";
                                    $scope.tieneError = true;
                                }
                            });
                        }
                    } else {
                        var ordenAct = { Orden: orden, Anotaciones: [] };
                        $scope.detalleActual.Ordenes.push(ordenAct);
                        $scope.ordenActual = ordenAct;
                        if (orden.Id != undefined) {
                            $http.get('/Comandas/AgregarOrdenEnDetalle?ordenId=' + orden.Id + '&detalleId=' + $scope.detalleActual.Id).then(function (result) {
                                if (result.data.Result) {
                                    $scope.ordenActual.Id = result.data.Id;
                                    $scope.tieneError = false;
                                } else {
                                    $scope.error = "Error al eliminar el orden.";
                                    $scope.tieneError = true;
                                }
                            });
                        }
                    }
                }

                $scope.selectOrden = function (orden) {
                    var det = $scope.buscarOrdenDetalle($scope.detalleActual, orden);
                    var index = $scope.detalleActual.Ordenes.indexOf(det);
                    if (index > -1) {
                        $scope.ordenActual = det;
                    }
                }

                $scope.toggleAnotacionesSelection = function (anotacion) {
                    if ($scope.ordenActual.Anotaciones == undefined) {
                        $scope.ordenActual.Anotaciones = [];
                    }
                    var a = $scope.buscarAnotacionOrdenDetalle(anotacion);
                    if (a != null) {
                        var index = $scope.ordenActual.Anotaciones.indexOf(a);
                        $scope.ordenActual.Anotaciones.splice(index, 1);
                        if ($scope.ordenActual.Id != undefined) {
                            $http.get('/Comandas/EliminarAnotacion?anotacionId=' + anotacion.Id + '&ordenId=' + $scope.ordenActual.Id).then(function (result) {
                                if (result.data) {
                                    $scope.tieneError = false;
                                } else {
                                    $scope.error = "Error al eliminar el anotacion.";
                                    $scope.tieneError = true;
                                }
                            });
                        }
                    } else {
                        $scope.ordenActual.Anotaciones.push(anotacion);
                        if ($scope.ordenActual.Id != undefined) {
                            $http.get('/Comandas/AgregarAnotacion?anotacionId=' + anotacion.Id + '&ordenId=' + $scope.ordenActual.Id).then(function (result) {
                                if (result.data) {
                                    $scope.tieneError = false;
                                } else {
                                    $scope.error = "Error al agregar la anotacion.";
                                    $scope.tieneError = true;
                                }
                            });
                        }
                    }
                }

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

                $scope.anotaciones = [];
                $scope.fetchAnotaciones = function () {
                    $http.get("/Anotaciones/Data/").then(function (result) {
                        $scope.anotaciones = result.data;
                    });
                }
                $scope.fetchAnotaciones();

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
                                $http.get('/Comandas/DisminuirAgregado?detalleId=' + $scope.detalleActual.Id + '&agregadoId=' + ag.Id).then(function (result) {
                                    if (result.data) {
                                        ag.Cantidad--;
                                        $scope.tieneError = false;
                                        if (ag.Cantidad == 0) {
                                            var index = $scope.detalleActual.Agregados.indexOf(ag);
                                            $scope.detalleActual.Agregados.splice(index, 1);
                                        }
                                    } else {
                                        $scope.error = "Error al disminuir la cantidad del menu.";
                                        $scope.tieneError = true;
                                    }
                                });
                            } else {
                                ag.Cantidad--;
                                if (ag.Cantidad == 0) {
                                    var index = $scope.detalleActual.Agregados.indexOf(ag);
                                    $scope.detalleActual.Agregados.splice(index, 1);
                                }
                            }
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
                        $scope.detalleActual.ImporteTotal = ($scope.detalleActual.ImporteTotal * 1) + (precio * 1);
                        ag = agregado;
                        $scope.detalleActual.Agregados.push(agregado);
                    } else {
                        var precio = ag.Precio * $scope.detalleActual.Cantidad;
                        $scope.importeTotal += precio;
                        $scope.detalleActual.ImporteTotal = ($scope.detalleActual.ImporteTotal * 1) + (precio * 1);
                        ag.Cantidad++;
                    }
                    if ($scope.detalleActual.Id != undefined) {
                        $http.get('/Comandas/AumentarAgregado?detalleId=' + $scope.detalleActual.Id + '&agregadoId=' + ag.Id).then(function (result) {
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
                        $scope.tieneError = false;
                    }
                }

                $scope.fetchMenus();

                $scope.detalles = [];
                $scope.newDetalle = {};

                $scope.toggleEdit = function () {
                    this.detalle.editMode = !this.detalle.editMode;
                };

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
                        if (valido) {
                            $scope.detalles.push(detalleNuevo);
                            $("#save-aprob-btn").removeClass('disabled');
                            $scope.newDetalle = {};
                            $scope.tieneError = false;
                            $scope.fetchMenus();
                        } 
                        $scope.ocupado = false;
                    } else {
                        var url = "/Comandas/AgregarMenuAVenta/";
                        detalleNuevo.ComandaId = $("#Id").val();
                        $http.post(url, detalleNuevo).then(function (result) {
                            valido = result.data.Result;
                            if (valido) {
                                detalleNuevo.Id = result.data.DetalleId;
                                detalleNuevo.Ordenes = [];
                                detalleNuevo.Anotaciones = [];
                                $scope.detalles.push(detalleNuevo);
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
                            $http.get('/Comandas/DisminuirMenu/' + detalle.Id).then(function (result) {
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
                    } else {
                        $http.get('/Comandas/AumentarMenu/' + detalle.Id).then(function (result) {
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
                        $http.get('/Comandas/EliminarDetalle/' + detalle.Id).then(function (result) {
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
                        $scope.detalles.splice(index, 1);
                    }
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

                $scope.mostrarOrdenes = function (detalle) {
                    $scope.detalleActual = detalle;
                    if ($scope.detalleActual.Ordenes == null) {
                        $scope.detalleActual.Ordenes = [];
                    } else {

                    }
                    $("#modal-orden").modal('show');
                }

                $scope.mostrarAnotaciones = function (detalle) {
                    //$scope.fetchAnotaciones();
                    $scope.detalleActual = detalle;
                    if ($scope.detalleActual.Anotaciones == null) {
                        $scope.detalleActual.Anotaciones = [];
                    } else {

                    }
                    $("#modal-anotaciones").modal('show');
                }
            }
]);
