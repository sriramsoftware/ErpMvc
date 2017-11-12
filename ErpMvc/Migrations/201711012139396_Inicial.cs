namespace ErpMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.cv_agregados",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ElaboracionId = c.Int(nullable: false),
                        ProductoId = c.Int(nullable: false),
                        Cantidad = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnidadDeMedidaId = c.Int(nullable: false),
                        Costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Precio = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Elaboracions", t => t.ElaboracionId)
                .ForeignKey("dbo.com_productos", t => t.ProductoId)
                .ForeignKey("dbo.UnidadDeMedidas", t => t.UnidadDeMedidaId, cascadeDelete: true)
                .Index(t => t.ElaboracionId)
                .Index(t => t.ProductoId)
                .Index(t => t.UnidadDeMedidaId);
            
            CreateTable(
                "dbo.Elaboracions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Preparacion = c.String(),
                        Presentacion = c.String(),
                        CostoPlanificado = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IndiceEsperado = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrecioDeVenta = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Activo = c.Boolean(nullable: false),
                        CentroDeCostoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.contb_centros_de_costo", t => t.CentroDeCostoId, cascadeDelete: true)
                .Index(t => t.CentroDeCostoId);
            
            CreateTable(
                "dbo.contb_centros_de_costo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductosPorElaboracions",
                c => new
                    {
                        ProductoId = c.Int(nullable: false),
                        ElaboracionId = c.Int(nullable: false),
                        Cantidad = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnidadDeMedidaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductoId, t.ElaboracionId })
                .ForeignKey("dbo.Elaboracions", t => t.ElaboracionId, cascadeDelete: true)
                .ForeignKey("dbo.com_productos", t => t.ProductoId, cascadeDelete: true)
                .ForeignKey("dbo.UnidadDeMedidas", t => t.UnidadDeMedidaId, cascadeDelete: true)
                .Index(t => t.ProductoId)
                .Index(t => t.ElaboracionId)
                .Index(t => t.UnidadDeMedidaId);
            
            CreateTable(
                "dbo.com_productos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                        Descripcion = c.String(),
                        Activo = c.Boolean(nullable: false),
                        EsInventariable = c.Boolean(nullable: false),
                        GrupoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GrupoDeProductoes", t => t.GrupoId, cascadeDelete: true)
                .Index(t => t.GrupoId);
            
            CreateTable(
                "dbo.GrupoDeProductoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                        ClasificacionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClasificacionDeProductoes", t => t.ClasificacionId, cascadeDelete: true)
                .Index(t => t.ClasificacionId);
            
            CreateTable(
                "dbo.ClasificacionDeProductoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UnidadDeMedidas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                        Siglas = c.String(nullable: false),
                        TipoDeUnidadDeMedidaId = c.Int(nullable: false),
                        FactorDeConversion = c.Decimal(nullable: false, precision: 15, scale: 5),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.com_tipos_de_unid_de_medidas", t => t.TipoDeUnidadDeMedidaId, cascadeDelete: true)
                .Index(t => t.TipoDeUnidadDeMedidaId);
            
            CreateTable(
                "dbo.com_tipos_de_unid_de_medidas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.rst_agregados_de_comandas",
                c => new
                    {
                        DetalleDeComandaId = c.Int(nullable: false),
                        AgregadoId = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DetalleDeComandaId, t.AgregadoId })
                .ForeignKey("dbo.cv_agregados", t => t.AgregadoId, cascadeDelete: true)
                .ForeignKey("dbo.rst_detalles_de_comandas", t => t.DetalleDeComandaId, cascadeDelete: true)
                .Index(t => t.DetalleDeComandaId)
                .Index(t => t.AgregadoId);
            
            CreateTable(
                "dbo.rst_detalles_de_comandas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ComandaId = c.Int(nullable: false),
                        ElaboracionId = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.rst_comandas", t => t.ComandaId, cascadeDelete: true)
                .ForeignKey("dbo.Elaboracions", t => t.ElaboracionId, cascadeDelete: true)
                .Index(t => t.ComandaId)
                .Index(t => t.ElaboracionId);
            
            CreateTable(
                "dbo.rst_comandas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VentaId = c.Int(),
                        DiaContableId = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        VendedorId = c.Int(nullable: false),
                        PuntoDeVentaId = c.Int(nullable: false),
                        CantidadPersonas = c.Int(nullable: false),
                        EstadoDeVenta = c.Int(nullable: false),
                        UsuarioId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.contb_dia_contable", t => t.DiaContableId, cascadeDelete: true)
                .ForeignKey("dbo.PuntoDeVentas", t => t.PuntoDeVentaId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UsuarioId)
                .ForeignKey("dbo.ident_personas", t => t.VendedorId, cascadeDelete: true)
                .ForeignKey("dbo.Ventas", t => t.VentaId)
                .Index(t => t.VentaId)
                .Index(t => t.DiaContableId)
                .Index(t => t.VendedorId)
                .Index(t => t.PuntoDeVentaId)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.rst_ordenes_comanda",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Numero = c.Int(nullable: false),
                        Comensal = c.Int(nullable: false),
                        ComandaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.rst_comandas", t => t.ComandaId)
                .Index(t => t.ComandaId);
            
            CreateTable(
                "dbo.rst_orden_por_detalle",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DetalleDeComandaId = c.Int(nullable: false),
                        OrdenId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.rst_detalles_de_comandas", t => t.DetalleDeComandaId, cascadeDelete: true)
                .ForeignKey("dbo.rst_ordenes_comanda", t => t.OrdenId, cascadeDelete: true)
                .Index(t => t.DetalleDeComandaId)
                .Index(t => t.OrdenId);
            
            CreateTable(
                "dbo.rst_anotaciones",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Abreviatura = c.String(),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.contb_dia_contable",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Fecha = c.DateTime(nullable: false),
                        Abierto = c.Boolean(nullable: false),
                        HoraEnQueCerro = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.contb_asientos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiaContableId = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        UsuarioId = c.String(nullable: false, maxLength: 128),
                        Detalle = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.contb_dia_contable", t => t.DiaContableId)
                .ForeignKey("dbo.AspNetUsers", t => t.UsuarioId, cascadeDelete: true)
                .Index(t => t.DiaContableId)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.contb_movimientos",
                c => new
                    {
                        AsientoId = c.Int(nullable: false),
                        CuentaId = c.Int(nullable: false),
                        Importe = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TipoDeOperacion = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AsientoId, t.CuentaId })
                .ForeignKey("dbo.contb_asientos", t => t.AsientoId, cascadeDelete: true)
                .ForeignKey("dbo.contb_cuentas", t => t.CuentaId, cascadeDelete: true)
                .Index(t => t.AsientoId)
                .Index(t => t.CuentaId);
            
            CreateTable(
                "dbo.contb_cuentas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NivelId = c.Int(nullable: false),
                        Naturaleza = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.contb_niveles", t => t.NivelId, cascadeDelete: true)
                .Index(t => t.NivelId);
            
            CreateTable(
                "dbo.contb_disponibilidades",
                c => new
                    {
                        CuentaId = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        Saldo = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CuentaId)
                .ForeignKey("dbo.contb_cuentas", t => t.CuentaId)
                .Index(t => t.CuentaId);
            
            CreateTable(
                "dbo.contb_niveles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Numero = c.String(nullable: false),
                        Nombre = c.String(nullable: false),
                        NivelSuperiorId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.contb_niveles", t => t.NivelSuperiorId)
                .Index(t => t.NivelSuperiorId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        Correo = c.String(),
                        Activo = c.Boolean(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ServicioInformaticoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PuntoDeVentas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        CentroDeCostoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.contb_centros_de_costo", t => t.CentroDeCostoId)
                .Index(t => t.CentroDeCostoId);
            
            CreateTable(
                "dbo.ident_personas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ci = c.String(nullable: false, maxLength: 11),
                        Nombres = c.String(nullable: false),
                        PrimerApellido = c.String(nullable: false),
                        SegundoApellido = c.String(nullable: false),
                        EstadoCivil = c.Int(nullable: false),
                        Sexo = c.Int(nullable: false),
                        NivelDeEscolaridad = c.Int(nullable: false),
                        DireccionId = c.Int(nullable: false),
                        Codigo = c.String(),
                        PuntoDeVentaId = c.Int(),
                        Discriminator = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ident_direcciones", t => t.DireccionId)
                .ForeignKey("dbo.PuntoDeVentas", t => t.PuntoDeVentaId)
                .Index(t => t.DireccionId)
                .Index(t => t.PuntoDeVentaId);
            
            CreateTable(
                "dbo.ident_caracts_pers",
                c => new
                    {
                        PersonaId = c.Int(nullable: false),
                        Foto = c.Binary(),
                        ColorDePiel = c.Int(nullable: false),
                        ColorDeOjos = c.Int(nullable: false),
                        TallaPantalon = c.Double(nullable: false),
                        TallaCamisa = c.Double(nullable: false),
                        TallaCalzado = c.Double(nullable: false),
                        OtrasCaracteristicas = c.String(),
                    })
                .PrimaryKey(t => t.PersonaId)
                .ForeignKey("dbo.ident_personas", t => t.PersonaId)
                .Index(t => t.PersonaId);
            
            CreateTable(
                "dbo.ident_direcciones",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Municipio = c.String(),
                        Provincia = c.String(),
                        Calle = c.String(),
                        Piso = c.Int(),
                        Rpto = c.String(),
                        No = c.String(),
                        Apto = c.String(),
                        EntreCalle1 = c.String(),
                        EntreCalle2 = c.String(),
                        Telefono = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.rh_organizaciones",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PuestoDeTrabajoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CargoId = c.Int(nullable: false),
                        UnidadOrganizativaId = c.Int(nullable: false),
                        Descripcion = c.String(),
                        CantidadPorPlantilla = c.Int(nullable: false),
                        JefeId = c.Int(),
                        Activo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cargoes", t => t.CargoId, cascadeDelete: true)
                .ForeignKey("dbo.PuestoDeTrabajoes", t => t.JefeId)
                .ForeignKey("dbo.UnidadOrganizativas", t => t.UnidadOrganizativaId, cascadeDelete: true)
                .Index(t => t.CargoId)
                .Index(t => t.UnidadOrganizativaId)
                .Index(t => t.JefeId);
            
            CreateTable(
                "dbo.Cargoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                        Sigla = c.String(),
                        NivelDeEscolaridad = c.Int(nullable: false),
                        GrupoEscalaId = c.Int(nullable: false),
                        Activo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GrupoEscalas", t => t.GrupoEscalaId, cascadeDelete: true)
                .Index(t => t.GrupoEscalaId);
            
            CreateTable(
                "dbo.GrupoEscalas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(),
                        CategoriaOcupacionalId = c.Int(nullable: false),
                        SalarioDiferenciado = c.Boolean(nullable: false),
                        SalarioEscala = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.rh_categorias_ocupacionales", t => t.CategoriaOcupacionalId, cascadeDelete: true)
                .Index(t => t.CategoriaOcupacionalId);
            
            CreateTable(
                "dbo.rh_categorias_ocupacionales",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HistoricoPuestoDeTrabajoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TrabajadorId = c.Int(nullable: false),
                        PuestoDeTrabajoId = c.Int(nullable: false),
                        FechaInicio = c.DateTime(nullable: false),
                        FechaTerminado = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PuestoDeTrabajoes", t => t.PuestoDeTrabajoId, cascadeDelete: true)
                .ForeignKey("dbo.rh_trabajadores", t => t.TrabajadorId)
                .Index(t => t.TrabajadorId)
                .Index(t => t.PuestoDeTrabajoId);
            
            CreateTable(
                "dbo.UnidadOrganizativas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(nullable: false),
                        Nombre = c.String(nullable: false),
                        TipoUnidadOrganizativaId = c.Int(nullable: false),
                        PerteneceAId = c.Int(),
                        Activa = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UnidadOrganizativas", t => t.PerteneceAId)
                .ForeignKey("dbo.rh_tipos_de_unidad_organizativa", t => t.TipoUnidadOrganizativaId, cascadeDelete: true)
                .Index(t => t.TipoUnidadOrganizativaId)
                .Index(t => t.PerteneceAId);
            
            CreateTable(
                "dbo.rh_tipos_de_unidad_organizativa",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                        Prioridad = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Ventas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiaContableId = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        VendedorId = c.Int(nullable: false),
                        PuntoDeVentaId = c.Int(nullable: false),
                        ClienteId = c.Int(),
                        Importe = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Codigo = c.String(),
                        CantidadPersonas = c.Int(nullable: false),
                        EstadoDeVenta = c.Int(nullable: false),
                        UsuarioId = c.String(maxLength: 128),
                        Observaciones = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.com_clientes", t => t.ClienteId)
                .ForeignKey("dbo.contb_dia_contable", t => t.DiaContableId, cascadeDelete: true)
                .ForeignKey("dbo.PuntoDeVentas", t => t.PuntoDeVentaId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UsuarioId)
                .ForeignKey("dbo.ident_personas", t => t.VendedorId)
                .Index(t => t.DiaContableId)
                .Index(t => t.VendedorId)
                .Index(t => t.PuntoDeVentaId)
                .Index(t => t.ClienteId)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.com_clientes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntidadId = c.Int(),
                        PersonaId = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.com_entidades", t => t.EntidadId, cascadeDelete: true)
                .ForeignKey("dbo.ident_personas", t => t.PersonaId, cascadeDelete: true)
                .Index(t => t.EntidadId)
                .Index(t => t.PersonaId);
            
            CreateTable(
                "dbo.com_entidades",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                        CodigoReup = c.String(nullable: false),
                        Direccion = c.String(nullable: false),
                        Nit = c.String(nullable: false),
                        CtaBancariaCuc = c.String(),
                        CtaBancariaMn = c.String(),
                        NombreCtaCuc = c.String(),
                        NombreCtaMn = c.String(),
                        Telefono = c.String(),
                        Fax = c.String(),
                        Correo = c.String(),
                        PerteneceUNE = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DetalleDeVentas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VentaId = c.Int(nullable: false),
                        ElaboracionId = c.Int(nullable: false),
                        Cantidad = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ImporteTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Elaboracions", t => t.ElaboracionId, cascadeDelete: true)
                .ForeignKey("dbo.Ventas", t => t.VentaId, cascadeDelete: true)
                .Index(t => t.VentaId)
                .Index(t => t.ElaboracionId);
            
            CreateTable(
                "dbo.AgregadosVendidos",
                c => new
                    {
                        DetalleDeVentaId = c.Int(nullable: false),
                        AgregadoId = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DetalleDeVentaId, t.AgregadoId })
                .ForeignKey("dbo.cv_agregados", t => t.AgregadoId, cascadeDelete: true)
                .ForeignKey("dbo.DetalleDeVentas", t => t.DetalleDeVentaId, cascadeDelete: true)
                .Index(t => t.DetalleDeVentaId)
                .Index(t => t.AgregadoId);
            
            CreateTable(
                "dbo.alm_almacenes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EntradaAlmacens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductoId = c.Int(nullable: false),
                        AlmacenId = c.Int(nullable: false),
                        DiaContableId = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        Cantidad = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UsuarioId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.alm_almacenes", t => t.AlmacenId, cascadeDelete: true)
                .ForeignKey("dbo.contb_dia_contable", t => t.DiaContableId, cascadeDelete: true)
                .ForeignKey("dbo.ProductoConcretoes", t => t.ProductoId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UsuarioId)
                .Index(t => t.ProductoId)
                .Index(t => t.AlmacenId)
                .Index(t => t.DiaContableId)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.ProductoConcretoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductoId = c.Int(nullable: false),
                        UnidadDeMedidaId = c.Int(nullable: false),
                        Cantidad = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProporcionDeMerma = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrecioUnitario = c.Decimal(nullable: false, precision: 15, scale: 12),
                        PrecioDeVenta = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.com_productos", t => t.ProductoId, cascadeDelete: true)
                .ForeignKey("dbo.UnidadDeMedidas", t => t.UnidadDeMedidaId, cascadeDelete: true)
                .Index(t => t.ProductoId)
                .Index(t => t.UnidadDeMedidaId);
            
            CreateTable(
                "dbo.ValeSalidaDeAlmacens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiaContableId = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        AlmacenId = c.Int(nullable: false),
                        CentroDeCostoId = c.Int(nullable: false),
                        Descripcion = c.String(),
                        UsuarioId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.alm_almacenes", t => t.AlmacenId)
                .ForeignKey("dbo.contb_centros_de_costo", t => t.CentroDeCostoId, cascadeDelete: true)
                .ForeignKey("dbo.contb_dia_contable", t => t.DiaContableId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UsuarioId)
                .Index(t => t.DiaContableId)
                .Index(t => t.AlmacenId)
                .Index(t => t.CentroDeCostoId)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.DetalleSalidaAlmacens",
                c => new
                    {
                        ProductoId = c.Int(nullable: false),
                        ValeId = c.Int(nullable: false),
                        Cantidad = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.ProductoId, t.ValeId })
                .ForeignKey("dbo.ExistenciaAlmacens", t => t.ProductoId, cascadeDelete: true)
                .ForeignKey("dbo.ValeSalidaDeAlmacens", t => t.ValeId)
                .Index(t => t.ProductoId)
                .Index(t => t.ValeId);
            
            CreateTable(
                "dbo.ExistenciaAlmacens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AlmacenId = c.Int(nullable: false),
                        ProductoId = c.Int(nullable: false),
                        ExistenciaEnAlmacen = c.Decimal(nullable: false, precision: 15, scale: 5),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.alm_almacenes", t => t.AlmacenId, cascadeDelete: true)
                .ForeignKey("dbo.ProductoConcretoes", t => t.ProductoId, cascadeDelete: true)
                .Index(t => t.AlmacenId)
                .Index(t => t.ProductoId);
            
            CreateTable(
                "dbo.Asistencias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VendedorId = c.Int(nullable: false),
                        DiaContableId = c.Int(nullable: false),
                        Entrada = c.DateTime(),
                        Salida = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.contb_dia_contable", t => t.DiaContableId, cascadeDelete: true)
                .ForeignKey("dbo.ident_personas", t => t.VendedorId, cascadeDelete: true)
                .Index(t => t.VendedorId)
                .Index(t => t.DiaContableId);
            
            CreateTable(
                "dbo.Cajas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DenominacionEnCajas",
                c => new
                    {
                        CajaId = c.Int(nullable: false),
                        DenominacionId = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CajaId, t.DenominacionId })
                .ForeignKey("dbo.Cajas", t => t.CajaId, cascadeDelete: true)
                .ForeignKey("dbo.DenominacionDeMonedas", t => t.DenominacionId, cascadeDelete: true)
                .Index(t => t.CajaId)
                .Index(t => t.DenominacionId);
            
            CreateTable(
                "dbo.DenominacionDeMonedas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MonedaId = c.Int(nullable: false),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Descripcion = c.String(),
                        Billete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Monedas", t => t.MonedaId, cascadeDelete: true)
                .Index(t => t.MonedaId);
            
            CreateTable(
                "dbo.Monedas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                        Sigla = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CierreDeCajas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiaContableId = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        CajaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cajas", t => t.CajaId, cascadeDelete: true)
                .ForeignKey("dbo.contb_dia_contable", t => t.DiaContableId, cascadeDelete: true)
                .Index(t => t.DiaContableId)
                .Index(t => t.CajaId);
            
            CreateTable(
                "dbo.DenominacionesEnCierreDeCajas",
                c => new
                    {
                        CierreDeCajaid = c.Int(nullable: false),
                        DenominacionDeMonedaId = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CierreDeCajaid, t.DenominacionDeMonedaId })
                .ForeignKey("dbo.CierreDeCajas", t => t.CierreDeCajaid, cascadeDelete: true)
                .ForeignKey("dbo.DenominacionDeMonedas", t => t.DenominacionDeMonedaId, cascadeDelete: true)
                .Index(t => t.CierreDeCajaid)
                .Index(t => t.DenominacionDeMonedaId);
            
            CreateTable(
                "dbo.contb_config_cuenta_mod",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                        CuentaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.contb_cuentas", t => t.CuentaId, cascadeDelete: true)
                .Index(t => t.CuentaId);
            
            CreateTable(
                "dbo.Compras",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiaContableId = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        EntidadId = c.Int(nullable: false),
                        Descripcion = c.String(),
                        UsuarioId = c.String(maxLength: 128),
                        TieneComprobante = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.contb_dia_contable", t => t.DiaContableId, cascadeDelete: true)
                .ForeignKey("dbo.com_entidades", t => t.EntidadId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UsuarioId)
                .Index(t => t.DiaContableId)
                .Index(t => t.EntidadId)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.DetalleDeCompras",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompraId = c.Int(nullable: false),
                        ProductoId = c.Int(nullable: false),
                        UnidadDeMedidaId = c.Int(nullable: false),
                        Cantidad = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ImporteTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MonedaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Compras", t => t.CompraId, cascadeDelete: true)
                .ForeignKey("dbo.Monedas", t => t.MonedaId, cascadeDelete: true)
                .ForeignKey("dbo.com_productos", t => t.ProductoId, cascadeDelete: true)
                .ForeignKey("dbo.UnidadDeMedidas", t => t.UnidadDeMedidaId, cascadeDelete: true)
                .Index(t => t.CompraId)
                .Index(t => t.ProductoId)
                .Index(t => t.UnidadDeMedidaId)
                .Index(t => t.MonedaId);
            
            CreateTable(
                "dbo.ConceptoDeGastoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExistenciaCentroDeCostoes",
                c => new
                    {
                        ProductoId = c.Int(nullable: false),
                        CentroDeCostoId = c.Int(nullable: false),
                        Cantidad = c.Decimal(nullable: false, precision: 15, scale: 5),
                    })
                .PrimaryKey(t => new { t.ProductoId, t.CentroDeCostoId })
                .ForeignKey("dbo.contb_centros_de_costo", t => t.CentroDeCostoId, cascadeDelete: true)
                .ForeignKey("dbo.ProductoConcretoes", t => t.ProductoId, cascadeDelete: true)
                .Index(t => t.ProductoId)
                .Index(t => t.CentroDeCostoId);
            
            CreateTable(
                "dbo.LicenciaInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Suscriptor = c.String(),
                        Aplicacion = c.String(),
                        FechaDeVencimiento = c.DateTime(nullable: false),
                        Hash = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LogDeAccesoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UsuarioId = c.String(maxLength: 128),
                        Fecha = c.DateTime(),
                        TipoDeAcceso = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UsuarioId)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.MovimientoDeProductoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiaContableId = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        ProductoId = c.Int(nullable: false),
                        Cantidad = c.Decimal(nullable: false, precision: 15, scale: 5),
                        CentroDeCostoId = c.Int(nullable: false),
                        TipoId = c.Int(nullable: false),
                        UsuarioId = c.String(maxLength: 128),
                        Costo = c.Decimal(nullable: false, precision: 18, scale: 12),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.contb_centros_de_costo", t => t.CentroDeCostoId, cascadeDelete: true)
                .ForeignKey("dbo.contb_dia_contable", t => t.DiaContableId, cascadeDelete: true)
                .ForeignKey("dbo.ProductoConcretoes", t => t.ProductoId, cascadeDelete: true)
                .ForeignKey("dbo.TipoDeMovimientoes", t => t.TipoId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UsuarioId)
                .Index(t => t.DiaContableId)
                .Index(t => t.ProductoId)
                .Index(t => t.CentroDeCostoId)
                .Index(t => t.TipoId)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.TipoDeMovimientoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                        Factor = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OtrosGastos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiaContableId = c.Int(nullable: false),
                        Importe = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ConceptoDeGastoId = c.Int(nullable: false),
                        PagadoPorCaja = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ConceptoDeGastoes", t => t.ConceptoDeGastoId, cascadeDelete: true)
                .ForeignKey("dbo.contb_dia_contable", t => t.DiaContableId, cascadeDelete: true)
                .Index(t => t.DiaContableId)
                .Index(t => t.ConceptoDeGastoId);
            
            CreateTable(
                "dbo.rst_porciento_menu",
                c => new
                    {
                        ElaboracioId = c.Int(nullable: false),
                        SeCalcula = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ElaboracioId)
                .ForeignKey("dbo.Elaboracions", t => t.ElaboracioId)
                .Index(t => t.ElaboracioId);
            
            CreateTable(
                "dbo.rst_propina",
                c => new
                    {
                        VentaId = c.Int(nullable: false),
                        Importe = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.VentaId)
                .ForeignKey("dbo.Ventas", t => t.VentaId)
                .Index(t => t.VentaId);
            
            CreateTable(
                "dbo.SalidaPorMermas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiaContableId = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        ExistenciaAlmacenId = c.Int(nullable: false),
                        Cantidad = c.Decimal(nullable: false, precision: 15, scale: 5),
                        UnidadDeMedidaId = c.Int(nullable: false),
                        Costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UsuarioId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.contb_dia_contable", t => t.DiaContableId, cascadeDelete: true)
                .ForeignKey("dbo.ExistenciaAlmacens", t => t.ExistenciaAlmacenId, cascadeDelete: true)
                .ForeignKey("dbo.UnidadDeMedidas", t => t.UnidadDeMedidaId)
                .ForeignKey("dbo.AspNetUsers", t => t.UsuarioId)
                .Index(t => t.DiaContableId)
                .Index(t => t.ExistenciaAlmacenId)
                .Index(t => t.UnidadDeMedidaId)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.TarjetaDeAsistencias",
                c => new
                    {
                        VendedorId = c.Int(nullable: false),
                        Usuario = c.String(),
                        Contraseña = c.String(),
                    })
                .PrimaryKey(t => t.VendedorId)
                .ForeignKey("dbo.ident_personas", t => t.VendedorId)
                .Index(t => t.VendedorId);
            
            CreateTable(
                "dbo.rst_anotaciones_orden_detalles",
                c => new
                    {
                        OrdenPorDetalle_Id = c.Int(nullable: false),
                        Anotacion_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrdenPorDetalle_Id, t.Anotacion_Id })
                .ForeignKey("dbo.rst_orden_por_detalle", t => t.OrdenPorDetalle_Id, cascadeDelete: true)
                .ForeignKey("dbo.rst_anotaciones", t => t.Anotacion_Id, cascadeDelete: true)
                .Index(t => t.OrdenPorDetalle_Id)
                .Index(t => t.Anotacion_Id);
            
            CreateTable(
                "dbo.ServicioInformaticoUsuarios",
                c => new
                    {
                        ServicioInformatico_Id = c.Int(nullable: false),
                        Usuario_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ServicioInformatico_Id, t.Usuario_Id })
                .ForeignKey("dbo.ServicioInformaticoes", t => t.ServicioInformatico_Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.Usuario_Id, cascadeDelete: true)
                .Index(t => t.ServicioInformatico_Id)
                .Index(t => t.Usuario_Id);
            
            CreateTable(
                "dbo.rh_organizaciones_de_trabajadores",
                c => new
                    {
                        TrabajadorId = c.Int(nullable: false),
                        OrganizacionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrabajadorId, t.OrganizacionId })
                .ForeignKey("dbo.rh_trabajadores", t => t.TrabajadorId, cascadeDelete: true)
                .ForeignKey("dbo.rh_organizaciones", t => t.OrganizacionId, cascadeDelete: true)
                .Index(t => t.TrabajadorId)
                .Index(t => t.OrganizacionId);
            
            CreateTable(
                "dbo.rh_trabajadores",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        NoExpedienteLaboral = c.String(nullable: false),
                        Estado = c.Int(nullable: false),
                        PuestoDeTrabajoId = c.Int(),
                        UsuarioId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ident_personas", t => t.Id)
                .ForeignKey("dbo.PuestoDeTrabajoes", t => t.PuestoDeTrabajoId)
                .ForeignKey("dbo.AspNetUsers", t => t.UsuarioId)
                .Index(t => t.Id)
                .Index(t => t.PuestoDeTrabajoId)
                .Index(t => t.UsuarioId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.rh_trabajadores", "UsuarioId", "dbo.AspNetUsers");
            DropForeignKey("dbo.rh_trabajadores", "PuestoDeTrabajoId", "dbo.PuestoDeTrabajoes");
            DropForeignKey("dbo.rh_trabajadores", "Id", "dbo.ident_personas");
            DropForeignKey("dbo.TarjetaDeAsistencias", "VendedorId", "dbo.ident_personas");
            DropForeignKey("dbo.SalidaPorMermas", "UsuarioId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SalidaPorMermas", "UnidadDeMedidaId", "dbo.UnidadDeMedidas");
            DropForeignKey("dbo.SalidaPorMermas", "ExistenciaAlmacenId", "dbo.ExistenciaAlmacens");
            DropForeignKey("dbo.SalidaPorMermas", "DiaContableId", "dbo.contb_dia_contable");
            DropForeignKey("dbo.rst_propina", "VentaId", "dbo.Ventas");
            DropForeignKey("dbo.rst_porciento_menu", "ElaboracioId", "dbo.Elaboracions");
            DropForeignKey("dbo.OtrosGastos", "DiaContableId", "dbo.contb_dia_contable");
            DropForeignKey("dbo.OtrosGastos", "ConceptoDeGastoId", "dbo.ConceptoDeGastoes");
            DropForeignKey("dbo.MovimientoDeProductoes", "UsuarioId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MovimientoDeProductoes", "TipoId", "dbo.TipoDeMovimientoes");
            DropForeignKey("dbo.MovimientoDeProductoes", "ProductoId", "dbo.ProductoConcretoes");
            DropForeignKey("dbo.MovimientoDeProductoes", "DiaContableId", "dbo.contb_dia_contable");
            DropForeignKey("dbo.MovimientoDeProductoes", "CentroDeCostoId", "dbo.contb_centros_de_costo");
            DropForeignKey("dbo.LogDeAccesoes", "UsuarioId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ExistenciaCentroDeCostoes", "ProductoId", "dbo.ProductoConcretoes");
            DropForeignKey("dbo.ExistenciaCentroDeCostoes", "CentroDeCostoId", "dbo.contb_centros_de_costo");
            DropForeignKey("dbo.Compras", "UsuarioId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DetalleDeCompras", "UnidadDeMedidaId", "dbo.UnidadDeMedidas");
            DropForeignKey("dbo.DetalleDeCompras", "ProductoId", "dbo.com_productos");
            DropForeignKey("dbo.DetalleDeCompras", "MonedaId", "dbo.Monedas");
            DropForeignKey("dbo.DetalleDeCompras", "CompraId", "dbo.Compras");
            DropForeignKey("dbo.Compras", "EntidadId", "dbo.com_entidades");
            DropForeignKey("dbo.Compras", "DiaContableId", "dbo.contb_dia_contable");
            DropForeignKey("dbo.contb_config_cuenta_mod", "CuentaId", "dbo.contb_cuentas");
            DropForeignKey("dbo.CierreDeCajas", "DiaContableId", "dbo.contb_dia_contable");
            DropForeignKey("dbo.DenominacionesEnCierreDeCajas", "DenominacionDeMonedaId", "dbo.DenominacionDeMonedas");
            DropForeignKey("dbo.DenominacionesEnCierreDeCajas", "CierreDeCajaid", "dbo.CierreDeCajas");
            DropForeignKey("dbo.CierreDeCajas", "CajaId", "dbo.Cajas");
            DropForeignKey("dbo.DenominacionEnCajas", "DenominacionId", "dbo.DenominacionDeMonedas");
            DropForeignKey("dbo.DenominacionDeMonedas", "MonedaId", "dbo.Monedas");
            DropForeignKey("dbo.DenominacionEnCajas", "CajaId", "dbo.Cajas");
            DropForeignKey("dbo.Asistencias", "VendedorId", "dbo.ident_personas");
            DropForeignKey("dbo.Asistencias", "DiaContableId", "dbo.contb_dia_contable");
            DropForeignKey("dbo.ValeSalidaDeAlmacens", "UsuarioId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DetalleSalidaAlmacens", "ValeId", "dbo.ValeSalidaDeAlmacens");
            DropForeignKey("dbo.DetalleSalidaAlmacens", "ProductoId", "dbo.ExistenciaAlmacens");
            DropForeignKey("dbo.ExistenciaAlmacens", "ProductoId", "dbo.ProductoConcretoes");
            DropForeignKey("dbo.ExistenciaAlmacens", "AlmacenId", "dbo.alm_almacenes");
            DropForeignKey("dbo.ValeSalidaDeAlmacens", "DiaContableId", "dbo.contb_dia_contable");
            DropForeignKey("dbo.ValeSalidaDeAlmacens", "CentroDeCostoId", "dbo.contb_centros_de_costo");
            DropForeignKey("dbo.ValeSalidaDeAlmacens", "AlmacenId", "dbo.alm_almacenes");
            DropForeignKey("dbo.EntradaAlmacens", "UsuarioId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EntradaAlmacens", "ProductoId", "dbo.ProductoConcretoes");
            DropForeignKey("dbo.ProductoConcretoes", "UnidadDeMedidaId", "dbo.UnidadDeMedidas");
            DropForeignKey("dbo.ProductoConcretoes", "ProductoId", "dbo.com_productos");
            DropForeignKey("dbo.EntradaAlmacens", "DiaContableId", "dbo.contb_dia_contable");
            DropForeignKey("dbo.EntradaAlmacens", "AlmacenId", "dbo.alm_almacenes");
            DropForeignKey("dbo.rst_detalles_de_comandas", "ElaboracionId", "dbo.Elaboracions");
            DropForeignKey("dbo.rst_comandas", "VentaId", "dbo.Ventas");
            DropForeignKey("dbo.Ventas", "VendedorId", "dbo.ident_personas");
            DropForeignKey("dbo.Ventas", "UsuarioId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Ventas", "PuntoDeVentaId", "dbo.PuntoDeVentas");
            DropForeignKey("dbo.DetalleDeVentas", "VentaId", "dbo.Ventas");
            DropForeignKey("dbo.DetalleDeVentas", "ElaboracionId", "dbo.Elaboracions");
            DropForeignKey("dbo.AgregadosVendidos", "DetalleDeVentaId", "dbo.DetalleDeVentas");
            DropForeignKey("dbo.AgregadosVendidos", "AgregadoId", "dbo.cv_agregados");
            DropForeignKey("dbo.Ventas", "DiaContableId", "dbo.contb_dia_contable");
            DropForeignKey("dbo.Ventas", "ClienteId", "dbo.com_clientes");
            DropForeignKey("dbo.com_clientes", "PersonaId", "dbo.ident_personas");
            DropForeignKey("dbo.com_clientes", "EntidadId", "dbo.com_entidades");
            DropForeignKey("dbo.rst_comandas", "VendedorId", "dbo.ident_personas");
            DropForeignKey("dbo.rst_comandas", "UsuarioId", "dbo.AspNetUsers");
            DropForeignKey("dbo.rst_comandas", "PuntoDeVentaId", "dbo.PuntoDeVentas");
            DropForeignKey("dbo.ident_personas", "PuntoDeVentaId", "dbo.PuntoDeVentas");
            DropForeignKey("dbo.ident_caracts_pers", "PersonaId", "dbo.ident_personas");
            DropForeignKey("dbo.UnidadOrganizativas", "TipoUnidadOrganizativaId", "dbo.rh_tipos_de_unidad_organizativa");
            DropForeignKey("dbo.PuestoDeTrabajoes", "UnidadOrganizativaId", "dbo.UnidadOrganizativas");
            DropForeignKey("dbo.UnidadOrganizativas", "PerteneceAId", "dbo.UnidadOrganizativas");
            DropForeignKey("dbo.PuestoDeTrabajoes", "JefeId", "dbo.PuestoDeTrabajoes");
            DropForeignKey("dbo.HistoricoPuestoDeTrabajoes", "TrabajadorId", "dbo.rh_trabajadores");
            DropForeignKey("dbo.HistoricoPuestoDeTrabajoes", "PuestoDeTrabajoId", "dbo.PuestoDeTrabajoes");
            DropForeignKey("dbo.PuestoDeTrabajoes", "CargoId", "dbo.Cargoes");
            DropForeignKey("dbo.GrupoEscalas", "CategoriaOcupacionalId", "dbo.rh_categorias_ocupacionales");
            DropForeignKey("dbo.Cargoes", "GrupoEscalaId", "dbo.GrupoEscalas");
            DropForeignKey("dbo.rh_organizaciones_de_trabajadores", "OrganizacionId", "dbo.rh_organizaciones");
            DropForeignKey("dbo.rh_organizaciones_de_trabajadores", "TrabajadorId", "dbo.rh_trabajadores");
            DropForeignKey("dbo.ident_personas", "DireccionId", "dbo.ident_direcciones");
            DropForeignKey("dbo.PuntoDeVentas", "CentroDeCostoId", "dbo.contb_centros_de_costo");
            DropForeignKey("dbo.rst_comandas", "DiaContableId", "dbo.contb_dia_contable");
            DropForeignKey("dbo.contb_asientos", "UsuarioId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ServicioInformaticoUsuarios", "Usuario_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ServicioInformaticoUsuarios", "ServicioInformatico_Id", "dbo.ServicioInformaticoes");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.contb_movimientos", "CuentaId", "dbo.contb_cuentas");
            DropForeignKey("dbo.contb_cuentas", "NivelId", "dbo.contb_niveles");
            DropForeignKey("dbo.contb_niveles", "NivelSuperiorId", "dbo.contb_niveles");
            DropForeignKey("dbo.contb_disponibilidades", "CuentaId", "dbo.contb_cuentas");
            DropForeignKey("dbo.contb_movimientos", "AsientoId", "dbo.contb_asientos");
            DropForeignKey("dbo.contb_asientos", "DiaContableId", "dbo.contb_dia_contable");
            DropForeignKey("dbo.rst_detalles_de_comandas", "ComandaId", "dbo.rst_comandas");
            DropForeignKey("dbo.rst_ordenes_comanda", "ComandaId", "dbo.rst_comandas");
            DropForeignKey("dbo.rst_orden_por_detalle", "OrdenId", "dbo.rst_ordenes_comanda");
            DropForeignKey("dbo.rst_orden_por_detalle", "DetalleDeComandaId", "dbo.rst_detalles_de_comandas");
            DropForeignKey("dbo.rst_anotaciones_orden_detalles", "Anotacion_Id", "dbo.rst_anotaciones");
            DropForeignKey("dbo.rst_anotaciones_orden_detalles", "OrdenPorDetalle_Id", "dbo.rst_orden_por_detalle");
            DropForeignKey("dbo.rst_agregados_de_comandas", "DetalleDeComandaId", "dbo.rst_detalles_de_comandas");
            DropForeignKey("dbo.rst_agregados_de_comandas", "AgregadoId", "dbo.cv_agregados");
            DropForeignKey("dbo.cv_agregados", "UnidadDeMedidaId", "dbo.UnidadDeMedidas");
            DropForeignKey("dbo.cv_agregados", "ProductoId", "dbo.com_productos");
            DropForeignKey("dbo.cv_agregados", "ElaboracionId", "dbo.Elaboracions");
            DropForeignKey("dbo.ProductosPorElaboracions", "UnidadDeMedidaId", "dbo.UnidadDeMedidas");
            DropForeignKey("dbo.UnidadDeMedidas", "TipoDeUnidadDeMedidaId", "dbo.com_tipos_de_unid_de_medidas");
            DropForeignKey("dbo.ProductosPorElaboracions", "ProductoId", "dbo.com_productos");
            DropForeignKey("dbo.com_productos", "GrupoId", "dbo.GrupoDeProductoes");
            DropForeignKey("dbo.GrupoDeProductoes", "ClasificacionId", "dbo.ClasificacionDeProductoes");
            DropForeignKey("dbo.ProductosPorElaboracions", "ElaboracionId", "dbo.Elaboracions");
            DropForeignKey("dbo.Elaboracions", "CentroDeCostoId", "dbo.contb_centros_de_costo");
            DropIndex("dbo.rh_trabajadores", new[] { "UsuarioId" });
            DropIndex("dbo.rh_trabajadores", new[] { "PuestoDeTrabajoId" });
            DropIndex("dbo.rh_trabajadores", new[] { "Id" });
            DropIndex("dbo.rh_organizaciones_de_trabajadores", new[] { "OrganizacionId" });
            DropIndex("dbo.rh_organizaciones_de_trabajadores", new[] { "TrabajadorId" });
            DropIndex("dbo.ServicioInformaticoUsuarios", new[] { "Usuario_Id" });
            DropIndex("dbo.ServicioInformaticoUsuarios", new[] { "ServicioInformatico_Id" });
            DropIndex("dbo.rst_anotaciones_orden_detalles", new[] { "Anotacion_Id" });
            DropIndex("dbo.rst_anotaciones_orden_detalles", new[] { "OrdenPorDetalle_Id" });
            DropIndex("dbo.TarjetaDeAsistencias", new[] { "VendedorId" });
            DropIndex("dbo.SalidaPorMermas", new[] { "UsuarioId" });
            DropIndex("dbo.SalidaPorMermas", new[] { "UnidadDeMedidaId" });
            DropIndex("dbo.SalidaPorMermas", new[] { "ExistenciaAlmacenId" });
            DropIndex("dbo.SalidaPorMermas", new[] { "DiaContableId" });
            DropIndex("dbo.rst_propina", new[] { "VentaId" });
            DropIndex("dbo.rst_porciento_menu", new[] { "ElaboracioId" });
            DropIndex("dbo.OtrosGastos", new[] { "ConceptoDeGastoId" });
            DropIndex("dbo.OtrosGastos", new[] { "DiaContableId" });
            DropIndex("dbo.MovimientoDeProductoes", new[] { "UsuarioId" });
            DropIndex("dbo.MovimientoDeProductoes", new[] { "TipoId" });
            DropIndex("dbo.MovimientoDeProductoes", new[] { "CentroDeCostoId" });
            DropIndex("dbo.MovimientoDeProductoes", new[] { "ProductoId" });
            DropIndex("dbo.MovimientoDeProductoes", new[] { "DiaContableId" });
            DropIndex("dbo.LogDeAccesoes", new[] { "UsuarioId" });
            DropIndex("dbo.ExistenciaCentroDeCostoes", new[] { "CentroDeCostoId" });
            DropIndex("dbo.ExistenciaCentroDeCostoes", new[] { "ProductoId" });
            DropIndex("dbo.DetalleDeCompras", new[] { "MonedaId" });
            DropIndex("dbo.DetalleDeCompras", new[] { "UnidadDeMedidaId" });
            DropIndex("dbo.DetalleDeCompras", new[] { "ProductoId" });
            DropIndex("dbo.DetalleDeCompras", new[] { "CompraId" });
            DropIndex("dbo.Compras", new[] { "UsuarioId" });
            DropIndex("dbo.Compras", new[] { "EntidadId" });
            DropIndex("dbo.Compras", new[] { "DiaContableId" });
            DropIndex("dbo.contb_config_cuenta_mod", new[] { "CuentaId" });
            DropIndex("dbo.DenominacionesEnCierreDeCajas", new[] { "DenominacionDeMonedaId" });
            DropIndex("dbo.DenominacionesEnCierreDeCajas", new[] { "CierreDeCajaid" });
            DropIndex("dbo.CierreDeCajas", new[] { "CajaId" });
            DropIndex("dbo.CierreDeCajas", new[] { "DiaContableId" });
            DropIndex("dbo.DenominacionDeMonedas", new[] { "MonedaId" });
            DropIndex("dbo.DenominacionEnCajas", new[] { "DenominacionId" });
            DropIndex("dbo.DenominacionEnCajas", new[] { "CajaId" });
            DropIndex("dbo.Asistencias", new[] { "DiaContableId" });
            DropIndex("dbo.Asistencias", new[] { "VendedorId" });
            DropIndex("dbo.ExistenciaAlmacens", new[] { "ProductoId" });
            DropIndex("dbo.ExistenciaAlmacens", new[] { "AlmacenId" });
            DropIndex("dbo.DetalleSalidaAlmacens", new[] { "ValeId" });
            DropIndex("dbo.DetalleSalidaAlmacens", new[] { "ProductoId" });
            DropIndex("dbo.ValeSalidaDeAlmacens", new[] { "UsuarioId" });
            DropIndex("dbo.ValeSalidaDeAlmacens", new[] { "CentroDeCostoId" });
            DropIndex("dbo.ValeSalidaDeAlmacens", new[] { "AlmacenId" });
            DropIndex("dbo.ValeSalidaDeAlmacens", new[] { "DiaContableId" });
            DropIndex("dbo.ProductoConcretoes", new[] { "UnidadDeMedidaId" });
            DropIndex("dbo.ProductoConcretoes", new[] { "ProductoId" });
            DropIndex("dbo.EntradaAlmacens", new[] { "UsuarioId" });
            DropIndex("dbo.EntradaAlmacens", new[] { "DiaContableId" });
            DropIndex("dbo.EntradaAlmacens", new[] { "AlmacenId" });
            DropIndex("dbo.EntradaAlmacens", new[] { "ProductoId" });
            DropIndex("dbo.AgregadosVendidos", new[] { "AgregadoId" });
            DropIndex("dbo.AgregadosVendidos", new[] { "DetalleDeVentaId" });
            DropIndex("dbo.DetalleDeVentas", new[] { "ElaboracionId" });
            DropIndex("dbo.DetalleDeVentas", new[] { "VentaId" });
            DropIndex("dbo.com_clientes", new[] { "PersonaId" });
            DropIndex("dbo.com_clientes", new[] { "EntidadId" });
            DropIndex("dbo.Ventas", new[] { "UsuarioId" });
            DropIndex("dbo.Ventas", new[] { "ClienteId" });
            DropIndex("dbo.Ventas", new[] { "PuntoDeVentaId" });
            DropIndex("dbo.Ventas", new[] { "VendedorId" });
            DropIndex("dbo.Ventas", new[] { "DiaContableId" });
            DropIndex("dbo.UnidadOrganizativas", new[] { "PerteneceAId" });
            DropIndex("dbo.UnidadOrganizativas", new[] { "TipoUnidadOrganizativaId" });
            DropIndex("dbo.HistoricoPuestoDeTrabajoes", new[] { "PuestoDeTrabajoId" });
            DropIndex("dbo.HistoricoPuestoDeTrabajoes", new[] { "TrabajadorId" });
            DropIndex("dbo.GrupoEscalas", new[] { "CategoriaOcupacionalId" });
            DropIndex("dbo.Cargoes", new[] { "GrupoEscalaId" });
            DropIndex("dbo.PuestoDeTrabajoes", new[] { "JefeId" });
            DropIndex("dbo.PuestoDeTrabajoes", new[] { "UnidadOrganizativaId" });
            DropIndex("dbo.PuestoDeTrabajoes", new[] { "CargoId" });
            DropIndex("dbo.ident_caracts_pers", new[] { "PersonaId" });
            DropIndex("dbo.ident_personas", new[] { "PuntoDeVentaId" });
            DropIndex("dbo.ident_personas", new[] { "DireccionId" });
            DropIndex("dbo.PuntoDeVentas", new[] { "CentroDeCostoId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.contb_niveles", new[] { "NivelSuperiorId" });
            DropIndex("dbo.contb_disponibilidades", new[] { "CuentaId" });
            DropIndex("dbo.contb_cuentas", new[] { "NivelId" });
            DropIndex("dbo.contb_movimientos", new[] { "CuentaId" });
            DropIndex("dbo.contb_movimientos", new[] { "AsientoId" });
            DropIndex("dbo.contb_asientos", new[] { "UsuarioId" });
            DropIndex("dbo.contb_asientos", new[] { "DiaContableId" });
            DropIndex("dbo.rst_orden_por_detalle", new[] { "OrdenId" });
            DropIndex("dbo.rst_orden_por_detalle", new[] { "DetalleDeComandaId" });
            DropIndex("dbo.rst_ordenes_comanda", new[] { "ComandaId" });
            DropIndex("dbo.rst_comandas", new[] { "UsuarioId" });
            DropIndex("dbo.rst_comandas", new[] { "PuntoDeVentaId" });
            DropIndex("dbo.rst_comandas", new[] { "VendedorId" });
            DropIndex("dbo.rst_comandas", new[] { "DiaContableId" });
            DropIndex("dbo.rst_comandas", new[] { "VentaId" });
            DropIndex("dbo.rst_detalles_de_comandas", new[] { "ElaboracionId" });
            DropIndex("dbo.rst_detalles_de_comandas", new[] { "ComandaId" });
            DropIndex("dbo.rst_agregados_de_comandas", new[] { "AgregadoId" });
            DropIndex("dbo.rst_agregados_de_comandas", new[] { "DetalleDeComandaId" });
            DropIndex("dbo.UnidadDeMedidas", new[] { "TipoDeUnidadDeMedidaId" });
            DropIndex("dbo.GrupoDeProductoes", new[] { "ClasificacionId" });
            DropIndex("dbo.com_productos", new[] { "GrupoId" });
            DropIndex("dbo.ProductosPorElaboracions", new[] { "UnidadDeMedidaId" });
            DropIndex("dbo.ProductosPorElaboracions", new[] { "ElaboracionId" });
            DropIndex("dbo.ProductosPorElaboracions", new[] { "ProductoId" });
            DropIndex("dbo.Elaboracions", new[] { "CentroDeCostoId" });
            DropIndex("dbo.cv_agregados", new[] { "UnidadDeMedidaId" });
            DropIndex("dbo.cv_agregados", new[] { "ProductoId" });
            DropIndex("dbo.cv_agregados", new[] { "ElaboracionId" });
            DropTable("dbo.rh_trabajadores");
            DropTable("dbo.rh_organizaciones_de_trabajadores");
            DropTable("dbo.ServicioInformaticoUsuarios");
            DropTable("dbo.rst_anotaciones_orden_detalles");
            DropTable("dbo.TarjetaDeAsistencias");
            DropTable("dbo.SalidaPorMermas");
            DropTable("dbo.rst_propina");
            DropTable("dbo.rst_porciento_menu");
            DropTable("dbo.OtrosGastos");
            DropTable("dbo.TipoDeMovimientoes");
            DropTable("dbo.MovimientoDeProductoes");
            DropTable("dbo.LogDeAccesoes");
            DropTable("dbo.LicenciaInfoes");
            DropTable("dbo.ExistenciaCentroDeCostoes");
            DropTable("dbo.ConceptoDeGastoes");
            DropTable("dbo.DetalleDeCompras");
            DropTable("dbo.Compras");
            DropTable("dbo.contb_config_cuenta_mod");
            DropTable("dbo.DenominacionesEnCierreDeCajas");
            DropTable("dbo.CierreDeCajas");
            DropTable("dbo.Monedas");
            DropTable("dbo.DenominacionDeMonedas");
            DropTable("dbo.DenominacionEnCajas");
            DropTable("dbo.Cajas");
            DropTable("dbo.Asistencias");
            DropTable("dbo.ExistenciaAlmacens");
            DropTable("dbo.DetalleSalidaAlmacens");
            DropTable("dbo.ValeSalidaDeAlmacens");
            DropTable("dbo.ProductoConcretoes");
            DropTable("dbo.EntradaAlmacens");
            DropTable("dbo.alm_almacenes");
            DropTable("dbo.AgregadosVendidos");
            DropTable("dbo.DetalleDeVentas");
            DropTable("dbo.com_entidades");
            DropTable("dbo.com_clientes");
            DropTable("dbo.Ventas");
            DropTable("dbo.rh_tipos_de_unidad_organizativa");
            DropTable("dbo.UnidadOrganizativas");
            DropTable("dbo.HistoricoPuestoDeTrabajoes");
            DropTable("dbo.rh_categorias_ocupacionales");
            DropTable("dbo.GrupoEscalas");
            DropTable("dbo.Cargoes");
            DropTable("dbo.PuestoDeTrabajoes");
            DropTable("dbo.rh_organizaciones");
            DropTable("dbo.ident_direcciones");
            DropTable("dbo.ident_caracts_pers");
            DropTable("dbo.ident_personas");
            DropTable("dbo.PuntoDeVentas");
            DropTable("dbo.ServicioInformaticoes");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.contb_niveles");
            DropTable("dbo.contb_disponibilidades");
            DropTable("dbo.contb_cuentas");
            DropTable("dbo.contb_movimientos");
            DropTable("dbo.contb_asientos");
            DropTable("dbo.contb_dia_contable");
            DropTable("dbo.rst_anotaciones");
            DropTable("dbo.rst_orden_por_detalle");
            DropTable("dbo.rst_ordenes_comanda");
            DropTable("dbo.rst_comandas");
            DropTable("dbo.rst_detalles_de_comandas");
            DropTable("dbo.rst_agregados_de_comandas");
            DropTable("dbo.com_tipos_de_unid_de_medidas");
            DropTable("dbo.UnidadDeMedidas");
            DropTable("dbo.ClasificacionDeProductoes");
            DropTable("dbo.GrupoDeProductoes");
            DropTable("dbo.com_productos");
            DropTable("dbo.ProductosPorElaboracions");
            DropTable("dbo.contb_centros_de_costo");
            DropTable("dbo.Elaboracions");
            DropTable("dbo.cv_agregados");
        }
    }
}
