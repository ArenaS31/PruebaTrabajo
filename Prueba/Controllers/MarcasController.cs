using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Prueba.Models;

namespace Prueba.Controllers
{
    public class MarcasController : Controller
    {
        private ConexionBD db = new ConexionBD();

//--------------------------------------------------------------Index Marcas--------------------------------------------------------------------------
        public ActionResult Index()
        {
            //Solicita la conexión con la bd y la tabla marcas para mostrarla en una lista
            return View(db.Marcas.ToList());
        }

//--------------------------------------------------------------Nueva marca--------------------------------------------------------------------------
        // GET
        public ActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken] // Asegura que la solicitud provenga de la misma aplicación.
        // El parámetro `marcas` es un objeto que contiene los datos del formulario. 
        public ActionResult Create([Bind(Include = "Id_Marca,Nombre_Marca,Representante")] Marcas marcas)
        {
            if (ModelState.IsValid) // Verifica si los datos son validos
            {
                db.Marcas.Add(marcas); // Agrega a la bd
                db.SaveChanges(); // Guarda los cambios hechos a la bd
                return RedirectToAction("Index"); // Redirige al index de la lista
            }

            // Si la validacion fallo, vuelve a mostrar el formulario
            return View(marcas);
        }

//--------------------------------------------------------------Edita marca--------------------------------------------------------------------------
        // GET
        public ActionResult Edit(int? id) // Recibe el parametro id
        {
            if (id == null) // Verifica si es nulo
            {
                // Si el id es nulo, devuelve un resultado de error HTTP 400 BadRequest, indicando que la solicitud es inválida.
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Busca el id en la tabla de la bd
            Marcas marcas = db.Marcas.Find(id);
            if (marcas == null) // Verifica si hay resultados
            {
                // Si no se encuentra ese id, devuelve un resultado HTTP 404 NotFound
                return HttpNotFound();
            }
            // Si se encuentra el id, se envia a la vista para continuar
            return View(marcas);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken] // Asegura que la solicitud provenga de la misma aplicación.
        // El parámetro `marcas` es un objeto que contiene los datos del formulario. 
        public ActionResult Edit([Bind(Include = "Id_Marca,Nombre_Marca,Representante")] Marcas marcas)
        {
            if (ModelState.IsValid) // Verifica si los datos son validos
            {
                db.Entry(marcas).State = EntityState.Modified; // Realiza la actualización en la bd
                db.SaveChanges(); // Guarda los combios hechos
                return RedirectToAction("Index");  // Retorna al index
            }
            return View(marcas); // Si no son validos los datos, retorna a la vista
        }

//--------------------------------------------------------------Elimina marca--------------------------------------------------------------------------
        // GET
        public ActionResult Delete(int? id) // Recibe el parametro id
        {

            if (id == null) // Verifica si hay un id
            {
                // Si el id es nulo, devuelve un resultado de error HTTP 400 BadRequest, indicando que la solicitud es inválida.
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Busca el id en la tabla de la bd
            Marcas marcas = db.Marcas.Find(id);
            if (marcas == null) // Verifica si encontro resultados
            {
                // Si no se encuentra ese id, devuelve un resultado HTTP 404 NotFound
                return HttpNotFound();
            }
            // Regresa a la vista para continuar con el proceso
            return View(marcas);
        }

        // POST
        // Este se ejecuta cuando se envia un formulario de eliminación
        [HttpPost, ActionName("Delete")] // Metodo post y alias del metodo delete
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Busca el id en la bd
                Marcas marcas = db.Marcas.Find(id);
                if (marcas == null)
                {
                    return HttpNotFound();
                }

                // Verifica si la marca tiene productos relacionados
                if (marcas.Productos.Any())
                {
                    // Si hay productos relacionados, muestra un mensaje de error
                    ModelState.AddModelError("", "No se puede eliminar la marca porque tiene productos asociados.");
                    return View("Delete", marcas); // Retorna con un mensaje de error
                }

                // Si no hay productos asociados entonces elimina
                db.Marcas.Remove(marcas);
                db.SaveChanges();

                // Redirecciona al index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Maneja cualquier otra excepción
                ModelState.AddModelError("", "Ocurrió un error al intentar eliminar la marca: " + ex.Message);
                return View("Delete", db.Marcas.Find(id)); // Retorna con un mensaje de error
            }
        }


//---------------------------------------------------------------------------------------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
