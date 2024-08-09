using System;
using System.Linq;
using System.Web.Mvc;
using Prueba.Models;
using System.Net.Mail;
using System.Net;

namespace Prueba.Controllers
{
    public class HomeController : Controller
    {
        private ConexionBD db = new ConexionBD();

//--------------------------------------------------------------CIERRE DE SESIÓN--------------------------------------------------------------------------
        public ActionResult Layout()
        {
            // Destruye la sesión
            Session.Abandon();

            // Redirige al login
            return RedirectToAction("Index", "Home");
        }

//--------------------------------------------------------------iNICIO DE SESIÓN--------------------------------------------------------------------------
        // GET
        public ActionResult Index()
        {
            return View();
        }

        // POST
        [HttpPost]
        public ActionResult Index(string usuario, string contraseña)// Recibe los datos del name de los imput
        {
            if (ModelState.IsValid)
            {
                // Verifica si el usuario y contraseña coinciden con un registro de la tabla
                var usuarioExistente = db.Usuarios.SingleOrDefault(u => u.Username == usuario && u.Pass == contraseña);
                if (usuarioExistente == null)
                {
                    // En caso de no coincidir retorna un mensaje de error
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                    return View();
                }

                // Crea una sesión temporal para guardar el usuario
                Session["Usuario"] = usuario;

                // Redirige a otra parte
                return RedirectToAction("Index", "Marcas");
            }

            return View();
        }

//--------------------------------------------------------------REGISTRO--------------------------------------------------------------------------
        // GET
        public ActionResult RegistrarCuenta()
        {
            return View();
        }

        // POST
        [HttpPost]
        public ActionResult RegistrarCuenta(string usuario, string correo, string contraseña)// Recibe los datos del name de los imput
        {
            if (ModelState.IsValid)
            {
                // Verificar si el usuario ya existe, corrobora con correo y con usuario
                var existeUsuario = db.Usuarios.Any(u => u.Username == usuario || u.Correo == correo);
                if (existeUsuario)
                {
                    // En caso de no concidir envia un error con mensaje
                    ModelState.AddModelError("", "El usuario o correo ya está registrado.");
                    return View();
                }

                // De ko contrario crear nuevo registro
                var nuevoUsuario = new Usuarios
                {
                    Username = usuario,
                    Pass = contraseña,
                    Correo = correo
                };

                //EnviarCorreoConfirmacion(correo);

                db.Usuarios.Add(nuevoUsuario);
                db.SaveChanges();

                // Redirige al Login
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        //// Prepara correo de confirmación y envio ********** no me funciono
        //private void EnviarCorreoConfirmacion(string correoDestinatario)
        //{
        //    var fromAddress = new MailAddress("19680099@Cuautla.tecnm.mx", "Emanuel");
        //    var toAddress = new MailAddress(correoDestinatario, "To Name");
        //    const string fromPassword = "Sistemas2019";
        //    const string subject = "Confirmación de cuenta";
        //    const string body = "Gracias por registrarte en mi pagina :)";

        //    var smtp = new SmtpClient
        //    {
        //        Host = "smtp.gmail.com",
        //        Port = 587,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        //    };
        //    using (var message = new MailMessage(fromAddress, toAddress)
        //    {
        //        Subject = subject,
        //        Body = body
        //    })
        //    {
        //        smtp.Send(message);
        //    }
        //}

//-------------------------------------------------------------CAMBIO DE CONTRASEÑA---------------------------------------------------------------------------
        // GET
        public ActionResult CambiarContraseña()
        {
            return View();
        }

        // POST
        [HttpPost]
        public ActionResult CambiarContraseña(string usuario, string correo, string nuevaContraseña)// Recibe los datos del name de los imput
        {
            if (ModelState.IsValid)
            {
                // Verifica si el usuario y correo son correctos
                var usuarioExistente = db.Usuarios.SingleOrDefault(u => u.Username == usuario && u.Correo == correo);
                if (usuarioExistente == null)
                {
                    // En caso de no concidir envia un error con mensaje
                    ModelState.AddModelError("", "Alguno o ambos datos son incorrectos, vuelve a intentar.");
                    return View();
                }

                // Actualizar la contraseña
                usuarioExistente.Pass = nuevaContraseña;
                db.SaveChanges();

                // Regresa un mensaje de confirmación para el cambio de contraseña
                ModelState.AddModelError("", "Contraseña cambiada.");
                // Redirige al Login
                return View();
            }

            return View();
        }
    }
}