$(document).ready(function(){
	$("#menu-form").validate({   
		rules : {
			nombre : {
				required: true
			},
			precio : {
				required : function(){
					if($("#precio-venta-div").hasClass("hidden"))
						return false;
					return true;
				},
				number : true
			},
			ccosto : {
				min : 1				
			},
			costoPlanificado : {
				required : true,
				number : true
			},
			indiceEsperado : {
				required : true,
				number : true
			}
		},
		messages : {
			nombre : {
				required: "Pro favor, especifique un nombre"
			},
			precio : {
				required : "Entre el precio",
				number : "Solo se adminten numeros"
			},
			ccosto : {
				min : "Escoja un centro de costo"				
			},
			costoPlanificado : {
				required : "Entre el costo planificado",
				number : "Solo se adminten numeros"
			},
			indiceEsperado : {
				required : "Entre el indice esperado",
				number : "Solo se adminten numeros"
			}
		},
		errorClass: "error", 
		validClass: "checked",
		errorPlacement: function(error, element) {
			$(element).tooltip('destroy');	
			$(element).addClass("errorDisplay");
			$(element).attr('title',$(error).text());
			$(element).attr('data-toggle','tooltip');		      
			$(element).tooltip();	    		
		},
		success: function(element) {
			$(element).addClass("checked");
			$(element).tooltip('destroy');
		},
		highlight: function (element, errorClass, validClass) {
			$(element).addClass(errorClass).removeClass(validClass);
			$(element).addClass("errorDisplay");
		},
		unhighlight: function (element, errorClass, validClass) {
			$(element).removeClass(errorClass).addClass(validClass);
			$(element).removeClass("errorDisplay");
			$(element).tooltip('destroy');	       
		}	    
	});
	
	$("#descripcion").focus();
	   
	$('#modal').on('shown.bs.modal', function (e) {  
		$("#nombre").focus();          
	});   
	
	editMenu(); 
	
	$("#descripcion").keyup(function(){
		searchMenu();
	});	
	
	$("#ccostosearch").change(function(){
		searchMenu();
	});	
		
	function searchMenu(){	
		var descripcion = $("#descripcion").val();  
		$.ajax({    
			url: "menusearch", 
			data: {nombre: descripcion, ccosto: $("#ccostosearch").val()},
			type: "POST",      
			dataType : "html",    
			success: function(data) { 		
				$("#div_productos").html(data);     
				editMenu();
			} 
		}); 
	} 
	
	$("#vermenunorentables").click(function(){
		$("#descripcion").val("");
		$.ajax({    
			url: "menunorentables", 
			type: "POST",      
			dataType : "html",    
			success: function(data) { 		
				$("#div_productos").html(data);     
				editMenu();
			} 
		}); 
	});
	
	$("#vermenunbajoprecioconindice").click(function(){
		$("#descripcion").val("");
		$.ajax({    
			url: "menusbajoprecioconindice", 
			type: "POST",      
			dataType : "html",    
			success: function(data) { 		
				$("#div_productos").html(data);     
				editMenu();
			} 
		}); 
	});
	
	function editMenu(){
	    $(".edit-menu").unbind();
	    $(document).on("click", ".edit-menu", function () {
			var id = $(this).attr("value");
			$.ajax({
				url: "/Menus/Menu/"+id, 
				type: "GET",
				dataType : "json",    
				success: function(data) { 	
					$("#Id").val(data.Id);
					$("#Costo").val(data.Costo);
					$("#Nombre").val(data.Nombre);
					$("#PrecioDeVenta").val(data.PrecioDeVenta);
					$("#CostoPlanificado").val(data.CostoPlanificado);
					$("#IndiceEsperado").val(data.IndiceEsperado);
					$("#Preparacion").val(data.Preparacion);
					$("#Presentacion").val(data.Presentacion);
                    $("#porCiento").val(data.PorCiento);
					if (data.PorCiento) {
					    $("#porCiento").prop('checked', 'checked');
				    } 
					$("#menu-form").attr("action","/Menus/Editar/");
					$("#button-submit").text("Modificar");
					$("#modal-title").text("Modificar Menu: "+data.Nombre);
					$("#precio-venta-div").removeClass("hidden");
					$("#modal").modal("show"); 
				}
			}); 
		}); 
		
		$(".ver-productos-relacionados").unbind();
		$(".ver-productos-relacionados").click(function(){
			var id = $(this).attr("value"); 
			$.ajax({
				url: "productosrelacionados",
				data: {id: id},
				type: "POST",
				dataType : "html",    
				success: function(data) { 	
					$("#bodyProductosRelacionados").html(data);
					$("#modalProductosRelacionados").modal("show");    
				}
			});
			$("#modalProductosRelacionados").modal("show");    
		});
		
		$(".edit-image").unbind();
		$(".edit-image").click(function(){
			var file = $(this).prev();
			var id = $(this).prev().prev().val();
			var form = $(this).parent();
			var parentForm = $(form).parent();
			var superior = $(parentForm).parent();
			var children = $(superior).children();
			var firstDiv = $(children).get(0);
			$(file).click();
			$(file).change(function(){
				$(form).submit();
			});			 
			$(form).ajaxSend({
				dataType: "text",
				beforeSubmit: function(a,f,o) {
				},  
				success: function(data,e,x){ 
					var date = new Date();
					$(firstDiv).html('<img src="'+imageFolder+id+'_small.jpg?time='+date.getTime()+'" onerror="this.remove();" class="img-thumbnail"/>');
				},
				error: function(data,e,x){  
					
				}
			});
		});
		
		$(".img-menu").unbind();
		$(".img-menu").click(function(){
			var src = $(this).attr("src");
			src = src.replace("small","original");
			$("#image-original").attr("src", src);
			$("#modalImagen").modal("show");
		});
	}
	
	$('#modal').on('hidden.bs.modal', function (e) {
		$("#modal input").val(""); 
		$("#id").val("0"); 
		$("#modal textarea").val("");   
		$("#menu-form").attr("action", "/Menus/AgregarMenu");
		$("#button-submit").text("Agregar");
		$("#modal-title").text("Agregar Nuevo Menu");
		$("input").removeClass("errorDisplay");
		$("input").tooltip('destroy');	
		$("textarea").removeClass("errorDisplay");
		$("textarea").tooltip('destroy');	 
		$("#IndiceEsperado").val("2.4");
		$("#CostoPlanificado").val("0.4");
		$("#precio-venta-div").addClass("hidden");
		$("#PrecioDeVenta").val("0");
		
		searchMenu();
	});
	 
	$("#menu-form").ajaxSend({
		dataType: "text",
		beforeSubmit: function(a,f,o) {
		},  
		success: function(data,e,x){ 
			var oldId = parseFloat($("#id").val());
			var newId = parseFloat(data);
			if(oldId == 0 && oldId != newId)
				window.location.href = "fichacosto?id="+newId;
			else{
				$("#modal").modal("hide"); 
				searchMenu();   
			}
		},
		error: function(data,e,x){  
			
		}
	}); 
	
	$("#nombre").blur(function(){
		$.ajax({  
			url: "existemenu", 
			type: "POST",
			data: {descripcion: $("#nombre").val()},   
			dataType : "json",     
			success: function(data) { 	  
				if(data.length>0 && data[0].id!=$("#id").val()){
					$("#alert-danger").html("Ya existe un menu con el nombre <a href='fichacosto?id="+
							data[0].id+"'>"+data[0].nombre+"</a>.");
					$("#alert-danger").removeClass("hidden");	  
					$("#nombre").val("");
				}
				else
					$("#alert-danger").addClass("hidden");	
			} ,      
			error: function() { 	
				$("#alert-danger").addClass("hidden");				
			}
		}); 
	});
	
		
		
	
});   