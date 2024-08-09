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
    public class ProductosController : Controller
    {
        private ConexionBD db = new ConexionBD();

//--------------------------------------------------------------INDEX Y BUSQUEDA--------------------------------------------------------------------------
        // GET
        public ActionResult Index(string searchTerm) // Recibe el término de búsqueda del formulario
        {
            // Obtiene todos los productos e incluye la relación con las marcas
            var productos = db.Productos.Include(p => p.Marcas);

            // Verifica si existe un dato a buscar
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Filtra los productos que tengan como nombre o marca el dato
                productos = productos.Where(p => p.Nombre_Producto.Contains(searchTerm) || p.Marcas.Nombre_Marca.Contains(searchTerm));
            }

            // Convierte la consulta en lista y la envia a la vista
            return View(productos.ToList());
        }

//--------------------------------------------------------------REGISTRO--------------------------------------------------------------------------
        // GET
        public ActionResult Create()
        {
            //Crea una lista desplegable que usa el id como codigo y el nombre de la marca como opción de la lista
            ViewBag.id_marca = new SelectList(db.Marcas, "Id_Marca", "Nombre_Marca");
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Producto,Nombre_Producto,Categoria,id_marca")] Productos productos)
        {
            if (ModelState.IsValid) // Verifica que los datos sean validos
            {
                db.Productos.Add(productos); // Agvrega en la tabla 
                db.SaveChanges(); // Guarda los cambios de la bd
                return RedirectToAction("Index"); // Redirecciona al index
            }
            //En caso de no ser validos, retorna a la vista
            ViewBag.id_marca = new SelectList(db.Marcas, "Id_Marca", "Nombre_Marca", productos.id_marca);
            return View(productos);
        }

//--------------------------------------------------------------EDITAR--------------------------------------------------------------------------
        // GET
        public ActionResult Edit(int? id) // Recibe el id de l formulario
        {
            if (id == null) // Verifica que el id no sea nulo
            {
                // Si el id es nulo, devuelve un resultado de error HTTP 400 BadRequest, indicando que la solicitud es inválida.
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Busca el id en la bd
            Productos productos = db.Productos.Find(id);
            if (productos == null)
            {
                // Si no se encuentra ese id, devuelve un resultado HTTP 404 NotFound
                return HttpNotFound();
            }
            ViewBag.id_marca = new SelectList(db.Marcas, "Id_Marca", "Nombre_Marca", productos.id_marca);
            return View(productos);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Producto,Nombre_Producto,Categoria,id_marca")] Productos productos) // Se ejecuta al confirmar la edición
        {
            if (ModelState.IsValid) // Verifica que los datos sean validos
            {
                db.Entry(productos).State = EntityState.Modified; // Realiza la actualizacián en la tabla
                db.SaveChanges(); // Guarda los cambios hechos
                return RedirectToAction("Index");
            }
            // Si los cambios no son validos, retorna a la vista
            ViewBag.id_marca = new SelectList(db.Marcas, "Id_Marca", "Nombre_Marca", productos.id_marca);
            return View(productos);
        }

//--------------------------------------------------------------ELIMINAR--------------------------------------------------------------------------
        // GET
        public ActionResult Delete(int? id) // Recibe el id del formulario 
        {
            if (id == null) // VErifica que el id no sea nulo
            {
                // Si el id es nulo, devuelve un resultado de error HTTP 400 BadRequest, indicando que la solicitud es inválida.
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Busca el id en la tabla de la bd
            Productos productos = db.Productos.Find(id);
            if (productos == null) // Verifica que el id exista
            {
                // Si no se encuentra ese id, devuelve un resultado HTTP 404 NotFound
                return HttpNotFound();
            }
            return View(productos); 
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) // Se ejecuta el comfirmar la eliminación
        {
            Productos productos = db.Productos.Find(id); // Busca el id en la tabla de la bd
            db.Productos.Remove(productos); // Elimina el registro
            db.SaveChanges(); // Guarda los cambios hechos
            return RedirectToAction("Index"); // Redirecciona al index
        }


//--------------------------------------------------------------ELIMINACIÓN MULTIPLE--------------------------------------------------------------------------
        // POST 
        [HttpPost]
        // Al activarse el metodo, guarda todos los id seleccionados en un array
        public ActionResult EliminarSeleccionados(int[] productosSeleccionados)
        {
            if (productosSeleccionados != null && productosSeleccionados.Any()) // Verifica que existan datos en el array
            {
                foreach (var id in productosSeleccionados) // Recorre el array
                {
                    var producto = db.Productos.Find(id); // Busca el id seleccionado en la bd
                    if (producto != null) // Si existe, procede a eliminarlo
                    {
                        db.Productos.Remove(producto);
                    }
                }
                db.SaveChanges(); // Guarda los cambios en la bd
            }

            return RedirectToAction("Index"); // Redirecciona al index
        }


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
