using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skymigration
{
    /// <summary>
    /// 0	apptNumber
    /// 1	date
    /// 2	resourceId
    /// 3	customerNumber
    /// 4	activityType
    /// 5	timeSlot
    /// 6	customerName
    /// 7	customerPhone
    /// 8	customerCell
    /// 9	customerEmail
    /// 10	streetAddress
    /// 11	city
    /// 12	country_code
    /// 13	longitude
    /// 14	latitude
    /// 15	slaWindowStart
    /// 16	slaWindowEnd
    /// 17	postalCode
    /// 18	stateProvince
    /// 19	points
    /// 20	XA_FechaAtencionProgramada
    /// 21	XA_HoraAtencionProgramada
    /// 22	XA_Prioridad
    /// 23	XA_ClaveCapturaVenta
    /// 24	XA_EquipoTerminal
    /// 25	XA_FolioBt
    /// 26	XA_MedioEntregaModem
    /// 27	XA_Nis
    /// 28	XA_NumAsignado
    /// 29	XA_Paquete
    /// 30	XA_Promocion
    /// 31	XA_TipoIdentificación
    /// 32	XA_NumeroIdentificacion
    /// 33	XA_CanalVenta
    /// 34	XA_TipoCuenta
    /// 35	XA_SubtipoCliente
    /// 36	XA_RFC
    /// 37	XA_Contrato
    /// 38	XA_TipoVenta
    /// 39	XA_SubtipoVenta
    /// 40	XA_NumeroExterior
    /// 41	XA_NumeroInterior
    /// 42	XA_Colonia
    /// 43	XA_Municipio
    /// 44	XA_EntreCalle1
    /// 45	XA_EntreCalle2
    /// 46	XA_ComentariosDireccion
    /// 47	XA_CodAreaTel1
    /// 48	XA_CodAreaTel2
    /// 49	XA_Telefono2
    /// 50	XA_ExtTel2
    /// 51	XA_CodAreaTel3
    /// 52	XA_Telefono3
    /// 53	XA_ExtTel3
    /// 54	XA_AreaServicio
    /// 55	XA_TipoServicio
    /// 56	XA_SubtipoServicio
    /// 57	XA_Zona
    /// 58	XA_IDLlamada
    /// 59	XA_LlamadaEspera
    /// 60	XA_Tripartita
    /// 61	XA_BuzonVoz
    /// 62	XA_Sigueme
    /// 63	XA_OrdenServicio
    /// 64	XA_FolioSV
    /// 65	XA_saldo_cuenta
    /// 66	XA_saldo_adeudado
    /// 67	XA_saldo_penalizado
    /// 68	XA_tipo_de_plazo
    /// 69	XA_tipo_instalacion
    /// 70	XA_id_instalador
    /// 71	XA_mov_tipo_eq
    /// 72	XA_red
    /// </summary>
    public class Activity 
    {
        public int activityId { get; set; }
        public string apptNumber { get; set; }
        public string date { get; set; }
        public string resourceId { get; set; }
        public string customerNumber { get; set; }
        public string activityType { get; set; }
        public string timeSlot { get; set; }
        public string customerName { get; set; }
        public string customerPhone { get; set; }
        public string customerCell { get; set; }
        public string customerEmail { get; set; }
        public string streetAddress { get; set; }
        public string city { get; set; }
        public string country_code { get; set; }
        public float longitude { get; set; }
        public float latitude { get; set; }
        public string slaWindowStart { get; set; }
        public string slaWindowEnd { get; set; }
        public string postalCode { get; set; }
        public string stateProvince { get; set; }
        public int points { get; set; }
        public string XA_FechaAtencionProgramada { get; set; }
        public string XA_HoraAtencionProgramada { get; set; }
        public string XA_Prioridad { get; set; }
        public string XA_ClaveCapturaVenta { get; set; }
        public string XA_EquipoTerminal { get; set; }
        public string XA_FolioBt { get; set; }
        public string XA_MedioEntregaModem { get; set; }
        public string XA_Nis { get; set; }
        public string XA_NumAsignado { get; set; }
        public string XA_Paquete { get; set; }
        public string XA_Promocion { get; set; }
        public string XA_TipoIdentificación { get; set; }
        public string XA_NumeroIdentificacion { get; set; }
        public string XA_CanalVenta { get; set; }
        public string XA_TipoCuenta { get; set; }
        public string XA_SubtipoCliente { get; set; }
        public string XA_RFC { get; set; }
        public string XA_Contrato { get; set; }
        public string XA_TipoVenta { get; set; }
        public string XA_SubtipoVenta { get; set; }
        public string XA_NumeroExterior { get; set; }
        public string XA_NumeroInterior { get; set; }
        public string XA_Colonia { get; set; }
        public string XA_Municipio { get; set; }
        public string XA_EntreCalle1 { get; set; }
        public string XA_EntreCalle2 { get; set; }
        public string XA_ComentariosDireccion { get; set; }
        public string XA_CodAreaTel1 { get; set; }
        public string XA_CodAreaTel2 { get; set; }
        public string XA_Telefono2 { get; set; }
        public string XA_ExtTel2 { get; set; }
        public string XA_CodAreaTel3 { get; set; }
        public string XA_Telefono3 { get; set; }
        public string XA_ExtTel3 { get; set; }
        public string XA_AreaServicio { get; set; }
        public string XA_TipoServicio { get; set; }
        public string XA_SubtipoServicio { get; set; }
        public string XA_Zona { get; set; }
        public string XA_IDLlamada { get; set; }
        public string XA_LlamadaEspera { get; set; }
        public string XA_Tripartita { get; set; }
        public string XA_BuzonVoz { get; set; }
        public string XA_Sigueme { get; set; }
        public string XA_OrdenServicio { get; set; }
        public string XA_FolioSV { get; set; }
        public string XA_saldo_cuenta { get; set; }
        public string XA_saldo_adeudado { get; set; }
        public string XA_saldo_penalizado { get; set; }
        public string XA_tipo_de_plazo { get; set; }
        // TODO
        // si no existe ver como omitir este valor
        // public string XA_tipo_instalacion { get; set; }
        public string XA_id_instalador { get; set; }
        public string XA_mov_tipo_eq { get; set; }
        public string XA_red { get; set; }

        public List<Inventory> Inventories { get; set; }
    }
}
