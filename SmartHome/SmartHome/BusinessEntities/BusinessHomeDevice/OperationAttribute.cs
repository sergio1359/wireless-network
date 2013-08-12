#region Using Statements

#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public class OperationAttribute : System.Attribute 
    {
        public bool Internal
        {
            get;
            set;
        }

        public OperationAttribute()
        {
        }
    }

    /// <summary>
    /// He añadido este atributo para que podamos poner una funcion en cada una de las clases para hacer actualizacion del estado concreto
    /// EJ: Conocer si una luz esta encendida o apagada
    /// En caso de no tener esta etiqueta se considera que el homedevice no tiene estado que actualizar
    /// </summary>
    public class OperationStateHomeDeviceAttribute : System.Attribute
    {
        public bool Internal
        {
            get;
            set;
        }

        public OperationStateHomeDeviceAttribute()
        {
        }
    }
}
