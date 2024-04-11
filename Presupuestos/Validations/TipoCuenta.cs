using System.ComponentModel.DataAnnotations;

namespace Presupuestos.Models
{
    public class TipoCuenta : IValidatableObject
    {
        public int Id { get; set; }

        [Required (ErrorMessage ="El campo {0} es requerido")]
        [StringLength (maximumLength:50, MinimumLength =3,ErrorMessage ="La longitud del campo {0} debe estar entre {2} y {1}")]
        [Display(Name ="Nombre del tipo de la cuenta")]

        //Probar los dos tipos de validaciones personalizadas
        //[ValidarMayuscula]
        public string? Nombre {  get; set; }
        public int UsuarioID { get; set; }
        public int Orden {  get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Nombre != null && Nombre.Length > 0)
            {
                var firstLetter = Nombre[0].ToString ();
                if(firstLetter != firstLetter.ToUpper ())
                {
                    yield return new ValidationResult("La primera letra debe ser mayuscula",
                        new[] {nameof(Nombre)});
                }
            }
        }
    }
}
